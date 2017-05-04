using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Testity.EngineComponents;
using Fasterflect;
using System.Threading;

namespace Testity.Common.Unity3D
{
	public static class EngineScriptComponentFactory
	{
		//Unity3D doesn't support recursive
		private static readonly ReaderWriterLockSlim syncObj = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		private static IDictionary<Type, Func<EngineScriptComponent>> cachedLambdaCreationFuncsMap
			= new Dictionary<Type, Func<EngineScriptComponent>>(300);

		//Based on: https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
		//Why are we doing this? new T() when constrained by new emits Activator.CreateInstance which is slow...
		//How slow? Well, in .Net 4.5 it is kinda faster but in .Net 3.5, or worse probably Unity3D's amalgamation of Mono 2.0 or whatever
		//it will likely run slower than .Net 3.5 which is already insanely slow.
		//This provides nearly naitive speed after the first call
		public static TEngineScriptComponentType Create<TEngineScriptComponentType>()
			where TEngineScriptComponentType : EngineScriptComponent, new()
		{
			syncObj.EnterReadLock();
			try
			{
				if (cachedLambdaCreationFuncsMap.ContainsKey(typeof(TEngineScriptComponentType)))
					return cachedLambdaCreationFuncsMap[typeof(TEngineScriptComponentType)]() as TEngineScriptComponentType;
			}
			finally
			{
				syncObj.ExitReadLock();
			}

			//We don't use upgradeable because reads should stop above
			syncObj.EnterWriteLock();
			try
			{
				//Double check locking is required. Could have been added inbetween the locks.
				if (!cachedLambdaCreationFuncsMap.ContainsKey(typeof(TEngineScriptComponentType)))
					cachedLambdaCreationFuncsMap[typeof(TEngineScriptComponentType)] = BuildCompiledCreationLambda<TEngineScriptComponentType>();	
			}
			finally
			{
				syncObj.ExitWriteLock();
			}
			
			//recur but don't do it around the readerwritelockslim. Unity3d doesn't support lock recursion.
			return Create<TEngineScriptComponentType>();
		}

		//Based on: https://vagifabilov.wordpress.com/2010/04/02/dont-use-activator-createinstance-or-constructorinfo-invoke-use-compiled-lambda-expressions/
		private static Func<EngineScriptComponent> BuildCompiledCreationLambda<TEngineScriptComponentType>()
			where TEngineScriptComponentType : EngineScriptComponent, new()
		{
			//We create a new expression and compile it into a delegate so we can call it next time
			NewExpression creationExpression = Expression.New(typeof(TEngineScriptComponentType).Constructor());

			LambdaExpression creationLambda = Expression.Lambda<Func<TEngineScriptComponentType>>(creationExpression);

			//.Net 3.5 doesn't have contravariant Func<T> so we have to make a lambda around this func
			//We wrap the compiled lambda func inside of a new func that fits the type
			//It should be preformant enough and it gives us the required type sig for the func
			Func<TEngineScriptComponentType> compiledLambda = creationLambda.Compile() as Func<TEngineScriptComponentType>;

			return () => compiledLambda();
		}
	}
}

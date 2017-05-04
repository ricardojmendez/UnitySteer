using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;
using Testity.Common;
using Testity.EngineComponents.Unity3D;

namespace Testity.BuildProcess.Unity3D
{
	public class AddMemberInitializationMethodStep : ITestityBuildStep
	{
		private readonly ITypeRelationalMapper typeResolver;

		private readonly ITypeMemberParser typeParser;

		private readonly IInitializationExpressionBuilderProvider initExpressionBuildProvider;

		public AddMemberInitializationMethodStep(ITypeRelationalMapper mapper, ITypeMemberParser parser, IInitializationExpressionBuilderProvider provider)
        {
			typeResolver = mapper;
			typeParser = parser;
			initExpressionBuildProvider = provider;
        }

		public void Process(IClassBuilder builder, Type typeToParse)
		{
			IEnumerable<MemberInfo> serializedMemberInfos = typeParser.Parse(MemberTypes.Field | MemberTypes.Property, typeToParse);
			List<IInitializationExpression> initExpressions = new List<IInitializationExpression>(serializedMemberInfos.Count());

			foreach (MemberInfo mi in serializedMemberInfos)
			{
				//find a an experssion builder for the source -> dest type
				IInitializationExpressionBuilder expressionBuilder = initExpressionBuildProvider.FromReflectionData(typeResolver.ResolveMappedType(mi.Type()), mi.Type());

				if (expressionBuilder == null)
					continue; //this is for testing. Don't do this in the future.
				else
				{
					IInitializationExpression expression = expressionBuilder.Build(new InitializationExpressionData(mi.Name, mi.MemberType, mi.Name), 
						typeof(TestityBehaviour<>).MembersWith<ImplementationField>(MemberTypes.Field, Flags.InstanceAnyVisibility).First().Name);  //get the testity field we need to assign this too

					if (expression == null)
						throw new InvalidOperationException("Unable to build expression for init for Member: " + mi.Name + " in Type: " + mi.Type().ToString());

					initExpressions.Add(expression);
				}	
			}

			//Give the block provider the expressions we want and find the name of the field in the MonoBehaviour that must be set.
			UnityInitializationMethodImplementationProvider blockProvider =
				new UnityInitializationMethodImplementationProvider(initExpressions);
			
			//Using the default member provider and the block provider we quite complexly just built above we can add the initialization method.
			builder.AddMemberMethod(new DefaultMemberImplementationProvider(typeof(void), MemberImplementationModifier.Override | MemberImplementationModifier.Protected, "InitializeScriptComponentMemberValues"),
                blockProvider);
		}
	}
}

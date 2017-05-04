using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Testity.BuildProcess.Unity3D
{
	public static class EntryPoint
	{
		public static int Main(string[] args)
		{
			//There may be uncaught exceptions so we'll handle them all gracefully.
			//There probably is no way to recover from an exception during this process so catching is fruitless really
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			if (args.Count() == 0)
			{
				Console.WriteLine("Testity requires the dll path to parse/compile from as an argument.");
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
				return 1;
			}
			else
			{
				string dllPath = args[0];

				Task<Task<bool>> result = Task.Factory.StartNew(async () => { return await TryBuildTask(dllPath); }, TaskCreationOptions.LongRunning);
				result.Wait();
				result.Result.Wait();

				bool val = result.Result.Result;

				if (val)
					return 0;
				else
				{
					Console.WriteLine("Compilation failed.");
					Console.WriteLine("Press any key to continue...");
					Console.ReadKey();
					return 1;
				}
            }		
		}

		private static async Task<bool> TryBuildTask(string dllPath)
		{
			Assembly loadedAss = LoadAssembly(dllPath);

			IEnumerable<Type> potentialBehaviourTypes = null;
			//We assume this is a testity assembly
			//If someone called this builder then they WANT a Testity generated dll either way.
			try
			{
				//load the EngineScriptComponentTypes
				potentialBehaviourTypes = LoadBehaviourTypes(loadedAss);
			}
			catch (ReflectionTypeLoadException e)
			{
				Console.Write(e.Message + " LoaderExceptions:");
				foreach (var sube in e.LoaderExceptions)
					Console.WriteLine(sube.Message);

				Console.ReadKey();
				return false;
			}

			List<Task<ClassFile>> monobehaviourClassCompilationResults = new List<Task<ClassFile>>(potentialBehaviourTypes.Count());

			//Create the type mapper
			List<ITypeRelationalMapper> mappers = new List<ITypeRelationalMapper>();
			mappers.Add(new StringTypeRelationalMapper());
			mappers.Add(new EngineTypeRelationalMapper());
			mappers.Add(new PrimitiveTypeRelationalMapper(new UnityPrimitiveTypeExclusion()));
			mappers.Add(new ActionTypeRelationalMapper());
			mappers.Add(new DefaultTypeRelationalMapper());
			UnityBuildProcessTypeRelationalMapper chainMapper = new UnityBuildProcessTypeRelationalMapper(mappers);

			//create the InitializationExpressionBuilder
			List<IInitializationExpressionBuilderProvider> initProviders = new List<IInitializationExpressionBuilderProvider>();
			initProviders.Add(new ComponentAdapterInitializationExpressionBuilderProvider(new UnityComponentAdapterParser()));
			initProviders.Add(new SameTypeInitializationExpressionBuilderProvider());
			initProviders.Add(new DefaultInitializationExpressionBuilderProvider());

			ChainInitializationExpressionBuilderProvider chainInitProvider = new ChainInitializationExpressionBuilderProvider(initProviders);

			//Create the serializedmember type exclusion service. Indicates what types should be excluded when serializing types
			SerializedMemberStepTypeExclusionService excluder = new SerializedMemberStepTypeExclusionService()
				.AddExclusionRules(new ActionDelegateTypeExclusion(new Type[] { typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>) }));

			//TestityEventTracking service. It tracks all used TestityEvent types
			TestityGenericEventTracker tracker = new TestityGenericEventTracker();

			//Create build steps
			List<ITestityBuildStep> buildSteps = new List<ITestityBuildStep>();
			buildSteps.Add(new AddTestityBehaviourBaseClassMemberStep());
			buildSteps.Add(new AddSerializedMemberStep(chainMapper, new SerializedMemberParser(), excluder));
			buildSteps.Add(new AddTestityEventSerializedDelegateStep(new ActionTypeRelationalMapper(), tracker, new SerializedMemberParser()));
			buildSteps.Add(new AddMemberInitializationMethodStep(chainMapper, new SerializedMemberParser(), chainInitProvider));

			//Handle all the behaviour types
			//These are not the only types that need handling
			foreach (Type t in potentialBehaviourTypes)
			{
				Task<ClassFile> classCompile = Task.Factory.StartNew(() => new ClassFile(GenerateMonobehaviourClass(chainMapper, buildSteps, t), t.Name + "Script"));
				monobehaviourClassCompilationResults.Add(classCompile);
			}

			string outputPath = dllPath.TrimEnd(".dll".ToCharArray()) + @"MonoBehaviours\";

			if (!Directory.CreateDirectory(outputPath).Exists)
				throw new InvalidOperationException("Failed to create MonoBehaviour directory.");
			
			//write out all the MonoBehaviours
			while(monobehaviourClassCompilationResults.Count != 0)
			{
				Task<ClassFile> newFile = await Task.WhenAny(monobehaviourClassCompilationResults);

				WriteMonobehaviourToFile(outputPath, newFile.Result);

				//We should check for new genericTestityEvents after a class has been generated
				//This isn't that slow since it will usually be empty
				//TODO: Change this so it buffers them into another collection.
				//We want to handle them seperately and write them to a seperate folder to avoid collisions
				foreach (var kvp in tracker.GetAdditionsAndClear())
				{
					Console.WriteLine("New File: " + kvp.Key);
					Console.ReadKey();

					Task<ClassFile> classCompile = Task.Factory.StartNew(() => new ClassFile(GenerateTestityGenericEventSerializableClass(kvp.Key, kvp.Value), kvp.Key));
					monobehaviourClassCompilationResults.Add(classCompile);
				}

				//Remove the file. If you don't we're stuck in the loop forever
				monobehaviourClassCompilationResults.Remove(newFile);
			}

			return true;
        }

		private static string GenerateTestityGenericEventSerializableClass(string genericEventNewTypeName, IEnumerable<Type> genericTypeArgs)
		{
			IClassBuilder builder = new TestityClassBuilder(genericEventNewTypeName);

			return builder.ToString();
		}

		private static string GenerateMonobehaviourClass(ITypeRelationalMapper mapper, IEnumerable<ITestityBuildStep> buildSteps, Type typeToBuildFrom)
		{
			IClassBuilder builder = new TestityClassBuilder(typeToBuildFrom.Name + "Script", MemberImplementationModifier.Public | MemberImplementationModifier.Sealed);

			foreach (ITestityBuildStep step in buildSteps)
			{
				step.Process(builder, typeToBuildFrom);
			}

			return builder.ToString();
		}

		private static void WriteMonobehaviourToFile(string filePath, ClassFile classFile)
		{
			File.WriteAllText(filePath + classFile.ClassName + ".cs", classFile.ClassData);
		}

		private static IEnumerable<Type> LoadBehaviourTypes(Assembly loadedAss)
		{
			EngineScriptComponentLocator locator = new EngineScriptComponentLocator(loadedAss);

			//Should return types that are EngineScriptComponents
			return locator.LoadTypes();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(sender.ToString() + " caused exception: " + e.ExceptionObject.ToString());
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
			Environment.Exit(1);
		}

		private static Assembly LoadAssembly(string path) 
		{
			//We need a hard load due to resolving dependent assemblies.
			//Do not load as reflection only
			return Assembly.LoadFrom(path);
		}
	}
}

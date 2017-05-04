using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.Common;
using Testity.EngineComponents;
using Testity.EngineMath;

namespace Testity.BuildProcess.Unity3D.Tests
{
	[TestFixture]
	public static class ClassSerializedMemberBuildTest
	{
		public class TestSerializedClass : EngineScriptComponent
		{
			
		}

		[Test(Author = "Andrew Blakely", Description = "Tests the components together in a system that generates class files.")]
		[MaxTime(100000)]
		public static void Test_Generate_Class_With_Serialized_Fields()
		{
			//arrange
			//generate mappers
			List<ITypeRelationalMapper> mappers = new List<ITypeRelationalMapper>();
			mappers.Add(new StringTypeRelationalMapper());
			mappers.Add(new EngineTypeRelationalMapper());
			mappers.Add(new PrimitiveTypeRelationalMapper(new UnityPrimitiveTypeExclusion()));
			mappers.Add(new DefaultTypeRelationalMapper());

			Mock<ITypeExclusion> exclusionMock = new Mock<ITypeExclusion>(MockBehavior.Strict);

			//setup always false exclusion
			exclusionMock.Setup(x => x.isExcluded(It.IsAny<Type>())).Returns(false);

			TestityClassBuilder builder = new TestityClassBuilder(typeof(TestSerializedClass).Name + "Script");

			UnityBuildProcessTypeRelationalMapper chainMapper = new UnityBuildProcessTypeRelationalMapper(mappers);

			AddSerializedMemberStep buildStep = new AddSerializedMemberStep(chainMapper, new SerializedMemberParser(), exclusionMock.Object);

			buildStep.Process(builder, typeof(TestSerializedClass));

			string classCompiled = builder.Compile();

			//Assert.NotNull(null, classCompiled);
			//Assert.("dfdhh", null, classCompiled);
        }
	}
}

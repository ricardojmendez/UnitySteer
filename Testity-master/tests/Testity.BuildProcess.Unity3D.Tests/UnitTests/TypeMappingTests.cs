using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.BuildProcess;
using Testity.EngineComponents;
using Testity.EngineMath;

namespace Testity.BuildProcess.Unity3D.Tests
{
	[TestFixture]
	public static class TypeMappingTests
	{
		//a lot of these types for testing are just picked at random. Just whatever pops up in intelisense

		[Test(Author = "Andrew Blakely", Description = "Ensures mapper returns null for invalid types.", TestOf = typeof(DefaultTypeRelationalMapper))]
		[TestCase(typeof(string))]
		[TestCase(typeof(StringBuilder))]
		[TestCase(typeof(EngineScriptComponentLocator))]
		public static void Test_DefaultTypeRelationMapper_Returns_Null_On_Invalid_Types(Type t)
		{
			//arrange
			ITypeRelationalMapper mapper = new DefaultTypeRelationalMapper();

			//assert
			Assert.IsNull(mapper.ResolveMappedType(t));
        }

		[Test(Author = "Andrew Blakely", Description = "Ensures mapper returns valid type for valid inputs.", TestOf = typeof(DefaultTypeRelationalMapper))]
		[TestCase(typeof(IServiceProvider))]
		[TestCase(typeof(IEnumerable<int>))]
		public static void Test_DefaultTypeRelationMapper_Returns_Expected_Type(Type t)
		{
			//arrange
			ITypeRelationalMapper mapper = new DefaultTypeRelationalMapper();

			//act
			Type mappedType = mapper.ResolveMappedType(t);

			//assert
			//if it's an interface it should be MonoBehaviour
			Assert.AreEqual(typeof(UnityEngine.MonoBehaviour), mappedType);
		}

		[Test(Author = "Andrew Blakely", Description = "Ensures mapper returns valid type for valid inputs.", TestOf = typeof(PrimitiveTypeRelationalMapper))]
		[TestCase(typeof(int))]
		[TestCase(typeof(Int32))]
		[TestCase(typeof(ushort))]
		[TestCase(typeof(byte))]
		public static void Test_PrimitiveTypeMapper_Returns_Expected_Type(Type t)
		{
			//arrange
			ITypeRelationalMapper mapper = new PrimitiveTypeRelationalMapper(Enumerable.Empty<Type>());

			//act
			Type mappedType = mapper.ResolveMappedType(t);

			//assert
			Assert.AreEqual(t, mappedType);
		}

		[Test(Author = "Andrew Blakely", Description = "Ensures mapper returns valid type for valid inputs with exclusion.", TestOf = typeof(PrimitiveTypeRelationalMapper))]
		[TestCase(typeof(int))]
		[TestCase(typeof(Int32))]
		[TestCase(typeof(ushort))]
		[TestCase(typeof(byte))]
		public static void Test_PrimitiveTypeMapper_Returns_Expected_Type_With_Exclusions(Type t)
		{
			//arrange
			ITypeRelationalMapper mapper = new PrimitiveTypeRelationalMapper(new UnityPrimitiveTypeExclusion());

			//act
			Type mappedType = mapper.ResolveMappedType(t);

			//assert
			Assert.AreEqual(t, mappedType);
		}

		[Test(Author = "Andrew Blakely", Description = "Ensures mapper returns null for invalid types.", TestOf = typeof(PrimitiveTypeRelationalMapper))]
		[TestCase(typeof(IServiceProvider))]
		[TestCase(typeof(IEnumerable<int>))]
		[TestCase(typeof(string))] //string is not a primitve
		public static void Test_PrimitiveTypeMapper_Returns_Null_On_Invalid_Type(Type t)
		{
			//arrange
			ITypeRelationalMapper mapper = new PrimitiveTypeRelationalMapper(Enumerable.Empty<Type>());

			//act
			Type mappedType = mapper.ResolveMappedType(t);

			//assert
			Assert.IsNull(mappedType);
		}

		[Test(Author = "Andrew Blakely", Description = "Ensures mapper throws on excluded type.", TestOf = typeof(PrimitiveTypeRelationalMapper))]
		[TestCase(typeof(IntPtr))]
		[TestCase(typeof(UIntPtr))]
		public static void Test_PrimitiveTypeMapper_Throws_On_Exclusion(Type t)
		{
			//arrange
			ITypeRelationalMapper mapper = new PrimitiveTypeRelationalMapper(new UnityPrimitiveTypeExclusion());

			//act
			Assert.Throws<InvalidOperationException>(() => mapper.ResolveMappedType(t));
		}


		[Test(Author = "Andrew Blakely", Description = "Ensures mapper returns valid type for valid inputs.", TestOf = typeof(EngineTypeRelationalMapper))]
		[TestCase(typeof(IEngineGameObject), typeof(UnityEngine.GameObject))]
		[TestCase(typeof(IEngineTransform), typeof(UnityEngine.Transform))]
		[TestCase(typeof(Vector3<float>), typeof(UnityEngine.Vector3))]
		public static void Test_EngineObjectTypeRelationalMapper_Returns_Expected_Type(Type inputType, Type expectedType)
		{
			//arrange
			ITypeRelationalMapper mapper = new EngineTypeRelationalMapper();

			//act
			Type mappedType = mapper.ResolveMappedType(inputType);

			//assert
			//if it's an interface it should be MonoBehaviour
			Assert.AreEqual(expectedType, mappedType);
		}
	}
}

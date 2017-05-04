using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Testity.Common;

namespace Testity.BuildProcess.Tests
{
	[TestFixture]
	public class SerializedMemberParserTests
	{
		[Test(Author = "Andrew Blakely", Description = "Tests SerializedMemberParser type empty parsing.", TestOf = typeof(SerializedMemberParser<>))]
		[TestCase(MemberTypes.Field, typeof(object))]
		[TestCase(MemberTypes.Property, typeof(object))]
		[TestCase(MemberTypes.Property, typeof(string))]
		[TestCase(MemberTypes.Property, typeof(PropertyInfo))]
		public static void Test_SerializedMemberParser_Should_Find_No_Members(MemberTypes memberInfoType, Type typeToParse)
		{
			//arranges
			IEnumerable objects = new SerializedMemberParser().Parse(memberInfoType, typeToParse);

			//assert
			//It should be empty but non-null
			Assert.IsNotNull(objects);
			Assert.IsEmpty(objects);
		}

		public class OneFieldIsMarked
		{
			[ExposeDataMemeber]
			private string fieldValueOne;
		}

		[Test(Author = "Andrew Blakely", Description = "Tests SerializedMemberParser parsing of mock class.", TestOf = typeof(SerializedMemberParser<>))]
		public static void Test_SerializedMemberParser_Should_Find_Members()
		{
			//arrange
			IEnumerable<FieldInfo> fieldInfos = (new SerializedMemberParser()).Generic<FieldInfo>().Parse<OneFieldIsMarked>();
			IEnumerable<PropertyInfo> propertyInfos = (new SerializedMemberParser()).Generic<PropertyInfo>().Parse<OneFieldIsMarked>();

			//assert
			//Should be 1 field
			Assert.IsNotNull(fieldInfos);
			Assert.IsNotEmpty(fieldInfos);
			Assert.AreEqual(1, fieldInfos.Count());

			//Should be empty
			Assert.IsNotNull(propertyInfos);
			Assert.IsEmpty(propertyInfos);
		}

		[Test(Author = "Andrew Blakely", Description = "Tests SerializedMemberParser cacheing.", TestOf = typeof(SerializedMemberParser<>))]
		public static void Test_SerializedMemberParser_Valid_Caching()
		{
			//arrange
			IEnumerable<FieldInfo> fieldInfos1 = (new SerializedMemberParser<FieldInfo>()).Parse<OneFieldIsMarked>();
			IEnumerable<FieldInfo> fieldInfos2 = (new SerializedMemberParser<FieldInfo>()).Parse<OneFieldIsMarked>();

			//assert
			//Should be 1 field
			Assert.IsNotNull(fieldInfos1);
			Assert.IsNotEmpty(fieldInfos1);
			Assert.AreEqual(1, fieldInfos1.Count());

			//Should be filled with 1 field also
			Assert.IsNotNull(fieldInfos2);
			Assert.IsNotEmpty(fieldInfos2);
			Assert.AreEqual(1, fieldInfos2.Count());

			//Check equivalence
			Assert.IsTrue(fieldInfos1.Intersect(fieldInfos2).Count() == 1);
			Assert.AreEqual(fieldInfos1.First(), fieldInfos2.First());
		}
	}
}

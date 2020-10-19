using System;
using NUnit.Framework;

namespace Haukcode.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscTimeTagTest and is intended
    ///to contain all OscTimeTagTest Unit Tests
    ///</summary>
	[TestFixture]
	public class OscTimeTagTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for OscTimeTag Constructor
		///</summary>
		[Test]
		public void OscTimeTagConstructorTest()
		{
			DateTime expected = new DateTime(632413223390120000, DateTimeKind.Utc);
			ulong value = 14236589681638796952; 
			OscTimeTag target = new OscTimeTag(value);

			DateTime datetime = target.ToDataTime();

			string valueString = datetime.ToString("dd/MM/yyyy HH:mm:ss") + " " + datetime.Millisecond;
			string expectedString = expected.ToString("dd/MM/yyyy HH:mm:ss") + " " + datetime.Millisecond;

			Assert.AreEqual(expectedString, valueString, "Date resolved to '{0}'", valueString);
		}

		/// <summary>
		///A test for FromDataTime
		///</summary>
		[Test]
		public void FromDataTimeTest()
		{
			DateTime datetime = new DateTime(632413223390120000, DateTimeKind.Utc);
			OscTimeTag expected = new OscTimeTag(14236589681638796952);
			OscTimeTag actual;
			actual = OscTimeTag.FromDataTime(datetime);
			Assert.IsTrue((expected.Value <= actual.Value + 1) && (expected.Value >= actual.Value - 1));			
		}

		/// <summary>
		///A test for ToDataTime
		///</summary>
		[Test]
		public void ToDataTimeTest()
		{
			OscTimeTag target = new OscTimeTag(14236589681638796952);
			DateTime expected = new DateTime(632413223390120000, DateTimeKind.Utc);
			DateTime actual;
			actual = target.ToDataTime();
			Assert.AreEqual(expected, actual);
		}
	}
}

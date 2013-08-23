using Rug.Osc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscBundleTest and is intended
    ///to contain all OscBundleTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OscBundleTest
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
		///A test for OscBundle Constructor
		///</summary>
		[TestMethod()]
		public void OscBundleConstructorTest()
		{
			OscTimeTag timestamp = new OscTimeTag(14236589681638796952); 
			OscMessage[] messages = new OscMessage[] { UnitTestHelper.Message_Array_Ints(),  UnitTestHelper.Message_Array_Ints() }; 
			OscBundle target = new OscBundle(timestamp, messages);

			Assert.AreEqual(timestamp, target.Timestamp);
			UnitTestHelper.AreEqual(messages, target.ToArray()); 
		}

		/* 
		/// <summary>
		///A test for IsBundle
		///</summary>
		[TestMethod()]
		public void IsBundleTest()
		{
			byte[] bytes = null; 
			int index = 0; 
			int count = 0; 
			bool expected = false; 
			bool actual;
			actual = OscBundle.IsBundle(bytes, index, count);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
		*/ 
		/// <summary>
		///A test for Read
		///</summary>
		[TestMethod()]
		public void ReadTest()
		{
			OscTimeTag timestamp = new OscTimeTag(14236589681638796952);
			OscMessage[] messages = new OscMessage[] { UnitTestHelper.Message_Array_Ints(), UnitTestHelper.Message_Array_Ints() };
			OscBundle expected = new OscBundle(timestamp, messages);

			byte[] bytes = expected.ToByteArray();	
			int index = 0;
			int count = bytes.Length;			
			OscBundle actual;
			actual = OscBundle.Read(bytes, index, count);
			UnitTestHelper.AreEqual(expected, actual);

			Assert.IsTrue(actual.Equals(expected)); 
		}

		/* 
		/// <summary>
		///A test for ToByteArray
		///</summary>
		[TestMethod()]
		public void ToByteArrayTest()
		{
			OscBundle target = null; // TODO: Initialize to an appropriate value
			byte[] expected = null; // TODO: Initialize to an appropriate value
			byte[] actual;
			actual = target.ToByteArray();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
		
		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			OscBundle target = null; // TODO: Initialize to an appropriate value
			string expected = string.Empty; // TODO: Initialize to an appropriate value
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}
		
		
		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest()
		{
			OscBundle target = null; // TODO: Initialize to an appropriate value
			byte[] data = null; // TODO: Initialize to an appropriate value
			int index = 0; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.Write(data, index);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		}

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest1()
		{
			OscBundle target = null; // TODO: Initialize to an appropriate value
			byte[] data = null; // TODO: Initialize to an appropriate value
			int expected = 0; // TODO: Initialize to an appropriate value
			int actual;
			actual = target.Write(data);
			Assert.AreEqual(expected, actual);
			Assert.Inconclusive("Verify the correctness of this test method.");
		} 
		*/

		/// <summary>
		///A test for TryParse
		///</summary>
		[TestMethod()]
		public void TryParseTest_Good()
		{
			bool expected = true;
			foreach (string str in UnitTestHelper.Bundles_Good)
			{
				OscBundle bundle = null; 		

				bool actual;
				actual = OscBundle.TryParse(str, out bundle);
				Assert.AreEqual(expected, actual, "While parsing good bundle '{0}'", str);
			}
		}

		/// <summary>
		///A test for TryParse
		///</summary>
		[TestMethod()]
		public void TryParseTest_Bad()
		{
			bool expected = false;
			foreach (string str in UnitTestHelper.Bundles_Bad)
			{
				OscBundle bundle = null; 		

				bool actual;
				actual = OscBundle.TryParse(str, out bundle);
				Assert.AreEqual(expected, actual, "While parsing bad bundle '{0}'", str);
			}
		}
	}
}

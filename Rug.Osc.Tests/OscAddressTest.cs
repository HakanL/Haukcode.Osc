using Rug.Osc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscAddressTest and is intended
    ///to contain all OscAddressTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OscAddressTest
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

		#region Match 

		/// <summary>
		///A test for IsMatch
		///</summary>
		[TestMethod()]
		public void IsMatchTest_LiteralMatch()
		{
			string addressPattern = "/container_A/method_A";
			string address = "/container_A/method_A";

			Assert.IsTrue(OscAddress.IsMatch(addressPattern, address)); 		
		}

		/// <summary>
		///A test for IsMatch
		///</summary>
		[TestMethod()]
		public void IsMatchTest_LiteralMissmatch1()
		{
			string addressPattern = "/container_A/method_A";
			string address = "/container_A/method_B";

			Assert.IsFalse(OscAddress.IsMatch(addressPattern, address));
		}

		/// <summary>
		///A test for IsMatch
		///</summary>
		[TestMethod()]
		public void IsMatchTest_LiteralMissmatch2()
		{
			string addressPattern = "/container_A/method_A";
			string address = "/container_B/method_A";

			Assert.IsFalse(OscAddress.IsMatch(addressPattern, address));
		}

		#endregion 

		#region Validate Address 

		/// <summary>
		///A test for IsValidAddressPattern
		///</summary>
		[TestMethod()]
		public void IsValidAddressPatternTest_Good()
		{
			for (int i = 0; i < UnitTestHelper.Good_AddressPatterns.Length; i++)
			{
				string address = UnitTestHelper.Good_AddressPatterns[i];

				bool result = OscAddress.IsValidAddressPattern(address);

				Assert.IsTrue(result, String.Format("Failed to validate address pattern {0} '{1}'", i, address));
			}
		}

		/// <summary>
		///A test for IsValidAddressPattern
		///</summary>
		[TestMethod()]
		public void IsValidAddressPatternTest_Bad()
		{
			for (int i = 0; i < UnitTestHelper.Bad_AddressPatterns.Length; i++)
			{
				string address = UnitTestHelper.Bad_AddressPatterns[i];

				bool result = OscAddress.IsValidAddressPattern(address);

				Assert.IsFalse(result, String.Format("Incorrectly validated address pattern {0} '{1}'", i, address));
			}
		}

		#endregion

		#region Parse Address

		/// <summary>
		///A test for Constructor
		///</summary>
		[TestMethod()]
		public void OscAddress_Constructor_Good()
		{
			for (int i = 0; i < UnitTestHelper.Good_AddressPatterns.Length; i++)
			{
				string address = UnitTestHelper.Good_AddressPatterns[i];

				OscAddress result = new OscAddress(address);

				Assert.AreEqual(address, result.ToString_Rebuild(), String.Format("Failed to parse address pattern {0} '{1}'", i, address));
			}
		}

		#endregion 
				
		#region Parse Address

		/// <summary>
		///A test for Constructor
		///</summary>
		[TestMethod()]
		public void AddressPatternMatches()
		{
			for (int i = 0; i < UnitTestHelper.Good_AddressPatterns.Length; i++)
			{
				string pattern = UnitTestHelper.Good_AddressPatterns[i];
				string address = UnitTestHelper.Good_AddressPatternMatches[i];

				OscAddress target = new OscAddress(pattern);

				bool result = target.Match(address);

				Assert.IsTrue(result, String.Format("Failed to match address pattern {0} '{1}' to '{2}'", i, pattern, address));
			}
		}

		#endregion 
		
	}
}

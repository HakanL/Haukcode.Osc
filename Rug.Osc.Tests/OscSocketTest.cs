using Rug.Osc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscSocketTest and is intended
    ///to contain all OscSocketTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OscSocketTest
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
		///A test for IsMulticastAddress
		///</summary>
		[TestMethod()]
		public void IsMulticastAddressTest_True()
		{
			IPAddress address = IPAddress.Parse("239.0.0.222"); 
			bool expected = true;
			bool actual;
			actual = OscSocket.IsMulticastAddress(address);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for IsMulticastAddress
		///</summary>
		[TestMethod()]
		public void IsMulticastAddressTest_False()
		{
			IPAddress address = IPAddress.Parse("127.0.0.1");
			bool expected = false;
			bool actual;
			actual = OscSocket.IsMulticastAddress(address);
			Assert.AreEqual(expected, actual);
		}
	}
}

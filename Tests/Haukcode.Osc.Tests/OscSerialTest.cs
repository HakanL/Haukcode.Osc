using Haukcode.Osc;
using NUnit.Framework;
using System;
using System.IO.Ports;

namespace Haukcode.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscSerialTest and is intended
    ///to contain all OscSerialTest Unit Tests
    ///</summary>
	[TestFixture]
	public class OscSerialTest
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

        /*
		/// <summary>
		///A test for Send
		///</summary>
		[Test]
		public void SendTest()
		{
			string portName = string.Empty; // TODO: Initialize to an appropriate value
			int baudRate = 0; // TODO: Initialize to an appropriate value
			bool rtsCtsEnabled = false; // TODO: Initialize to an appropriate value
			Parity parity = new Parity(); // TODO: Initialize to an appropriate value
			int dataBits = 0; // TODO: Initialize to an appropriate value
			StopBits stopBits = new StopBits(); // TODO: Initialize to an appropriate value
			int slipBufferSize = 0; // TODO: Initialize to an appropriate value
			OscSerial target = new OscSerial(portName, baudRate, rtsCtsEnabled, parity, dataBits, stopBits, slipBufferSize); // TODO: Initialize to an appropriate value
			OscPacket packet = null; // TODO: Initialize to an appropriate value
			target.Send(packet);
			Assert.Inconclusive("A method that does not return a value cannot be verified.");
		}
        */ 
	}
}

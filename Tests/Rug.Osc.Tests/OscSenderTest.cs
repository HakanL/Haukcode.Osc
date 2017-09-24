using System;
using System.Net;
using NUnit.Framework;

namespace Rug.Osc.Tests
{       
    /// <summary>
    ///This is a test class for OscSenderTest and is intended
    ///to contain all OscSenderTest Unit Tests
    ///</summary>
	[TestFixture]
	public class OscSenderTest
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
		///A test for Send
		///</summary>
		[Test]
		public void SendTest()
		{
			try
			{
				// This is the ip address we are going to send to
				IPAddress address = IPAddress.Loopback;

				// This is the port we are going to send to 
				int port = 12345;

				// Create a new sender instance
				using (OscSender sender = new OscSender(address, port))
				{
					// Connect the sender socket  
					sender.Connect();

					// Send a new message
					sender.Send(new OscMessage("/test", 1, 2, 3, 4));
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}
	}
}

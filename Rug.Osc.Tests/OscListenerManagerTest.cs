using Rug.Osc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Rug.Osc.Tests
{       
    /// <summary>
    ///This is a test class for OscListenerManagerTest and is intended
    ///to contain all OscListenerManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OscListenerManagerTest
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
		///A test for OscListenerManager Constructor
		///</summary>
		[TestMethod()]
		public void OscListenerManagerConstructorTest()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{

				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Attach
		///</summary>
		[TestMethod()]
		public void AttachTest()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{
					string address = "/test";

					OscMessageEvent @event = new OscMessageEvent((OscMessage message) => { Debug.WriteLine(message.ToString()); });

					target.Attach(address, @event);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Detach
		///</summary>
		[TestMethod()]
		public void DetachTest()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{
					string address = "/test";

					OscMessageEvent @event = new OscMessageEvent((OscMessage message) => { Debug.WriteLine(message.ToString()); });

					target.Attach(address, @event);

					target.Detach(address, @event);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Invoke
		///</summary>
		[TestMethod()]
		public void InvokeTest_Good()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{
					string address = "/test";
					OscMessageEvent @event = new OscMessageEvent((OscMessage msg) => { Debug.WriteLine(msg.ToString()); });
					target.Attach(address, @event);

					OscMessage message = new OscMessage("/test");
					bool expected = true;
					bool actual;
					
					actual = target.Invoke(message);

					Assert.AreEqual(expected, actual);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Invoke
		///</summary>
		[TestMethod()]
		public void InvokeTest_Bad()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{
					string address = "/test";
					OscMessageEvent @event = new OscMessageEvent((OscMessage msg) => { Debug.WriteLine(msg.ToString()); });
					target.Attach(address, @event);

					OscMessage message = new OscMessage("/foo");
					bool expected = false;
					bool actual;

					actual = target.Invoke(message);

					Assert.AreEqual(expected, actual);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Invoke
		///</summary>
		[TestMethod()]
		public void InvokeTest1()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{
					string address = "/test";
					OscMessageEvent @event = new OscMessageEvent((OscMessage msg) => { Debug.WriteLine(msg.ToString()); });
					target.Attach(address, @event);

					OscBundle bundle = new OscBundle(DateTime.Now, new OscMessage("/test"));
					bool expected = true;
					bool actual;

					actual = target.Invoke(bundle);

					Assert.AreEqual(expected, actual);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		/// <summary>
		///A test for Invoke
		///</summary>
		[TestMethod()]
		public void InvokeTest1_Bad()
		{
			try
			{
				using (OscListenerManager target = new OscListenerManager())
				{
					string address = "/test";
					OscMessageEvent @event = new OscMessageEvent((OscMessage msg) => { Debug.WriteLine(msg.ToString()); });
					target.Attach(address, @event);

					OscBundle bundle = new OscBundle(DateTime.Now, new OscMessage("/foo"));
					bool expected = false;
					bool actual;

					actual = target.Invoke(bundle);

					Assert.AreEqual(expected, actual);
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}
	}
}

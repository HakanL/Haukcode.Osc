using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Rug.Osc.Tests
{       
    /// <summary>
    ///This is a test class for OscListenerManagerTest and is intended
    ///to contain all OscListenerManagerTest Unit Tests
    ///</summary>
	[TestFixture]
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
		[Test]
		public void OscListenerManagerConstructorTest()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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
		[Test]
		public void AttachTest()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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
		[Test]
		public void DetachTest()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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
		[Test]
		public void InvokeTest_Good()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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
		[Test]
		public void InvokeTest_Bad()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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
		[Test]
		public void InvokeTest1()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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
		[Test]
		public void InvokeTest1_Bad()
		{
			try
			{
				using (OscAddressManager target = new OscAddressManager())
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

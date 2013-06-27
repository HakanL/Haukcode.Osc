using Rug.Osc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscMessageTest and is intended
    ///to contain all OscMessageTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OscMessageTest
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

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_EmptyArgs()
		{
			string address = "/test";

			OscMessage_Accessor target = new OscMessage_Accessor(address);

			Assert.AreEqual(target.Address, address);
			Assert.IsTrue(target.IsEmpty);
			Assert.IsNotNull(target.Arguments); 
			Assert.AreEqual(target.Arguments.Length, 0);
			Assert.AreEqual(target.MessageSize, 12); 		
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_SingleArg_Int()
		{
			string address = "/test";
			int value = 42;
			OscMessage_Accessor target = new OscMessage_Accessor(address, value);

			Assert.AreEqual(target.Address, address);
			Assert.IsFalse(target.IsEmpty);
			Assert.IsNotNull(target.Arguments);
			Assert.AreEqual(target.Arguments.Length, 1);
			Assert.AreEqual(target.Arguments[0], value);
			Assert.AreEqual(target.MessageSize, 16);
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_SingleArg_Float()
		{
			string address = "/test";
			float value = 42;
			OscMessage_Accessor target = new OscMessage_Accessor(address, value);

			Assert.AreEqual(target.Address, address);
			Assert.IsFalse(target.IsEmpty);
			Assert.IsNotNull(target.Arguments);
			Assert.AreEqual(target.Arguments.Length, 1);
			Assert.AreEqual(target.Arguments[0], value);
			Assert.AreEqual(target.MessageSize, 16);
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_SingleArg_String()
		{
			string address = "/test";
			string value = "42";
			OscMessage_Accessor target = new OscMessage_Accessor(address, value);

			Assert.AreEqual(target.Address, address);
			Assert.IsFalse(target.IsEmpty);
			Assert.IsNotNull(target.Arguments);
			Assert.AreEqual(target.Arguments.Length, 1);
			Assert.AreEqual(target.Arguments[0], value);
			Assert.AreEqual(target.MessageSize, 16);
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_SingleArg_Blob()
		{
			string address = "/test";
			byte[] value = new byte[] { 4, 2 };

			OscMessage_Accessor target = new OscMessage_Accessor(address, value);

			Assert.AreEqual(target.Address, address);
			Assert.IsFalse(target.IsEmpty);
			Assert.IsNotNull(target.Arguments);
			Assert.AreEqual(target.Arguments.Length, 1);
			Assert.AreEqual(target.Arguments[0], value);
			Assert.AreEqual(target.MessageSize, 20);
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_NoAddress()
		{
			string address = null;

			try
			{
				OscMessage_Accessor target = new OscMessage_Accessor(address);

				Assert.Fail(); 
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(ArgumentNullException));
			}
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_BadArg()
		{
			string address = "/test";
			long value = 42;

			try
			{
				OscMessage_Accessor target = new OscMessage_Accessor(address, value);

				Assert.Fail();
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(ArgumentException));
			}
		}

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_NullArg()
		{
			string address = "/test";			

			try
			{
				OscMessage_Accessor target = new OscMessage_Accessor(address, null);

				Assert.Fail();
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(ArgumentException));
			}
		}

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest1()
		{			
			string address = "/test";
			int value = 42;

			OscMessage target = new OscMessage(address, value);

			byte[] data = new byte[24];
			byte[] expectedData = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'i', 0, 0, 
				// value
				0, 0, 0, 0x2A
			}; 
			
			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);

			for (int i = 0; i < actual; i++)
			{
				Assert.AreEqual(expectedData[i], data[i]); 
			}
		}


		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest2()
		{
			string address = "/test";
			int value = 42;

			OscMessage target = new OscMessage(address, value);

			byte[] expectedData = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'i', 0, 0, 
				// value
				0, 0, 0, 0x2A
			};

			int actual;
			byte[] data; 
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			Assert.AreEqual(expectedData.Length, data.Length);

			for (int i = 0; i < actual; i++)
			{
				Assert.AreEqual(expectedData[i], data[i]);
			}
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest()
		{
			string address = "/test";
			int value = 42;

			OscMessage expected = new OscMessage(address, value);

			byte[] bytes = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'i', 0, 0, 
				// value
				0, 0, 0, 0x2A
			};

			int count = bytes.Length; 			
			OscMessage actual;
			
			actual = OscMessage.Parse(bytes, count);

			Assert.AreEqual(expected.Arguments.Length, actual.Arguments.Length);

			for (int i = 0; i < actual.Arguments.Length; i++)
			{
				Assert.AreEqual(expected.Arguments[i], actual.Arguments[i]);
			}
		}

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest()
		{
			string address = "/test";
			int value = 42;

			OscMessage target = new OscMessage(address, value);

			Assert.AreEqual(target.MessageSize, 16); 
		}
	}
}

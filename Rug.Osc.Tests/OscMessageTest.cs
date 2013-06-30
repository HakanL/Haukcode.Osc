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

		#region Constructor

		/// <summary>
		///A test for OscMessage Constructor
		///</summary>
		[TestMethod()]
		[DeploymentItem("Rug.Osc.dll")]
		public void OscMessageConstructorTest_EmptyArgs()
		{
			string address = "/test";

			OscMessage target = new OscMessage(address);

			UnitTestHelper.AreEqual(target, address, 8); 
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
			OscMessage target = new OscMessage(address, value);

			UnitTestHelper.AreEqual(target, address, 16, value);
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
			OscMessage target = new OscMessage(address, value);

			UnitTestHelper.AreEqual(target, address, 16, value);
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
			OscMessage target = new OscMessage(address, value);

			UnitTestHelper.AreEqual(target, address, 16, value);
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

			OscMessage target = new OscMessage(address, value);

			UnitTestHelper.AreEqual(target, address, 20, value);
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
				OscMessage target = new OscMessage(address);

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
				OscMessage target = new OscMessage(address, value);

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
				OscMessage target = new OscMessage(address, null);

				Assert.Fail();
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, typeof(ArgumentException));
			}
		}

		#endregion

		#region Int

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Int_1()
		{
			OscMessage target = UnitTestHelper.Message_Int();

			byte[] data = new byte[UnitTestHelper.MessageBody_Int.Length];
			byte[] expectedData = UnitTestHelper.MessageBody_Int; 
			
			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}


		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Int_2()
		{
			OscMessage target = UnitTestHelper.Message_Int();
			byte[] expectedData = UnitTestHelper.MessageBody_Int; 

			int actual;
			byte[] data; 
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest_Int()
		{
			OscMessage expected = UnitTestHelper.Message_Int();
			byte[] bytes = UnitTestHelper.MessageBody_Int; 

			int count = bytes.Length; 			
			OscMessage actual;
			
			actual = OscMessage.Parse(bytes, count);

			UnitTestHelper.AreEqual(expected, actual);
		}		

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest_Int()
		{
			OscMessage target = UnitTestHelper.Message_Int();
			Assert.AreEqual(target.MessageSize, UnitTestHelper.MessageBody_Int.Length);
		}

		#endregion

		#region Float

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Float_1()
		{
			OscMessage target = UnitTestHelper.Message_Float();

			byte[] data = new byte[UnitTestHelper.MessageBody_Float.Length];
			byte[] expectedData = UnitTestHelper.MessageBody_Float;

			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}


		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Float_2()
		{
			OscMessage target = UnitTestHelper.Message_Float();
			byte[] expectedData = UnitTestHelper.MessageBody_Float;

			int actual;
			byte[] data;
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest_Float()
		{
			OscMessage expected = UnitTestHelper.Message_Float();
			byte[] bytes = UnitTestHelper.MessageBody_Float;

			int count = bytes.Length;
			OscMessage actual;

			actual = OscMessage.Parse(bytes, count);

			UnitTestHelper.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest_Float()
		{
			OscMessage target = UnitTestHelper.Message_Float();
			Assert.AreEqual(target.MessageSize, UnitTestHelper.MessageBody_Float.Length);
		}

		#endregion

		#region String

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_String_1()
		{
			OscMessage target = UnitTestHelper.Message_String();

			byte[] data = new byte[UnitTestHelper.MessageBody_String.Length];
			byte[] expectedData = UnitTestHelper.MessageBody_String;

			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_String_2()
		{
			OscMessage target = UnitTestHelper.Message_String();
			byte[] expectedData = UnitTestHelper.MessageBody_String;

			int actual;
			byte[] data;
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest_String()
		{
			OscMessage expected = UnitTestHelper.Message_String();
			byte[] bytes = UnitTestHelper.MessageBody_String;

			int count = bytes.Length;
			OscMessage actual;

			actual = OscMessage.Parse(bytes, count);

			UnitTestHelper.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest_String()
		{
			OscMessage target = UnitTestHelper.Message_String();
			Assert.AreEqual(target.MessageSize, UnitTestHelper.MessageBody_String.Length);
		}

		#endregion

		#region Blob

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Blob_1()
		{
			OscMessage target = UnitTestHelper.Message_Blob();

			byte[] data = new byte[UnitTestHelper.MessageBody_Blob.Length];
			byte[] expectedData = UnitTestHelper.MessageBody_Blob;

			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Blob_2()
		{
			OscMessage target = UnitTestHelper.Message_Blob();
			byte[] expectedData = UnitTestHelper.MessageBody_Blob;

			int actual;
			byte[] data;
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest_Blob()
		{
			OscMessage expected = UnitTestHelper.Message_Blob();
			byte[] bytes = UnitTestHelper.MessageBody_Blob;

			int count = bytes.Length;
			OscMessage actual;

			actual = OscMessage.Parse(bytes, count);

			UnitTestHelper.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest_Blob()
		{
			OscMessage target = UnitTestHelper.Message_Blob();
			Assert.AreEqual(target.MessageSize, UnitTestHelper.MessageBody_Blob.Length);
		}

		#endregion

		#region Multiple Args

		#region Float 2

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Float2()
		{
			OscMessage target = UnitTestHelper.Message_Float2();

			byte[] data = new byte[UnitTestHelper.MessageBody_Float2.Length];
			byte[] expectedData = UnitTestHelper.MessageBody_Float2;

			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}


		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Float2_2()
		{
			OscMessage target = UnitTestHelper.Message_Float2();
			byte[] expectedData = UnitTestHelper.MessageBody_Float2;

			int actual;
			byte[] data;
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest_Float2()
		{
			OscMessage expected = UnitTestHelper.Message_Float2();
			byte[] bytes = UnitTestHelper.MessageBody_Float2;

			int count = bytes.Length;
			OscMessage actual;

			actual = OscMessage.Parse(bytes, count);

			UnitTestHelper.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest_Float2()
		{
			OscMessage target = UnitTestHelper.Message_Float2();
			Assert.AreEqual(target.MessageSize, UnitTestHelper.MessageBody_Float2.Length);
		}

		#endregion

		#region Float 3

		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Float3()
		{
			OscMessage target = UnitTestHelper.Message_Float3();

			byte[] data = new byte[UnitTestHelper.MessageBody_Float3.Length];
			byte[] expectedData = UnitTestHelper.MessageBody_Float3;

			int actual;
			actual = target.GetDatagram(data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}


		/// <summary>
		///A test for GetDatagram
		///</summary>
		[TestMethod()]
		public void GetDatagramTest_Float3_2()
		{
			OscMessage target = UnitTestHelper.Message_Float3();
			byte[] expectedData = UnitTestHelper.MessageBody_Float3;

			int actual;
			byte[] data;
			actual = target.GetDatagram(out data);

			Assert.AreEqual(expectedData.Length, actual);
			UnitTestHelper.AreEqual(expectedData, data);
		}

		/// <summary>
		///A test for Parse
		///</summary>
		[TestMethod()]
		public void ParseTest_Float3()
		{
			OscMessage expected = UnitTestHelper.Message_Float3();
			byte[] bytes = UnitTestHelper.MessageBody_Float3;

			int count = bytes.Length;
			OscMessage actual;

			actual = OscMessage.Parse(bytes, count);

			UnitTestHelper.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for MessageSize
		///</summary>
		[TestMethod()]
		public void MessageSizeTest_Float3()
		{
			OscMessage target = UnitTestHelper.Message_Float3();
			Assert.AreEqual(target.MessageSize, UnitTestHelper.MessageBody_Float3.Length);
		}

		#endregion

		#endregion

		#region Badly Formed Message

		[TestMethod()]
		public void BadlyFormedMessage_PacketLength()
		{		
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_PacketLength;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.InvalidSegmentLength); 
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_Address1()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_Address1;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.MissingAddress);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_Address2()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_Address2;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.MissingAddress);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_MissingComma()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_MissingComma;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.MissingComma);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_MissingTypeTag()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_MissingTypeTag;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.MissingTypeTag);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_MissingArgs()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_MissingArgs;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.InvalidSegmentLength);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_UnknownArguemntType()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_UnknownArguemntType;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.UnknownArguemntType);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_ErrorParsingBlob()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_ErrorParsingBlob;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.ErrorParsingBlob);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_ErrorParsingBlob2()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_ErrorParsingBlob2;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.ErrorParsingBlob);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}	

		[TestMethod()]
		public void BadlyFormedMessage_ErrorParsingString()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_ErrorParsingString;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.ErrorParsingString);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_ErrorParsingString2()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_ErrorParsingString2;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.ErrorParsingString);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_ErrorParsingInt()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_ErrorParsingInt;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.ErrorParsingInt);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		[TestMethod()]
		public void BadlyFormedMessage_ErrorParsingFloat()
		{
			try
			{
				byte[] data = UnitTestHelper.BadlyFormedMessage_ErrorParsingFloat;

				OscMessage actual = OscMessage.Parse(data, data.Length);

				Assert.AreEqual(actual.Error, OscMessageError.ErrorParsingFloat);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.Message);
			}
		}

		#endregion

		// all non-errors from the spec
	}
}

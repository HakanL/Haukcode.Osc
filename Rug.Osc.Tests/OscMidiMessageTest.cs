using NUnit.Framework;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscMidiMessageTest and is intended
    ///to contain all OscMidiMessageTest Unit Tests
    ///</summary>
	[TestFixture]
	public class OscMidiMessageTest
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
		///A test for OscMidiMessage Constructor
		///</summary>
		[Test]
		public void OscMidiMessageConstructorTest()
		{
			OscMidiMessage expected = new OscMidiMessage(0x03962200);

			byte portID = 3;
			OscMidiMessageType type = OscMidiMessageType.NoteOn;			
			byte channel = 6; 
			byte data1 = 34; 
			OscMidiMessage target = new OscMidiMessage(portID, type, channel, data1);

			Assert.AreEqual(expected, target);
			Assert.AreEqual(target.PortID, portID);
			Assert.AreEqual(target.Channel, channel);
			Assert.AreEqual(target.MessageType, type);
			Assert.AreEqual(target.Data1, data1);
			Assert.AreEqual(target.Data2, 0);
		}

		/// <summary>
		///A test for OscMidiMessage Constructor
		///</summary>
		[Test]
		public void OscMidiMessageConstructorTest1()
		{
			OscMidiMessage expected = new OscMidiMessage(0x03F35626);

			byte portID = 3; 
			OscMidiSystemMessageType type = OscMidiSystemMessageType.SongSelect; 
			ushort value = 0x1356; 

			OscMidiMessage target = new OscMidiMessage(portID, type, value);

			Assert.AreEqual(expected, target);
			Assert.AreEqual(target.PortID, portID);
			Assert.AreEqual(target.MessageType, OscMidiMessageType.SystemExclusive);
			Assert.AreEqual(target.SystemMessageType, type);
			Assert.AreEqual(target.Data14BitValue, value);
		}

		/// <summary>
		///A test for OscMidiMessage Constructor
		///</summary>
		[Test]
		public void OscMidiMessageConstructorTest2()
		{
			uint value = 0x03F35626; 
			OscMidiMessage target = new OscMidiMessage(value);

			byte portID = 3; 
			OscMidiSystemMessageType type = OscMidiSystemMessageType.SongSelect;
			ushort data14BitValue = 0x1356;

			OscMidiMessage expected = new OscMidiMessage(portID, type, data14BitValue);

			Assert.AreEqual(expected, target);
			Assert.AreEqual(target.PortID, portID);
			Assert.AreEqual(target.MessageType, OscMidiMessageType.SystemExclusive);
			Assert.AreEqual(target.SystemMessageType, type);
			Assert.AreEqual(target.Data14BitValue, data14BitValue);
			Assert.AreEqual(target.Data1, 0x56);
			Assert.AreEqual(target.Data2, 0x26);

		}

		/// <summary>
		///A test for Equals
		///</summary>
		[Test]
		public void EqualsTest()
		{
			OscMidiMessage target = new OscMidiMessage(0x03F35626);
			uint obj = 0x03F35626; 
			bool expected = true; 
			bool actual;
			actual = target.Equals(obj);
			Assert.AreEqual(expected, actual);			
		}


		/// <summary>
		///A test for Equals
		///</summary>
		[Test]
		public void EqualsTest2()
		{
			OscMidiMessage target = new OscMidiMessage(0x03F35626);
			uint obj = 0x0832626;
			bool expected = false;
			bool actual;
			actual = target.Equals(obj);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for GetHashCode
		///</summary>
		[Test]
		public void GetHashCodeTest()
		{
			OscMidiMessage target = new OscMidiMessage(0x03F35626);
			int expected = 0x03F35626;
			int actual;
			actual = target.GetHashCode();
			Assert.AreEqual(expected, actual);			
		}

		/// <summary>
		///A test for Data14BitValue
		///</summary>
		[Test]
		public void Data14BitValueTest()
		{
			ushort expected = 0x1356;
			OscMidiMessage target = new OscMidiMessage(0x03F35626);
			ushort actual;
			actual = target.Data14BitValue;
			Assert.AreEqual(expected, actual);	
		}
	}
}

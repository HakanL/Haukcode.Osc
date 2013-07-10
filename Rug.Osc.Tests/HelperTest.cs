using Rug.Osc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for HelperTest and is intended
    ///to contain all HelperTest Unit Tests
    ///</summary>
	[TestClass()]
	public class HelperTest
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

		#region Float

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_Float()
		{
			byte[] expected = new byte[] { 0x41, 0xCA, 0x00, 0x00 };
			float value = 25.25F;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{				
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length); 
			}
		}

		/// <summary>
		///A test for ReadSingle
		///</summary>
		[TestMethod()]
		public void ReadSingleTest()
		{
			byte[] data = new byte[] { 0x41, 0xCA, 0x00, 0x00 };
			float expected = 25.25F;

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream))
			{				
				float actual;
				actual = Helper.ReadSingle(reader);
				Assert.AreEqual(expected, actual);
			}
		}

		#endregion

		#region Int32

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_Int32()
		{
			byte[] expected = new byte[] { 0xAB, 0xCD, 0xEF, 0x42 };
			int value = unchecked((int)0xABCDEF42);

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length);
			}
		}

		/// <summary>
		///A test for ReadInt32
		///</summary>
		[TestMethod()]
		public void ReadInt32Test()
		{
			byte[] data = new byte[] { 0xAB, 0xCD, 0xEF, 0x42 };
			int expected = unchecked((int)0xABCDEF42);

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				int actual;
				actual = Helper.ReadInt32(reader);
				Assert.AreEqual(expected, actual);
			}
		} 
		#endregion

		#region UInt32

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_UInt32()
		{
			byte[] expected = new byte[] { 0xAB, 0xCD, 0xEF, 0xF2 };
			uint value = 0xABCDEFF2;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length);
			}
		}

		/// <summary>
		///A test for ReadUInt32
		///</summary>
		[TestMethod()]
		public void ReadUInt32Test()
		{
			byte[] data = new byte[] { 0xAB, 0xCD, 0xEF, 0xF2 };
			uint expected = 0xABCDEFF2;

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				uint actual;
				actual = Helper.ReadUInt32(reader);
				Assert.AreEqual(expected, actual);
			}
		} 
		#endregion

		#region Int64

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_Int64()
		{
			byte[] expected = new byte[] { 0xA1, 0xC2, 0xE3, 0xF4, 0xA5, 0xC6, 0xE7, 0xF8 };
			long value = unchecked((long)0xA1C2E3F4A5C6E7F8);

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length);
			}
		}

		/// <summary>
		///A test for ReadInt64
		///</summary>
		[TestMethod()]
		public void ReadInt64Test()
		{
			byte[] data = new byte[] { 0xA1, 0xC2, 0xE3, 0xF4, 0xA5, 0xC6, 0xE7, 0xF8 };
			long expected = unchecked((long)0xA1C2E3F4A5C6E7F8);

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				long actual;
				actual = Helper.ReadInt64(reader);
				Assert.AreEqual(expected, actual);
			}
		} 

		#endregion

		#region UInt64

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_UInt64()
		{
			byte[] expected = new byte[] { 0xA1, 0xC2, 0xE3, 0xF4, 0xA5, 0xC6, 0xE7, 0xF8 };
			ulong value = 0xA1C2E3F4A5C6E7F8;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length);
			}
		}

		/// <summary>
		///A test for ReadInt64
		///</summary>
		[TestMethod()]
		public void ReadUInt64Test()
		{
			byte[] data = new byte[] { 0xA1, 0xC2, 0xE3, 0xF4, 0xA5, 0xC6, 0xE7, 0xF8 };
			ulong expected = 0xA1C2E3F4A5C6E7F8;

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				ulong actual;
				actual = Helper.ReadUInt64(reader);
				Assert.AreEqual(expected, actual);
			}
		} 

		#endregion

		#region Double

		/// <summary>
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_Double()
		{
			// 0x4028d8c7e28240b8
			// 12.4234
			byte[] expected = new byte[] { 0x40, 0x28, 0xd8, 0xc7, 0xe2, 0x82, 0x40, 0xb8 };
			double value = 12.4234;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length);
			}
		}

		/// <summary>
		///A test for ReadDouble
		///</summary>
		[TestMethod()]
		public void ReadDoubleTest()
		{
			byte[] data = new byte[] { 0x40, 0x28, 0xd8, 0xc7, 0xe2, 0x82, 0x40, 0xb8 };
			double expected = 12.4234;

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				double actual;
				actual = Helper.ReadDouble(reader);
				Assert.AreEqual(expected, actual);
			}
		} 

		#endregion

		#region Padding

		/// <summary>
		///A test for WritePadding
		///</summary>
		[TestMethod()]
		public void WritePaddingTest_0()
		{
			long expected = 0;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.WritePadding(writer, stream.Position);
				Assert.AreEqual(stream.Position, expected);
			}
		}

		/// <summary>
		///A test for WritePadding
		///</summary>
		[TestMethod()]
		public void WritePaddingTest_1()
		{
			long expected = 4;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write((byte)23);

				Helper.WritePadding(writer, stream.Position);

				Assert.AreEqual(stream.Position, expected);
			}
		}

		/// <summary>
		///A test for WritePadding
		///</summary>
		[TestMethod()]
		public void WritePaddingTest_2()
		{
			long expected = 4;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write((byte)23);
				writer.Write((byte)23);

				Helper.WritePadding(writer, stream.Position);

				Assert.AreEqual(stream.Position, expected);
			}
		}

		/// <summary>
		///A test for WritePadding
		///</summary>
		[TestMethod()]
		public void WritePaddingTest_3()
		{
			long expected = 4;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write((byte)23);
				writer.Write((byte)23);
				writer.Write((byte)23);

				Helper.WritePadding(writer, stream.Position);

				Assert.AreEqual(stream.Position, expected);
			}
		}

		/// <summary>
		///A test for WritePadding
		///</summary>
		[TestMethod()]
		public void WritePaddingTest_4()
		{
			long expected = 4;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				writer.Write((byte)23);
				writer.Write((byte)23);
				writer.Write((byte)23);
				writer.Write((byte)23);

				Helper.WritePadding(writer, stream.Position);

				Assert.AreEqual(stream.Position, expected);
			}
		}	 

		#endregion	
	}
}

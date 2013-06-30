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
		///A test for Write
		///</summary>
		[TestMethod()]
		public void WriteTest_Int()
		{
			byte[] expected = new byte[] { 0x00, 0x00, 0xCA,0x41 };
			int value = 51777;

			using (MemoryStream stream = new MemoryStream())
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				Helper.Write(writer, value);

				UnitTestHelper.AreEqual(expected, stream.GetBuffer(), stream.Length); 
			}
		}

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
	}
}

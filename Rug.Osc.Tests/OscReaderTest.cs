using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rug.Osc.Tests
{
    
    
    /// <summary>
    ///This is a test class for OscReaderTest and is intended
    ///to contain all OscReaderTest Unit Tests
    ///</summary>
	[TestClass()]
	public class OscReaderTest
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
		///A test for Read 
		///</summary>
		[TestMethod()]
		public void ReadTest_MultiLine()
		{			
			using (MemoryStream stream = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(stream))
			using (OscReader target = new OscReader(stream, OscPacketFormat.String))
			{
				writer.WriteLine(UnitTestHelper.BundleString_MultiLineString);
				writer.Flush(); 
				stream.Position = 0;

				OscPacket expected = UnitTestHelper.Bundle_MultiLineString(); 
				OscPacket actual;

				actual = target.Read();

				UnitTestHelper.AreEqual(expected, actual);				
			}
		}
	}
}

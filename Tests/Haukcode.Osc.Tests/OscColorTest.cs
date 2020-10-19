using NUnit.Framework;

namespace Haukcode.Osc.Tests
{
    /// <summary>
    ///This is a test class for OscColorTest and is intended
    ///to contain all OscColorTest Unit Tests
    ///</summary>
    [TestFixture]
    public class OscColorTest
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

        #endregion Additional test attributes

        /// <summary>
        ///A test for OscColor Constructor
        ///</summary>
        [Test]
        public void OscColorConstructorTest()
        {
            int value = unchecked((int)0xFFFFFFFF);

            OscColor target = new OscColor(value);

            Assert.AreEqual(value, target.ARGB);

            Assert.AreEqual(255, target.A);
            Assert.AreEqual(255, target.R);
            Assert.AreEqual(255, target.G);
            Assert.AreEqual(255, target.B);
        }

        /// <summary>
        ///A test for FromArgb
        ///</summary>
        [Test]
        public void FromArgbTest_R()
        {
            byte alpha = 255;
            byte red = 255;
            byte green = 0;
            byte blue = 0;
            int argb = unchecked((int)0xFFFF0000);

            OscColor expected = new OscColor(argb);
            OscColor actual;
            actual = OscColor.FromArgb(alpha, red, green, blue);

            Assert.AreEqual(expected, actual);

            Assert.AreEqual(argb, actual.ARGB);

            Assert.AreEqual(alpha, actual.A);
            Assert.AreEqual(red, actual.R);
            Assert.AreEqual(green, actual.G);
            Assert.AreEqual(blue, actual.B);
        }

        /// <summary>
        ///A test for FromArgb
        ///</summary>
        [Test]
        public void FromArgbTest_G()
        {
            byte alpha = 255;
            byte red = 0;
            byte green = 255;
            byte blue = 0;
            int argb = unchecked((int)0xFF00FF00);

            OscColor expected = new OscColor(argb);
            OscColor actual;
            actual = OscColor.FromArgb(alpha, red, green, blue);

            Assert.AreEqual(expected, actual);

            Assert.AreEqual(argb, actual.ARGB);

            Assert.AreEqual(alpha, actual.A);
            Assert.AreEqual(red, actual.R);
            Assert.AreEqual(green, actual.G);
            Assert.AreEqual(blue, actual.B);
        }

        /// <summary>
        ///A test for FromArgb
        ///</summary>
        [Test]
        public void FromArgbTest_B()
        {
            byte alpha = 255;
            byte red = 0;
            byte green = 0;
            byte blue = 255;
            int argb = unchecked((int)0xFF0000FF);

            OscColor expected = new OscColor(argb);
            OscColor actual;
            actual = OscColor.FromArgb(alpha, red, green, blue);

            Assert.AreEqual(expected, actual);

            Assert.AreEqual(argb, actual.ARGB);

            Assert.AreEqual(alpha, actual.A);
            Assert.AreEqual(red, actual.R);
            Assert.AreEqual(green, actual.G);
            Assert.AreEqual(blue, actual.B);
        }
    }
}
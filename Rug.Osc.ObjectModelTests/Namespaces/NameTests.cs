using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rug.Osc.Namespaces.Tests
{
    [TestClass()]
    public class NameTests
    {
        private const string testPath0 = "/";
        private const string testPath1 = "/moop";
        private const string testPath2 = "/do-the/moop";

        private const string testPathEmpty = "";
        private const string testPathGreater = "/1";
        private const string testPathLesser = "/0";
        private const string testPathPattern = "/*thing";

        [TestMethod()]
        public void CompareToTest_Name()
        {
            Name name = new Name(testPath2, true);

            Assert.AreEqual(0, name.CompareTo(name));
        }

        [TestMethod()]
        public void CompareToTest_Name2()
        {
            Name name0 = new Name(testPathLesser, true);
            Name name1 = new Name(testPathGreater, true);

            Assert.AreEqual(0, name0.CompareTo(name0));
            Assert.AreEqual(0, name1.CompareTo(name1));

            Assert.AreEqual(-1, name0.CompareTo(name1));
            Assert.AreEqual(+1, name1.CompareTo(name0));
        }

        [TestMethod()]
        public void CompareToTest_OscAddress()
        {
            Name name = new Name(testPath2, true);
            OscAddress oscAddress = new OscAddress(testPath2);

            Assert.AreEqual(0, name.CompareTo(oscAddress));
        }

        [TestMethod()]
        public void CompareToTest_OscAddress2()
        {
            Name name = new Name(testPathLesser, true);
            OscAddress oscAddress = new OscAddress(testPathGreater);

            Assert.AreEqual(-1, name.CompareTo(oscAddress));
        }

        [TestMethod()]
        public void CompareToTest_String()
        {
            Name name = new Name(testPath2, true);

            Assert.AreEqual(0, name.CompareTo(testPath2));
        }

        [TestMethod()]
        public void CompareToTest_String2()
        {
            Name name0 = new Name(testPathLesser, true);
            string name1 = testPathGreater;

            Assert.AreEqual(-1, name0.CompareTo(name1));
        }

        [TestMethod()]
        public void EqualsTest_Name()
        {
            Name name0 = new Name(testPath1, true);
            Name name1 = new Name(testPath1, true);

            Assert.AreEqual(true, name0.Equals(name1));
        }

        [TestMethod()]
        public void EqualsTest_Name2()
        {
            Name name0 = new Name(testPath1, true);
            Name name1 = new Name(testPath2, true);

            Assert.AreEqual(false, name0.Equals(name1));
        }

        [TestMethod()]
        public void EqualsTest_OscAddress()
        {
            Name name0 = new Name(testPath1, true);
            OscAddress oscAddress = new OscAddress(testPath1);

            Assert.AreEqual(true, name0.Equals(oscAddress));
        }

        [TestMethod()]
        public void EqualsTest_OscAddress2()
        {
            Name name0 = new Name(testPath1, true);
            OscAddress oscAddress = new OscAddress(testPath2);

            Assert.AreEqual(false, name0.Equals(oscAddress));
        }

        [TestMethod()]
        public void EqualsTest_String()
        {
            Name name0 = new Name(testPath1, true);
            string stringName = testPath1;

            Assert.AreEqual(true, name0.Equals(stringName));
        }

        [TestMethod()]
        public void EqualsTest_String2()
        {
            Name name0 = new Name(testPath1, true);
            string stringName = testPath2;

            Assert.AreEqual(false, name0.Equals(stringName));
        }

        [TestMethod()]
        public void NameTest_Named()
        {
            Name name = new Name(testPath1, true);

            Assert.AreEqual(testPath1, name.Value);
        }

        [TestMethod()]
        public void NameTest_Named_Invalid_Empty()
        {
            try
            {
                Name name = new Name(testPathEmpty, true);

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void NameTest_Named_Invalid_Pattern()
        {
            try
            {
                Name name = new Name(testPathPattern, true);

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void NameTest_Named2Step()
        {
            Name name = new Name(testPath2, true);

            Assert.AreEqual(testPath2, name.Value);
        }

        [TestMethod()]
        public void NameTest_Root()
        {
            Name name = new Name(testPath0, true);

            Assert.AreEqual(testPath0, name.Value);
        }

        [TestMethod()]
        public void ToStringTest()
        {
            Name name = new Name(testPath2, true);

            Assert.AreEqual(testPath2, name.ToString());
        }

        [TestMethod()]
        public void ChangeName()
        {
            Name name = new Name(testPath1, false);

            name.Value = testPath2;

            Assert.AreEqual(testPath2, name.Value);
        }

        [TestMethod()]
        public void ChangeName_Invalid_Pattern()
        {
            try
            {
                Name name = new Name(testPath1, false);

                name.Value = testPathPattern;

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void ChangeName_Invalid_Readonly()
        {
            try
            {
                Name name = new Name(testPath1, true);

                name.Value = testPath2;

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod()]
        public void ChangeName_Invalid_Empty()
        {
            try
            {
                Name name = new Name(testPath1, false);

                name.Value = string.Empty;

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
    }
}
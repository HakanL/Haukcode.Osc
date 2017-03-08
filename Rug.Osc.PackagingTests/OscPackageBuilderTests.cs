using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rug.Osc.Packaging.Tests
{
    [TestClass()]
    public class OscPackageBuilderTests
    {
        [TestMethod()]
        public void OscPackageBuilderTest()
        {
            OscPackageBuilder builder = new OscPackageBuilder(123) { Mode = OscPackageBuilderMode.PackagedAndQueued };

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void AddTest()
        {
            OscMessage message = new OscMessage("/moop");

            OscPackageBuilder builder = new OscPackageBuilder(123) { Mode = OscPackageBuilderMode.PackagedAndQueued };

            builder.Add(message);

            Assert.IsTrue(true);
        }

        //[TestMethod()]
        //public void AddPacketIDMessageTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void FlushTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void CreatePacketIDMessageTest()
        //{
        //    Assert.Fail();
        //}
    }
}
using NUnit.Framework;

namespace Haukcode.Osc.Packaging.Tests
{
    [TestFixture]
    public class OscPackageBuilderTests
    {
        [Test]
        public void OscPackageBuilderTest()
        {
            OscPackageBuilder builder = new OscPackageBuilder(123) { Mode = OscPackageBuilderMode.PackagedAndQueued };

            Assert.IsTrue(true);
        }

        [Test]
        public void AddTest()
        {
            OscMessage message = new OscMessage("/moop");

            OscPackageBuilder builder = new OscPackageBuilder(123) { Mode = OscPackageBuilderMode.PackagedAndQueued };

            builder.Add(message);

            Assert.IsTrue(true);
        }

        //[Test]
        //public void AddPacketIDMessageTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void FlushTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void CreatePacketIDMessageTest()
        //{
        //    Assert.Fail();
        //}
    }
}
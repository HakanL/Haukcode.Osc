using Rug.Osc.Namespaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rug.Osc.Namespaces.Tests
{
    [TestFixture]
    public class OscNamespaceAttributeTests
    {
        string[] validNamespaces = new string[]
        {
            "/",
            "/moop",
            "/the/doop",
        };

        string[] invalidNamespaces = new string[]
        {
            null,
            "",
            " /thing",
            "//thing",
            "/*thing",
        };


        [Test]
        public void OscNamespaceAttributeTest_Valid()
        {
            foreach (string address in validNamespaces)
            {
                OscNamespaceAttribute oscNamespaceAttribute = new OscNamespaceAttribute(address);

                Assert.AreEqual(address, oscNamespaceAttribute.Namesapce); 
            }
        }

        [Test]
        public void OscNamespaceAttributeTest_Invalid()
        {
            foreach (string address in invalidNamespaces)
            {
                try
                {
                    OscNamespaceAttribute oscNamespaceAttribute = new OscNamespaceAttribute(address);

                    Assert.Fail();
                }
                catch
                {
                    Assert.IsTrue(true);     
                }
            }
        }
    }
}
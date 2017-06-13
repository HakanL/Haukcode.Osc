using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rug.Osc.Namespaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rug.Osc.Namespaces.Tests
{
    [TestClass()]
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


        [TestMethod()]
        public void OscNamespaceAttributeTest_Valid()
        {
            foreach (string address in validNamespaces)
            {
                OscNamespaceAttribute oscNamespaceAttribute = new OscNamespaceAttribute(address);

                Assert.AreEqual(address, oscNamespaceAttribute.Namesapce); 
            }
        }

        [TestMethod()]
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
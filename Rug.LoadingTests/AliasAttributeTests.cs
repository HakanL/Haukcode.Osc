using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rug.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rug.Loading.Tests
{
    [TestClass()]
    public class AliasAttributeTests
    {
        [TestMethod()]
        public void AliasAttributeTest()
        {
            string[] args = new string[] { "Moop", "The.Doop", "TheyThing" };

            AliasAttribute aliasAttribute = new AliasAttribute(args);

            Assert.AreEqual(args.Length, aliasAttribute.Alias.Length);

            for (int i = 0; i < args.Length; i++)
            {
                Assert.AreEqual(args[i], aliasAttribute.Alias[i]); 
            }
        }
    }
}
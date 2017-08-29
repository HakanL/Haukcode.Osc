using Rug.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rug.Loading.Tests
{
    [TestFixture]
    public class AliasAttributeTests
    {
        [Test]
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
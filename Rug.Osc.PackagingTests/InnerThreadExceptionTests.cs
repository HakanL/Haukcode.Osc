using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rug.Osc.Packaging.Tests
{
    [TestClass()]
    public class InnerThreadExceptionTests
    {
        [TestMethod()]
        public void InnerThreadExceptionTest()
        {
            const string message = "TEST MESSAGE";

            InnerThreadException innerThreadException = new InnerThreadException(message);

            Assert.AreEqual(message, innerThreadException.Message);
        }

        [TestMethod()]
        public void InnerThreadExceptionTest1()
        {
            const string message = "TEST MESSAGE";
            const string innerMessage = "TEST MESSAGE 2";
            Exception innerException = new Exception(innerMessage);

            InnerThreadException innerThreadException = new InnerThreadException(message, innerException);

            Assert.AreEqual(message, innerThreadException.Message);
            Assert.AreNotEqual(null, innerThreadException.InnerException);
            Assert.AreEqual(innerMessage, innerThreadException.InnerException.Message);
        }
    }
}
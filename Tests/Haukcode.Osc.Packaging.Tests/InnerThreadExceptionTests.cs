using System;
using NUnit.Framework;

namespace Haukcode.Osc.Packaging.Tests
{
    [TestFixture]
    public class InnerThreadExceptionTests
    {
        [Test]
        public void InnerThreadExceptionTest()
        {
            const string message = "TEST MESSAGE";

            InnerThreadException innerThreadException = new InnerThreadException(message);

            Assert.AreEqual(message, innerThreadException.Message);
        }

        [Test]
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
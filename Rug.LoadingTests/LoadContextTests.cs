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
    public class LoadContextTests
    {
        [TestMethod()]
        public void LoadContextTest()
        {
            LoadContext context = new LoadContext(new DebugReporter()); 
        }

        [TestMethod()]
        public void ErrorTest()
        {
            LoadContext context = new LoadContext(new DebugReporter());

            string errorString1 = "Some error string";
            string errorString2 = "Some other error string";

            context.Error(errorString1, false, null);

            Assert.AreEqual(1, context.Errors.Count);
            Assert.AreEqual(false, context.HasHadCriticalError);
            Assert.AreEqual(errorString1, context.Errors[0].Message);

            context.Error(errorString2, true, null);

            Assert.AreEqual(2, context.Errors.Count);
            Assert.AreEqual(true, context.HasHadCriticalError);
            Assert.AreEqual(errorString2, context.Errors[1].Message);
        }

        [TestMethod()]
        public void ReportErrorsTest()
        {
            LoadContext context = new LoadContext(new DebugReporter());

            string errorString1 = "Some error string";
            string errorString2 = "Some other error string";

            context.Error(errorString1);
            context.Error(errorString2, true, null);

            context.ReportErrors(); 
        }
    }
}
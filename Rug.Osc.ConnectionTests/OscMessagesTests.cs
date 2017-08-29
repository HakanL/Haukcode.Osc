using Rug.Osc.Connection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rug.Osc.Connection.Tests
{
    [TestFixture()]
    public class OscMessagesTests
    {
        [Test()]
        public void BindingErrorTest()
        {
            DebugReporter reporter = new DebugReporter();
            OscMessage message;

            message = OscMessages.BindingError(BindingErrorType.ObjectInvalidType, "/moop", typeof(int), "/the/doop", typeof(float));
            OscMessages.Print.BindingError(reporter, message);
            Assert.AreEqual(
                @"/error, ""ObjectInvalidType"", ""Could not bind because the object at the destination is of an incompatible type. Expected int found float."", ""/moop"", ""/the/doop"", ""int"", ""float""",
                message.ToString()
                );

            message = OscMessages.BindingError(BindingErrorType.ObjectNotFound, "/moop", typeof(int), "/the/doop", typeof(float));
            OscMessages.Print.BindingError(reporter, message);
            Assert.AreEqual(
                @"/error, ""ObjectNotFound"", ""Could not bind because no object exists at address."", ""/moop"", ""/the/doop""",
                message.ToString()
                );

            message = OscMessages.BindingError(BindingErrorType.Unspecified, "/moop", typeof(int), "/the/doop", typeof(float));
            OscMessages.Print.BindingError(reporter, message);
            Assert.AreEqual(
                @"/error, ""Unspecified"", ""Could not bind for an unspecified reason."", ""/moop"", ""/the/doop""",
                message.ToString()
                );

            OscMessages.Print.BindingError(reporter, new OscMessage("/error", "MOOP"));
        }

        [Test()]
        public void ErrorTest()
        {
            DebugReporter reporter = new DebugReporter();
            OscMessage message;

            message = OscMessages.Error("String");
            OscMessages.Print.Error(reporter, message);
            Assert.AreEqual(
                @"/error, """", ""String""",
                message.ToString()
                );

            message = OscMessages.Error("String {0} {1}", "A", "B");
            OscMessages.Print.Error(reporter, message);
            Assert.AreEqual(
                @"/error, """", ""String A B""",
                message.ToString()
                );
        }

        [Test()]
        public void MethodDescriptorTest()
        {
            DebugReporter reporter = new DebugReporter();
            OscMessage message;

            message = OscMessages.MethodDescriptor("Moop", "/the/doop", new List<Type> {typeof(float), typeof(float), typeof(float),}, new List<string> {"x", "y", "z"});
            OscMessages.Print.MethodDescriptor(reporter, message);
            Assert.AreEqual(
                "/method, \"Moop\", \"/the/doop\", \"float:x\", \"float:y\", \"float:z\"",
                message.ToString()
                );
        }        

        //[TestMethod()]
        //public void ObjectErrorTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void PropertyDescriptorTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void TypeDescriptorTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void TypeOfTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void UsageTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void UsageTest1()
        //{
        //    Assert.Fail();
        //}
    }
}
using NUnit.Framework;
using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rug.Osc.Tests
{
    [TestFixture]
    public class OscHelperTests
    {
        [Test]
        public void ArgumentsToStringTest()
        {
            Assert.AreEqual("", OscHelper.ArgumentsToString(new object[0]));
            Assert.AreEqual("", OscHelper.ArgumentsToString(null));

            Assert.AreEqual("\"MOOP\"", OscHelper.ArgumentsToString(new object[] { "MOOP" }));
            Assert.AreEqual("1", OscHelper.ArgumentsToString(new object[] { 1 }));
            Assert.AreEqual("1.5f", OscHelper.ArgumentsToString(new object[] { 1.5f }));
            Assert.AreEqual("1.5d", OscHelper.ArgumentsToString(new object[] { 1.5d }));
            Assert.AreEqual("1L", OscHelper.ArgumentsToString(new object[] { 1L }));

            Assert.AreEqual("\"MOOP\", \"THE DOOP\"", OscHelper.ArgumentsToString(new object[] { "MOOP", "THE DOOP" }));
            Assert.AreEqual("\"MOOP\", \"THE DOOP\"", OscHelper.ArgumentsToString(new object[] { "MOOP", "THE DOOP" }));

            ////Assert.Fail();
            //bool first = true;

            //foreach (object obj in args)
            //{
            //    if (first == false)
            //    {
            //        sb.Append(", ");
            //    }
            //    else
            //    {
            //        first = false;
            //    }

            //    if (obj is object[])
            //    {
            //        sb.Append('[');

            //        ArgumentsToString(sb, provider, obj as object[]);

            //        sb.Append(']');
            //    }
            //    else if (obj is int)
            //    {
            //        sb.Append(((int)obj).ToString(provider));
            //    }
            //    else if (obj is long)
            //    {
            //        sb.Append(((long)obj).ToString(provider) + "L");
            //    }
            //    else if (obj is float)
            //    {
            //        sb.Append(((float)obj).ToString(provider) + "f");
            //    }
            //    else if (obj is double)
            //    {
            //        sb.Append(((double)obj).ToString(provider) + "d");
            //    }
            //    else if (obj is byte)
            //    {
            //        sb.Append("'" + (char)(byte)obj + "'");
            //    }
            //    else if (obj is OscColor)
            //    {
            //        sb.Append("{ Color: " + Helper.ToStringColor((OscColor)obj) + " }");
            //    }
            //    else if (obj is OscTimeTag)
            //    {
            //        sb.Append("{ Time: " + ((OscTimeTag)obj).ToString() + " }");
            //    }
            //    else if (obj is OscMidiMessage)
            //    {
            //        sb.Append("{ Midi: " + ((OscMidiMessage)obj).ToString() + " }");
            //    }
            //    else if (obj is bool)
            //    {
            //        sb.Append(((bool)obj).ToString());
            //    }
            //    else if (obj is OscNull)
            //    {
            //        sb.Append(((OscNull)obj).ToString());
            //    }
            //    else if (obj is OscImpulse)
            //    {
            //        sb.Append(((OscImpulse)obj).ToString());
            //    }
            //    else if (obj is string)
            //    {
            //        sb.Append("\"" + OscHelper.EscapeString(obj.ToString()) + "\"");
            //    }
            //    else if (obj is OscSymbol)
            //    {
            //        sb.Append(OscHelper.EscapeString(obj.ToString()));
            //    }
            //    else if (obj is byte[])
            //    {
            //        sb.Append("{ Blob: " + OscHelper.ToStringBlob(obj as byte[]) + " }");
            //    }
            //    else
            //    {
            //        throw new Exception(String.Format(Strings.Arguments_UnsupportedType, obj.GetType().ToString()));
            //    }
            //}

        }

        //[Test]
        //public void ArgumentsToStringTest1()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void ParseArgumentTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void ParseArgumentTest1()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void EscapeStringTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void UnescapeStringTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void EscapeTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void ToStringBlobTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void IsValidEscapeTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void UnescapeTest()
        //{
        //    Assert.Fail();
        //}
    }
}
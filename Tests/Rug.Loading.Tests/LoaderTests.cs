using NUnit.Framework;
using Rug.Loading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace Rug.Loading.Tests
{
    [TestFixture]
    public class LoaderTests
    {
        [Test]
        public void GetNameTest_TypeNotFound()
        {
            try
            {
                Loader.GetName(typeof(Object));

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.ToString());
            }
        }

        //[TestMethod()]
        //public void GetTypeOfTypeTest()
        
        //{
        //    Assert.Fail();
        //}

        [Test]
        public void IsTypeLoadableTest()
        {
            Assert.IsTrue(Loader.IsTypeLoadable(typeof(SimpleLoadable)));
            Assert.IsTrue(Loader.IsTypeLoadable(typeof(SimpleLoadable2)));
            Assert.IsFalse(Loader.IsTypeLoadable(typeof(object)));
        }

        [Test]
        public void LoadObjectTest()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest1()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject<SimpleLoadable2>(context, document.Root, LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest2()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop type=\"SimpleLoadable2\" /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject<SimpleLoadable2>(context, document.Root, "Moop", LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest3()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable2\" /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, "Moop", LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest4()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable2\" /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject<SimpleLoadable2>(context, document.Root, "Moop", new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest5()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop type=\"SimpleLoadable2\" /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, "Moop", new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest6()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject<SimpleLoadable2>(context, document.Root, new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest7()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /></Objects>"));

            Assert.IsNotNull(Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError));
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest_MissingTypeAttribute()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop /></Objects>"));

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, "Moop", LoaderMode.UnknownNodesError);

                Assert.Fail(); 
            }
            catch (Exception ex)
            {

            }

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest_InvalidTypeAttribute()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"NoType\"/></Objects>"));

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, "Moop", LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectTest_InvalidConstructor()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /></Objects>"));

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, new Type[] { typeof(string) }, new []{ "INVALID ARGUMENT" }, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            context.ReportErrors();
        }


        [Test]
        public void LoadObjectTest_NoNode()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects></Objects>"));

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, "Moop", LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, new Type[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            try
            {
                Loader.LoadObject<SimpleLoadable2>(context, document.Root, "Moop", new Type[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }


            try
            {
                Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            try
            {
                Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, "Moop", LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            try
            {
                Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, new Type[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            try
            {
                Loader.LoadObject(typeof(SimpleLoadable2), context, document.Root, "Moop", new Type[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

                Assert.Fail();
            }
            catch (Exception ex)
            {

            }

            context.ReportErrors();
        }


        [Test]
        public void LoadObjectsTest()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /><SimpleLoadable2 /><SimpleLoadable2 /></Objects>"));

            ILoadable[] loadables = Loader.LoadObjects(typeof(SimpleLoadable2), context, document.Root, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectsTest1()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /><SimpleLoadable2 /><SimpleLoadable2 /></Objects>"));

            ILoadable[] loadables = Loader.LoadObjects(typeof(SimpleLoadable2), context, document.Root, new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectsTest2()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /><SimpleLoadable2 /><SimpleLoadable2 /></Objects>"));

            SimpleLoadable2[] loadables = Loader.LoadObjects<SimpleLoadable2>(context, document.Root, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectsTest3()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable2 /><SimpleLoadable2 /><SimpleLoadable2 /></Objects>"));

            SimpleLoadable2[] loadables = Loader.LoadObjects<SimpleLoadable2>(context, document.Root, new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }




        [Test]
        public void LoadObjectsTest4()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /></Objects>"));

            ILoadable[] loadables = Loader.LoadObjects(typeof(SimpleLoadable2), context, document.Root, "Moop", LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test]
        public void LoadObjectsTest5()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop type=\"SimpleLoadable2\" /><Moop type=\"SimpleLoadable2\" /></Objects>"));

            ILoadable[] loadables = Loader.LoadObjects(typeof(SimpleLoadable2), context, document.Root, "Moop", new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test()]
        public void LoadObjectsTest6()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop type=\"SimpleLoadable2\" /><Moop type=\"SimpleLoadable2\" /></Objects>"));

            SimpleLoadable2[] loadables = Loader.LoadObjects<SimpleLoadable2>(context, document.Root, "Moop", LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test()]
        public void LoadObjectsTest7()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /><Moop type=\"SimpleLoadable2\" /></Objects>"));

            SimpleLoadable2[] loadables = Loader.LoadObjects<SimpleLoadable2>(context, document.Root, "Moop", new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test()]
        public void LoadObjectsTest7_NullNode()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());

            try
            {
                SimpleLoadable2[] loadables = Loader.LoadObjects<SimpleLoadable2>(context, null, "Moop", new[] {typeof(int)}, new object[] {123}, LoaderMode.UnknownNodesError);

                Assert.Fail(); 

                Assert.AreEqual(3, loadables.Length);
                Assert.AreEqual(0, context.Errors.Count);

                context.ReportErrors();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            } 
        }


        [Test()]
        public void LoadObjectsTest8()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><Moop Type=\"SimpleLoadable3a\" /><Moop Type=\"SimpleLoadable3b\" /><Moop type=\"SimpleLoadable3c\" /></Objects>"));

            SimpleLoadable3[] loadables = Loader.LoadObjects<SimpleLoadable3>(context, document.Root, "Moop", new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test()]
        public void LoadObjectsTest9()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = XDocument.Load(new StringReader("<Objects><SimpleLoadable3a /><SimpleLoadable3b /><SimpleLoadable3c /></Objects>"));

            SimpleLoadable3[] loadables = Loader.LoadObjects<SimpleLoadable3>(context, document.Root, new[] { typeof(int) }, new object[] { 123 }, LoaderMode.UnknownNodesError);

            Assert.AreEqual(3, loadables.Length);
            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();
        }

        [Test()]
        public void SaveObjectTest()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            SimpleLoadable2 savable = new SimpleLoadable2();

            Loader.SaveObject(context, document, "Moop", savable);

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<Moop Type=\"SimpleLoadable2\" />", document.InnerXml());
        }

        [Test()]
        public void SaveObjectTest_NullTag()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            SimpleLoadable2 savable = new SimpleLoadable2();

            Loader.SaveObject(context, document, null, savable);

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<SimpleLoadable2 />", document.InnerXml());
        }

        [Test()]
        public void SaveObjectTest_NotLoadable()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();
            object notSavable = new object();

            try
            {
                Loader.SaveObject(context, document, "Moop", notSavable);

                Assert.Fail(); 

                Assert.AreEqual(0, context.Errors.Count);

                context.ReportErrors();

                Debug.Print(document.InnerXml());
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true); 
            }
        }

        [Test()]
        public void SaveObjectTest1()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            SimpleLoadable2 savable = new SimpleLoadable2();                 

            Loader.SaveObject(context, document, savable);

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<SimpleLoadable2 />", document.InnerXml()); 
        }

        [Test()]
        public void SaveObjectsTest()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            SimpleLoadable2 savable1 = new SimpleLoadable2();
            SimpleLoadable2 savable2 = new SimpleLoadable2();
            SimpleLoadable2 savable3 = new SimpleLoadable2();

            Loader.SaveObjects(context, document, "Objects", "Moop", savable1, savable2, savable3);

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /></Objects>", document.InnerXml());
        }

        [Test()]
        public void SaveObjectsTest1()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            SimpleLoadable2 savable1 = new SimpleLoadable2();
            SimpleLoadable2 savable2 = new SimpleLoadable2();
            SimpleLoadable2 savable3 = new SimpleLoadable2();

            Loader.SaveObjects(context, document, "Objects", "Moop", new List<object>() { savable1, savable2, savable3 });

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /></Objects>", document.InnerXml());
        }

        [Test()]
        public void SaveObjectsTest2()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            XElement node = new XElement("Objects");

            document.Add(node); 

            SimpleLoadable2 savable1 = new SimpleLoadable2();
            SimpleLoadable2 savable2 = new SimpleLoadable2();
            SimpleLoadable2 savable3 = new SimpleLoadable2();

            Loader.SaveObjects(context, node, null, null, savable1, savable2, savable3);

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<Objects><SimpleLoadable2 /><SimpleLoadable2 /><SimpleLoadable2 /></Objects>", document.InnerXml());
        }

        [Test()]
        public void SaveObjectsTest3()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            LoadContext context = new LoadContext(new DebugReporter());
            XDocument document = new XDocument();

            XElement node = new XElement("Objects"); 

            document.Add(node); 

            SimpleLoadable2 savable1 = new SimpleLoadable2();
            SimpleLoadable2 savable2 = new SimpleLoadable2();
            SimpleLoadable2 savable3 = new SimpleLoadable2();

            Loader.SaveObjects(context, node, null, "Moop", new List<object>() { savable1, savable2, savable3 });

            Assert.AreEqual(0, context.Errors.Count);

            context.ReportErrors();

            Debug.Print(document.InnerXml());

            Assert.AreEqual("<Objects><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /><Moop Type=\"SimpleLoadable2\" /></Objects>", document.InnerXml());
        }

        [Test()]
        public void ToXElementNameTest()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            Assert.AreEqual($"{SimpleLoadable.TypeName}", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(SimpleLoadable))));
            Assert.AreEqual($"{SimpleLoadable2.TypeName}", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(SimpleLoadable2))));

            Assert.AreEqual($"{GenericLoadable<bool>.TypeName}_{Loader.GetTypeName(typeof(bool))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<bool>))));
            Assert.AreEqual($"{GenericLoadable<int>.TypeName}_{Loader.GetTypeName(typeof(int))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<int>))));
            Assert.AreEqual($"{GenericLoadable<float>.TypeName}_{Loader.GetTypeName(typeof(float))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<float>))));
            Assert.AreEqual($"{GenericLoadable<long>.TypeName}_{Loader.GetTypeName(typeof(long))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<long>))));
            Assert.AreEqual($"{GenericLoadable<double>.TypeName}_{Loader.GetTypeName(typeof(double))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<double>))));
            Assert.AreEqual($"{GenericLoadable<string>.TypeName}_{Loader.GetTypeName(typeof(string))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<string>))));
            Assert.AreEqual($"{GenericLoadable<char>.TypeName}_{Loader.GetTypeName(typeof(char))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable<char>))));

            Assert.AreEqual($"{GenericLoadable2<bool, bool>.TypeName}_{Loader.GetTypeName(typeof(bool))}.{Loader.GetTypeName(typeof(bool))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<bool, bool>))));
            Assert.AreEqual($"{GenericLoadable2<int, int>.TypeName}_{Loader.GetTypeName(typeof(int))}.{Loader.GetTypeName(typeof(int))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<int, int>))));
            Assert.AreEqual($"{GenericLoadable2<float, float>.TypeName}_{Loader.GetTypeName(typeof(float))}.{Loader.GetTypeName(typeof(float))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<float, float>))));
            Assert.AreEqual($"{GenericLoadable2<long, long>.TypeName}_{Loader.GetTypeName(typeof(long))}.{Loader.GetTypeName(typeof(long))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<long, long>))));
            Assert.AreEqual($"{GenericLoadable2<double, double>.TypeName}_{Loader.GetTypeName(typeof(double))}.{Loader.GetTypeName(typeof(double))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<double, double>))));
            Assert.AreEqual($"{GenericLoadable2<string, string>.TypeName}_{Loader.GetTypeName(typeof(string))}.{Loader.GetTypeName(typeof(string))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<string, string>))));
            Assert.AreEqual($"{GenericLoadable2<char, char>.TypeName}_{Loader.GetTypeName(typeof(char))}.{Loader.GetTypeName(typeof(char))}_", Loader.ToXmlNodeName(Loader.GetTypeName(typeof(GenericLoadable2<char, char>))));
        }

        [Test()]
        public void GetTypeNameTest1()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            Assert.AreEqual(SimpleLoadable.TypeName, Loader.GetTypeName(typeof(SimpleLoadable)));
            Assert.AreEqual(SimpleLoadable2.TypeName, Loader.GetTypeName(typeof(SimpleLoadable2)));
        }

        [Test()]
        public void GetTypeNameTest_Generic()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            Assert.AreEqual($"{GenericLoadable<bool>.TypeName}<{Loader.GetTypeName(typeof(bool))}>", Loader.GetTypeName(typeof(GenericLoadable<bool>)));
            Assert.AreEqual($"{GenericLoadable<int>.TypeName}<{Loader.GetTypeName(typeof(int))}>", Loader.GetTypeName(typeof(GenericLoadable<int>)));
            Assert.AreEqual($"{GenericLoadable<float>.TypeName}<{Loader.GetTypeName(typeof(float))}>", Loader.GetTypeName(typeof(GenericLoadable<float>)));
            Assert.AreEqual($"{GenericLoadable<long>.TypeName}<{Loader.GetTypeName(typeof(long))}>", Loader.GetTypeName(typeof(GenericLoadable<long>)));
            Assert.AreEqual($"{GenericLoadable<double>.TypeName}<{Loader.GetTypeName(typeof(double))}>", Loader.GetTypeName(typeof(GenericLoadable<double>)));
            Assert.AreEqual($"{GenericLoadable<string>.TypeName}<{Loader.GetTypeName(typeof(string))}>", Loader.GetTypeName(typeof(GenericLoadable<string>)));
            Assert.AreEqual($"{GenericLoadable<char>.TypeName}<{Loader.GetTypeName(typeof(char))}>", Loader.GetTypeName(typeof(GenericLoadable<char>)));
        }

        [Test()]
        public void GetTypeNameTest_Generic2()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            Assert.AreEqual($"{GenericLoadable<bool>.TypeName}", Loader.GetTypeName(typeof(GenericLoadable<bool>).GetGenericTypeDefinition()));
        }


        [Test()]
        public void GetTypeNameTest_Generic3()
        {
            Loader.CacheLoadables(this.GetType().Assembly);

            Assert.AreEqual($"{GenericLoadable2<bool, char>.TypeName}<{Loader.GetTypeName(typeof(bool))}, {Loader.GetTypeName(typeof(char))}>", Loader.GetTypeName(typeof(GenericLoadable2<bool, char>)));
            Assert.AreEqual($"{GenericLoadable2<int, string>.TypeName}<{Loader.GetTypeName(typeof(int))}, {Loader.GetTypeName(typeof(string))}>", Loader.GetTypeName(typeof(GenericLoadable2<int, string>)));
            Assert.AreEqual($"{GenericLoadable2<float, double>.TypeName}<{Loader.GetTypeName(typeof(float))}, {Loader.GetTypeName(typeof(double))}>", Loader.GetTypeName(typeof(GenericLoadable2<float, double>)));
            Assert.AreEqual($"{GenericLoadable2<long, long>.TypeName}<{Loader.GetTypeName(typeof(long))}, {Loader.GetTypeName(typeof(long))}>", Loader.GetTypeName(typeof(GenericLoadable2<long, long>)));
            Assert.AreEqual($"{GenericLoadable2<double, float>.TypeName}<{Loader.GetTypeName(typeof(double))}, {Loader.GetTypeName(typeof(float))}>", Loader.GetTypeName(typeof(GenericLoadable2<double, float>)));
            Assert.AreEqual($"{GenericLoadable2<string, int>.TypeName}<{Loader.GetTypeName(typeof(string))}, {Loader.GetTypeName(typeof(int))}>", Loader.GetTypeName(typeof(GenericLoadable2<string, int>)));
            Assert.AreEqual($"{GenericLoadable2<char, bool>.TypeName}<{Loader.GetTypeName(typeof(char))}, {Loader.GetTypeName(typeof(bool))}>", Loader.GetTypeName(typeof(GenericLoadable2<char, bool>)));
        }

        [Name(TypeName)]
        public class SimpleLoadable : ILoadable
        {
            public const string TypeName = "SomethingNotTheTypeName";

            /// <inheritdoc />
            public void Load(LoadContext context, XElement node)
            {

            }

            /// <inheritdoc />
            public void Save(LoadContext context, XElement element)
            {

            }
        }

        public class SimpleLoadable2 : ILoadable
        {
            public const string TypeName = "SimpleLoadable2";

            public SimpleLoadable2()
            {

            }

            public SimpleLoadable2(int argument)
            {
                Assert.AreEqual(123, argument);
            }

            /// <inheritdoc />
            public void Load(LoadContext context, XElement node)
            {

            }

            /// <inheritdoc />
            public void Save(LoadContext context, XElement element)
            {

            }
        }

        [Name(TypeName), Alias("SimpleLoadable3a", "SimpleLoadable3b", "SimpleLoadable3c")] 
        public class SimpleLoadable3 : ILoadable
        {
            public const string TypeName = "SimpleLoadable3";

            public SimpleLoadable3()
            {

            }

            public SimpleLoadable3(int argument)
            {
                Assert.AreEqual(123, argument);
            }

            /// <inheritdoc />
            public void Load(LoadContext context, XElement node)
            {

            }

            /// <inheritdoc />
            public void Save(LoadContext context, XElement element)
            {

            }
        }

        [Name(TypeName)]
        public class GenericLoadable<T> : ILoadable
        {
            public const string TypeName = "GenericLoadable";

            public T Value { get; set; }

            /// <inheritdoc />
            public void Load(LoadContext context, XElement node)
            {

            }

            /// <inheritdoc />
            public void Save(LoadContext context, XElement element)
            {

            }
        }

        [Name(TypeName)]
        public class GenericLoadable2<T1, T2> : ILoadable
        {
            public const string TypeName = "GenericLoadable2";

            public T1 Value1 { get; set; }
            public T2 Value2 { get; set; }

            /// <inheritdoc />
            public void Load(LoadContext context, XElement node)
            {

            }

            /// <inheritdoc />
            public void Save(LoadContext context, XElement element)
            {

            }
        }
    }
}
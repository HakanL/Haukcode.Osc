using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    [TestClass()]
    public class NamespaceTests
    {
        [TestMethod()]
        public void AddRangeTest()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                List<INamespaceObject> objects = new List<INamespaceObject>();

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    objects.Add(@object);
                }

                parent.AddRange(objects);

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent.Count);

                foreach (INamespaceObject @object in objects)
                {
                    Assert.AreEqual(parent, @object.Name.Namespace);
                }
            }
        }

        [TestMethod()]
        public void AddTest()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    parent.Add(@object);

                    Assert.AreEqual(parent, @object.Name.Namespace);
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent.Count);
            }
        }


        [TestMethod()]
        public void AddTestGeneric()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                int count = 0;
                parent.Add(new NamespaceTestObjectBool("/bool")); count++;
                parent.Add(new NamespaceTestObjectFloat("/float")); count++;
                parent.Add(new NamespaceTestObjectString("/string")); count++;
                parent.Add(new NamespaceTestObjectDouble("/double")); count++;
                parent.Add(new NamespaceTestObjectLong("/long")); count++;

                Assert.AreEqual(count, parent.Count);
            }
        }

        [TestMethod()]
        public void AddTest_Move()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Namespace parent1 = NamespaceTestHelper.CreateParent(validNamespaceAddress);
                Namespace parent2 = NamespaceTestHelper.CreateParent(NamespaceTestHelper.ParentPath2);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    parent1.Add(@object);

                    Assert.AreEqual(parent1, @object.Name.Namespace);

                    if (validNamespaceAddress == "/")
                    {
                        Assert.AreEqual(validObjectAddress, @object.Name.OscAddress);
                    }
                    else
                    {
                        Assert.AreEqual(validNamespaceAddress + validObjectAddress, @object.Name.OscAddress);
                    }
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent1.Count);
                Assert.AreEqual(0, parent2.Count);

                List<INamespaceObject> objects = new List<INamespaceObject>(parent1);

                foreach (INamespaceObject @object in objects)
                {
                    parent2.Add(@object);

                    Assert.AreEqual(parent2, @object.Name.Namespace);

                    Assert.AreEqual(NamespaceTestHelper.ParentPath2 + @object.Name.Value, @object.Name.OscAddress);
                }

                Assert.AreEqual(0, parent1.Count);
                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent2.Count);
            }
        }

        [TestMethod()]
        public void AddTest_Nested()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                NamespaceTestNamespace namespaceTestNamespace = new NamespaceTestNamespace();
                namespaceTestNamespace.Name.Value = NamespaceTestHelper.ParentPath2;

                parent.Add(namespaceTestNamespace);

                Assert.AreEqual(parent, namespaceTestNamespace.Name.Namespace);
                Assert.AreEqual(parent, namespaceTestNamespace.Namespace.Parent);

                if (validNamespaceAddress == "/")
                {
                    Assert.AreEqual(NamespaceTestHelper.ParentPath2, namespaceTestNamespace.Name.OscAddress);
                }
                else
                {
                    Assert.AreEqual(validNamespaceAddress + NamespaceTestHelper.ParentPath2, namespaceTestNamespace.Name.OscAddress);
                }

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    namespaceTestNamespace.Namespace.Add(@object);

                    Assert.AreEqual(namespaceTestNamespace.Namespace, @object.Name.Namespace);

                    if (validNamespaceAddress == "/")
                    {
                        Assert.AreEqual(NamespaceTestHelper.ParentPath2 + validObjectAddress, @object.Name.OscAddress);
                    }
                    else
                    {
                        Assert.AreEqual(validNamespaceAddress + NamespaceTestHelper.ParentPath2 + validObjectAddress, @object.Name.OscAddress);
                    }
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, namespaceTestNamespace.Namespace.Count);
                Assert.AreEqual(1, parent.Count);
            }
        }

        [TestMethod()]
        public void AddTest_Nested_Move()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Namespace parent1 = NamespaceTestHelper.CreateParent(validNamespaceAddress);
                Namespace parent2 = NamespaceTestHelper.CreateParent(NamespaceTestHelper.ParentPath2);

                NamespaceTestNamespace namespaceTestNamespace = new NamespaceTestNamespace();
                namespaceTestNamespace.Name.Value = NamespaceTestHelper.ParentPath3;

                parent1.Add(namespaceTestNamespace);

                if (validNamespaceAddress == "/")
                {
                    Assert.AreEqual(NamespaceTestHelper.ParentPath3, namespaceTestNamespace.Name.OscAddress);
                }
                else
                {
                    Assert.AreEqual(validNamespaceAddress + NamespaceTestHelper.ParentPath3, namespaceTestNamespace.Name.OscAddress);
                }

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    namespaceTestNamespace.Namespace.Add(@object);

                    Assert.AreEqual(namespaceTestNamespace.Namespace, @object.Name.Namespace);

                    Assert.AreEqual(namespaceTestNamespace.Name.OscAddress + validObjectAddress, @object.Name.OscAddress);
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, namespaceTestNamespace.Namespace.Count);
                Assert.AreEqual(1, parent1.Count);
                Assert.AreEqual(0, parent2.Count);

                parent2.Add(namespaceTestNamespace);

                Assert.AreEqual(NamespaceTestHelper.ParentPath2 + NamespaceTestHelper.ParentPath3, namespaceTestNamespace.Name.OscAddress);

                Assert.AreEqual(parent2, namespaceTestNamespace.Name.Namespace);
                Assert.AreEqual(parent2, namespaceTestNamespace.Namespace.Parent);

                Assert.AreEqual(0, parent1.Count);
                Assert.AreEqual(1, parent2.Count);

                foreach (INamespaceObject @object in namespaceTestNamespace.Namespace)
                {
                    Assert.AreEqual(namespaceTestNamespace.Namespace, (@object as INamespaceObject).Name.Namespace);

                    Assert.AreEqual(namespaceTestNamespace.Name.OscAddress + @object.Name.Value, @object.Name.OscAddress);
                }
            }
        }

        [TestMethod()]
        public void AssignParent_Root()
        {
            NamespaceTestObject @object = new NamespaceTestObject();

            @object.Name.Value = NamespaceTestHelper.NamePath1;

            INamespace parent = NamespaceTestHelper.CreateParent(NamespaceTestHelper.ParentPath0);

            parent.Add(@object);

            Assert.AreEqual(NamespaceTestHelper.NamePath1, @object.Name.OscAddress);
        }

        [TestMethod()]
        public void AssignParent_Valid()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject { Name = { Value = validObjectAddress } };


                    INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                    parent.Add(@object);

                    if (validNamespaceAddress == "/")
                    {
                        Assert.AreEqual(validObjectAddress, @object.Name.OscAddress);
                    }
                    else
                    {
                        Assert.AreEqual(validNamespaceAddress + validObjectAddress, @object.Name.OscAddress);
                    }
                }
            }
        }

        //[TestMethod()]
        //public void AttachTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void CreateTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void CreateTest1()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DestroyTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DestroyTest1()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DetachTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void GetEnumeratorTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void LoadTest()
        //{
        //    Assert.Fail();
        //}

        [TestMethod()]
        public void NamespaceTest_Invalid()
        {
            foreach (string invalidAddress in NamespaceTestHelper.InvalidAddresses)
            {
                try
                {
                    Namespace @namespace = new Namespace() { Name = invalidAddress };

                    Assert.Fail();
                }
                catch
                {
                    Assert.IsTrue(true);
                }
            }
        }

        [TestMethod()]
        public void NamespaceTest_Valid()
        {
            foreach (string validAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Namespace @namespace = new Namespace() { Name = validAddress };
                Assert.AreEqual(validAddress, @namespace.OscAddress);
            }
        }

        [TestMethod()]
        public void RemoveTest()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Namespace parent1 = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    parent1.Add(@object);

                    Assert.AreEqual(parent1, @object.Name.Namespace);

                    if (validNamespaceAddress == "/")
                    {
                        Assert.AreEqual(validObjectAddress, @object.Name.OscAddress);
                    }
                    else
                    {
                        Assert.AreEqual(validNamespaceAddress + validObjectAddress, @object.Name.OscAddress);
                    }
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent1.Count);

                List<INamespaceObject> objects = new List<INamespaceObject>(parent1);

                foreach (INamespaceObject @object in objects)
                {
                    parent1.Remove((@object as INamespaceObject));

                    Assert.AreEqual(null, (@object as INamespaceObject).Name.Namespace);

                    Assert.AreEqual(@object.Name.Value, @object.Name.OscAddress);
                }

                Assert.AreEqual(0, parent1.Count);
            }
        }

        [TestMethod()]
        public void SaveLoadTest()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Debug.Print("********************************** SaveLoadTest for address \"{0}\"", validNamespaceAddress);
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    parent.Add(@object);
                }

                LoadContext context = new LoadContext(new DebugReporter());

                Loader.CacheLoadables(typeof(INamespace).Assembly);
                Loader.CacheLoadables(this.GetType().Assembly);

                XmlDocument doc = new XmlDocument();

                Loader.SaveObject(context, doc, parent);

                context.ReportErrors();

                string cachedString = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                context = new LoadContext(new DebugReporter());

                INamespace loadedParent = Loader.LoadObject<INamespace>(context, doc, LoaderMode.UnknownNodesError);

                context.ReportErrors();

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreNotEqual(null, loadedParent);

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, loadedParent.Count);

                Debug.Print("**********************************");
                Debug.Print("");


                context = new LoadContext(new DebugReporter());
                XmlDocument doc2 = new XmlDocument();

                Loader.SaveObject(context, doc2, loadedParent);

                context.ReportErrors();

                string cachedString2 = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString2);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreEqual(cachedString, cachedString2);
            }
        }

        [TestMethod()]
        public void SaveLoadTest2()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Debug.Print("********************************** SaveLoadTest for address \"{0}\"", validNamespaceAddress);
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject2 @object = new NamespaceTestObject2();

                    @object.Name.Value = validObjectAddress;

                    parent.Add(@object);
                }

                LoadContext context = new LoadContext(new DebugReporter());

                Loader.CacheLoadables(typeof(INamespace).Assembly);
                Loader.CacheLoadables(this.GetType().Assembly);

                XmlDocument doc = new XmlDocument();

                Loader.SaveObject(context, doc, parent);

                context.ReportErrors();

                string cachedString = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                context = new LoadContext(new DebugReporter());

                INamespace loadedParent = Loader.LoadObject<INamespace>(context, doc, LoaderMode.UnknownNodesError);

                context.ReportErrors();

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreNotEqual(null, loadedParent);

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, loadedParent.Count);

                Debug.Print("**********************************");
                Debug.Print("");


                context = new LoadContext(new DebugReporter());
                XmlDocument doc2 = new XmlDocument();

                Loader.SaveObject(context, doc2, loadedParent);

                context.ReportErrors();

                string cachedString2 = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString2);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreEqual(cachedString, cachedString2);
            }
        }


        [TestMethod()]
        public void SaveLoadTest3()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Debug.Print("********************************** SaveLoadTest for address \"{0}\"", validNamespaceAddress);
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                foreach (string validObjectNamespaceAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestNamespace parentNamespace = new NamespaceTestNamespace(validObjectNamespaceAddress);

                    parent.Add(parentNamespace);

                    foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                    {
                        NamespaceTestObject2 @object = new NamespaceTestObject2();

                        @object.Name.Value = validObjectAddress;

                        parentNamespace.Namespace.Add(@object);
                    }
                }

                LoadContext context = new LoadContext(new DebugReporter());

                Loader.CacheLoadables(typeof(INamespace).Assembly);
                Loader.CacheLoadables(this.GetType().Assembly);

                XmlDocument doc = new XmlDocument();

                Loader.SaveObject(context, doc, parent);

                context.ReportErrors();

                string cachedString = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                context = new LoadContext(new DebugReporter());

                INamespace loadedParent = Loader.LoadObject<INamespace>(context, doc, LoaderMode.UnknownNodesError);

                context.ReportErrors();

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreNotEqual(null, loadedParent);

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, loadedParent.Count);

                Debug.Print("**********************************");
                Debug.Print("");

                context = new LoadContext(new DebugReporter());
                XmlDocument doc2 = new XmlDocument();

                Loader.SaveObject(context, doc2, loadedParent);

                context.ReportErrors();

                string cachedString2 = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString2);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreEqual(cachedString, cachedString2);
            }
        }


        [TestMethod()]
        public void SaveLoadTestGeneric()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidNamespaceAddresses)
            {
                Debug.Print("********************************** SaveLoadTest for address \"{0}\"", validNamespaceAddress);
                INamespace parent = NamespaceTestHelper.CreateParent(validNamespaceAddress);

                foreach (string validObjectNamespaceAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestNamespace parentNamespace = new NamespaceTestNamespace(validObjectNamespaceAddress);

                    parent.Add(parentNamespace);

                    int count = 0;
                    parentNamespace.Namespace.Add(new NamespaceTestObjectBool("/bool")); count++;
                    parentNamespace.Namespace.Add(new NamespaceTestObjectFloat("/float")); count++;
                    parentNamespace.Namespace.Add(new NamespaceTestObjectString("/string")); count++;
                    parentNamespace.Namespace.Add(new NamespaceTestObjectDouble("/double")); count++;
                    parentNamespace.Namespace.Add(new NamespaceTestObjectLong("/long")); count++;

                    Assert.AreEqual(count, parentNamespace.Namespace.Count);
                }

                LoadContext context = new LoadContext(new DebugReporter());

                Loader.CacheLoadables(typeof(INamespace).Assembly);
                Loader.CacheLoadables(this.GetType().Assembly);

                XmlDocument doc = new XmlDocument();

                Loader.SaveObject(context, doc, parent);

                context.ReportErrors();

                string cachedString = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                context = new LoadContext(new DebugReporter());

                INamespace loadedParent = Loader.LoadObject<INamespace>(context, doc, LoaderMode.UnknownNodesError);

                context.ReportErrors();

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreNotEqual(null, loadedParent);

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, loadedParent.Count);

                Debug.Print("**********************************");
                Debug.Print("");

                context = new LoadContext(new DebugReporter());
                XmlDocument doc2 = new XmlDocument();

                Loader.SaveObject(context, doc2, loadedParent);

                context.ReportErrors();

                string cachedString2 = doc.OuterXml;

                Debug.Print("************ XML ************");
                Debug.Print(cachedString2);
                Debug.Print("************ XML ************");

                Assert.AreEqual(0, context.Errors.Count);

                Assert.AreEqual(cachedString, cachedString2);
            }
        }

        //[TestMethod()]
        //public void SendTest()
        //{
        //    Assert.Fail();
        //}
    }
}
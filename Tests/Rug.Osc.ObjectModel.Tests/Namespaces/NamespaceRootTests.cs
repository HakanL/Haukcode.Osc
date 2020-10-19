using Rug.Osc.Namespaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NUnit.Framework;

namespace Rug.Osc.Namespaces.Tests
{
    [TestFixture]
    public class NamespaceRootTests
    {
        [Test]
        public void AddTest_Move()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidObjectAddresses)
            {
                OscAddressManager oscAddressManager = new OscAddressManager();
                NamespaceRoot root = new NamespaceRoot(oscAddressManager);

                NamespaceTestNamespace parent1 = NamespaceTestHelper.CreateTestNamespaceParent(validNamespaceAddress);
                NamespaceTestNamespace parent2 = NamespaceTestHelper.CreateTestNamespaceParent(NamespaceTestHelper.ParentPath2);

                root.Namespace.Add(parent1);
                root.Namespace.Add(parent2);

                Assert.AreEqual(true, oscAddressManager.Contains(validNamespaceAddress + "/name"));
                Assert.AreEqual(true, oscAddressManager.Contains(validNamespaceAddress + "/test-member1"));

                Assert.AreEqual(true, oscAddressManager.Contains(NamespaceTestHelper.ParentPath2 + "/name"));
                Assert.AreEqual(true, oscAddressManager.Contains(NamespaceTestHelper.ParentPath2 + "/test-member1"));

                Assert.AreEqual(root.Namespace, parent1.Name.Namespace);
                Assert.AreEqual(root.Namespace, parent1.Namespace.Parent);
                Assert.AreEqual(root.Namespace, parent2.Name.Namespace);
                Assert.AreEqual(root.Namespace, parent2.Namespace.Parent);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    parent1.Namespace.Add(@object);

                    Assert.AreEqual(parent1.Namespace, @object.Name.Namespace);

                    string address = NamespaceTestHelper.GetAddress(validNamespaceAddress, validObjectAddress);

                    Assert.AreEqual(address, @object.Name.OscAddress);

                    Assert.AreEqual(true, oscAddressManager.Contains(address + "/name"));
                    Assert.AreEqual(true, oscAddressManager.Contains(address + "/test-member1"));
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent1.Namespace.Count);
                Assert.AreEqual(0, parent2.Namespace.Count);

                List<INamespaceObject> objects = new List<INamespaceObject>(parent1.Namespace);

                foreach (INamespaceObject @object in objects)
                {
                    parent2.Namespace.Add((@object as INamespaceObject));

                    Assert.AreEqual(parent2.Namespace, (@object as INamespaceObject).Name.Namespace);

                    string objectAddress = NamespaceTestHelper.ParentPath2 + @object.Name.Value;

                    Assert.AreEqual(objectAddress, @object.Name.OscAddress);

                    Assert.AreEqual(true, oscAddressManager.Contains(objectAddress + "/name"));
                    Assert.AreEqual(true, oscAddressManager.Contains(objectAddress + "/test-member1"));
                }

                Assert.AreEqual(0, parent1.Namespace.Count);
                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, parent2.Namespace.Count);
            }
        }

        [Test]
        public void AddTest_Nested()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidObjectAddresses)
            {
                OscAddressManager oscAddressManager = new OscAddressManager();
                NamespaceRoot root = new NamespaceRoot(oscAddressManager);

                NamespaceTestNamespace parent = NamespaceTestHelper.CreateTestNamespaceParent(validNamespaceAddress);
                root.Namespace.Add(parent);

                NamespaceTestNamespace namespaceTestNamespace = new NamespaceTestNamespace();
                namespaceTestNamespace.Name.Value = NamespaceTestHelper.ParentPath2;

                parent.Namespace.Add(namespaceTestNamespace);

                Assert.AreEqual(parent.Namespace, namespaceTestNamespace.Name.Namespace);
                Assert.AreEqual(parent.Namespace, namespaceTestNamespace.Namespace.Parent);

                string address = NamespaceTestHelper.GetAddress(validNamespaceAddress, NamespaceTestHelper.ParentPath2);

                Assert.AreEqual(address, namespaceTestNamespace.Name.OscAddress);

                Assert.AreEqual(true, oscAddressManager.Contains(namespaceTestNamespace.Name.OscAddress + "/name"));
                Assert.AreEqual(true, oscAddressManager.Contains(namespaceTestNamespace.Name.OscAddress + "/test-member1"));

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    namespaceTestNamespace.Namespace.Add(@object);

                    Assert.AreEqual(namespaceTestNamespace.Namespace, @object.Name.Namespace);

                    string objectAddress = NamespaceTestHelper.GetAddress(validNamespaceAddress, NamespaceTestHelper.ParentPath2 + validObjectAddress);

                    Assert.AreEqual(objectAddress, @object.Name.OscAddress);

                    Assert.AreEqual(true, oscAddressManager.Contains(@object.Name.OscAddress + "/name"));
                    Assert.AreEqual(true, oscAddressManager.Contains(@object.Name.OscAddress + "/test-member1"));

                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, namespaceTestNamespace.Namespace.Count);
                Assert.AreEqual(1, parent.Namespace.Count);
            }
        }

        [Test]
        public void AddTest_Nested_Move()
        {
            List<INamespaceObject> objectsWeCareAbout = new List<INamespaceObject>();

            Debug.Print("*************************** AddTest_Nested_Move");

            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidObjectAddresses)
            {
                OscAddressManager oscAddressManager = new OscAddressManager();
                NamespaceRoot root = new NamespaceRoot(oscAddressManager);

                //root.Namespace.Name = "/root"; 

                NamespaceTestNamespace parent1 = NamespaceTestHelper.CreateTestNamespaceParent(validNamespaceAddress);
                NamespaceTestNamespace parent2 = NamespaceTestHelper.CreateTestNamespaceParent(NamespaceTestHelper.ParentPath2);

                objectsWeCareAbout.Add(parent1);
                objectsWeCareAbout.Add(parent2);

                root.Namespace.Add(parent1);
                root.Namespace.Add(parent2);

                NamespaceTestNamespace namespaceTestNamespace = new NamespaceTestNamespace();
                namespaceTestNamespace.Name.Value = NamespaceTestHelper.ParentPath3;

                parent1.Namespace.Add(namespaceTestNamespace);

                objectsWeCareAbout.Add(namespaceTestNamespace);

                string address = NamespaceTestHelper.GetAddress(root.Namespace.Name, validNamespaceAddress, NamespaceTestHelper.ParentPath3);

                Assert.AreEqual(address, namespaceTestNamespace.Name.OscAddress);

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    objectsWeCareAbout.Add(@object);

                    namespaceTestNamespace.Namespace.Add(@object);

                    Assert.AreEqual(namespaceTestNamespace.Namespace, @object.Name.Namespace);

                    Assert.AreEqual(namespaceTestNamespace.Name.OscAddress + validObjectAddress, @object.Name.OscAddress);

                    Assert.AreEqual(true, oscAddressManager.Contains(@object.Name.OscAddress + "/name"));
                    Assert.AreEqual(true, oscAddressManager.Contains(@object.Name.OscAddress + "/test-member1"));
                }

                Assert.AreEqual(NamespaceTestHelper.ValidObjectAddresses.Length, namespaceTestNamespace.Namespace.Count);
                Assert.AreEqual(1, parent1.Namespace.Count);
                Assert.AreEqual(0, parent2.Namespace.Count);

                Debug.Print("*************************** objectsWeCareAbout");
                foreach (object obj in objectsWeCareAbout)
                {
                    Debug.Print("        " + obj.ToString());
                }

                Debug.Print("*************************** parent1.Namespace.Remove(namespaceTestNamespace);");
                parent1.Namespace.Remove(namespaceTestNamespace);

                Debug.Print("*************************** objectsWeCareAbout");
                foreach (object obj in objectsWeCareAbout)
                {
                    Debug.Print("        " + obj.ToString());
                }

                Debug.Print("*************************** parent1.Namespace.Add(namespaceTestNamespace);");
                parent2.Namespace.Add(namespaceTestNamespace);

                Debug.Print("*************************** objectsWeCareAbout");
                foreach (object obj in objectsWeCareAbout)
                {
                    Debug.Print("        " + obj.ToString());
                }

                Assert.AreEqual(NamespaceTestHelper.GetAddress(root.Namespace.Name, NamespaceTestHelper.ParentPath2, NamespaceTestHelper.ParentPath3), namespaceTestNamespace.Name.OscAddress);

                Assert.AreEqual(parent2.Namespace, namespaceTestNamespace.Name.Namespace);
                Assert.AreEqual(parent2.Namespace, namespaceTestNamespace.Namespace.Parent);

                Assert.AreEqual(0, parent1.Namespace.Count);
                Assert.AreEqual(1, parent2.Namespace.Count);

                foreach (INamespaceObject @object in namespaceTestNamespace.Namespace)
                {
                    Assert.AreEqual(namespaceTestNamespace.Namespace, (@object as INamespaceObject).Name.Namespace);

                    Assert.AreEqual(namespaceTestNamespace.Name.OscAddress + @object.Name.Value, @object.Name.OscAddress);

                    Assert.AreEqual(true, oscAddressManager.Contains(@object.Name.OscAddress + "/name"));
                    Assert.AreEqual(true, oscAddressManager.Contains(@object.Name.OscAddress + "/test-member1"));
                }
            }
        }

        [Test]
        public void InvokeTest()
        {
            foreach (string validNamespaceAddress in NamespaceTestHelper.ValidObjectAddresses)
            {
                OscAddressManager oscAddressManager = new OscAddressManager();
                NamespaceRoot root = new NamespaceRoot(oscAddressManager);

                root.Namespace.Name = validNamespaceAddress; 

                foreach (string validObjectAddress in NamespaceTestHelper.ValidObjectAddresses)
                {
                    NamespaceTestObject @object = new NamespaceTestObject();

                    @object.Name.Value = validObjectAddress;

                    root.Namespace.Add(@object);

                    Assert.AreEqual(root.Namespace, @object.Name.Namespace);

                    string address = NamespaceTestHelper.GetAddress(validNamespaceAddress, validObjectAddress);

                    Assert.AreEqual(address, @object.Name.OscAddress);

                    Assert.AreEqual(true, oscAddressManager.Contains(address + "/name"));
                    Assert.AreEqual(true, oscAddressManager.Contains(address + "/test-member1"));

                    List<object> arguments = new List<object>(); 
                    for (int i = 0; i <= 8; i++)
                    {
                        Assert.AreEqual(true, oscAddressManager.Contains(address + "/test-method" + i));

                        oscAddressManager.Invoke(new OscMessage(address + "/test-method" + i, arguments.ToArray()));

                        arguments.Add(i + 1); 
                    }                    
                }
            }
        }

        //[Test]
        //public void NamespaceRootTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void AttachTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void DetachTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void FindObjectTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void FindObjectTest1()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void FindObjectsTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void SendTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void SendTest1()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void TypeOfTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void UsageTest()
        //{
        //    Assert.Fail();
        //}

        //[Test]
        //public void UsageTest1()
        //{
        //    Assert.Fail();
        //}
    }
}
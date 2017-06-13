using System;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    [Name("Tests.TestObject")]
    public class NamespaceTestObject : INamespaceObject
    {
        [OscMember]
        public Name Name { get; } = new Name("/unnamed");

        [OscMember]
        public string TestMember1 { get; set; }

        public NamespaceTestObject()
        {
        }

        public NamespaceTestObject(string name)
        {
            Name.Value = name;
        }

        public void Load(LoadContext context, System.Xml.XmlNode node)
        {
            OscType.Load(this, context, node);
        }

        public void Save(LoadContext context, System.Xml.XmlElement element)
        {
            OscType.Save(this, context, element);
        }

        [OscMethod()]
        public void State()
        {
            OscType.State(this);
        }

        [OscMethod]
        public void TestMethod_Int(int i)
        {
        }

        [OscMethod]
        public void TestMethod_String(string str)
        {
        }

        [OscMethod]
        public void TestMethod0()
        {
        }

        [OscMethod]
        public void TestMethod1(int i1)
        {
        }

        [OscMethod]
        public void TestMethod2(int i1, int i2)
        {
        }

        [OscMethod]
        public void TestMethod3(int i1, int i2, int i3)
        {
        }

        [OscMethod]
        public void TestMethod4(int i1, int i2, int i3, int i4)
        {
        }

        [OscMethod]
        public void TestMethod5(int i1, int i2, int i3, int i4, int i5)
        {
        }

        [OscMethod]
        public void TestMethod6(int i1, int i2, int i3, int i4, int i5, int i6)
        {
        }

        [OscMethod]
        public void TestMethod7(int i1, int i2, int i3, int i4, int i5, int i6, int i7)
        {
        }

        [OscMethod]
        public void TestMethod8(int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8)
        {
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
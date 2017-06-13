using System;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    [Name("Tests.TestNamespace")]
    public class NamespaceTestNamespace : INamespaceObject
    {
        [OscMember]
        public readonly string TestMember4 = "THIS IS THE VALUE OF TestMember4";

        [OscMember]
        public bool TestMember_Bool1 = true;

        [OscMember]
        public double TestMember_Double1 = 123d;

        [OscMember]
        public float TestMember_Float1 = 123f;

        [OscMember]
        public int TestMember_Int1 = 123;

        [OscMember]
        public long TestMember_Long1 = 123L;

        [OscMember]
        public string TestMember3 = "THIS IS THE VALUE OF TestMember3";

        [OscMember]
        public Name Name { get; } = new Name("/unnamed");

        [OscNamespace]
        public Namespace Namespace { get; }

        [OscMember]
        public bool TestMember_Bool2 { get; set; } = true;

        [OscMember]
        public double TestMember_Double2 { get; set; } = 123d;

        [OscMember]
        public float TestMember_Float2 { get; set; } = 123f;

        [OscMember]
        public int TestMember_Int2 { get; set; } = 123;

        [OscMember]
        public long TestMember_Long2 { get; set; } = 123L;

        [OscMember]
        public string TestMember1 { get; set; }

        [OscMember]
        public string TestMember2 { get; private set; } = "THIS IS THE VALUE OF TestMember2";

        public NamespaceTestNamespace()
        {
            Namespace = new Namespace(Name);
        }

        public NamespaceTestNamespace(string address)
        {
            Namespace = new Namespace(Name);
            Name.Value = address;
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
            return Name.ToString() + " (" + Namespace.Count + ")";
        }
    }
}
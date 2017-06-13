using System;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    [Name("Tests.TestObject2")]
    public class NamespaceTestObject2 : INamespaceObject
    {
        [OscMember]
        public Name Name { get; private set; } = new Name("/unnamed");

        [OscMember]
        public NamespaceTestObject TestLoadableMember1 { get; set; } = new NamespaceTestObject("/test-loadable-member1");

        [OscMember]
        public NamespaceTestObject TestLoadableMember2 { get; private set; } = new NamespaceTestObject("/test-loadable-member2");

        public void Load(LoadContext context, System.Xml.XmlNode node)
        {
            OscType.Load(this, context, node);
        }

        public void Save(LoadContext context, System.Xml.XmlElement element)
        {
            OscType.Save(this, context, element);
        }

        public void State()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
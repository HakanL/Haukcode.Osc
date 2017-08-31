using System;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    [Name("Tests.TestObject2")]
    public class NamespaceTestObject2 : INamespaceObject
    {
        private NamespaceTestObject testLoadableMember1;

        [OscMember]
        public Name Name { get; } = new Name("/unnamed");

        [OscNamespace, NonLoadable]
        public Namespace Namespace { get; }

        [OscMember]
        public NamespaceTestObject TestLoadableMember1 
        {
            get { return testLoadableMember1; }
            set
            {
                if (testLoadableMember1 != null)
                {
                    Namespace.Remove(testLoadableMember1);
                }

                testLoadableMember1 = value;

                if (testLoadableMember1 != null)
                {
                    Namespace.Add(testLoadableMember1);
                }
            }
        }

        [OscMember]
        public NamespaceTestObject TestLoadableMember2 { get; } = new NamespaceTestObject("/test-loadable-member2");

        public NamespaceTestObject2()
        {
            Namespace = new Namespace(Name)
            {
                TestLoadableMember2,
            };

            TestLoadableMember1 = new NamespaceTestObject("/test-loadable-member1");
        }

        public void Load(LoadContext context, System.Xml.Linq.XElement node)
        {
            OscType.Load(this, context, node);
        }

        public void Save(LoadContext context, System.Xml.Linq.XElement element)
        {
            OscType.Save(this, context, element);
        }

        [OscMethod()]
        public void State()
        {
            OscType.State(this);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
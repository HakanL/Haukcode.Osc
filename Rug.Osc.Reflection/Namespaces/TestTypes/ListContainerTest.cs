using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.TestTypes
{
    [Name("ItemListContainerTest")]
    public class ItemListContainerTest : INamespaceObject
    {
        [OscMember]
        public Name Name { get; } = new Name("/Unamed");

        [OscNamespace, NonLoadable] 
        public Namespace Namespace { get; }

        public Collections.List<ItemTest> List { get; }

        public ItemListContainerTest()
        {
            Namespace = new Namespace(Name);

            List = new Collections.List<ItemTest>(Namespace); 
        }

        public void Load(LoadContext context, XElement node)
        {
            OscType.Load(this, context, node);

            List.AddRange(Loader.LoadObjects<ItemTest>(context, node, LoaderMode.UnknownNodesError));
        }

        public void Save(LoadContext context, XElement element)
        {
            OscType.Save(this, context, element);

            Loader.SaveObjects(context, element, List); 
        }

        [OscMethod]
        public void State()
        {
            OscType.State(this);
        }
    }


    [Name("ItemTest")]
    public class ItemTest : INamespaceObject
    {
        [OscMember]
        public Name Name { get; } = new Name("/Unamed");

        [OscMember]
        public float Value { get; set; }

        public void Load(LoadContext context, XElement node)
        {
            OscType.Load(this, context, node);
        }

        public void Save(LoadContext context, XElement element)
        {
            OscType.Save(this, context, element);
        }

        [OscMethod]
        public void State()
        {
            OscType.State(this);
        }
    }
}

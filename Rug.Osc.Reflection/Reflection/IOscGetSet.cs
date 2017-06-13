using System;
using System.Xml;
using Rug.Loading;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Reflection
{
    public interface IOscGetSet : IOscMember
    {
        string[] Alias { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        bool IsExcluded { get; }

        bool IsRequired { get; }

        bool IsNamespace { get; }

        string Namespace { get; }

        bool IsLoadable { get; }

        IOscSerializer OscSerializer { get; }

        Type ValueType { get; }

        void AppendAttributeAndValue(object instance, LoadContext context, XmlElement element);

        object Get(object instance);

        void GetAttributeValue(object instance, LoadContext context, XmlNode node);

        void Set(object instance, object value);
    }
}
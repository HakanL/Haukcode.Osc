using System;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    [Name("Tests.TestObjectBool")]
    public class NamespaceTestObjectBool : NamespaceTestObjectGeneric<bool>
    {
        public const bool Value1Default = true;
        public const bool Value2Default = true;
        public const bool Value3Default = true;
        public const bool Value4Default = true;

        public NamespaceTestObjectBool() : base(Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }

        public NamespaceTestObjectBool(string address) : base(address, Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }
    }

    [Name("Tests.TestObjectDouble")]
    public class NamespaceTestObjectDouble : NamespaceTestObjectGeneric<double>
    {
        public const double Value1Default = 1234.567d;
        public const double Value2Default = 1234.567d;
        public const double Value3Default = 1234.567d;
        public const double Value4Default = 1234.567d;

        public NamespaceTestObjectDouble() : base(Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }

        public NamespaceTestObjectDouble(string address) : base(address, Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }
    }

    [Name("Tests.TestObjectFloat")]
    public class NamespaceTestObjectFloat : NamespaceTestObjectGeneric<float>
    {
        public const float Value1Default = 1234.567f;
        public const float Value2Default = 1234.567f;
        public const float Value3Default = 1234.567f;
        public const float Value4Default = 1234.567f;

        public NamespaceTestObjectFloat() : base(Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }

        public NamespaceTestObjectFloat(string address) : base(address, Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }
    }

    [Name("Tests.TestObjectGeneric")]
    public class NamespaceTestObjectGeneric<T> : NamespaceTestObjectGenericBase
    {
        [OscMember]
        public readonly T Value3;

        [OscMember]
        public T Value4;

        [OscMember]
        public T Value1 { get; private set; }

        [OscMember]
        public T Value2 { get; set; }

        public NamespaceTestObjectGeneric(T value1, T value2, T value3, T value4) : base()
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }

        public NamespaceTestObjectGeneric(string address, T value1, T value2, T value3, T value4) : base(address)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
            Value4 = value4;
        }

        public void Load(LoadContext context, System.Xml.Linq.XElement node)
        {
            OscType.Load(this, context, node);
        }

        public void Save(LoadContext context, System.Xml.Linq.XElement element)
        {
            OscType.Save(this, context, element);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    [Name("Tests.TestObjectGenericBase")]
    public class NamespaceTestObjectGenericBase : INamespaceObject
    {
        [OscMember]
        public Name Name { get; private set; }

        [OscNamespace]
        public Namespace Namespace { get; } = null;

        [OscMember]
        public NamespaceTestObject TestLoadableMember1 { get; set; } = new NamespaceTestObject("/test-loadable-member1");

        [OscMember]
        public NamespaceTestObject TestLoadableMember2 { get; private set; } = new NamespaceTestObject("/test-loadable-member2");

        public NamespaceTestObjectGenericBase()
        {
            Name = new Name("/unnamed");
        }

        public NamespaceTestObjectGenericBase(string address)
        {
            Name = new Name(address);
        }

        public void Load(LoadContext context, System.Xml.Linq.XElement node)
        {
            OscType.Load(this, context, node);
        }

        public void Save(LoadContext context, System.Xml.Linq.XElement element)
        {
            OscType.Save(this, context, element);
        }

        [OscMethod]
        public void State()
        {
            OscType.State(this);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    [Name("Tests.TestObjectInt")]
    public class NamespaceTestObjectInt : NamespaceTestObjectGeneric<int>
    {
        public const int Value1Default = 1234;
        public const int Value2Default = 1234;
        public const int Value3Default = 1234;
        public const int Value4Default = 1234;

        public NamespaceTestObjectInt() : base(Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }

        public NamespaceTestObjectInt(string address) : base(address, Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }
    }

    [Name("Tests.TestObjectLong")]
    public class NamespaceTestObjectLong : NamespaceTestObjectGeneric<long>
    {
        public const long Value1Default = 1234L;
        public const long Value2Default = 1234L;
        public const long Value3Default = 1234L;
        public const long Value4Default = 1234L;

        public NamespaceTestObjectLong() : base(Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }

        public NamespaceTestObjectLong(string address) : base(address, Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }
    }

    [Name("Tests.TestObjectString")]
    public class NamespaceTestObjectString : NamespaceTestObjectGeneric<string>
    {
        public const string Value1Default = "READONLY PROPERTY TEST STRING";
        public const string Value2Default = "PROPERTY TEST STRING";
        public const string Value3Default = "READONLY FIELD TEST STRING";
        public const string Value4Default = "FIELD TEST STRING";

        public NamespaceTestObjectString() : base(Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }

        public NamespaceTestObjectString(string address) : base(address, Value1Default, Value2Default, Value3Default, Value4Default)
        {
        }
    }
}
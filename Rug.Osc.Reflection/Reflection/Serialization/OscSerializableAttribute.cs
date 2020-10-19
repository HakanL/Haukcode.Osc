using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Reflection.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OscSerializableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class OscValueAttribute : Attribute
    {
        public string Usage { get; set; } = string.Empty;

        public OscValueAttribute(string usage = "")
        {
            Usage = usage;
        }
    }
}

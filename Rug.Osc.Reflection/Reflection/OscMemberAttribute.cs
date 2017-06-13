using System;

namespace Rug.Osc.Reflection
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class OscMemberAttribute : Attribute
    {
        public string OscAddress { get; set; }

        public string Usage { get; set; } = string.Empty; 

        public OscMemberAttribute(string oscAddress = null, string usage = "")
        {
            OscAddress = oscAddress;
            Usage = usage;
        }
    }
}
using System;

namespace Rug.Osc.Reflection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OscMethodAttribute : Attribute
    {
        public string OscAddress { get; set; }

        public string Usage { get; set; } = string.Empty;

        public OscMethodAttribute(string oscAddress = null, string usage = "")
        {
            OscAddress = oscAddress;
            Usage = usage;
        }
    }
}
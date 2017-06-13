using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Reflection
{
    [AttributeUsage(AttributeTargets.Method)]
    public class OscOutputAttribute : OscMethodAttribute
    {
        public OscOutputAttribute(string oscAddress = null, string usage = "") : base(oscAddress, usage) 
        {
        }
    }
}

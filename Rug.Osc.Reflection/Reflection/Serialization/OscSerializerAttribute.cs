using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Reflection.Serialization
{    
    public class OscSerializerAttribute : Attribute 
    {
        public readonly Type OscSerializerType; 

        public OscSerializerAttribute(Type oscSerializerType)
        {
            OscSerializerType = oscSerializerType; 
        }
    }
}

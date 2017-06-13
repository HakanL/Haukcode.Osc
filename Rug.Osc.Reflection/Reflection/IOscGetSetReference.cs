using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Reflection
{
    interface IOscGetSetReference : IOscGetSet
    {
        IOscGetSet SerializableValue { get; }
    }
}

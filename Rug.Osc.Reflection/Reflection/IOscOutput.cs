using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Reflection
{
    public interface IOscOutput : IOscMember
    {
        IList<Type> ArgumentTypes { get; }

        IList<string> ArgumentNames { get; }
    }
}

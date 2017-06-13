using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rug.Osc.Reflection
{
    public interface IOscMethod : IOscMember
    {
        IList<Type> ArgumentTypes { get; }

        IList<string> ArgumentNames { get; }

        MethodInfo Method { get; }
    }
}

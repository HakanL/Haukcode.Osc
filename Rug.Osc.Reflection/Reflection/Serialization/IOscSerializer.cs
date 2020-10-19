using System;
using System.Collections.Generic;
using System.Text;
using Rug.Loading;

namespace Rug.Osc.Reflection.Serialization
{
    public interface IOscSerializer
    {
        Type SerializerType { get; } 

        int ArgumentCount { get; }

        Type[] ArgumentTypes { get; }

        string[] ArgumentNames { get; }

        bool FromMessage(OscMessage message, ref int argumentIndex, out object value, out string errorMessage);

        bool ToMessage(object[] arguments, ref int argumentIndex, object value, out string errorMessage);

        string ToString(object value);

        bool TryFromString(LoadContext context, string valueString, out object value);
    }

    public interface IOscSerializer<T> : IOscSerializer
    {
        bool FromMessage(OscMessage message, ref int argumentIndex, out T value, out string errorMessage);

        bool ToMessage(object[] arguments, ref int argumentIndex, T value, out string errorMessage);

        string ToString(T value);

        bool TryFromString(LoadContext context, string valueString, out T value);
    }
}

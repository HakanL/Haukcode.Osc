using System;
using System.Globalization;
using Rug.Loading;

namespace Rug.Osc.Reflection.Serialization
{
    internal class BoolSerializer : SimpleOscSerializer<bool>
    {
        public override string ToString(bool value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool TryFromString(LoadContext context, string valueString, out bool value)
        {
            return bool.TryParse(valueString, out value);
        }
    }

    internal class DoubleSerializer : SimpleOscSerializer<double>
    {
        public override string ToString(double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool TryFromString(LoadContext context, string valueString, out double value)
        {
            return double.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }

    internal class FloatSerializer : SimpleOscSerializer<float>
    {
        public override string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool TryFromString(LoadContext context, string valueString, out float value)
        {
            return float.TryParse(valueString, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }

    internal class IntSerializer : SimpleOscSerializer<int>
    {
        public override string ToString(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool TryFromString(LoadContext context, string valueString, out int value)
        {
            return int.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
    }

    internal class LongSerializer : SimpleOscSerializer<long>
    {
        public override string ToString(long value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool TryFromString(LoadContext context, string valueString, out long value)
        {
            return long.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }
    }

    internal class SerializerHelpers
    {
        public const string TooFewArguments = "Too few arguments.";
        public const string UnexpectedArgument = "Argument {0} is {1} expected {2}.";
    }

    internal abstract class SimpleOscSerializer<T> : IOscSerializer<T>
    {
        public int ArgumentCount => 1;

        public Type[] ArgumentTypes { get; } = { typeof(T) };

        public string[] ArgumentNames { get; } = { };

        public Type SerializerType { get; } = typeof(T);

        public bool FromMessage(OscMessage message, ref int argumentIndex, out object value, out string errorMessage)
        {
            T valueTyped;

            bool result = FromMessage(message, ref argumentIndex, out valueTyped, out errorMessage);

            value = valueTyped;

            return result;
        }

        public bool FromMessage(OscMessage message, ref int argumentIndex, out T value, out string errorMessage)
        {
            if (argumentIndex + 1 > message.Count)
            {
                errorMessage = SerializerHelpers.TooFewArguments; value = default(T); return false;
            }

            if ((message[argumentIndex] is T) == false)
            {
                if ((message[argumentIndex] is OscNull) == false)
                {
                    errorMessage = string.Format(SerializerHelpers.UnexpectedArgument, argumentIndex,
                        Loader.GetTypeName(message[argumentIndex].GetType()),
                        Loader.GetTypeName(typeof(T))
                        );
                    value = default(T); return false;
                }

                value = default(T);
                argumentIndex++;
            }
            else
            {
                value = (T)message[argumentIndex++];
            }

            errorMessage = string.Empty;

            return true;
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, object value, out string errorMessage)
        {
            return ToMessage(arguments, ref argumentIndex, (T)value, out errorMessage);
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, T value, out string errorMessage)
        {
            if (argumentIndex + 1 > arguments.Length)
            {
                errorMessage = SerializerHelpers.TooFewArguments; return false;
            }

            if (value == null)
            {
                arguments[argumentIndex++] = OscNull.Value;
            }
            else
            {
                arguments[argumentIndex++] = value;
            }

            errorMessage = string.Empty;

            return true;
        }

        public string ToString(object value)
        {
            return ToString((T)value);
        }

        public abstract string ToString(T value);

        public bool TryFromString(LoadContext context, string valueString, out object value)
        {
            T tValue;

            bool result = TryFromString(context, valueString, out tValue);

            value = tValue;

            return result;
        }

        public abstract bool TryFromString(LoadContext context, string valueString, out T value);
    }

    internal class StringSerializer : SimpleOscSerializer<string>
    {
        public override string ToString(string value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool TryFromString(LoadContext context, string valueString, out string value)
        {
            value = valueString;
            return true;
        }
    }
}
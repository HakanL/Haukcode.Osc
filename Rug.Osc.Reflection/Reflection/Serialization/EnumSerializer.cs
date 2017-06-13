using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Rug.Loading;

namespace Rug.Osc.Reflection.Serialization
{
    public class EnumSerializer<TEnum> : IOscSerializer<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        private const string TooFewArguments = "Too few arguments.";
        private const string UnexpectedArgument = "Argument {0} is {1} expected {2}.";

        public int ArgumentCount => 1;

        public string[] ArgumentNames { get; } = { };

        public Type[] ArgumentTypes { get; }

        public Type SerializerType { get; } = typeof(TEnum);

        public bool AsInt { get; }

        public EnumSerializer(bool asInt = false)
        {
            if (typeof(TEnum).IsEnum == false)
            {
                throw new ArgumentException($@"Type {Loader.GetTypeName(typeof(TEnum))} is not an enumeration type.", nameof(TEnum));
            }

            AsInt = asInt;

            ArgumentTypes = new[] { AsInt ? typeof(int) : typeof(string) };
        }

        public bool FromMessage(OscMessage message, ref int argumentIndex, out object value, out string errorMessage)
        {
            TEnum valueTyped;

            bool result = FromMessage(message, ref argumentIndex, out valueTyped, out errorMessage);

            value = valueTyped;

            return result;
        }

        public bool FromMessage(OscMessage message, ref int argumentIndex, out TEnum value, out string errorMessage)
        {
            errorMessage = string.Empty;
            value = default(TEnum);

            if (argumentIndex + 1 > message.Count)
            {
                errorMessage = TooFewArguments;
                return false;
            }

            if ((message[argumentIndex] is int) == true)
            {
                value = (TEnum)message[argumentIndex];
                return true;
            }

            if ((message[argumentIndex] is string) == true)
            {
                if (TryFromString(message[argumentIndex].ToString(), out value) == true)
                {
                    return true;
                }

                errorMessage = $"Could not parse {Loader.GetTypeName(typeof(TEnum))} invalid value.";

                return false; 
            }

            errorMessage = string.Format(UnexpectedArgument, argumentIndex,
                            Loader.GetTypeName(message[argumentIndex].GetType()),
                            Loader.GetTypeName(AsInt ? typeof(int) : typeof(string))
                            );

            return false;
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, object value, out string errorMessage)
        {
            return ToMessage(arguments, ref argumentIndex, (TEnum)value, out errorMessage);
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, TEnum value, out string errorMessage)
        {
            if (argumentIndex + 1 > arguments.Length)
            {
                errorMessage = TooFewArguments; return false;
            }

            if (AsInt == true)
            {
                arguments[argumentIndex++] = Convert.ToInt32(value);
            }
            else
            {
                arguments[argumentIndex++] = ToString(value);
            }

            errorMessage = string.Empty;

            return true;
        }

        public string ToString(TEnum value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public string ToString(object value)
        {
            if ((value is TEnum) == false)
            {
                throw new ArgumentException($@"Value is not a {Loader.GetTypeName(typeof(TEnum))}", nameof(value));
            }

            return ToString((TEnum)value);
        }

        public bool TryFromString(LoadContext context, string valueString, out TEnum value)
        {
            try
            {
                value = (TEnum)Enum.Parse(typeof(TEnum), valueString, true);

                return true;
            }
            catch (Exception ex)
            {
                context.Error($"Exception while parsing {Loader.GetTypeName(typeof(TEnum))}. {ex.Message}", true, ex);

                value = default(TEnum);

                return false;
            }
        }

        public bool TryFromString(string valueString, out TEnum value)
        {
            try
            {
                value = (TEnum)Enum.Parse(typeof(TEnum), valueString, true);

                return true;
            }
            catch (Exception ex)
            {
                value = default(TEnum);

                return false;
            }
        }

        public bool TryFromString(LoadContext context, string valueString, out object value)
        {
            TEnum typedValue;

            bool result = TryFromString(context, valueString, out typedValue);

            value = typedValue;

            return result;
        }
    }
}

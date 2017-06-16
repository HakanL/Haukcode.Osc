using Rug.Loading;
using System;

namespace Rug.Osc.Reflection.Serialization
{
    internal class UriSerializer : IOscSerializer<Uri>
    {
        public int ArgumentCount => 1;

        public string[] ArgumentNames { get; } = { };

        public Type[] ArgumentTypes { get; } = { typeof(string) };

        public Type SerializerType { get; } = typeof(Uri);

        public bool FromMessage(OscMessage message, ref int argumentIndex, out object value, out string errorMessage)
        {
            Uri valueTyped;

            bool result = FromMessage(message, ref argumentIndex, out valueTyped, out errorMessage);

            value = valueTyped;

            return result;
        }

        public bool FromMessage(OscMessage message, ref int argumentIndex, out Uri value, out string errorMessage)
        {
            if (argumentIndex + 1 > message.Count)
            {
                errorMessage = SerializerHelpers.TooFewArguments; value = default(Uri); return false;
            }

            if ((message[argumentIndex] is string) == false)
            {
                if ((message[argumentIndex] is OscNull) == false)
                {
                    errorMessage = string.Format(SerializerHelpers.UnexpectedArgument, argumentIndex,
                        Loader.GetTypeName(message[argumentIndex].GetType()),
                        Loader.GetTypeName(typeof(Uri))
                        );
                    value = default(Uri); return false;
                }

                value = default(Uri);
                argumentIndex++;
            }
            else
            {
                value = new Uri((string)message[argumentIndex++]);
            }

            errorMessage = string.Empty;

            return true;
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, object value, out string errorMessage)
        {
            return ToMessage(arguments, ref argumentIndex, (Uri)value, out errorMessage);
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, Uri value, out string errorMessage)
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
                arguments[argumentIndex++] = value.ToString();
            }

            errorMessage = string.Empty;

            return true;
        }

        public string ToString(object value)
        {
            return ToString((Uri)value);
        }

        public string ToString(System.Uri value)
        {
            return value.ToString();
        }

        public bool TryFromString(LoadContext context, string valueString, out object value)
        {
            Uri tValue;

            bool result = TryFromString(context, valueString, out tValue);

            value = tValue;

            return result;
        }

        public bool TryFromString(LoadContext context, string valueString, out System.Uri value)
        {
            if (Uri.IsWellFormedUriString(valueString, UriKind.RelativeOrAbsolute) == false)
            {
                value = default(Uri);
                return false;
            }

            value = new System.Uri(valueString);

            return true;
        }
    }
}
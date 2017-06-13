using System;
using System.Collections.Generic;
using System.Text;
using Rug.Loading;

namespace Rug.Osc.Reflection.Serialization
{
    public class ArraySerializer<T> : IOscSerializer<T[]>
    {
        public int ArgumentCount => 1;

        public Type[] ArgumentTypes { get; } = { typeof(T[]) };

        public string[] ArgumentNames { get; } = { };

        public Type SerializerType { get; } = typeof(T[]);

        private readonly IOscSerializer<T> elementOscSerializer;

        public ArraySerializer()
        {
            if (typeof(T) == typeof(T[]))
            {
                throw new ArgumentException($@"Cannot create array serializer for type {Loader.GetTypeName(typeof(T))}.", nameof(T)); 
            }

            elementOscSerializer = OscSerializer.GetOscSerializer<T>(); 
        }

        public bool FromMessage(OscMessage message, ref int argumentIndex, out object value, out string errorMessage)
        {
            T[] valueTyped;

            bool result = FromMessage(message, ref argumentIndex, out valueTyped, out errorMessage);

            value = valueTyped;

            return result;
        }

        public bool FromMessage(OscMessage message, ref int argumentIndex, out T[] value, out string errorMessage)
        {
            if (argumentIndex + 1 > message.Count)
            {
                errorMessage = SerializerHelpers.TooFewArguments; value = default(T[]);
                return false;
            }

            errorMessage = string.Empty;

            if (message[argumentIndex] is T[])
            {
                value = (T[])message[argumentIndex++];

                return true; 
            }

            if (message[argumentIndex] is object[])
            {
                object[] valueObjectArray = (object[])message[argumentIndex++];

                value = new T[valueObjectArray.Length];

                for (int i = 0; i < valueObjectArray.Length; i++)
                {
                    if (valueObjectArray[i] is T)
                    {
                        value[i] = (T)valueObjectArray[i];
                        continue;
                    }

                    errorMessage = string.Format(SerializerHelpers.UnexpectedArgument, $"at index {i} of argumnet {argumentIndex}", Loader.GetTypeName(valueObjectArray[i].GetType()), Loader.GetTypeName(typeof(T)));
                    value = default(T[]);

                    return false;
                }

                return true;
            }

            if ((message[argumentIndex] is OscNull) == false)
            {
                errorMessage = string.Format(SerializerHelpers.UnexpectedArgument, argumentIndex,
                    Loader.GetTypeName(message[argumentIndex].GetType()),
                    Loader.GetTypeName(typeof(T[]))
                    );
                value = default(T[]);

                return false;
            }

            value = default(T[]);
            argumentIndex++;

            return true;
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, object value, out string errorMessage)
        {
            return ToMessage(arguments, ref argumentIndex, (T[])value, out errorMessage);
        }

        public bool ToMessage(object[] arguments, ref int argumentIndex, T[] value, out string errorMessage)
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
                object[] valueObjectArray = new object[value.Length];

                for (int i = 0; i < valueObjectArray.Length; i++)
                {
                    if (value[i] == null)
                    {
                        valueObjectArray[i] = OscNull.Value;
                        continue;
                    }

                    valueObjectArray[i] = value[i]; 
                }

                arguments[argumentIndex++] = valueObjectArray;
            }

            errorMessage = string.Empty;

            return true;
        }

        public string ToString(object value)
        {
            return ToString((T[])value);
        }

        public string ToString(T[] value)
        {
            object[] stringValues = new object[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == null)
                {
                    stringValues[i] = null; 
                    continue;
                }

                stringValues[i] = elementOscSerializer.ToString(value[i]);
            }

            return Helper.ToCsv(stringValues); 
        }

        public bool TryFromString(LoadContext context, string valueString, out object value)
        {
            T[] tValue;

            bool result = TryFromString(context, valueString, out tValue);

            value = tValue;

            return result;
        }

        public bool TryFromString(LoadContext context, string valueString, out T[] value)
        {
            string[] stringValues = Helper.FromCsv(valueString); 

            value = new T[stringValues.Length];

            for (int i = 0; i < stringValues.Length; i++)
            {
                T item;

                if (elementOscSerializer.TryFromString(context, stringValues[i], out item) == false)
                {
                    value = default(T[]);

                    context.Error($@"Could not parse element {i} of array argument. ""{stringValues[i]}"" is not a valid {Loader.GetTypeName(typeof(T))}.");

                    return false; 
                }

                value[i] = item; 
            }

            return true; 
        }
    }
}

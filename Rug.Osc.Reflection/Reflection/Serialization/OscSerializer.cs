using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Rug.Loading;

namespace Rug.Osc.Reflection.Serialization
{
    public static class OscSerializer
    {
        private static readonly object syncLock = new object();
        private static readonly Dictionary<Type, IOscSerializer> oscSerializers = new Dictionary<Type, IOscSerializer>();

        static OscSerializer()
        {
            Register(typeof(Uri), new UriSerializer());
            Register(typeof(string), new StringSerializer());
            Register(typeof(bool), new BoolSerializer());
            Register(typeof(float), new FloatSerializer());
            Register(typeof(double), new DoubleSerializer());
            Register(typeof(int), new IntSerializer());
            Register(typeof(long), new LongSerializer());
            Register(typeof(OscMidiNote), new EnumSerializer<OscMidiNote>(false)); 

            Register(typeof(string[]), new ArraySerializer<string>());
            Register(typeof(bool[]), new ArraySerializer<bool>());
            Register(typeof(float[]), new ArraySerializer<float>());
            Register(typeof(double[]), new ArraySerializer<double>());
            Register(typeof(int[]), new ArraySerializer<int>());
            Register(typeof(long[]), new ArraySerializer<long>());
            Register(typeof(OscMidiNote[]), new ArraySerializer<OscMidiNote>());
        }

        public static OscMessage ToOscMessage<T>(string address, T obj)
        {
            IOscSerializer<T> serializer = GetOscSerializer<T>();

            object[] args = new object[serializer.ArgumentCount];
            int index = 0;
            string errorMessage;

            if (serializer.ToMessage(args, ref index, obj, out errorMessage) == false)
            {
                throw new Exception(errorMessage); 
            }

            return new OscMessage(address, args); 
        }

        public static T FromOscMessage<T>(OscMessage message)
        {
            IOscSerializer<T> serializer = GetOscSerializer<T>();

            T result = default(T);
            int index = 0;
            string errorMessage;

            if (serializer.FromMessage(message, ref index, out result, out errorMessage) == false)
            {
                throw new Exception(errorMessage);
            }

            return result; 
        }

        public static IOscSerializer<T> GetOscSerializer<T>()
        {
            return GetOscSerializer(typeof(T)) as IOscSerializer<T>;
        }

        public static IOscSerializer GetOscSerializer(Type type)
        {
            IOscSerializer oscSerializer;

            if (TryGetOscSerializer(type, out oscSerializer) == false)
            {
                throw new Exception($"No OSC serialize found for the type \"{Loader.GetTypeName(type)}\".");
            }

            return oscSerializer;
        }

        public static bool TryGetOscSerializer<T>(out IOscSerializer<T> oscSerializer)
        {
            IOscSerializer untypedOscSerializer;

            if (TryGetOscSerializer(typeof(T), out untypedOscSerializer) == false)
            {
                oscSerializer = null; 

                return false; 
            }

            oscSerializer = untypedOscSerializer as IOscSerializer<T>;

            return true; 
        }

        public static bool TryGetOscSerializer(Type type, out IOscSerializer oscSerializer)
        {
            lock (syncLock)
            {
                if (oscSerializers.TryGetValue(type, out oscSerializer) == true)
                {
                    return true;
                }

                OscSerializerAttribute oscSerializerAttribute;
                if (Helper.TryGetSingleAttribute(type, true, out oscSerializerAttribute) == false)
                {
                    return false;
                }

                oscSerializer = (IOscSerializer)Activator.CreateInstance(oscSerializerAttribute.OscSerializerType);

                oscSerializers.Add(type, oscSerializer);

                return true;
            }
        }

        public static void Register(Type type, IOscSerializer oscSerializer)
        {
            lock (syncLock)
            {
                oscSerializers.Add(type, oscSerializer);
            }
        }
    }
}

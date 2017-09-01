using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Rug.Loading;

namespace Rug.Osc.Reflection.Serialization
{
    public static class OscSerializer
    {
        //private static readonly object syncLock = new object();
        private static readonly ConcurrentDictionary<Type, IOscSerializer> OscSerializers = new ConcurrentDictionary<Type, IOscSerializer>();

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

            if (serializer.ToMessage(args, ref index, obj, out string errorMessage) == false)
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

            if (serializer.FromMessage(message, ref index, out result, out string errorMessage) == false)
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
            if (TryGetOscSerializer(type, out IOscSerializer oscSerializer) == false)
            {
                throw new Exception($@"No OSC serialize found for the type ""{Loader.GetTypeName(type)}"".");
            }

            return oscSerializer;
        }

        public static bool TryGetOscSerializer<T>(out IOscSerializer<T> oscSerializer)
        {
            if (TryGetOscSerializer(typeof(T), out IOscSerializer untypedOscSerializer) == false)
            {
                oscSerializer = null; 

                return false; 
            }

            oscSerializer = untypedOscSerializer as IOscSerializer<T>;

            return oscSerializer != null; 
        }

        public static bool TryGetOscSerializer(Type type, out IOscSerializer oscSerializer)
        {
            try
            {
                oscSerializer = OscSerializers.GetOrAdd(type, func =>
                {
                    if (Helper.TryGetSingleAttribute(type, true, out OscSerializerAttribute oscSerializerAttribute) == false)
                    {
                        throw new Exception($@"Type ""{Loader.GetTypeName(type)}"" does not support OSC serialization");
                    }

                    return (IOscSerializer) Activator.CreateInstance(oscSerializerAttribute.OscSerializerType);
                });

                return true;
            }
            catch
            {
                oscSerializer = null; 
                return false;
            }
        }

        public static void Register(Type type, IOscSerializer oscSerializer)
        {
            OscSerializers[type] = oscSerializer;
        }
    }
}

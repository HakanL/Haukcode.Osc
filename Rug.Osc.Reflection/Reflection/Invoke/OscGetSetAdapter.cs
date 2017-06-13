using System;
using System.Reflection;
using System.Reflection.Emit;
using Rug.Loading;
using Rug.Osc.Connection;
using Rug.Osc.Namespaces;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Reflection
{
    internal class OscGetSetAdapter<T> : IOscGetSetAdapter
    {
        private delegate bool FromMessage(OscMessage message, ref int argumentIndex, out T value, out string errorMessage);

        private delegate T GetDelegate();

        private delegate void SetDelegate(T value);

        private delegate bool ToMessage(object[] arguments, ref int argumentIndex, T value, out string errorMessage);

        public readonly object Instance;
        public readonly string OscAddress;
        public readonly IOscGetSet OscGetSet;

        private readonly int messageArgumentCount;
        private readonly IOscSerializer<T> oscSerializer;

        private GetDelegate getter;
        private FromMessage parser;
        private ToMessage serializer;
        private SetDelegate setter;

        public bool CanRead { get; }

        public bool CanWrite { get; }         

        public T Value { get { return Get(); } set { Set(value); } }

        public OscGetSetAdapter(string oscAddress, object instance, IOscGetSet oscGetSet, bool cacheOscSerializers)
        {
            OscAddress = oscAddress;
            Instance = instance;
            OscGetSet = oscGetSet;

            if (oscGetSet.MemberInfo is PropertyInfo)
            {
                PropertyInfo propertyInfo = oscGetSet.MemberInfo as PropertyInfo;

                CanRead = OscGetSet.CanRead;
                CanWrite = OscGetSet.CanWrite;

                if (CanRead == true)
                {
                    getter = (GetDelegate)Delegate.CreateDelegate(typeof(GetDelegate), Instance, propertyInfo.GetGetMethod());
                }

                if (CanWrite == true)
                {
                    setter = (SetDelegate)Delegate.CreateDelegate(typeof(SetDelegate), Instance, propertyInfo.GetSetMethod());
                }
            }
            else if (oscGetSet.MemberInfo is FieldInfo)
            {
                FieldInfo fieldInfo = oscGetSet.MemberInfo as FieldInfo;

                CanRead = true;
                CanWrite = (fieldInfo.IsInitOnly || fieldInfo.IsLiteral) != true;

                if (CanRead == true)
                {
                    DynamicMethod dm = new DynamicMethod("get_" + oscGetSet.MemberName, typeof(T), new Type[] { Instance.GetType() }, Instance.GetType());

                    ILGenerator il = dm.GetILGenerator();
                    // Load the instance of the object (argument 0) onto the stack
                    il.Emit(OpCodes.Ldarg_0);

                    // Get
                    // Load the value of the object's field (fieldInfo) onto the stack
                    il.Emit(OpCodes.Ldfld, fieldInfo);
                    // return the value on the top of the stack
                    il.Emit(OpCodes.Ret);

                    getter = (GetDelegate)dm.CreateDelegate(typeof(GetDelegate), Instance);
                }

                if (CanWrite == true)
                {
                    DynamicMethod dm = new DynamicMethod("set_" + oscGetSet.MemberName, typeof(void), new Type[] { Instance.GetType(), typeof(T) }, Instance.GetType());

                    ILGenerator il = dm.GetILGenerator();
                    // Load the instance of the object (argument 0) onto the stack
                    il.Emit(OpCodes.Ldarg_0);

                    // Set
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Stfld, fieldInfo); // store into fieldInfo

                    setter = (SetDelegate)dm.CreateDelegate(typeof(SetDelegate), Instance);
                }
            }

            if (cacheOscSerializers == true && OscSerializer.TryGetOscSerializer<T>(out oscSerializer) == true)
            {
                messageArgumentCount = oscSerializer.ArgumentCount;

                if (CanRead == true)
                {
                    serializer = oscSerializer.ToMessage;
                }

                if (CanWrite == true)
                {
                    parser = oscSerializer.FromMessage;
                }
            }
        }

        public void Dispose()
        {
            getter = null;
            setter = null;

            serializer = null;
            parser = null;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get()
        {
            return getter();
        }

        public void Invoke(OscMessage message)
        {
            try
            {
                if (message.Count == 0)
                {
                    if (CanRead == false)
                    {
                        OscConnection.Send(OscMessages.ObjectError(OscAddress, "Property cannot be read from."));
                        return;
                    }

                    State();
                }
                else
                {
                    if (CanWrite == false)
                    {
                        OscConnection.Send(OscMessages.ObjectError(OscAddress, "Property cannot be written to."));
                        return;
                    }

                    int index = 0;

                    T value;
                    string errorString;

                    if (parser(message, ref index, out value, out errorString) == false)
                    {
                        OscConnection.Send(OscMessages.ObjectError(OscAddress, errorString));
                        return;
                    }

                    Set(value);

                    if (CanRead == true)
                    {
                        State();
                    }
                }
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        object IOscGetSetAdapter.Get()
        {
            return Get();
        }

        void IOscGetSetAdapter.Set(object value)
        {
            Set((T)value);
        }

        public void State()
        {
            T value = Get();

            if (value == null)
            {
                OscConnection.Send(OscAddress, OscNull.Value);
                return;
            }

            if (value is INamespaceObject)
            {
                // TODO Determine if this is needed or not 
                //(value as INamespaceObject).Send();

                return;
            }

            if (serializer == null)
            {
                throw new Exception($"Type {Loader.GetTypeName(typeof(T))} does not have a serializer.");
            }

            object[] arguments = new object[messageArgumentCount];
            int argumentIndex = 0;
            string errorMessage;

            if (serializer(arguments, ref argumentIndex, value, out errorMessage) == false)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, errorMessage));

                return;
            }

            OscConnection.Send(OscAddress, arguments);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(T value)
        {
            setter(value);
        }

        public void TypeOf()
        {
            OscConnection.Send(OscMessages.TypeOf(OscAddress, typeof(T)));
        }

        public void Usage()
        {
            OscMessages.MemberAccess propertyAccess =
                (CanRead ? OscMessages.MemberAccess.Get : OscMessages.MemberAccess.Method) |
                (CanWrite ? OscMessages.MemberAccess.Set : OscMessages.MemberAccess.Method); 

            if (oscSerializer != null && oscSerializer.ArgumentCount > 1)
            {
                OscConnection.Send(OscMessages.Usage(OscAddress, OscGetSet.Usage, propertyAccess, Instance.GetType(), oscSerializer.ArgumentTypes, oscSerializer.ArgumentNames));
            }
            else
            {
                OscConnection.Send(OscMessages.Usage(OscAddress, OscGetSet.Usage, propertyAccess, Instance.GetType(), new[] { typeof(T) }, new[] { OscGetSet.MemberName }));
            }
        }
    }
}
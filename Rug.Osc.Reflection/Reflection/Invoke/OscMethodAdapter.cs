using System;
using Rug.Osc.Connection;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Reflection
{
    internal class OscMethodAdapter : OscMethodAdapterBase
    {
        private delegate void MethodDelegate();

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[0])
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                @delegate?.Invoke();
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex-1}"));
                    Usage(); 
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2, T3> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2, T3 t3);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2), typeof(T3) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;
                T3 t3;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t3) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2, t3);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2, T3, T4> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2, T3 t3, T4 t4);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;
                T3 t3;
                T4 t4;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t3) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t4) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2, t3, t4);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2, T3, T4, T5> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;
                T3 t3;
                T4 t4;
                T5 t5;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t3) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t4) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t5) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2, t3, t4, t5);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2, T3, T4, T5, T6> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;
                T3 t3;
                T4 t4;
                T5 t5;
                T6 t6;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t3) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t4) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t5) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t6) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2, t3, t4, t5, t6);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2, T3, T4, T5, T6, T7> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;
                T3 t3;
                T4 t4;
                T5 t5;
                T6 t6;
                T7 t7;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t3) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t4) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t5) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t6) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t7) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2, t3, t4, t5, t6, t7);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal class OscMethodAdapter<T1, T2, T3, T4, T5, T6, T7, T8> : OscMethodAdapterBase
    {
        private delegate void MethodDelegate(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8);

        private MethodDelegate @delegate;

        public OscMethodAdapter(string oscAddress, object instance, IOscMethod method) : base(oscAddress, instance, method, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) })
        {
            @delegate = CreateMethodDelegate();
        }

        public override void Dispose()
        {
            @delegate = null;
        }

        public override void Invoke(OscMessage message)
        {
            try
            {
                int messageArgumentIndex = 0;
                int typeIndex = 0;

                T1 t1;
                T2 t2;
                T3 t3;
                T4 t4;
                T5 t5;
                T6 t6;
                T7 t7;
                T8 t8;

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t1) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t2) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t3) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t4) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t5) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t6) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t7) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                if (ParseArguments(message, typeIndex++, ref messageArgumentIndex, out t8) == false)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, $"Invalid argument {typeIndex - 1}"));
                    Usage();
                    return;
                }

                @delegate?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        private MethodDelegate CreateMethodDelegate()
        {
            return (MethodDelegate)Delegate.CreateDelegate(typeof(MethodDelegate), Instance, OscMethod.Method);
        }
    }

    internal abstract class OscMethodAdapterBase : IOscMemberAdapter
    {
        public readonly string OscAddress;
        public readonly object Instance;
        public readonly IOscMethod OscMethod;

        protected readonly IOscSerializer[] Serializers;
        protected readonly int TotalMessageArgumentCount;

        //public Type MemberType => typeof(void);

        //public OscType MemberOscType => null; 

        public OscMethodAdapterBase(string oscAddress, object instance, IOscMethod method, Type[] types)
        {
            OscAddress = oscAddress;

            Instance = instance;

            OscMethod = method;

            Serializers = new IOscSerializer[types.Length];
            int parserIndex = 0;

            foreach (Type type in types)
            {
                Serializers[parserIndex++] = OscSerializer.GetOscSerializer(type);
            }
        }

        public abstract void Dispose();

        public void State() { }

        public abstract void Invoke(OscMessage message);

        public virtual void Usage()
        {
            OscConnection.Send(OscMessages.Usage(OscAddress, OscMethod.Usage, OscMessages.MemberAccess.Method, Instance.GetType(), OscMethod.ArgumentTypes, OscMethod.ArgumentNames));
        }

        protected bool ParseArguments<T>(OscMessage message, int typeIndex, ref int messageArgumentIndex, out T value)
        {
            object @object;
            string errorMessage;

            if (Serializers[typeIndex].FromMessage(message, ref messageArgumentIndex, out @object, out errorMessage) == false)
            {
                value = default(T);

                return false;
            }

            value = (T)@object;

            return true;
        }

        public void TypeOf()
        {
            OscConnection.Send(OscMessages.TypeOf(OscAddress, typeof(void)));
        }
    }
}
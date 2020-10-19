using System;
using Rug.Osc.Connection;

namespace Rug.Osc.Reflection
{
    internal class OscGetSetReferenceAdapter<T> : IOscMemberAdapter where T : class
    {
        public readonly bool CanRead;
        public readonly bool CanWrite;
        public readonly object Instance;
        public readonly string OscAddress;
        public readonly IOscGetSetReference OscGetSetReference;

        private readonly object syncLock = new object();
        private T cachedInstance = null;
        private IOscGetSetAdapter instanceMemberAdapter;
        private readonly OscGetSetAdapter<T> referenceAdapter;

        public OscGetSetReferenceAdapter(string oscAddress, object instance, IOscGetSetReference oscGetSetReference)
        {
            OscAddress = oscAddress;
            Instance = instance;
            OscGetSetReference = oscGetSetReference;

            CanRead = OscGetSetReference.CanRead;
            CanWrite = OscGetSetReference.CanWrite;

            if (CanRead == false)
            {
                throw new Exception($"Cannot read reference property {oscGetSetReference.MemberName} on {oscAddress}.");
            }

            referenceAdapter = new OscGetSetAdapter<T>(OscAddress, instance, oscGetSetReference, false);
        }

        public void Dispose()
        {
            referenceAdapter.Dispose();
            instanceMemberAdapter?.Dispose();
        }

        public void Invoke(OscMessage message)
        {
            try
            {
                CacheInstanceMemberAdapter();

                if (instanceMemberAdapter == null)
                {
                    OscConnection.Send(OscMessages.ObjectError(OscAddress, "Reference member with null value."));
                    return;
                }

                instanceMemberAdapter?.Invoke(message);
            }
            catch (Exception ex)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, ex.Message));
            }
        }

        public void State()
        {
            CacheInstanceMemberAdapter();

            if (instanceMemberAdapter == null)
            {
                OscConnection.Send(OscMessages.ObjectError(OscAddress, "Reference member with null value."));
                return;
            }

            instanceMemberAdapter?.State();
        }

        public void TypeOf()
        {
            referenceAdapter.TypeOf();

            //OscConnection.Send(OscMessages.TypeOf(OscAddress, OscGetSetReference.SerializableValue.ValueType));
        }

        public void Usage()
        {
            //referenceAdapter.Usage();
            CacheInstanceMemberAdapter();

            OscMessages.MemberAccess propertyAccess;

            if (instanceMemberAdapter == null)
            {
                propertyAccess =
                    (CanRead ? OscMessages.MemberAccess.Get : OscMessages.MemberAccess.Method) |
                    (CanWrite ? OscMessages.MemberAccess.Set : OscMessages.MemberAccess.Method);
            }
            else
            {
                propertyAccess =
                    (instanceMemberAdapter.CanRead ? OscMessages.MemberAccess.Get : OscMessages.MemberAccess.Method) |
                    (instanceMemberAdapter.CanWrite ? OscMessages.MemberAccess.Set : OscMessages.MemberAccess.Method);
            }

            OscConnection.Send(OscMessages.Usage(OscAddress, OscGetSetReference.Usage, propertyAccess, typeof(T), new Type[] { OscGetSetReference.SerializableValue.ValueType }, new string[] { OscGetSetReference.MemberName }));
        }

        private void CacheInstanceMemberAdapter()
        {
            T instance = referenceAdapter.Get();

            if (cachedInstance == instance)
            {
                return;
            }

            lock (syncLock)
            {
                cachedInstance = instance;

                if (cachedInstance == null)
                {
                    instanceMemberAdapter?.Dispose();
                    instanceMemberAdapter = null;
                }

                instanceMemberAdapter = (IOscGetSetAdapter)Activator.CreateInstance(typeof(OscGetSetAdapter<>).MakeGenericType(OscGetSetReference.SerializableValue.ValueType), OscAddress, instance, OscGetSetReference.SerializableValue, true);
            }
        }
    }
}
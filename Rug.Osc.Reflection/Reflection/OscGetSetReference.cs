using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using Rug.Loading;
using Rug.Osc.Connection;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Reflection
{
    internal class OscGetSetReference : IOscGetSetReference
    {
        private IOscGetSet property;

        public string[] Alias { get; private set; }

        public bool CanRead { get; private set; }

        public bool CanWrite { get; private set; }

        public bool IsExcluded { get; private set; }

        public bool IsLoadable { get { return false; } }

        public bool IsNamespace { get { return false; } }

        public string Namespace { get { return null; } }

        public bool IsRequired { get; private set; }

        public MemberInfo MemberInfo { get { return property.MemberInfo; } }

        public string MemberName { get { return property.MemberName; } }

        public string OscAddress { get { return property.OscAddress; } }

        public IOscSerializer OscSerializer { get; private set; }

        public OscType OscType { get { return property.OscType; } }

        public IOscGetSet SerializableValue { get; private set; }

        public string Usage { get { return property.Usage; } }

        public Type ValueType { get { return property.ValueType; } }

        public OscGetSetReference(IOscGetSet property)
        {
            this.property = property;

            OscType type = OscType.GetType(ValueType);

            SerializableValue = type.SerializableValue;

            if (SerializableValue == null)
            {
                throw new ArgumentException($"Missing {nameof(SerializableValue)} on type {type.TypeName} referenced by property {property.MemberName} on type {property.OscType.TypeName}.", nameof(property));
            }

            CanRead = property.CanRead;
            CanWrite = property.CanWrite;

            OscSerializer = null;
        }

        public void AppendAttributeAndValue(object instance, LoadContext context, XmlElement element)
        {
            if (IsExcluded == true)
            {
                return;
            }

            if (CanRead == false)
            {
                return;
            }

            object value = Get(instance);

            string valueValue = SerializableValue.OscSerializer.ToString(value);

            Helper.AppendAttributeAndValue(element, MemberName, valueValue);
        }

        public void Describe()
        {
            property.Describe();
        }

        public object Get(object instance)
        {
            if (CanRead == false)
            {
                throw new Exception($"Cannot get the value of {MemberName} on ({OscType.TypeName}){instance.ToString()}.");
            }

            object refrenceInstance;

            if (MemberInfo is FieldInfo)
            {
                refrenceInstance = (MemberInfo as FieldInfo).GetValue(instance);
            }
            else
            {
                refrenceInstance = (MemberInfo as PropertyInfo).GetValue(instance, null);
            }

            return SerializableValue.Get(refrenceInstance);
        }

        public void GetAttributeValue(object instance, LoadContext context, XmlNode node)
        {
            if (IsExcluded == true)
            {
                return;
            }

            if (SerializableValue.CanWrite == false)
            {
                return;
            }

            bool found = false;
            string valueString;
            if (Helper.TryGetAttributeValue(node, MemberName, out valueString) == true)
            {
                found = true;
            }
            else if (Alias != null)
            {
                found = Alias.Any(alias => Helper.TryGetAttributeValue(node, alias, out valueString));
            }

            if (found == false)
            {
                if (IsRequired == true)
                {
                    context.Error($"Required attribute {OscType.TypeName}.{MemberName} not found on node {node}.", node);
                }

                return;
            }

            object value;
            if (SerializableValue.OscSerializer.TryFromString(context, valueString, out value) == false)
            {
                return;
            }

            Set(instance, value);
        }

        public void Set(object instance, object value)
        {
            if (CanRead == false)
            {
                throw new Exception($"Cannot get the value of {MemberName} on ({OscType.TypeName}){instance}.");
            }

            if (SerializableValue.CanWrite == false)
            {
                throw new Exception(string.Format("Cannot set the value of {0} on ({1}){2}.", SerializableValue.MemberName, SerializableValue.OscType.TypeName, instance.ToString()));
            }

            object refrenceInstance;

            if (MemberInfo is FieldInfo)
            {
                refrenceInstance = (MemberInfo as FieldInfo).GetValue(instance);
            }
            else
            {
                refrenceInstance = (MemberInfo as PropertyInfo).GetValue(instance, null);
            }

            SerializableValue.Set(refrenceInstance, value);
        }

        public override string ToString()
        {
            return OscMessages.PropertyDescriptor(OscType.TypeName, OscAddress, ValueType, CanRead, CanWrite).ToString();
        }
    }
}
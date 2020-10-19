using System;
using System.Reflection;
using System.Xml.Linq;
using Rug.Loading;
using Rug.Osc.Connection;
using Rug.Osc.Namespaces;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Reflection
{
    internal class OscGetSet : IOscGetSet
    {
        private readonly OscMemberAttribute oscMemberAttribute;

        public string[] Alias { get; } = new string[0];

        public bool CanRead { get; }

        public bool CanWrite { get; }

        public bool HasOscSerializer { get; }

        public bool IsExcluded { get; }

        public bool IsNamespace { get; }

        public bool IsRequired { get; }

        public bool IsLoadable { get; } 

        public MemberInfo MemberInfo { get; }

        public string MemberName { get; }

        public string OscAddress { get; }

        public IOscSerializer OscSerializer { get; }

        public OscType OscType { get; }

        public string Usage { get; }

        public Type ValueType { get; }

        public string Namespace { get; }

        public OscGetSet(OscType type, OscMemberAttribute oscMemberAttribute, MemberInfo memberInfo)
        {
            this.oscMemberAttribute = oscMemberAttribute;

            OscType = type;

            NameAttribute name;

            if (Helper.TryGetSingleAttribute(memberInfo, true, out name) == true)
            {
                MemberName = name.Name;
            }
            else
            {
                MemberName = memberInfo.Name;
            }

            Usage = oscMemberAttribute.Usage;

            AliasAttribute aliasAttribute = Helper.GetSingleAttribute<AliasAttribute>(memberInfo, true);

            if (aliasAttribute != null) 
            {
                Alias = aliasAttribute.Alias;
            }

            IsExcluded = Helper.HasAttribute<NonLoadableAttribute>(memberInfo, true);

            IsRequired = Helper.HasAttribute<RequiredLoadableAttribute>(memberInfo, true);

            OscNamespaceAttribute @namespace = Helper.GetSingleAttribute<OscNamespaceAttribute>(memberInfo, true);

            IsNamespace = @namespace != null;
            Namespace = @namespace?.Namesapce; 

            OscAddress = string.IsNullOrEmpty(oscMemberAttribute.OscAddress) ? "/" + Helper.ToLowerSplitWithHyphen(memberInfo.Name) : oscMemberAttribute.OscAddress;

            MemberInfo = memberInfo;

            if (memberInfo is FieldInfo)
            {
                FieldInfo fieldInfo = memberInfo as FieldInfo;

                ValueType = fieldInfo.FieldType;

                CanRead = true;
                CanWrite = (fieldInfo.IsInitOnly || fieldInfo.IsLiteral) != true;
            }
            else if (memberInfo is PropertyInfo)
            {
                PropertyInfo propertyInfo = memberInfo as PropertyInfo;

                ValueType = propertyInfo.PropertyType;

                bool canRead = propertyInfo.CanRead;
                bool canWrite = propertyInfo.CanWrite;

                if (canRead == true)
                {
                    canRead = propertyInfo.GetGetMethod() != null;
                }

                if (canWrite == true)
                {
                    canWrite = propertyInfo.GetSetMethod() != null;
                }

                CanRead = canRead;
                CanWrite = canWrite;
            }

            IsLoadable = Loader.IsTypeLoadable(ValueType); 

            IOscSerializer oscSerializer;
            HasOscSerializer = Serialization.OscSerializer.TryGetOscSerializer(ValueType, out oscSerializer);
            OscSerializer = oscSerializer;
        }

        public void AppendAttributeAndValue(object instance, LoadContext context, XElement element)
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

            if (value == null)
            {
                return; 
            }

            string valueValue = OscSerializer.ToString(value);

            Helper.AppendAttributeAndValue(element, MemberName, valueValue);
        }

        public void Describe()
        {
            OscConnection.Send(OscMessages.PropertyDescriptor(OscType.TypeName, OscAddress, ValueType, CanRead, CanWrite));
        }

        public object Get(object instance)
        {
            if (CanRead == false)
            {
                throw new Exception($"Cannot get the value of {MemberName} on ({OscType.TypeName}){instance}.");
            }

            if (MemberInfo is FieldInfo)
            {
                return (MemberInfo as FieldInfo).GetValue(instance);
            }
            else
            {
                return (MemberInfo as PropertyInfo).GetValue(instance, null);
            }
        }

        public void GetAttributeValue(object instance, LoadContext context, XElement node)
        {
            if (IsExcluded == true)
            {
                return;
            }

            if (CanWrite == false)
            {
                return;
            }

            bool found = false;
            string valueString;
            if (Helper.TryGetAttributeValue(node, MemberName, out valueString) == true)
            {
                found = true;
            }
            else
            {
                foreach (string alias in Alias)
                {
                    if (Helper.TryGetAttributeValue(node, alias, out valueString) == false)
                    {
                        continue;
                    }

                    found = true;

                    break;
                }
            }

            if (found == false)
            {
                if (IsRequired == true)
                {
                    context.Error($"Required attribute {OscType.TypeName}.{MemberName} not found on node {node.Name}.", node);
                }

                return;
            }

            if (OscSerializer == null)
            {
                context.Error($"Cannot parse value for {OscType.TypeName}.{MemberName}. Type {Loader.GetTypeName(ValueType)} does not have an OscSerializer.", node);
                return; 
            }

            object value;
            if (OscSerializer.TryFromString(context, valueString, out value) == false)
            {
                return;
            }

            Set(instance, value);
        }

        public void Set(object instance, object value)
        {
            if (CanWrite == false)
            {
                throw new Exception($"Cannot set the value of {MemberName} on ({OscType.TypeName}){instance}.");
            }

            if (MemberInfo is FieldInfo)
            {
                FieldInfo fieldInfo = MemberInfo as FieldInfo;

                fieldInfo.SetValue(instance, value);
            }
            else
            {
                PropertyInfo propertyInfo = MemberInfo as PropertyInfo;

                propertyInfo.SetValue(instance, value, null);
            }
        }

        public override string ToString()
        {
            return OscMessages.PropertyDescriptor(OscType.TypeName, OscAddress, ValueType, CanRead, CanWrite).ToString();
        }
    }
}
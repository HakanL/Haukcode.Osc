using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using Rug.Loading;
using Rug.Osc.Connection;
using Rug.Osc.Namespaces;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Reflection
{
    public class OscType
    {
        public readonly bool IsSerializable;
        public readonly OscMemberCollection Members;
        public readonly OscMemberCollection<IOscMethod> Methods;
        public readonly OscMemberCollection<IOscOutput> Outputs;
        public readonly OscMemberCollection<IOscGetSet> Properties;

        public readonly IOscGetSet SerializableValue;
        public readonly Type Type;
        public readonly string TypeName;        

        private readonly Dictionary<string, IOscMember> memberLookup = new Dictionary<string, IOscMember>();

        private OscType(Type type)
        {
            string typeName = Loader.GetTypeName(type); 

            Debug.Print($@"Create OSC type ""{typeName}"" for type ""{type.FullName}"".");

            oscTypes.Add(type, this);
            oscTypeLookup.Add(typeName, this);

            Type = type;

            IsSerializable = Helper.HasAttribute<OscSerializableAttribute>(type, true);

            TypeName = Loader.GetTypeName(type);

            List<IOscMember> members = new List<IOscMember>();

            List<IOscGetSet> properties = new List<IOscGetSet>();

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

            foreach (MemberInfo memberInfo in type.GetMembers(bindingFlags))
            {                
                OscMemberAttribute oscMemberAttribute; 
                if (Helper.TryGetSingleAttribute(memberInfo, true, out oscMemberAttribute) == false)                    
                {
                    continue;
                }
                
                bool hasOscValueAttributes = Helper.HasAttribute<OscValueAttribute>(memberInfo, true);
                bool hasOscNamespaceAttributes = Helper.HasAttribute<OscNamespaceAttribute>(memberInfo, true);

                IOscGetSet oscProperty = new OscGetSet(this, oscMemberAttribute, memberInfo);

                if (OscType.GetType(oscProperty.ValueType).IsSerializable == true)
                {
                    oscProperty = new OscGetSetReference(oscProperty);
                }

                if (hasOscValueAttributes == true)
                {
                    if (SerializableValue != null)
                    {
                        throw new TypeLoadException($"Osc member {oscProperty.MemberName} on type {type.FullName} has a duplicate {nameof(OscValueAttribute)} attribute.");
                    }

                    SerializableValue = oscProperty;
                }

                Debug.Print($@"Create property ""{typeName}.{oscProperty.MemberName}"" ""{oscProperty.MemberName}"" {oscProperty}");

                properties.Add(oscProperty);
                members.Add(oscProperty);
                memberLookup.Add(memberInfo.Name, oscProperty);
            }

            Properties = new OscMemberCollection<IOscGetSet>(properties);

            List<IOscMethod> methods = new List<IOscMethod>();
            List<IOscOutput> outputs = new List<IOscOutput>();

            foreach (MethodInfo methodInfo in type.GetMethods(bindingFlags))
            {
                OscMethodAttribute oscMethodAttribute;
                if (Helper.TryGetSingleAttribute(methodInfo, true, out oscMethodAttribute) == false)
                {
                    continue;
                }

                ParameterInfo[] parameters = methodInfo.GetParameters();

                if (methodInfo.ReturnType != typeof(void))
                {
                    throw new TypeLoadException($"Osc method {methodInfo.Name} on type {type.FullName} does not have the correct signature. Invalid return type.");
                }

                if (oscMethodAttribute is OscOutputAttribute)
                {
                    OscOutput oscOutput = new OscOutput(this, oscMethodAttribute as OscOutputAttribute, methodInfo);

                    Debug.Print($@"Create output ""{typeName}.{oscOutput.MemberName}"" {oscOutput}");

                    outputs.Add(oscOutput);
                    members.Add(oscOutput);
                    memberLookup.Add(methodInfo.Name, oscOutput);
                }
                else
                {
                    OscMethod oscMethod = new OscMethod(this, oscMethodAttribute, methodInfo);

                    Debug.Print($@"Create method ""{typeName}.{oscMethod.MemberName}"" {oscMethod}");

                    methods.Add(oscMethod);
                    members.Add(oscMethod);
                    memberLookup.Add(methodInfo.Name, oscMethod);
                }
            }

            Methods = new OscMemberCollection<IOscMethod>(methods);
            Outputs = new OscMemberCollection<IOscOutput>(outputs); 

            Members = new OscMemberCollection(members);
        }

        public static void State(INamespaceObject @object)
        {
            @object.Name.Namespace?.NamespaceRoot?.StateOf(@object.Name.OscAddress, @object);
        }

        public void Describe()
        {
            OscConnection.Send(OscMessages.TypeDescriptor(this.TypeName));

            foreach (IOscMember member in Members)
            {
                member.Describe();
            }
        }

        public override string ToString()
        {
            return $"{OscMessages.TypeDescriptor(this.TypeName)} ({Members.Count} members)";
        }

        public string GetMemberAddress(string memberName)
        {
            IOscMember member;

            if (memberLookup.TryGetValue(memberName, out member) == true)
            {
                return member.OscAddress;
            }

            if (memberName.StartsWith("get_") && memberLookup.TryGetValue(memberName.Substring(4), out member) == true)
            {
                return member.OscAddress;
            }

            if (memberName.StartsWith("set_") && memberLookup.TryGetValue(memberName.Substring(4), out member) == true)
            {
                return member.OscAddress;
            }

            throw new Exception($@"Unknown member ""{memberName}"" in type ""{TypeName}"".");
        }

        #region Type Manager

        private static readonly Dictionary<string, OscType> oscTypeLookup = new Dictionary<string, OscType>();
        private static readonly Dictionary<Type, OscType> oscTypes = new Dictionary<Type, OscType>();
        private static readonly object syncObject = new object();

        public static string GetMemberAddress(Type type, string baseAddress, string memberName)
        {
            return baseAddress + GetType(type).GetMemberAddress(memberName);
        }

        public static OscType GetType(Type type)
        {
            lock (syncObject)
            {
                OscType oscType;

                if (oscTypes.TryGetValue(type, out oscType) == false)
                {
                    oscType = new OscType(type);
                }

                return oscType;
            }
        }

        public static void Load(object @object, LoadContext context, XElement node)
        {
            try
            {
                OscType type = GetType(@object.GetType());

                Debug.Print($@"Load type ""{type.TypeName}"".");

                foreach (IOscGetSet getSet in type.Properties)
                {
                    if (getSet.IsExcluded == true)
                    {
                        //Debug.Print($@"Member ""{type.TypeName}.{getSet.MemberName}"" ""{getSet}"" is excluded from loading.");
                        continue; 
                    }

                    if (getSet.IsNamespace == true)
                    {
                        if (getSet.CanWrite == false)
                        {
                            INamespace @namespace = getSet.Get(@object) as INamespace;

                            if (@namespace == null)
                            {
                                continue;
                            }

                            XElement namespaceNode = node.Element(getSet.MemberName);

                            if (namespaceNode == null)
                            {
                                continue; 
                            }

                            //Debug.Print($@"Load ""{type.TypeName}.{getSet.MemberName}"" ""{getSet}"" as namespace.");

                            @namespace.Load(context, namespaceNode);
                        }
                        else
                        {
                            //Debug.Print($@"Load and set ""{type.TypeName}.{getSet.MemberName}"" ""{getSet}"" as namespace.");

                            getSet.Set(@object, Loader.LoadObject(getSet.ValueType, context, node, getSet.MemberName, LoaderMode.UnknownNodesError));
                        }
                    }
                    else if (getSet.IsLoadable == true)
                    {
                        if (getSet.CanWrite == false)
                        {
                            ILoadable loabable = getSet.Get(@object) as ILoadable;

                            if (loabable == null)
                            {
                                continue;
                            }

                            XElement loadableNode = node.Element(getSet.MemberName);

                            if (loadableNode == null)
                            {
                                continue;
                            }

                            //Debug.Print($@"Load ""{type.TypeName}.{getSet.MemberName}"" ""{getSet}"" as loadable.");

                            loabable.Load(context, loadableNode);
                        }
                        else
                        {
                            //Debug.Print($@"Load and set ""{type.TypeName}.{getSet.MemberName}"" ""{getSet}"" as loadable.");

                            getSet.Set(@object, Loader.LoadObject(getSet.ValueType, context, node, getSet.MemberName, LoaderMode.UnknownNodesError));
                        }
                    }
                    else
                    {
                        //Debug.Print($@"Load ""{type.TypeName}.{getSet.MemberName}"" ""{getSet}"" as value.");

                        getSet.GetAttributeValue(@object, context, node);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Error($"Exception while loading Osc type {@object.GetType()}. {ex.Message}", node, true, ex);
            }
        }

        public static void Save(object @object, LoadContext context, XElement element)
        {
            try
            {
                OscType type = GetType(@object.GetType());

                Debug.Print($@"Save type ""{type.TypeName}"".");

                foreach (IOscGetSet getSet in type.Properties)
                {
                    if (getSet.IsExcluded == true)
                    {
                        continue;
                    }

                    if (getSet.IsNamespace == true)
                    {
                        INamespace @namespace = getSet.Get(@object) as INamespace;

                        if (@namespace == null)
                        {
                            continue; 
                        }

                        Loader.SaveObject(context, element, getSet.MemberName, @namespace); 
                    }
                    else if (getSet.IsLoadable == true)
                    {
                        ILoadable loadable = getSet.Get(@object) as ILoadable;

                        if (loadable == null)
                        {
                            continue;
                        }

                        Loader.SaveObject(context, element, getSet.MemberName, loadable);
                    }
                    else
                    {
                        getSet.AppendAttributeAndValue(@object, context, element);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Error($@"Exception while saving Osc type {@object.GetType()}. {ex.Message}", element, exception: ex);
            }
        }

        public static bool TryGetType(string typeName, out OscType oscType)
        {
            lock (syncObject)
            {
                return oscTypeLookup.TryGetValue(typeName, out oscType);
            }
        }

        #endregion Type Manager
    }
}
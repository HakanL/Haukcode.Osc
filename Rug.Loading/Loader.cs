﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Rug.Loading
{
    public enum LoaderMode
    {
        UnknownNodesError,
        UnknownNodesWarning,
        UnknownNodesIgnored, 
    } 

    /// <summary>
    /// Load manager class.
    /// </summary>
    /// <autogeneratedoc />
    public static class Loader
    {
        private static readonly List<LoadableType> AllTypes = new List<LoadableType>();

        private static readonly Dictionary<Type, Dictionary<string, Type>> TypeLookup = new Dictionary<Type, Dictionary<string, Type>>();

        private static readonly Dictionary<Type, string> TypeToNameLookup = new Dictionary<Type, string>();

        static Loader()
        {
            CacheLoadables(typeof(Loader).Assembly);
        }

        /// <summary>
        /// Scans the assembly for <see cref="ILoadable"/> types.
        /// </summary>
        /// <param name="assembly">The assembly to scan.</param>
        public static int CacheLoadables(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (Helper.ImplementsInterface(type, typeof(ILoadable)) == false)
                {
                    continue;
                }

                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

                string name = type.Name;

                NameAttribute nameAttribute;
                if (Helper.TryGetSingleAttribute(type, false, out nameAttribute) == true)
                {
                    name = nameAttribute.Name;
                }

                string[] alias = new string[0];

                AliasAttribute aliasAttribute;
                if (Helper.TryGetSingleAttribute(type, false, out aliasAttribute) == true)
                {
                    alias = aliasAttribute.Alias;
                }

                LoadableType loadableType = new LoadableType
                {
                    Type = type,
                    Name = name,
                    Aliases = alias
                };

                count++;

                if (!AllTypes.Contains(loadableType))
                {
                    AllTypes.Add(loadableType);
                }

                if (TypeToNameLookup.ContainsKey(type) == false)
                {
                    TypeToNameLookup.Add(type, name);
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the XML name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">No loadable type information could be found for the specified type.</exception>
        public static string GetName(Type type)
        {
            string name;

            if (TypeToNameLookup.TryGetValue(type, out name) == false)
            {
                throw new KeyNotFoundException($@"No loadable type information could be found for the type ""{type.Name}"".");
            }

            return name;
        }

        public static string GetTypeName(Type type)
        {
            string name;

            if (Loader.TryGetXmlNodeName(type, out name) == true)
            {
                return name;
            }

            if (type.IsArray == true)
            {
                return $"{GetTypeName(type.GetElementType())}[]";
            }
            else if (type.IsGenericType == true)
            {
                name = type.Name.Substring(0, type.Name.IndexOf('`'));

                name += "<";
                bool first = true;
                foreach (Type genericType in type.GetGenericArguments())
                {
                    if (first == false)
                    {
                        name += ", ";
                    }

                    name += GetTypeName(genericType);

                    first = false;
                }
                name += ">";

                return name;
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(long))
            {
                return "long";
            }

            if (type == typeof(float))
            {
                return "float";
            }

            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(char))
            {
                return "char";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            return type.Name;
        }

        /// <summary>
        /// Gets the all compatible <see cref="ILoadable"/> types for the specified base type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        /// <returns>Dictionary&lt;System.String, Type&gt;.</returns>
        public static Dictionary<string, Type> GetTypeOfType(Type baseType)
        {
            if (TypeLookup.ContainsKey(baseType))
            {
                return TypeLookup[baseType];
            }

            Dictionary<string, Type> types = new Dictionary<string, Type>();

            bool inclusive = !(baseType.IsAbstract || baseType.IsInterface);

            foreach (LoadableType type in AllTypes)
            {
                if ((type.Type.IsSubclassOf(baseType) == false) &&
                    (baseType.IsInterface == false || new List<Type>(type.Type.GetInterfaces()).Contains(baseType) == false) &&
                    (inclusive == false || (type.Type != baseType)))
                {
                    continue;
                }

                types.Add(type.Name, type.Type);

                foreach (string alias in type.Aliases)
                {
                    types.Add(alias, type.Type);
                }
            }

            TypeLookup.Add(baseType, types);

            return types;
        }

        public static bool IsTypeLoadable(Type type)
        {
            return Helper.ImplementsInterface(type, typeof(ILoadable));
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceNode">The source node.</param>
        /// <returns>T.</returns>
        public static T LoadObject<T>(LoadContext context, XmlNode sourceNode, LoaderMode loaderMode) where T : ILoadable
        {
            T[] loaded = LoadObjects<T>(context, sourceNode, null, loaderMode);

            if (loaded.Length == 0)
            {
                return default(T);
            }
            else
            {
                return loaded[0];
            }
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag">The tag.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <returns>T.</returns>
        public static T LoadObject<T>(LoadContext context, XmlNode sourceNode, string tag, LoaderMode loaderMode) where T : ILoadable
        {
            T[] loaded = LoadObjects<T>(context, sourceNode, tag, loaderMode);

            if (loaded.Length == 0)
            {
                return default(T);
            }
            else
            {
                return loaded[0];
            }
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T LoadObject<T>(LoadContext context, XmlNode sourceNode, Type[] argTypes, object[] arguments, LoaderMode loaderMode) where T : ILoadable
        {
            T[] loaded = LoadObjects<T>(context, sourceNode, null, argTypes, arguments, loaderMode);

            if (loaded.Length == 0)
            {
                return default(T);
            }
            else
            {
                return loaded[0];
            }
        }

        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag">The tag.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T LoadObject<T>(LoadContext context, XmlNode sourceNode, string tag, Type[] argTypes, object[] arguments, LoaderMode loaderMode) where T : ILoadable
        {
            T[] loaded = LoadObjects<T>(context, sourceNode, tag, argTypes, arguments, loaderMode);

            if (loaded.Length == 0)
            {
                return default(T);
            }
            else
            {
                return loaded[0];
            }
        }

        public static ILoadable LoadObject(Type baseType, LoadContext context, XmlNode sourceNode, LoaderMode loaderMode)
        {
            ILoadable[] loaded = LoadObjects(baseType, context, sourceNode, null, loaderMode);

            if (loaded.Length == 0)
            {
                return null;
            }
            else
            {
                return loaded[0];
            }
        }

        public static ILoadable LoadObject(Type baseType, LoadContext context, XmlNode sourceNode, string tag, LoaderMode loaderMode)
        {
            ILoadable[] loaded = LoadObjects(baseType, context, sourceNode, tag, loaderMode);

            if (loaded.Length == 0)
            {
                return null;
            }
            else
            {
                return loaded[0];
            }
        }

        public static ILoadable LoadObject(Type baseType, LoadContext context, XmlNode sourceNode, Type[] argTypes, object[] arguments, LoaderMode loaderMode)
        {
            ILoadable[] loaded = LoadObjects(baseType, context, sourceNode, null, argTypes, arguments, loaderMode);

            if (loaded.Length == 0)
            {
                return null;
            }
            else
            {
                return loaded[0];
            }
        }

        public static ILoadable LoadObject(Type baseType, LoadContext context, XmlNode sourceNode, string tag, Type[] argTypes, object[] arguments, LoaderMode loaderMode)
        {
            ILoadable[] loaded = LoadObjects(baseType, context, sourceNode, tag, argTypes, arguments, loaderMode);

            if (loaded.Length == 0)
            {
                return null;
            }
            else
            {
                return loaded[0];
            }
        }

        /// <summary>
        /// Loads the objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceNode">The source node.</param>
        /// <returns>T[].</returns>
        public static T[] LoadObjects<T>(LoadContext context, XmlNode sourceNode, LoaderMode loaderMode) where T : ILoadable
        {
            return LoadObjects<T>(context, sourceNode, null, new Type[0], new object[0], loaderMode);
        }

        public static ILoadable[] LoadObjects(Type baseType, LoadContext context, XmlNode sourceNode, LoaderMode loaderMode)
        {
            return LoadObjects(baseType, context, sourceNode, null, new Type[0], new object[0], loaderMode);
        }

        /// <summary>
        /// Loads the objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag">The tag.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <returns>T[].</returns>
        public static T[] LoadObjects<T>(LoadContext context, XmlNode sourceNode, string tag, LoaderMode loaderMode) where T : ILoadable
        {
            return LoadObjects<T>(context, sourceNode, tag, new Type[0], new object[0], loaderMode);
        }

        /// <summary>
        /// Loads the objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T[].</returns>
        public static T[] LoadObjects<T>(LoadContext context, XmlNode sourceNode, Type[] argTypes, object[] arguments, LoaderMode loaderMode) where T : ILoadable
        {
            return LoadObjects<T>(context, sourceNode, null, argTypes, arguments, loaderMode);
        }

        /// <summary>
        /// Loads the objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag">The tag.</param>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="argTypes">The argument types.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T[].</returns>
        public static T[] LoadObjects<T>(LoadContext context, XmlNode sourceNode, string tag, Type[] argTypes, object[] arguments, LoaderMode loaderMode) where T : ILoadable
        {
            return LoadObjects<T>(typeof(T), context, sourceNode, tag, argTypes, arguments, loaderMode);
        }

        public static ILoadable[] LoadObjects(Type baseType, LoadContext context, XmlNode sourceNode, string tag, LoaderMode loaderMode)
        {
            return LoadObjects(baseType, context, sourceNode, tag, new Type[0], new object[0], loaderMode);
        }

        public static ILoadable[] LoadObjects(Type baseType, LoadContext context, XmlNode sourceNode, Type[] argTypes, object[] arguments, LoaderMode loaderMode)
        {
            return LoadObjects(baseType, context, sourceNode, null, argTypes, arguments, loaderMode);
        }

        public static ILoadable[] LoadObjects(Type baseType, LoadContext context, XmlNode sourceNode, string tag, Type[] argTypes, object[] arguments, LoaderMode loaderMode)
        {
            return LoadObjects<ILoadable>(baseType, context, sourceNode, tag, argTypes, arguments, loaderMode);
        }

        /// <summary>
        /// Saves an object.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="obj">The object.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>XmlElement.</returns>
        public static void SaveObject(LoadContext context, XmlNode parent, object obj)
        {
            SaveObject(context, parent, null, obj);
        }

        /// <summary>
        /// Saves an object.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="obj">The object.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>XmlElement.</returns>
        /// <exception cref="System.Exception">Object is not loadable</exception>
        public static void SaveObject(LoadContext context, XmlNode parent, string tag, object obj)
        {
            if (obj is ILoadable)
            {
                ILoadable loadable = obj as ILoadable;

                string name = Loader.GetName(obj.GetType());

                XmlElement element;
                XmlDocument document = parent as XmlDocument ?? parent.OwnerDocument;

                if (Helper.IsNullOrEmpty(tag) == false)
                {
                    element = document.CreateElement(tag);

                    element.Attributes.Append(document.CreateAttribute("Type"));
                    element.Attributes["Type"].Value = name;
                }
                else
                {
                    element = document.CreateElement(ToXmlNodeName(name));
                }

                loadable.Save(context, element);

                parent.AppendChild(element);
            }
            else
            {
                throw new Exception("Object is not loadable");
            }
        }

        /// <summary>
        /// Saves the objects.
        /// </summary>
        /// <param name="context">Load context.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="objects">The objects.</param>
        public static void SaveObjects(LoadContext context, XmlNode parent, System.Collections.IEnumerable objects)
        {
            SaveObjects(context, parent, null, null, objects);
        }

        /// <summary>
        /// Saves the objects.
        /// </summary>
        /// <param name="context">Load context.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="objects">The objects.</param>
        public static void SaveObjects(LoadContext context, XmlNode parent, string collectionName, string tag, System.Collections.IEnumerable objects)
        {
            XmlElement element;

            if (Helper.IsNullOrEmpty(collectionName) == false)
            {
                element = parent.AppendChild(Helper.CreateElement(parent, collectionName)) as XmlElement;
            }
            else
            {
                element = parent as XmlElement;
            }

            foreach (object obj in objects)
            {
                SaveObject(context, element, tag, obj);
            }
        }

        /// <summary>
        /// Saves the objects.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="objects">The objects.</param>
        /// <param name="parent">The parent.</param>
        public static void SaveObjects(LoadContext context, XmlNode parent, string collectionName, string tag, params object[] objects)
        {
            XmlElement element;

            if (Helper.IsNullOrEmpty(collectionName) == false)
            {
                element = parent.AppendChild(Helper.CreateElement(parent, collectionName)) as XmlElement;
            }
            else
            {
                element = parent as XmlElement;
            }

            foreach (object obj in objects)
            {
                SaveObject(context, element, tag, obj);
            }
        }

        /// <summary>
        /// Gets the name of the XML tag.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <returns>System.String.</returns>
        /// <autogeneratedoc />
        public static string ToXmlNodeName(string original)
        {
            string name = original;

            name = name.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("<", "_").Replace(">", "_").Replace(",", ".");

            return name;
        }

        public static bool TryGetXmlNodeName(Type type, out string name)
        {
            if (TypeToNameLookup.TryGetValue(type, out name) == false)
            {
                return false;
            }

            return true;
        }

        private static T[] LoadObjects<T>(Type baseType, LoadContext context, XmlNode sourceNode, string tag, Type[] argTypes, object[] arguments, LoaderMode loaderMode) where T : ILoadable
        {
            if (sourceNode == null)
            {
                //context.Error($@"Source node is null.", null, false);

                return new T[0];
            }

            bool hasTag = string.IsNullOrEmpty(tag) == false;

            Dictionary<string, Type> types = Loader.GetTypeOfType(baseType);
            List<T> objects = new List<T>();

            XmlNodeList nodes = (hasTag ? sourceNode.SelectNodes(tag) : sourceNode.ChildNodes);

            if (nodes == null)
            {
                context.Error(hasTag ?
                    $@"Cannot find any node with the ""{tag}"" is not valid in this context." :
                    $@"Node ""{sourceNode.Name}"" has null children.",
                    sourceNode);

                return new T[0];
            }

            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                //string typeName;

                // Allow case insensitivity for attribute "Type".
                XmlAttribute typeAttribute = node.Attributes == null ? null : node.Attributes["Type"] ?? node.Attributes["type"];

                bool hasTypeAttribute = typeAttribute != null;

                Type type = null;

                if (hasTypeAttribute == true && types.TryGetValue(typeAttribute.Value, out type) == true)
                {
                    //context.Error($@"Type ""{typeAttribute.Value}"" is not valid in this context.");

                    //continue;
                }
                else if (types.TryGetValue(node.Name, out type) == false)
                {
                    switch (loaderMode)
                    {
                        case LoaderMode.UnknownNodesError:
                            context.Error($@"Type ""{node.Name}"" is not valid in this context.", node);
                            continue;
                        case LoaderMode.UnknownNodesWarning:
                            context.Error($@"Type ""{node.Name}"" is not valid in this context.", node, false, null);
                            continue;
                        case LoaderMode.UnknownNodesIgnored:
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(loaderMode), loaderMode, null);
                    }

                    //type = baseType;
                }

                bool canInstansiateType = (type.IsGenericType || type.IsAbstract || type.IsInterface) == false;

                if (canInstansiateType == false)
                {
                    context.Error($@"Type ""{Loader.GetTypeName(type)}"" cannot be instansiated because is either generic, abstract or an interface.", node);

                    continue;
                }

                ConstructorInfo contructorInfo = type.GetConstructor(argTypes);

                if (contructorInfo == null)
                {
                    context.Error($@"Unable to find a constructor on type ""{Loader.GetTypeName(type)}"" with the supplied arguments.", node);

                    continue;
                }

                T obj = (T)contructorInfo.Invoke(arguments);

                obj.Load(context, node);

                objects.Add(obj);
            }

            return objects.ToArray();
        }

        /// <summary>
        /// Struct for loadable type meta data.
        /// </summary>
        private struct LoadableType
        {
            /// <summary>
            /// Aliases to search for when loading the type.
            /// </summary>
            /// <autogeneratedoc />
            public string[] Aliases;

            /// <summary>
            /// The XML node name for the type.
            /// </summary>
            public string Name;

            /// <summary>
            /// The type.
            /// </summary>
            public Type Type;

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare with the current instance.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                return ToString().Equals(obj.ToString());
            }

            ///// <summary>
            ///// Returns a hash code for this instance.
            ///// </summary>
            ///// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            //public override int GetHashCode()
            //{
            //    return ToString().GetHashCode();
            //}

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
            public override string ToString()
            {
                return $"{Name}:{Type.FullName}";
            }
        }
    }
}
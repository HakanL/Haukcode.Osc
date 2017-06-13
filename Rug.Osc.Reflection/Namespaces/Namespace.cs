using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces
{
    public class Namespace : Namespace<INamespaceObject>
    {
        public Namespace() : base()
        {
        }

        public Namespace(Name name) : base(name)
        {
        }

        internal Namespace(NamespaceRoot namespaceRoot) : base(namespaceRoot)
        {
        }
    }

    public class Namespace<T> : INamespace<T> where T : INamespaceObject
    {
        private readonly Dictionary<string, T> objects = new Dictionary<string, T>();
        private readonly object syncLock = new object();
        private Name parentName;
        private string name;
        private NamespaceRoot namespaceRoot;
        private INamespace parent;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value != null && value != "/" && Rug.Osc.OscAddress.IsValidAddressLiteral(value) == false)
                {
                    throw new ArgumentException(string.Format("\"{0}\" is not a valid namespace.", value), nameof(value));
                }

                lock (syncLock)
                {
                    if (parent != null)
                    {
                        Detach();
                    }

                    name = value;

                    UpdateOscAddress();

                    if (parent != null)
                    {
                        Attach();
                    }
                }
            }
        }

        public NamespaceRoot NamespaceRoot
        {
            get
            {
                if (namespaceRoot != null)
                {
                    return namespaceRoot;
                }

                if (Parent != null)
                {
                    return Parent.NamespaceRoot;
                }

                return null;
            }
        }

        public string OscAddress { get; private set; } = "/";

        public INamespaceObject Owner { get; private set; }

        public INamespace Parent
        {
            get
            {
                return parent;
            }

            set
            {
                lock (syncLock)
                {
                    if (value == this)
                    {
                        throw new Exception("Attempt to make a namespace parent of itself");
                    }

                    if (parent != null)
                    {
                        parent.Changed -= Parent_Changed;
                        Detach();
                    }

                    parent = value;

                    UpdateOscAddress();

                    if (parent != null)
                    {
                        parent.Changed += Parent_Changed;
                        Attach();
                    }
                }
            }
        }

        public int Count { get { return objects.Count; } }

        INamespaceObject INamespace.this[string name]
        {
            get
            {
                T @object = this[name];

                return @object;
            }
        }

        public T this[string name]
        {
            get
            {
                T @object;

                if (TryGetObject(name, out @object) == false)
                {
                    throw new KeyNotFoundException(string.Format("No object found at the address \"{0}\" in the namespace \"{1}\".", name, OscAddress));
                }

                return @object;
            }
        }

        public event NamespaceEvent Changed;

        public Namespace()
        {
        }

        public Namespace(Name name)
        {
            parentName = name;
            parentName.OscAddressChanged += ParentName_OscAddressChanged;
        }

        private void ParentName_OscAddressChanged(Name objectName, NameChangedEventArgs args)
        {
            Debug.Print($"[Namespace ParentName_OscAddressChanged] Parent name {args.OldAddress} changed to {args.NewAddress}");

            UpdateOscAddress();
        }

        internal Namespace(NamespaceRoot namespaceRoot)
        {
            this.namespaceRoot = namespaceRoot;
        }

        public void Add(INamespaceObject @object)
        {
            if ((@object is T) == false)
            {
                throw new ArgumentException(
                    $"Cannot add object to namespace {OscAddress}. Object is of a incompatible type. " +
                    $"Expected {Loader.GetTypeName(typeof(T))} got {Loader.GetTypeName(@object.GetType())}.",
                    nameof(@object));
            }

            Add((T)@object);
        }

        public void Add(T @object)
        {
            Debug.Print($"[Namespace Add(T @object)] Add object {@object} to namespace {OscAddress}");

            lock (syncLock)
            {
                if (@object.Name.Namespace == this)
                {
                    return;
                }

                if (objects.ContainsKey(@object.Name.Value) == true)
                {
                    throw new ArgumentException($"The namespace \"{OscAddress}{@object.Name.Value}\" already exists.", nameof(@object));
                }

                if (@object.Name.Namespace != null)
                {
                    //throw new Exception(string.Format("Object {0} is already a member of namespace {1}.", @object, @object.Name.Namespace.OscAddress));
                    Debug.Print($"[Namespace Add(T @object)] Object {@object} already exists in namespace {@object.Name.Namespace.OscAddress}");
                    @object.Name.Namespace.Remove(@object);
                    Debug.Print($"[Namespace Add(T @object)] Result OscAddress {@object.Name.OscAddress}");
                }

                @object.Name.Namespace = this;

                @object.Name.OscAddressChanged += Name_OscAddressChanged;
                @object.Name.NameChanged += Name_NameChanged;
                @object.Name.NameChanging += Name_NameChanging;

                objects.Add(@object.Name.Value, @object);

                NamespaceRoot?.Attach(@object.Name.OscAddress, @object);

                foreach (INamespace @namespace in GetNamespaces(@object))
                {
                    Debug.Print($"--> [Namespace Add(T @object)] Assign namespace {@namespace.OscAddress} parent {@object.Name.Namespace.OscAddress}");
                    @namespace.Parent = this;
                    Debug.Print($"--> [Namespace Add(T @object)] Result OscAddress {@namespace.OscAddress}");
                }
            }
        }

        public void AddRange(IEnumerable<INamespaceObject> objects)
        {
            foreach (INamespaceObject @object in objects)
            {
                Add(@object);
            }
        }

        public void AddRange(IEnumerable<T> objects)
        {
            foreach (T @object in objects)
            {
                Add(@object);
            }
        }

        public void Attach()
        {
            Debug.Print($"[Namespace Attach()] Attach all object in namespace {OscAddress}");

            foreach (T @object in objects.Values)
            {
                Debug.Print($"--> [Namespace Attach()] Attach object {@object.Name.OscAddress}");

                foreach (INamespace @namespace in GetNamespaces(@object))
                {
                    Debug.Print($"--> --> [Namespace Attach()] Detach namespace {@namespace.OscAddress}");

                    @namespace.Attach();

                    Debug.Print($"--> --> [Namespace Attach()] Result OscAddress {@namespace.OscAddress}");
                }

                NamespaceRoot?.Attach(@object.Name.OscAddress, @object);
            }
        }

        public T Create(OscType type, string name)
        {
            throw new NotImplementedException();
        }

        public T Create(string typeName, string name)
        {
            throw new NotImplementedException();
        }

        public void Destroy(string name)
        {
            throw new NotImplementedException();
        }

        public void Destroy(T @object)
        {
            throw new NotImplementedException();
        }

        public void Detach()
        {
            Debug.Print($"[Namespace Detach()] Detach all object in namespace {OscAddress}");

            foreach (T @object in objects.Values)
            {
                Debug.Print($"--> [Namespace Detach()] Detach object {@object.Name.OscAddress}");

                foreach (INamespace @namespace in GetNamespaces(@object))
                {
                    Debug.Print($"--> --> [Namespace Detach()] Detach namespace {@namespace.OscAddress}");

                    @namespace.Detach();

                    Debug.Print($"--> --> [Namespace Detach()] Result OscAddress {@namespace.OscAddress}");
                }

                NamespaceRoot?.Detach(@object.Name.OscAddress, @object);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (objects.Values as IEnumerable).GetEnumerator();
        }

        INamespaceObject INamespace.Create(string typeName, string name)
        {
            throw new NotImplementedException();
        }

        INamespaceObject INamespace.Create(OscType type, string name)
        {
            throw new NotImplementedException();
        }

        public void Remove(INamespaceObject @object)
        {
            if ((@object is T) == false)
            {
                throw new ArgumentException(
                    $"Cannot remove object from namespace {OscAddress}. Object is of a incompatible type. " +
                    $"Expected {Loader.GetTypeName(typeof(T))} got {Loader.GetTypeName(@object.GetType())}.",
                    nameof(@object));
            }

            Remove((T)@object);
        }

        public void Remove(T @object)
        {
            Debug.Print($"[Namespace Remove(T @object)] Remove {@object.Name.Value} from {OscAddress}");

            if (objects.Remove(@object.Name.Value) == true)
            {
                foreach (INamespace @namespace in GetNamespaces(@object))
                {
                    Debug.Print($"--> [Namespace Remove(T @object)] Detach namespace {@namespace.OscAddress}");

                    @namespace.Parent = null;

                    Debug.Print($"--> [Namespace Remove(T @object)] Result OscAddress {@namespace.OscAddress}");
                }

                Debug.Print($"[Namespace Remove(T @object)] Detach object {@object.Name.OscAddress}");

                NamespaceRoot?.Detach(@object.Name.OscAddress, @object);

                @object.Name.OscAddressChanged -= Name_OscAddressChanged;
                @object.Name.NameChanged -= Name_NameChanged;
                @object.Name.NameChanging -= Name_NameChanging;

                Debug.Print("[Namespace Remove(T @object)] Nullify namespace");

                @object.Name.Namespace = null;

                Debug.Print($"[Namespace Remove(T @object)] Result OscAddress {@object.Name.OscAddress}");
            }
            else
            {
                throw new Exception($"No object with the name {@object.Name.Value} found in namespace {OscAddress}");
            }

        }

        public void State()
        {
            lock (syncLock)
            {
                foreach (T @object in objects.Values)
                {
                    @object.State();
                }
            }
        }

        private string AddressOrEmpty(string oscAddress)
        {
            if (oscAddress == null)
            {
                return string.Empty;
            }

            return oscAddress == "/" ? string.Empty : oscAddress;
        }

        private string AddressOrEmpty(Name name)
        {
            if (name == null)
            {
                return string.Empty;
            }

            string oscAddress = name.OscAddress;

            return oscAddress == "/" ? string.Empty : oscAddress;
        }

        private string AddressOrEmpty(INamespace @namespace)
        {
            if (@namespace == null)
            {
                return string.Empty;
            }

            string oscAddress = @namespace.OscAddress;

            return oscAddress == "/" ? string.Empty : oscAddress;
        }

        private IEnumerable<INamespace> GetNamespaces(T @object)
        {
            OscType type = OscType.GetType(@object.GetType());

            foreach (IOscGetSet member in type.Properties)
            {
                if (member.IsNamespace == false)
                {
                    continue;
                }

                INamespace result = member.Get(@object) as INamespace;

                if (result == null)
                {
                    continue;
                }

                yield return result;
            }
        }

        #region Loadable

        public void Load(LoadContext context, XmlNode node)
        {
            Name = Helper.GetAttributeValue(node, nameof(Name), Name);

            AddRange(Loader.LoadObjects<T>(context, node, LoaderMode.UnknownNodesError));
        }

        public void Save(LoadContext context, XmlElement element)
        {
            Helper.AppendAttributeAndValue(element, nameof(Name), Name);

            lock (syncLock)
            {
                Loader.SaveObjects(context, element, null, null, objects.Values);
            }
        }

        #endregion Loadable

        private void Name_NameChanged(Name objectName, NameChangedEventArgs args)
        {
            T @object;

            if (objects.TryGetValue(args.OldAddress, out @object) == false)
            {
                return;
            }

            objects.Remove(args.OldAddress);
            objects.Add(@object.Name.Value, @object);
        }

        private void Name_NameChanging(Name objectName, string name)
        {
            if (objects.ContainsKey(name) == true)
            {
                throw new ArgumentException($"The namespace \"{OscAddress}{name}\" already exists.", nameof(name));
            }
        }

        private void Name_OscAddressChanged(Name objectName, NameChangedEventArgs args)
        {
            T @object;

            if (objects.TryGetValue(objectName.Value, out @object) == false)
            {
                return;
            }

            NamespaceRoot?.Detach(args.OldAddress, @object);
            NamespaceRoot?.Attach(args.NewAddress, @object);
        }

        private void Parent_Changed(INamespace @namespace)
        {
            //Detach();

            UpdateOscAddress();

            //Attach();
        }

        private bool TryGetObject(string addressOrName, out T @object)
        {
            lock (syncLock)
            {
                string prefix;

                if (Parent != null)
                {
                    prefix = Parent.OscAddress + Name;
                }
                else
                {
                    prefix = Name;
                }

                string resolvedName;

                if (addressOrName.StartsWith(prefix) == true)
                {
                    resolvedName = addressOrName.Substring(prefix.Length);
                }
                else
                {
                    resolvedName = addressOrName;
                }

                return objects.TryGetValue(resolvedName, out @object);
            }
        }

        private void UpdateOscAddress()
        {
            lock (syncLock)
            {
                string resolvedAddress;

                if (parentName != null)
                {
                    resolvedAddress = AddressOrEmpty(parentName) + Name;
                }
                else
                {
                    resolvedAddress = AddressOrEmpty(Parent) + Name;
                }

                bool changed = OscAddress != resolvedAddress;

                Debug.Print($"[Namespace UpdateOscAddress()] Namespace OscAddress changed from {OscAddress} to {resolvedAddress}");

                OscAddress = resolvedAddress;

                if (changed == true)
                {
                    Changed?.Invoke(this);
                }
            }
        }
    }
}
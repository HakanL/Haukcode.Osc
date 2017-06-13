using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces
{
    public sealed class NamespaceRoot
    {
        public readonly Namespace Namespace;

        private readonly Dictionary<string, INamespaceObject> objects = new Dictionary<string, INamespaceObject>();
        private readonly OscInvoker OscMemberInvoker;

        public NamespaceRoot(IOscAddressManager oscAddressManager)
        {
            OscMemberInvoker = new OscInvoker(oscAddressManager);

            Namespace = new Namespace(this);
        }

        public IEnumerable<INamespaceObject> Objects => objects.Values;

        public void Attach(string address, INamespaceObject @object)
        {
            try
            {
                Debug.Print("[NamespaceRoot Attach(string address, INamespaceObject @object)] Attach {0} at address {1} to namespace root", @object, address);

                OscMemberInvoker.Attach(address, @object);

                objects.Add(address, @object);
            }
            catch
            {
                throw;
            }
        }

        public void Detach(string address, INamespaceObject @object)
        {
            try
            {
                //if (objects.Remove(address) == false)
                //{
                //    return;                     
                //}

                //if (OscMemberInvoker.OscAddressManager.Contains(address) == false)
                //{
                //    return;
                //}

                Debug.Print("[NamespaceRoot Detach(string address, INamespaceObject @object)] Detach {0} at address {1} from namespace root", @object, address);

                OscMemberInvoker.Detach(address, @object);

                objects.Remove(address); 
            }
            catch
            {
                throw;
            }
        }

        public INamespaceObject FindObject(string queryAddress)
        {
            INamespaceObject found = null;

            if (objects.TryGetValue(queryAddress, out found) == false)
            {
                return null;
            }

            return found;
        }

        public T FindObject<T>(string name) where T : INamespaceObject
        {
            INamespaceObject found = null;

            if (objects.TryGetValue(name, out found) == false)
            {
                return default(T);
            }

            if ((found is T) == false)
            {
                return default(T);
            }

            return (T)found;
        }

        public IEnumerable<T> FindObjects<T>() where T : INamespaceObject
        {
            List<T> found = new List<T>();

            foreach (INamespaceObject @object in objects.Values)
            {
                if ((@object is T) == false)
                {
                    continue;
                }

                found.Add((T)@object);
            }

            return found;
        }

        public IEnumerable<INamespaceObject> FindObjects(string queryAddress)
        {
            OscAddress address = new OscAddress(queryAddress); 

            List<INamespaceObject> found = new List<INamespaceObject>();

            foreach (INamespaceObject @object in objects.Values)
            {
                if (address.Match(@object.Name.OscAddress) == false)
                {
                    continue; 
                }

                found.Add(@object);
            }

            return found;
        }

        public void StateOf(string oscAddress)
        {
            INamespaceObject found = FindObject(oscAddress);

            OscMemberInvoker.StateOf(oscAddress, found);
        }

        public void StateOf(string oscAddress, INamespaceObject @object)
        {
            OscMemberInvoker.StateOf(oscAddress, @object);
        }

        public bool TypeOf(string queryAddress)
        {
            return OscMemberInvoker.TypeOf(queryAddress);
        }

        public void Usage(string address)
        {
            OscMemberInvoker.Usage(address);
        }

        public void Dispose()
        {
            foreach (INamespaceObject @object in objects.Values)
            {
                (@object as IDisposable)?.Dispose();
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using Rug.Osc.Namespaces;
using Rug.Osc.Reflection;

namespace Rug.Osc.Reflection
{
    /// <summary>
    /// Manages osc address event listening
    /// </summary>
    public sealed class OscInvoker : IDisposable
    {
        private readonly object syncLock = new object();
        private readonly Dictionary<string, IOscMemberAdapter> oscMemberAdapters = new Dictionary<string, IOscMemberAdapter>();

        public IOscAddressManager OscAddressManager { get; private set; } 

        public OscInvoker(IOscAddressManager oscAddressManager)
        {
            OscAddressManager = oscAddressManager; 
        }

        public void Attach(string baseAddress, object instance)
        {
            if (baseAddress == null)
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (baseAddress == "/")
            {
                baseAddress = string.Empty; 
            }

            if (baseAddress != string.Empty && OscAddress.IsValidAddressLiteral(baseAddress) == false)
            {
                throw new ArgumentException($@"Invalid container address ""{baseAddress}""", nameof(baseAddress));
            }

            OscType type = OscType.GetType(instance.GetType()); 

            lock (syncLock)
            {
                List<string> oscAddresses = new List<string>(); 

                foreach (IOscMember member in type.Members)
                {
                    IOscGetSet oscGetSet = member as IOscGetSet;

                    if (oscGetSet != null && oscGetSet.IsNamespace == true)
                    {
                        //INamespace @namespace = oscGetSet.Get(instance) as INamespace;

                        //@namespace?.Attach();

                        continue;
                    }

                    string address = baseAddress + member.OscAddress; 

                    if (oscMemberAdapters.ContainsKey(address) == true ||
                        oscAddresses.Contains(address) == true)
                    {
                        throw new Exception($@"A member already exists at the address ""{address}"".");
                    }

                    if (OscAddress.IsValidAddressLiteral(address) == false)
                    {
                        throw new Exception($@"Invalid osc address ""{address}"".");
                    }

                    oscAddresses.Add(address); 
                }

                foreach (IOscMember member in type.Members)
                {
                    if ((member is IOscGetSet) && (member as IOscGetSet).IsNamespace == true)
                    {
                        continue;
                    }

                    string address = baseAddress + member.OscAddress;

                    IOscMemberAdapter adapter = OscMemberAdapterFactory.Create(address, instance, member);

                    // add it to the lookup 
                    oscMemberAdapters.Add(address, adapter);
                    OscAddressManager.Attach(address, adapter.Invoke);
                }
            }
        }
        
        public void Detach(string baseAddress, object instance)
        {
            if (baseAddress == null)
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (baseAddress == "/")
            {
                baseAddress = string.Empty;
            }

            if (baseAddress != string.Empty && OscAddress.IsValidAddressLiteral(baseAddress) == false)
            {
                throw new ArgumentException($@"Invalid container address ""{baseAddress}""", nameof(baseAddress));
            }

            OscType type = OscType.GetType(instance.GetType());            

            lock (syncLock)
            {
                List<string> oscAddresses = new List<string>(); 

                foreach (IOscMember member in type.Members)
                {
                    IOscGetSet oscGetSet = member as IOscGetSet;

                    if (oscGetSet != null && oscGetSet.IsNamespace == true)
                    {
                        //INamespace @namespace = oscGetSet.Get(instance) as INamespace;

                        //@namespace?.Detach();

                        continue;
                    }

                    string address = baseAddress + member.OscAddress;

                    if (oscMemberAdapters.ContainsKey(address) == false)
                    {
                        throw new Exception($@"No member exists at the address ""{address}"".");
                    }

                    if (OscAddress.IsValidAddressLiteral(address) == false)
                    {
                        throw new Exception($@"Invalid osc address ""{address}"".");
                    }

                    oscAddresses.Add(address); 
                }

                foreach (string oscAddress in oscAddresses)
                {
                    IOscMemberAdapter adapter;

                    if (oscMemberAdapters.TryGetValue(oscAddress, out adapter) == false)
                    {
                        // no container was found so abort
                        return;
                    }

                    OscAddressManager.Detach(oscAddress, adapter.Invoke);

                    oscMemberAdapters.Remove(oscAddress);

                    // TODO, is this vital or not?
                    //adapter.Dispose();
                }
            }
        }

        public void StateOf(string baseAddress, object instance)
        {
            if (baseAddress == null)
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (baseAddress == "/")
            {
                baseAddress = string.Empty;
            }

            if (baseAddress != string.Empty && OscAddress.IsValidAddressLiteral(baseAddress) == false)
            {
                throw new ArgumentException($@"Invalid container address ""{baseAddress}""", nameof(baseAddress));
            }

            OscType type = OscType.GetType(instance.GetType());

            lock (syncLock)
            {
                foreach (IOscMember member in type.Members)
                {
                    IOscGetSet oscGetSet = member as IOscGetSet;

                    if (oscGetSet != null && oscGetSet.IsNamespace == true)
                    {
                        INamespace @namespace = oscGetSet.Get(instance) as INamespace;

                        @namespace?.State(); 

                        continue;
                    }

                    string address = baseAddress + member.OscAddress;

                    if (OscAddress.IsValidAddressLiteral(address) == false)
                    {
                        throw new Exception($@"Invalid osc address ""{address}"".");
                    }

                    IOscMemberAdapter adapter;

                    if (oscMemberAdapters.TryGetValue(address, out adapter) == false)
                    {
                        // no container was found so abort
                        throw new Exception($@"Unknown osc address ""{address}"".");
                    }

                    adapter.State();
                }
            }
        }

        public bool TypeOf(string queryAddress)
        {
            lock (syncLock)
            {
                IOscMemberAdapter adapter;

                if (oscMemberAdapters.TryGetValue(queryAddress, out adapter) == false)
                {
                    return false; 
                }

                adapter.TypeOf();                

                return true;
            }     
        }
        
        public bool Usage(string address)
        {
            bool invoked = false;

            if (address == "/" || OscAddress.IsValidAddressLiteral(address) == true)
            {
                IOscMemberAdapter adapter = null;

                lock (syncLock)
                {
                    invoked = oscMemberAdapters.TryGetValue(address, out adapter);
                }

                adapter?.Usage();
            }
            else
            {
                List<IOscMemberAdapter> adapters = new List<IOscMemberAdapter>();

                OscAddress oscAddress = new OscAddress(address);

                lock (syncLock)
                {
                    foreach (KeyValuePair<string, IOscMemberAdapter> value in oscMemberAdapters)
                    {
                        if (oscAddress.Match(value.Key) == true)
                        {
                            adapters.Add(value.Value);

                            invoked = true;
                        }
                    }
                }

                foreach (IOscMemberAdapter adapter in adapters)
                {
                    adapter.Usage();
                }
            }

            return invoked;
        }

        public void Dispose()
        {
            lock (syncLock)
            {
                foreach (KeyValuePair<string, IOscMemberAdapter> value in oscMemberAdapters)
                {
                    OscAddressManager.Detach(value.Key, value.Value.Invoke); 

                    value.Value.Dispose();
                }

                oscMemberAdapters.Clear();
            }
        }
    }
}
using System;
using System.Diagnostics;
using Rug.Osc.Reflection;
using Rug.Osc.Reflection.Serialization;

namespace Rug.Osc.Namespaces
{
    public delegate void NameEvent(Name objectName, string name);

    public delegate void NameChangedEvent(Name objectName, NameChangedEventArgs args);

    [OscSerializable]
    public class Name :
        IEquatable<Name>, IComparable<Name>,
        IEquatable<string>, IComparable<string>,
        IEquatable<OscAddress>, IComparable<OscAddress>
    {
        private readonly object syncLock = new object();
        private INamespace @namespace;
        private string value = "/";

        private readonly bool isReadOnly; 

        public Name()
        {
            isReadOnly = true;

            UpdateOscAddress();
        }

        public Name(string value, bool isReadOnly = false)
        {
            Value = value;

            this.isReadOnly = isReadOnly; 

            UpdateOscAddress();
        }

        public NamespaceRoot Root => Namespace?.NamespaceRoot;

        [OscMember, OscValue]
        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                if (value != "/" && Rug.Osc.OscAddress.IsValidAddressLiteral(value) == false)
                {
                    throw new ArgumentException($@"""{value}"" is not a valid namespace.", nameof(value));
                }

                lock (syncLock)
                {
                    if (isReadOnly == true && value != this.value)
                    {
                        throw new Exception("Cannot change name, it is read-only.");
                    }

                    if (value == this.value)
                    {
                        return; 
                    }

                    NameChanging?.Invoke(this, value);

                    string oldValue = this.value;

                    this.value = value;

                    NameChanged?.Invoke(this, new NameChangedEventArgs(oldValue, value));

                    UpdateOscAddress();
                }
            }
        }

        public INamespace Namespace
        {
            get
            {
                return @namespace;
            }

            internal set
            {
                lock (syncLock)
                {
                    if (@namespace != null)
                    {
                        @namespace.Changed -= Namespace_Changed;
                    }

                    @namespace = value;

                    if (@namespace != null)
                    {
                        @namespace.Changed += Namespace_Changed;
                    }

                    UpdateOscAddress();
                }
            }
        }

        private void Namespace_Changed(INamespace @namespace)
        {
            UpdateOscAddress();
        }

        public string OscAddress { get; private set; }

        public event NameChangedEvent NameChanged;

        public event NameEvent NameChanging;

        public event NameChangedEvent OscAddressChanged;

        public event NameEvent OscAddressChanging;

        public int CompareTo(OscAddress other)
        {
            return string.Compare(OscAddress, other.OrigialString, StringComparison.InvariantCulture);
        }

        public int CompareTo(string other)
        {
            return string.Compare(OscAddress, other, StringComparison.InvariantCulture);
        }

        public int CompareTo(Name other)
        {
            return string.Compare(OscAddress, other.OscAddress, StringComparison.InvariantCulture);
        }

        public bool Equals(Name other)
        {
            return OscAddress.Equals(other.OscAddress);
        }

        public bool Equals(string other)
        {
            return OscAddress.Equals(other);
        }

        public bool Equals(OscAddress other)
        {
            return OscAddress.Equals(other.OrigialString);
        }

        public override string ToString()
        {
            return OscAddress;
        }

        private void UpdateOscAddress()
        {
            lock (syncLock)
            {
                string oldOscAddress = OscAddress; 
                string resolvedAddress;

                if (Namespace?.OscAddress != null && Namespace.OscAddress != "/")
                {
                    resolvedAddress = Namespace.OscAddress + Value; // + Value.Substring(1);
                }
                else
                {
                    resolvedAddress = Value;
                }

                OscAddressChanging?.Invoke(this, OscAddress);

                Debug.Print($"[Name UpdateOscAddress()] OscAddress changed from {OscAddress} to {resolvedAddress}"); 

                OscAddress = resolvedAddress;

                //OscAddressChanged?.Invoke(this, oldOscAddress);
                OscAddressChanged?.Invoke(this, new NameChangedEventArgs(oldOscAddress, OscAddress));
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Rug.Osc.Packaging
{
    public sealed class OscPackageListener : IDisposable
    {
        private readonly Dictionary<ulong, DateTime> activePackages = new Dictionary<ulong, DateTime>();
        private readonly ManualResetEvent expungeComplete = new ManualResetEvent(true);
        private readonly object syncLock = new object();
        private readonly List<ulong> toRemove = new List<ulong>();
        private Thread thread;

        public readonly OscAddressManager OscAddressManager;

        public readonly OscReceiver OscReceiver;

        public event Action<IPEndPoint> BeginSessionContext;

        public event Action<IPEndPoint> EndSessionContext;

        public event Action<Exception> InnerExceptionThrown;

        public event Action<ulong> PackageConfirmed;

        public event EventHandler PackageExpired;

        public event Action<OscPacket> PacketReceived;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler<UnknownAddressEventArgs> UnknownAddress;

        public OscPackageListener(int port) : this()
        {
            OscReceiver = new OscReceiver(port);
        }

        public OscPackageListener(IPAddress address, int port) : this()
        {
            OscReceiver = new OscReceiver(address, port);
        }

        public OscPackageListener(int port, int messageBufferSize, int maxPacketSize) : this()
        {
            OscReceiver = new OscReceiver(port, messageBufferSize, maxPacketSize);
        }

        public OscPackageListener(IPAddress address, IPAddress multicast, int port) : this()
        {
            OscReceiver = new OscReceiver(address, multicast, port);
        }

        public OscPackageListener(IPAddress address, int port, int messageBufferSize, int maxPacketSize) : this()
        {
            OscReceiver = new OscReceiver(address, port, messageBufferSize, maxPacketSize);
        }

        public OscPackageListener(IPAddress address, IPAddress multicast, int port, int messageBufferSize, int maxPacketSize) : this()
        {
            OscReceiver = new OscReceiver(address, multicast, port, messageBufferSize, maxPacketSize);
        }

        private OscPackageListener()
        {
            OscAddressManager = new OscAddressManager();
            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(this.OnUnknownAddress);
        }

        public void Attach(string address, OscMessageEvent @event)
        {
            OscAddressManager.Attach(address, @event);
        }

        public void Cancel(ulong id)
        {
            lock (syncLock)
            {
                activePackages.Remove(id);
            }
        }

        public void Clear()
        {
            lock (syncLock)
            {
                activePackages.Clear();
            }
        }

        public void Close()
        {
            OscReceiver.Close();
        }

        public void Connect()
        {
            OscReceiver.Connect();
            thread = new Thread(new ThreadStart(this.ListenLoop));
            thread.Name = "Osc Package Listener " + OscReceiver.ToString();
            thread.Start();
        }

        public void Detach(string address, OscMessageEvent @event)
        {
            OscAddressManager.Detach(address, @event);
        }

        public void Dispose()
        {
            OscReceiver.Dispose();
            OscAddressManager.Dispose();
        }

        public void Expunge()
        {
            lock (syncLock)
            {
                expungeComplete.WaitOne();

                DateTime time = DateTime.Now;

                foreach (KeyValuePair<ulong, DateTime> record in activePackages)
                {
                    if (record.Value == DateTime.MaxValue)
                    {
                        continue;
                    }

                    if (record.Value >= time)
                    {
                        continue;
                    }

                    toRemove.Add(record.Key);
                }

                foreach (ulong id in toRemove)
                {
                    activePackages.Remove(id);
                }

                expungeComplete.Reset();
            }

            try
            {
                if (PackageExpired != null)
                {
                    foreach (ulong id in toRemove)
                    {
                        PackageExpired?.Invoke(id, EventArgs.Empty);
                    }
                }
            }
            finally
            {
                toRemove.Clear();
                expungeComplete.Set();
            }
        }

        public void FilterPacket(ulong id, float seconds = float.PositiveInfinity)
        {
            lock (syncLock)
            {
                if (seconds == float.PositiveInfinity)
                {
                    activePackages.Add(id, DateTime.MaxValue);
                }
                else
                {
                    activePackages.Add(id, DateTime.Now.AddSeconds(seconds));
                }
            }
        }

        private void ListenLoop()
        {
            try
            {
                while (OscReceiver.State != OscSocketState.Closed)
                {
                    if (OscReceiver.State == OscSocketState.Connected)
                    {
                        OscPacket packet = this.OscReceiver.Receive();

                        BeginSessionContext?.Invoke(packet.Origin);

                        if (ShouldProcessPackage(packet) == false)
                        {
                            continue;
                        }

                        switch (OscAddressManager.ShouldInvoke(packet))
                        {
                            case OscPacketInvokeAction.Invoke:
                                PacketReceived?.Invoke(packet);
                                OscAddressManager.Invoke(packet);
                                break;
                        }

                        EndSessionContext?.Invoke(packet.Origin);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is OscSocketStateException && OscReceiver.State != OscSocketState.Connected)
                {
                    return;
                }

                InnerExceptionThrown?.Invoke(new InnerThreadException("Exception in listen loop", ex));
            }
        }

        private void OnUnknownAddress(object sender, UnknownAddressEventArgs e)
        {
            UnknownAddress?.Invoke(this, e);
        }

        private bool ShouldProcessPackage(OscPacket packet)
        {
            if (packet is OscBundle == false)
            {
                return true;
            }

            OscBundle bundle = packet as OscBundle;

            if (bundle.Count == 0)
            {
                return true;
            }

            if (bundle[0] is OscMessage == false)
            {
                return true;
            }

            OscMessage message = bundle[0] as OscMessage;

            if (message.Address != OscPackage.PackageAddress)
            {
                return true;
            }

            if (message.Count == 0)
            {
                return true;
            }

            if (message[0] is long == false)
            {
                return true;
            }

            ulong id = unchecked((ulong)(long)(message[0]));

            lock (syncLock)
            {
                if (activePackages.ContainsKey(id) == false)
                {
                    return true;
                }
            }

            //Cancel(id);

            PackageConfirmed?.Invoke(id);

            return false;
        }
    }
}
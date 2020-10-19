using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Rug.Osc.Packaging
{
    public sealed class OscPackageListener : IDisposable
    {
        private readonly ConcurrentDictionary<ulong, DateTime> activePackages = new ConcurrentDictionary<ulong, DateTime>();
        //private readonly ManualResetEvent expungeComplete = new ManualResetEvent(true);
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
            activePackages.TryRemove(id, out DateTime _);
        }

        public void Clear()
        {
            activePackages.Clear();
        }

        public void Close()
        {
            OscReceiver.Close();
        }

        public void Connect()
        {
            OscReceiver.Connect();
            thread = new Thread(this.ListenLoop) { Name = "Osc Package Listener " + OscReceiver };
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
            //expungeComplete.WaitOne();

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

                activePackages.TryRemove(record.Key, out DateTime _);
                PackageExpired?.Invoke(record.Key, EventArgs.Empty);
            }

            //expungeComplete.Reset();
        }

        //finally
        //{
        //    toRemove.Clear();
        //    expungeComplete.Set();
        //}

        public void FilterPacket(ulong id, float seconds = float.PositiveInfinity)
        {
            if (seconds == float.PositiveInfinity)
            {
                activePackages[id] = DateTime.MaxValue;
            }
            else
            {
                activePackages[id] = DateTime.Now.AddSeconds(seconds);
            }
        }

        private void ListenLoop()
        {
            try
            {
                while (OscReceiver.State != OscSocketState.Closed)
                {
                    if (OscReceiver.State != OscSocketState.Connected)
                    {
                        continue;
                    }

                    OscPacket packet = this.OscReceiver.Receive();

                    BeginSessionContext?.Invoke(packet.Origin);

                    if (ShouldProcessPackage(packet) == false)
                    {
                        continue;
                    }

                    if (packet.Error == OscPacketError.None)
                    {
                        PacketReceived?.Invoke(packet);
                        OscAddressManager.Invoke(packet);
                    }                        

                    EndSessionContext?.Invoke(packet.Origin);
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

            if (activePackages.ContainsKey(id) == false)
            {
                return true;
            }

            //Cancel(id);

            PackageConfirmed?.Invoke(id);

            return false;
        }
    }
}
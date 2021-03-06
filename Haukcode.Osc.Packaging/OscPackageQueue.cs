﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Haukcode.Osc.Packaging
{
    /// <summary>
    /// Holds a queue of OSC bundles that will be attempted to be sent repeatedly until hey are confirmed.
    /// </summary>
    public sealed class OscPackageQueue : IOscPackageBuilder, IDisposable
    {
        public readonly uint QueueIdentifier;

        private readonly OscPackageBuilder builder;

        private readonly OscPackageListener bundleListener;

        private readonly Queue<QueuedOscBundle> queue = new Queue<QueuedOscBundle>();

        private readonly long retryFrequency;

        private readonly OscSender sender;

        private readonly object syncLock = new object();

        private long lastPumpTime;

        public int Count => queue.Count;

        public OscPackageBuilderMode Mode
        {
            get
            {
                return builder.Mode;
            }

            set
            {
                builder.Mode = value;
            }
        }

        public event OscPackegeEvent BundleComplete;

        public event Action<OscPacket> PacketTransmitted;

        public OscPackageQueue(uint identifier, IPAddress address, int port, OscPackageListener bundleListener) :
            this(identifier, IPAddress.Any, 0, address, port, bundleListener)
        {
        }

        public OscPackageQueue(uint identifier, IPAddress adapterAddress, int localPort, IPAddress address, int port, OscPackageListener bundleListener)
        {
            double frequency = Stopwatch.Frequency;

            retryFrequency = (long)(frequency / 10);

            builder = new OscPackageBuilder(identifier);
            builder.BundleComplete += Builder_BundleComplete;

            QueueIdentifier = identifier;

            this.sender = new OscSender(adapterAddress, localPort, address, port);

            this.bundleListener = bundleListener;

            bundleListener.Attach(OscPackage.ReturnAddress, OnReturnMessage);
        }

        public void Add(params OscPacket[] packets)
        {
            builder.Add(packets);
        }

        public void Close()
        {
            sender.Close();
        }

        public void Connect()
        {
            sender.Connect();
        }

        public void Flush()
        {
            builder.Flush();
        }

        public void SendPacketReturnMessage(ulong id)
        {
            OscMessage message = OscPackageBuilder.CreatePacketIDMessage(id, true);

            sender.Send(message);

            PacketTransmitted?.Invoke(message);
        }

        public void Pump()
        {
            if (queue.Count == 0)
            {
                return;
            }

            long nextPumpTime = Stopwatch.GetTimestamp();

            if (lastPumpTime > nextPumpTime - retryFrequency)
            {
                return;
            }

            lastPumpTime = nextPumpTime;

            OscBundle bundle = queue.Peek().Bundle;

            PacketTransmitted?.Invoke(bundle);

            sender.Send(bundle);
        }

        private void Builder_BundleComplete(ulong packetID, OscBundle bundle)
        {
            BundleComplete?.Invoke(packetID, bundle);

            if (builder.Mode != OscPackageBuilderMode.PackagedAndQueued)
            {
                sender.Send(bundle);
                return;
            }

            queue.Enqueue(new QueuedOscBundle() { PacketID = packetID, Bundle = bundle });

            if (queue.Count == 1)
            {
                Pump();
            }
        }

        private void OnReturnMessage(OscMessage message)
        {
            if (message.Count != 1)
            {
                return;
            }

            if ((message[0] is long) == false)
            {
                return;
            }

            ulong id = unchecked((ulong)(long)message[0]);

            if (queue.Count == 0)
            {
                return;
            }

            if (queue.Peek().PacketID == id)
            {
                queue.Dequeue();
            }
        }

        public void Dispose()
        {
            bundleListener.Detach(OscPackage.ReturnAddress, OnReturnMessage);
            sender.Dispose();
            queue.Clear();
        }

        private struct QueuedOscBundle
        {
            public OscBundle Bundle;
            public ulong PacketID;
        }
    }
}
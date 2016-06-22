﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Rug.Osc.Packaging
{
    /// <summary>
    /// Holds a queue of OSC bundles that will be attempted to be sent repeatedly until hey are confirmed.
    /// </summary>
    /// <autogeneratedoc />
    /// TODO Edit XML Comment Template for ReliableBundleQueue
    public sealed class OscPackageQueue : IOscPackageBuilder
    {
        public readonly uint QueueIdentifier;

        private readonly OscPackageBuilder builder;

        private readonly OscPackageListener bundleListener;

        private readonly Queue<QueuedOscBundle> queue = new Queue<QueuedOscBundle>();

        private readonly long retryFrequency;

        private readonly OscSender sender;

        private readonly object syncLock = new object();

        private bool immediateMode = false;

        private long lastPumpTime;

        public int Count { get { return queue.Count; } }

        public bool ImmediateMode
        {
            get
            {
                return immediateMode;
            }

            set
            {
                Flush();

                immediateMode = value;
            }
        }

        public event Action<OscPacket> PacketTransmitted;

        public OscPackageQueue(uint identifier, IPAddress address, int port, OscPackageListener bundleListener)
        {
            double frequency = Stopwatch.Frequency;

            retryFrequency = (long)(frequency / 10);

            builder = new OscPackageBuilder(identifier, true);
            builder.BundleComplete += Builder_BundleComplete;

            QueueIdentifier = identifier;

            this.sender = new OscSender(address, 0, port);

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
            if (builder.ImmediateMode == true)
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

        private struct QueuedOscBundle
        {
            public OscBundle Bundle;
            public ulong PacketID;
        }
    }
}
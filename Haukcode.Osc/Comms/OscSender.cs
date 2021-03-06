﻿/*
 * Haukcode.Osc
 *
 * Copyright (C) 2013 Phill Tew (peatew@gmail.com)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * IN THE SOFTWARE.
 *
 */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Haukcode.Osc
{
    /// <summary>
    /// Osc udp sender
    /// </summary>
    public sealed class OscSender : OscSocket
    {
        /// <summary>
        /// The default number of messages that can be queued for sending before messages start to get dropped
        /// </summary>
        public const int DefaultMessageBufferSize = 600;

        private readonly byte[] buffer;
        private readonly AutoResetEvent queueEmpty = new AutoResetEvent(true);
        private readonly ConcurrentQueue<OscPacket> sendQueue = new ConcurrentQueue<OscPacket>();
        private readonly int messageBufferSize;

        public event OscPacketEvent PacketSent;

        //public string DEBUG_ConnectedState { get { return Socket != null ? "Socket Connected: " + Socket.Connected : "NO SOCKET"; } }

        /// <summary>
        /// Use a value greater than 0 to set the disconnect time out in milliseconds use a value less than or equal to 0 for an infinite timeout
        /// </summary>
        public int DisconnectTimeout { get; set; }

        public override OscSocketType OscSocketType { get; } = Osc.OscSocketType.Send;

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        public OscSender(IPAddress address, int port)
            : this(address, port, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="localPort">the local port to bind, use 0 for dynamically assigned</param>
        /// <param name="remotePort">the port to send to</param>
        public OscSender(IPAddress address, int localPort, int remotePort)
            : this(address, localPort, remotePort, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="local">the ip address to send from</param>
        /// <param name="remote">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        public OscSender(IPAddress local, IPAddress remote, int port)
            : this(local, remote, port, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="local">the ip address to send from</param>
        /// <param name="remote">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        /// <param name="timeToLive">TTL value to apply to packets</param>
        public OscSender(IPAddress local, IPAddress remote, int port, int timeToLive)
            : this(local, remote, port, timeToLive, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
            : this(address.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any,
                    address, port, messageBufferSize, maxPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="localPort">the local port to bind, use 0 for dynamically assigned</param>
        /// <param name="remotePort">the port to send to</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress address, int localPort, int remotePort, int messageBufferSize, int maxPacketSize)
            : this(address.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any, localPort,
                    address, remotePort, DefaultMulticastTimeToLive, messageBufferSize, maxPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="local">the ip address to send from</param>
        /// <param name="remote">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress local, IPAddress remote, int port, int messageBufferSize, int maxPacketSize)
            : this(local, remote, port, DefaultMulticastTimeToLive, messageBufferSize, maxPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="local">the ip address to send from</param>
        /// <param name="remote">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        /// <param name="timeToLive">TTL value to apply to packets</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress local, IPAddress remote, int port, int timeToLive, int messageBufferSize, int maxPacketSize)
            : this(local, port, remote, port, timeToLive, messageBufferSize, maxPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="local">the ip address to send from</param>
        /// <param name="localPort">the local port to bind, use 0 for dynamically assigned</param>
        /// <param name="remote">the ip address to send to</param>
        /// <param name="remotePort">the port to send to</param>
        public OscSender(IPAddress local, int localPort, IPAddress remote, int remotePort)
            : this(local, localPort, remote, remotePort, DefaultMulticastTimeToLive, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="local">the ip address to send from</param>
        /// <param name="localPort">the local port to bind, use 0 for dynamically assigned</param>
        /// <param name="remote">the ip address to send to</param>
        /// <param name="remotePort">the port to send to</param>
        /// <param name="timeToLive">TTL value to apply to packets</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress local, int localPort, IPAddress remote, int remotePort, int timeToLive, int messageBufferSize, int maxPacketSize)
            : base(local, localPort, remote, remotePort, timeToLive)
        {
            // set the default time out
            DisconnectTimeout = 1000;

            buffer = new byte[maxPacketSize];

            this.messageBufferSize = messageBufferSize;
        }

        /// <summary>
        /// Add a osc message to the send queue
        /// </summary>
        /// <param name="message">message to send</param>
        public void Send(OscPacket message)
        {
            if (State != OscSocketState.Connected)
            {
                return;
            }

            queueEmpty.Reset();

            if (sendQueue.Count >= messageBufferSize)
            {
                return;
            }

            sendQueue.Enqueue(message);

            if (sendQueue.Count != 1)
            {
                return;
            }

            int size = message.Write(buffer);

            if (Statistics != null)
            {
                message.IncrementSendStatistics(Statistics);

                Statistics.BytesSent.Increment(size);
            }

            PacketSent?.Invoke(message);

            Socket.BeginSend(buffer, 0, size, SocketFlags, Send_Callback, message);
        }

        /// <summary>
        /// Wait till all messages in the queue have been sent
        /// </summary>
        public void WaitForAllMessagesToComplete()
        {
            queueEmpty.WaitOne(Math.Max(-1, DisconnectTimeout));
        }

        protected override void OnClosing()
        {
            WaitForAllMessagesToComplete();
        }

        protected override void OnConnect()
        {
            // set the timeout for send
            // Socket.SendTimeout = 1000;
        }

        private void Send_Callback(IAsyncResult ar)
        {
            bool shouldClose = false;

            try
            {
                SocketError error;

                Socket.EndSend(ar, out error);

                shouldClose = Socket.Connected == false;

                OscPacket packet;

                if (sendQueue.TryDequeue(out packet) == false)
                {
                    Debug.WriteLine("Could not dequeue packet");
                    return;
                }

                if (packet.IsSameInstance(ar.AsyncState as OscPacket) == false)
                {
                    Debug.WriteLine("Queue packet and async objects do not match");
                }

                if (sendQueue.TryPeek(out packet) && State == OscSocketState.Connected)
                {
                    int size = packet.Write(buffer);

                    PacketSent?.Invoke(packet);

                    Socket.BeginSend(buffer, 0, size, SocketFlags, Send_Callback, packet);
                }
                else
                {
                    queueEmpty.Set();
                }
            }
            catch
            {
                queueEmpty.Set();
            }
            finally
            {
                if (shouldClose == true)
                {
                    Dispose();
                }
            }
        }
    }
}
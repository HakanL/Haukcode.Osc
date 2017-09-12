/*
 * Rug.Osc
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Rug.Osc
{
    /// <summary>
    /// Osc UDP receiver
    /// </summary>
    public sealed class OscReceiver : OscSocket
    {
        /// <summary>
        /// The default number of messages that can be queued for processing after being received before messages start to get dropped
        /// </summary>
        public const int DefaultMessageBufferSize = 600;

        private readonly byte[] buffer;
        private readonly AutoResetEvent messageReceived = new AutoResetEvent(false);
        private readonly ConcurrentQueue<OscPacket> receiveQueue = new ConcurrentQueue<OscPacket>();
        private bool isReceiving = false;
        private int messageBufferSize;

        public override OscSocketType OscSocketType { get; } = Osc.OscSocketType.Receive;

        /// <summary>
        /// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="multicast">a multicast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscReceiver(IPAddress address, IPAddress multicast, int port, int messageBufferSize, int maxPacketSize)
            : base(address, multicast, port)
        {
            buffer = new byte[maxPacketSize];

            if (IsMulticastEndPoint == false)
            {
                throw new ArgumentException("The suppied address must be a multicast address", nameof(multicast));
            }

            this.messageBufferSize = messageBufferSize;
        }

        /// <summary>
		/// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
		/// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscReceiver(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
            : base(address, port)
        {
            buffer = new byte[maxPacketSize];
            this.messageBufferSize = messageBufferSize;
        }

        /// <summary>
		/// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="port">the port to listen on</param>
        public OscReceiver(IPAddress address, int port)
            : this(address, port, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="multicast">a multicast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        public OscReceiver(IPAddress address, IPAddress multicast, int port)
            : this(address, multicast, port, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
		/// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscReceiver(int port, int messageBufferSize, int maxPacketSize)
            : base(port)
        {
            buffer = new byte[maxPacketSize];
            this.messageBufferSize = messageBufferSize;
        }

        /// <summary>
		/// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        public OscReceiver(int port)
            : this(port, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        /// <summary>
        /// Receive a osc message, this method is blocking and will only return once a message is recived
        /// </summary>
        /// <returns>an osc message</returns>
        public OscPacket Receive()
        {
            try
            {
                if (State == OscSocketState.Connected)
                {
                    // if we are not receiving then start
                    if (isReceiving == false && State == OscSocketState.Connected)
                    {
                        BeginReceiving();
                    }

                    OscPacket message = null;

                    while (State == OscSocketState.Connected && receiveQueue.TryDequeue(out message) == false)
                    {
                        // wait for a new message
                        messageReceived.WaitOne();
                        //messageReceived.Reset();
                    }

                    if (message != null)
                    {
                        return message;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new OscSocketException(this, "An unexpected error occured while waiting for a message", ex);
            }

            if (State == OscSocketState.Connected)
            {
                throw new OscSocketException(this, "An unexpected error occured while waiting for a message");
            }

            throw new OscSocketStateException(this, OscSocketState.Closed, "The receiver socket is not connected");
        }

        public override string ToString()
        {
            return "RX " + LocalAddress + ":" + LocalPort + " <- " + RemoteEndPoint.Port;
        }

        /// <summary>
        /// Try to receive a osc message, this method is non-blocking and will return imediatly with a message or null
        /// </summary>
        /// <param name="message">an osc message if one is ready else null if there are none</param>
        /// <returns>true if a message was ready</returns>
        public bool TryReceive(out OscPacket message)
        {
            message = null;

            if (State != OscSocketState.Connected)
            {
                return false;
            }

            if (receiveQueue.TryDequeue(out message) == false)
            {
                // wait for a new message
                messageReceived.WaitOne();
                //messageReceived.Reset();
            }

            // if we are not receiving then start
            if (isReceiving == false && State == OscSocketState.Connected)
            {
                BeginReceiving();
            }

            return false;
        }

        protected override void OnClosing()
        {
            messageReceived.Set();
        }

        protected override void OnConnect()
        {
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, buffer.Length * 4);

            isReceiving = false;
        }

        private void BeginReceiving()
        {
            isReceiving = true;
            //messageReceived.Reset();

            // create an empty origin
            EndPoint origin = UseIPv6 ? Helper.EmptyEndPointIPv6 : Helper.EmptyEndPoint;

            Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags, ref origin, Receive_Callback, null);
        }

        private void Receive_Callback(IAsyncResult ar)
        {
            try
            {
                // create an empty origin
                EndPoint origin = UseIPv6 ? Helper.EmptyEndPointIPv6 : Helper.EmptyEndPoint;

                int count = Socket.EndReceiveFrom(ar, ref origin);

                Statistics?.BytesReceived.Increment(count);

                OscPacket message = OscPacket.Read(buffer, count, (IPEndPoint)origin);

                if (Statistics != null && message.Error != OscPacketError.None)
                {
                    Statistics.ReceiveErrors.Increment(1);
                }

                if (receiveQueue.Count < messageBufferSize)
                {
                    receiveQueue.Enqueue(message);

                    messageReceived.Set();
                }

                //lock (syncLock)
                //{
                //    if (this.count < receiveQueue.Length)
                //    {
                //        receiveQueue[writeIndex] = message;

                //        writeIndex = NextWriteIndex;

                //        this.count++;

                //        // if this was the first message then signal
                //        if (this.count == 1)
                //        {
                //            messageReceived.Set();
                //        }
                //    }
                //}
            }
            catch
            {
            }

            if (State == OscSocketState.Connected)
            {
                // create an empty origin
                EndPoint origin = UseIPv6 ? Helper.EmptyEndPointIPv6 : Helper.EmptyEndPoint;

                Socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags, ref origin, Receive_Callback, null);
            }
        }
    }
}
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

        #region Private Members

        private readonly object syncLock = new object();
        private readonly AutoResetEvent messageReceived = new AutoResetEvent(false);

        private readonly byte[] buffer;

        private readonly OscPacket[] receiveQueue;

        private int writeIndex = 0;
        private int readIndex = 0;
        private int count = 0;

        private bool isReceiving = false;

        #endregion Private Members

        #region Properties

        public override OscSocketType OscSocketType
        {
            get { return Osc.OscSocketType.Receive; }
        }

        /// <summary>
        /// The next queue index to write messages to
        /// </summary>
        private int NextWriteIndex
        {
            get
            {
                int index = writeIndex + 1;

                if (index >= receiveQueue.Length)
                {
                    index -= receiveQueue.Length;
                }

                return index;
            }
        }

        /// <summary>
        /// The next queue index to read messages from
        /// </summary>
        private int NextReadIndex
        {
            get
            {
                int index = readIndex + 1;

                if (index >= receiveQueue.Length)
                {
                    index -= receiveQueue.Length;
                }

                return index;
            }
        }

        #endregion Properties

        #region Constructors

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
            receiveQueue = new OscPacket[messageBufferSize];

            if (IsMulticastEndPoint == false)
            {
                throw new ArgumentException(Strings.Receiver_NotMulticastAddress, "multicast");
            }
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
            receiveQueue = new OscPacket[messageBufferSize];
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
            receiveQueue = new OscPacket[messageBufferSize];
        }

        /// <summary>
		/// Create a new Osc UDP receiver. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        public OscReceiver(int port)
            : this(port, DefaultMessageBufferSize, DefaultPacketSize)
        {
        }

        #endregion Constructors

        public override string ToString()
        {
            return "RX " + LocalAddress + ":" + LocalPort + " <- " + RemoteEndPoint.Port;
        }

        #region Protected Overrides

        protected override void OnConnect()
        {
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, buffer.Length * 4);

            isReceiving = false;
        }

        protected override void OnClosing()
        {
            messageReceived.Set();
        }

        #endregion Protected Overrides

        #region Receive

        /// <summary>
        /// Try to receive a osc message, this method is non-blocking and will return imediatly with a message or null
        /// </summary>
        /// <param name="message">an osc message if one is ready else null if there are none</param>
        /// <returns>true if a message was ready</returns>
        public bool TryReceive(out OscPacket message)
        {
            message = null;

            if (State == OscSocketState.Connected)
            {
                if (count > 0)
                {
                    lock (syncLock)
                    {
                        message = receiveQueue[readIndex];

                        readIndex = NextReadIndex;

                        count--;

                        return true;
                    }
                }
                // if we are not receiving then start
                else if (isReceiving == false)
                {
                    lock (syncLock)
                    {
                        if (isReceiving == false && State == OscSocketState.Connected)
                        {
                            BeginReceiving();
                        }
                    }
                }
            }

            return false;
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
                    if (isReceiving == false)
                    {
                        lock (syncLock)
                        {
                            if (isReceiving == false && State == OscSocketState.Connected)
                            {
                                BeginReceiving();
                            }
                        }
                    }

                    if (count > 0)
                    {
                        lock (syncLock)
                        {
                            OscPacket message = receiveQueue[readIndex];

                            readIndex = NextReadIndex;

                            count--;

                            // if we have eaten all the messages then reset the signal
                            if (count == 0)
                            {
                                messageReceived.Reset();
                            }

                            return message;
                        }
                    }

                    // wait for a new message
                    messageReceived.WaitOne();
                    messageReceived.Reset();

                    if (count > 0)
                    {
                        lock (syncLock)
                        {
                            OscPacket message = receiveQueue[readIndex];

                            readIndex = NextReadIndex;

                            count--;

                            return message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new OscSocketException(this, Strings.Receiver_ErrorWhileWaitingForMessage, ex);
            }

            if (State == OscSocketState.Connected)
            {
                throw new OscSocketException(this, Strings.Receiver_ErrorWhileWaitingForMessage);
            }

            throw new OscSocketStateException(this, OscSocketState.Closed, Strings.Receiver_SocketIsClosed);
        }

        #endregion Receive

        #region Private Methods

        private void BeginReceiving()
        {
            isReceiving = true;
            messageReceived.Reset();

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

                if (Statistics != null)
                {
                    Statistics.BytesReceived.Increment(count);
                }

                OscPacket message = OscPacket.Read(buffer, count, (IPEndPoint)origin);

                if (Statistics != null && message.Error != OscPacketError.None)
                {
                    Statistics.ReceiveErrors.Increment(1);
                }

                lock (syncLock)
                {
                    if (this.count < receiveQueue.Length)
                    {
                        receiveQueue[writeIndex] = message;

                        writeIndex = NextWriteIndex;

                        this.count++;

                        // if this was the first message then signal
                        if (this.count == 1)
                        {
                            messageReceived.Set();
                        }
                    }
                }
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

        #endregion Private Methods
    }
}
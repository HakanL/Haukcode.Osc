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
using System.Threading;

namespace Rug.Osc
{
    /// <summary>
    /// Preforms common functions needed when listening for osc messages
    /// </summary>
    public class OscListener : IDisposable
    {
        public readonly IOscAddressManager OscAddressManager = new OscAddressManager();
        public readonly OscReceiver OscReceiver;

        private Thread thread;

        public event OscPacketEvent PacketProcessed;

        public event OscPacketEvent PacketReceived;

        /// <summary>
        /// This event will be raised whenever an unknown address is encountered
        /// </summary>
        public event EventHandler<UnknownAddressEventArgs> UnknownAddress;

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="multicast">a multicast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscListener(IPAddress address, IPAddress multicast, int port, int messageBufferSize, int maxPacketSize)
        {
            OscReceiver = new OscReceiver(address, multicast, port, messageBufferSize, maxPacketSize);

            OscAddressManager.UnknownAddress += OnUnknownAddress;
        }

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
		/// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscListener(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
        {
            OscReceiver = new OscReceiver(address, port, messageBufferSize, maxPacketSize);

            OscAddressManager.UnknownAddress += OnUnknownAddress;
        }

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="port">the port to listen on</param>
		public OscListener(IPAddress address, int port)
        {
            OscReceiver = new OscReceiver(address, port);

            OscAddressManager.UnknownAddress += OnUnknownAddress;
        }

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="multicast">a multicast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        public OscListener(IPAddress address, IPAddress multicast, int port)
        {
            OscReceiver = new OscReceiver(address, multicast, port);

            OscAddressManager.UnknownAddress += OnUnknownAddress;
        }

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
		public OscListener(int port, int messageBufferSize, int maxPacketSize)
        {
            OscReceiver = new OscReceiver(port, messageBufferSize, maxPacketSize);

            OscAddressManager.UnknownAddress += OnUnknownAddress;
        }

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
		public OscListener(int port)
        {
            OscReceiver = new OscReceiver(port);

            OscAddressManager.UnknownAddress += OnUnknownAddress;
        }

        /// <summary>
        /// Attach an event listener on to the given address
        /// </summary>
        /// <param name="address">the address of the contianer</param>
        /// <param name="event">the event to attach</param>
        public void Attach(string address, OscMessageEvent @event)
        {
            OscAddressManager.Attach(address, @event);
        }

        /// <summary>
        /// Close the receiver
        /// </summary>
        public void Close()
        {
            OscReceiver.Close();
        }

        /// <summary>
        /// Connect the receiver and start listening
        /// </summary>
        public void Connect()
        {
            OscReceiver.Connect();

            thread = new Thread(ListenLoop)
            {
                Name = "Osc Listener " + OscReceiver
            };

            thread.Start();
        }

        /// <summary>
        /// Detach an event listener
        /// </summary>
        /// <param name="address">the address of the container</param>
        /// <param name="event">the event to remove</param>
        public void Detach(string address, OscMessageEvent @event)
        {
            OscAddressManager.Detach(address, @event);
        }

        /// <summary>
        /// Dispose of all resources
        /// </summary>
        public void Dispose()
        {
            OscReceiver.Dispose();
            OscAddressManager.Dispose();
        }

        private void ListenLoop()
        {
            try
            {
                while (OscReceiver.State != OscSocketState.Closed)
                {
                    // if we are in a state to receive
                    if (OscReceiver.State != OscSocketState.Connected)
                    {
                        continue;
                    }

                    // get the next message
                    // this will block until one arrives or the socket is closed
                    OscPacket packet = OscReceiver.Receive();

                    PacketReceived?.Invoke(packet);

                    if (packet.Error == OscPacketError.None)
                    {
                        OscAddressManager.Invoke(packet);
                    }                    

                    PacketProcessed?.Invoke(packet);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void OnUnknownAddress(object sender, UnknownAddressEventArgs e)
        {
            UnknownAddress?.Invoke(this, e);
        }
    }
}
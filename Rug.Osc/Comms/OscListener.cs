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
		readonly OscAddressManager addressManager = new OscAddressManager();
		readonly OscReceiver receiver;
		Thread thread;

		/// <summary>
		/// This event will be raised whenever an unknown address is encountered
		/// </summary>
		public event EventHandler<UnknownAddressEventArgs> UnknownAddress;

        public event OscPacketEvent PacketReceived;
        public event OscPacketEvent PacketProcessed;

        #region Constructors

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="multicast">a multicast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscListener(IPAddress address, IPAddress multicast, int port, int messageBufferSize, int maxPacketSize)		
		{
			receiver = new OscReceiver(address, multicast, port, messageBufferSize, maxPacketSize);

			addressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
		/// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscListener(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
        {
			receiver = new OscReceiver(address, port, messageBufferSize, maxPacketSize);

			addressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="port">the port to listen on</param>
		public OscListener(IPAddress address, int port)
        {
			receiver = new OscReceiver(address, port);

			addressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

		/// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
		/// </summary>
		/// <param name="address">the local ip address to listen to</param>
		/// <param name="multicast">a multicast address to join</param>
		/// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
		public OscListener(IPAddress address, IPAddress multicast, int port)
		{
			receiver = new OscReceiver(address, multicast, port);

			addressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
		public OscListener(int port, int messageBufferSize, int maxPacketSize)            
        {
			receiver = new OscReceiver(port, messageBufferSize, maxPacketSize);

			addressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
		public OscListener(int port)
        {
			receiver = new OscReceiver(port);

			addressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        #endregion

		/// <summary>
		/// Connect the receiver and start listening
		/// </summary>
		public void Connect()
		{
			receiver.Connect();
			
			thread = new Thread(new ThreadStart(ListenLoop));

			thread.Start();
		}

		/// <summary>
		/// Close the receiver
		/// </summary>
		public void Close()
		{
			receiver.Close(); 			
		}

		/// <summary>
		/// Dispose of all resources
		/// </summary>
		public void Dispose()
		{
			receiver.Dispose();
			addressManager.Dispose(); 
		}

		/// <summary>
		/// Attach an event listener on to the given address
		/// </summary>
		/// <param name="address">the address of the contianer</param>
		/// <param name="event">the event to attach</param>
		public void Attach(string address, OscMessageEvent @event)
		{
			addressManager.Attach(address, @event); 
		}

		/// <summary>
		/// Detach an event listener 
		/// </summary>
		/// <param name="address">the address of the container</param>
		/// <param name="event">the event to remove</param>
		public void Detach(string address, OscMessageEvent @event)
		{
			addressManager.Detach(address, @event); 
		}

		void OnUnknownAddress(object sender, UnknownAddressEventArgs e)
		{
			if (UnknownAddress != null)
			{
				UnknownAddress(this, e); 
			}
		}

		private void ListenLoop()
		{
			try
			{
				while (receiver.State != OscSocketState.Closed)
				{
					// if we are in a state to receive
					if (receiver.State == OscSocketState.Connected)
					{
						// get the next message 
						// this will block until one arrives or the socket is closed
						OscPacket packet = receiver.Receive();

                        PacketReceived?.Invoke(packet); 

                        switch (addressManager.ShouldInvoke(packet))
						{
							case OscPacketInvokeAction.Invoke:
								addressManager.Invoke(packet);
								break;
							case OscPacketInvokeAction.DontInvoke:
								break;
							case OscPacketInvokeAction.HasError:
								break;
							case OscPacketInvokeAction.Pospone:
								break;
							default:
								break;
						}

                        PacketProcessed?.Invoke(packet); 
                    }
				}
			}
			catch (Exception ex)
			{

			}
		}
	}
}

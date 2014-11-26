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
		private readonly OscAddressManager m_Listener = new OscAddressManager();
		private readonly OscReceiver m_Receiver;
		private Thread m_Thread;

		/// <summary>
		/// This event will be raised whenever an unknown address is encountered
		/// </summary>
		public event EventHandler<UnknownAddressEventArgs> UnknownAddress; 

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
			m_Receiver = new OscReceiver(address, multicast, port, messageBufferSize, maxPacketSize);

			m_Listener.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
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
			m_Receiver = new OscReceiver(address, port, messageBufferSize, maxPacketSize);

			m_Listener.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="port">the port to listen on</param>
		public OscListener(IPAddress address, int port)
        {
			m_Receiver = new OscReceiver(address, port);

			m_Listener.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

		/// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
		/// </summary>
		/// <param name="address">the local ip address to listen to</param>
		/// <param name="multicast">a multicast address to join</param>
		/// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
		public OscListener(IPAddress address, IPAddress multicast, int port)
		{
			m_Receiver = new OscReceiver(address, multicast, port);

			m_Listener.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
		public OscListener(int port, int messageBufferSize, int maxPacketSize)            
        {
			m_Receiver = new OscReceiver(port, messageBufferSize, maxPacketSize);

			m_Listener.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
		public OscListener(int port)
        {
			m_Receiver = new OscReceiver(port);

			m_Listener.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
		}

        #endregion

		/// <summary>
		/// Connect the receiver and start listening
		/// </summary>
		public void Connect()
		{
			m_Receiver.Connect();
			
			m_Thread = new Thread(new ThreadStart(ListenLoop));

			m_Thread.Start();
		}

		/// <summary>
		/// Close the receiver
		/// </summary>
		public void Close()
		{
			m_Receiver.Close(); 			
		}

		/// <summary>
		/// Dispose of all resources
		/// </summary>
		public void Dispose()
		{
			m_Receiver.Dispose();
			m_Listener.Dispose(); 
		}

		/// <summary>
		/// Attach an event listener on to the given address
		/// </summary>
		/// <param name="address">the address of the contianer</param>
		/// <param name="event">the event to attach</param>
		public void Attach(string address, OscMessageEvent @event)
		{
			m_Listener.Attach(address, @event); 
		}

		/// <summary>
		/// Detach an event listener 
		/// </summary>
		/// <param name="address">the address of the container</param>
		/// <param name="event">the event to remove</param>
		public void Detach(string address, OscMessageEvent @event)
		{
			m_Listener.Detach(address, @event); 
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
				while (m_Receiver.State != OscSocketState.Closed)
				{
					// if we are in a state to receive
					if (m_Receiver.State == OscSocketState.Connected)
					{
						// get the next message 
						// this will block until one arrives or the socket is closed
						OscPacket packet = m_Receiver.Receive();

						switch (m_Listener.ShouldInvoke(packet))
						{
							case OscPacketInvokeAction.Invoke:
								m_Listener.Invoke(packet);
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
					}
				}
			}
			catch (Exception ex)
			{

			}
		}
	}
}

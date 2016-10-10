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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Rug.Osc
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

		#region Private Members

		readonly object syncLock = new object();
		readonly AutoResetEvent queueEmpty = new AutoResetEvent(true);

		readonly byte[] buffer;

		readonly OscPacket[] sendQueue;

		int writeIndex = 0;
		int readIndex = 0;
		int count = 0;

		#endregion

		#region Properties

		public override OscSocketType OscSocketType
		{
			get { return Osc.OscSocketType.Send; }
		}

        public event OscPacketEvent PacketSent;

        /// <summary>
        /// Use a value greater than 0 to set the disconnect time out in milliseconds use a value less than or equal to 0 for an infinite timeout
        /// </summary>
        public int DisconnectTimeout { get; set; } 

		/// <summary>
		/// The next queue index to write messages to 
		/// </summary>
		private int NextWriteIndex
		{
			get
			{
				int index = writeIndex + 1;

				if (index >= sendQueue.Length)
				{
					index -= sendQueue.Length;
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

				if (index >= sendQueue.Length)
				{
					index -= sendQueue.Length;
				}

				return index;
			}
		}

		public string DEBUG_ConnectedState { get { return Socket != null ? "Socket Connected: " + Socket.Connected : "NO SOCKET"; } }

		#endregion

		#region Constructors

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
			sendQueue = new OscPacket[messageBufferSize];
		}

		#endregion

		#region Protected Overrides

		protected override void OnConnect()
		{
			// set the timeout for send
			// Socket.SendTimeout = 1000; 
		}

		protected override void OnClosing()
		{
			WaitForAllMessagesToComplete();
		}

		#endregion

		#region Send

		/// <summary>
		/// Add a osc message to the send queue
		/// </summary>
		/// <param name="message">message to send</param>
		public void Send(OscPacket message)
		{
			if (State == OscSocketState.Connected)
			{
				lock (syncLock)
				{
					queueEmpty.Reset();

					if (count >= sendQueue.Length)
					{
						return;
					}

					sendQueue[writeIndex] = message;

					writeIndex = NextWriteIndex;
					count++;

					if (count == 1)
					{
						int size = message.Write(buffer);

						if (Statistics != null)
						{
							message.IncrementSendStatistics(Statistics); 

							Statistics.BytesSent.Increment(size);
						}

                        PacketSent?.Invoke(message); 

                        Socket.BeginSend(buffer, 0, size, SocketFlags, Send_Callback, message);
					}
				}
			}
		}

		#endregion

		#region Wait For All Messages To Complete

		/// <summary>
		/// Wait till all messages in the queue have been sent
		/// </summary>
		public void WaitForAllMessagesToComplete()
		{
			queueEmpty.WaitOne(Math.Max(-1, DisconnectTimeout));
		}

		#endregion

		#region Private Methods

		void Send_Callback(IAsyncResult ar)
		{
			bool shouldClose = false; 

			lock (syncLock)
			{
				try
				{
					SocketError error;

					Socket.EndSend(ar, out error);

					if (sendQueue[readIndex].IsSameInstance(ar.AsyncState as OscPacket) == false)
					{
						Debug.WriteLine("Objects do not match at index " + readIndex);
					}

					shouldClose = Socket.Connected == false;

					count--;
					readIndex = NextReadIndex;

					if (count > 0 && State == OscSocketState.Connected)
					{
						OscPacket packet = sendQueue[readIndex];

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
			}

			if (shouldClose == true)
			{
				Dispose();
			}
		}

		#endregion
	}
}
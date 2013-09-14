﻿/* 
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

namespace Rug.Osc
{
	#region Osc Socket State

	/// <summary>
	/// The connection state of an osc socket
	/// </summary>
	public enum OscSocketState
	{
		/// <summary>
		/// The socket has never been connected
		/// </summary>
		NotConnected,

		/// <summary>
		/// The socket is connected
		/// </summary>
		Connected,

		/// <summary>
		/// The socket is closing
		/// </summary>
		Closing,

		/// <summary>
		/// The socket has been closed
		/// </summary>
		Closed,
	}

	#endregion

	#region Osc Socket Type

	/// <summary>
	/// Socket direction type
	/// </summary>
	public enum OscSocketType
	{
		/// <summary>
		/// Sender
		/// </summary>
		Send,

		/// <summary>
		/// Receiver
		/// </summary>
		Receive
	}

	#endregion

	/// <summary>
	/// Base class for all osc UDP communication
	/// </summary>
	public abstract class OscSocket : IDisposable
	{
		/// <summary>
		/// Default maximum packet size
		/// </summary>
		public const int DefaultPacketSize = 2048;

		/// <summary>
		/// Default time to live for multicast packets
		/// </summary>
		public const int DefaultMulticastTimeToLive = 8;

		/// <summary>
		/// Is the supplied address a UDP multicast address
		/// </summary>
		/// <param name="address">An address</param>
		/// <returns>true if it is a multicast address</returns>
		public static bool IsMulticastAddress(IPAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return address.IsIPv6Multicast;
			}
			else
			{
				return (address.GetAddressBytes()[0] & MulticastAddressMask) == MulticastAddressValue;
			}
		}

		#region Private Members

		private const byte MulticastAddressMask = 0xF0;
		private const byte MulticastAddressValue = 0xE0;

		private readonly object m_Lock = new object();

		private Socket m_Socket;
		private readonly int m_Port;

		private readonly IPAddress m_RemoteAddress;
		private readonly IPEndPoint m_RemoteEndPoint;

		private readonly IPAddress m_LocalAddress;
		private readonly IPEndPoint m_LocalEndPoint;

		private readonly bool m_IsMulticastEndPoint;
		private readonly int m_TimeToLive;

		private OscSocketState m_State = OscSocketState.NotConnected;
		private readonly System.Net.Sockets.SocketFlags m_SocketFlags;

		#endregion

		#region Properties

		/// <summary>
		/// Network port
		/// </summary>
		public int Port { get { return m_Port; } }

		/// <summary>
		/// Local IP address
		/// </summary>
		public IPAddress LocalAddress { get { return m_LocalAddress; } }

		/// <summary>
		/// Local Ip end point 
		/// </summary>
		public IPEndPoint LocalEndPoint { get { return m_LocalEndPoint; } }

		/// <summary>
		/// Remote IP address
		/// </summary>
		public IPAddress RemoteAddress { get { return m_RemoteAddress; } }

		/// <summary>
		/// Remote Ip end point 
		/// </summary>
		public IPEndPoint RemoteEndPoint { get { return m_RemoteEndPoint; } }

		/// <summary>
		/// Is the remote endpoint a multicast address
		/// </summary>
		public bool IsMulticastEndPoint { get { return m_IsMulticastEndPoint; } }

		/// <summary>
		/// Time to live for multicast packets
		/// </summary>
		public int TimeToLive { get { return m_TimeToLive; } }

		/// <summary>
		/// The current state of the socket
		/// </summary>
		public OscSocketState State { get { return m_State; } protected set { m_State = value; } }

		/// <summary>
		/// The socket type
		/// </summary>
		public abstract OscSocketType OscSocketType { get; }

		/// <summary>
		/// The instance of the socket 
		/// </summary>
		protected Socket Socket { get { return m_Socket; } }

		/// <summary>
		/// Flags for the socket
		/// </summary>
		protected System.Net.Sockets.SocketFlags SocketFlags { get { return m_SocketFlags; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="local">The ip address of the local end point</param>
		/// <param name="remote">The ip address of the remote end point</param>
		/// <param name="port">the port</param>
		/// <param name="timeToLive">TTL value to apply to packets</param>
		internal OscSocket(IPAddress local, IPAddress remote, int port, int timeToLive)
		{
			if (local.AddressFamily != AddressFamily.InterNetwork &&
				local.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(String.Format(Strings.Socket_UnsupportedAddressFamily, local.AddressFamily), "local");
			}

			if (remote.AddressFamily != AddressFamily.InterNetwork &&
				remote.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(String.Format(Strings.Socket_UnsupportedAddressFamily, local.AddressFamily), "remote");
			}

			if (local.AddressFamily != remote.AddressFamily)
			{
				throw new Exception(Strings.Socket_AddressFamilyIncompatible);
			}

			m_LocalAddress = local;
			m_RemoteAddress = remote;
			m_Port = port;

			m_RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);
			m_LocalEndPoint = new IPEndPoint(LocalAddress, Port);

			m_TimeToLive = timeToLive;

			m_IsMulticastEndPoint = IsMulticastAddress(m_RemoteAddress);

			m_SocketFlags = System.Net.Sockets.SocketFlags.None;
		}

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="local">The ip address of the local end point</param>
		/// <param name="remote">The ip address of the remote end point</param>
		/// <param name="port">the port</param>
		internal OscSocket(IPAddress local, IPAddress remote, int port)
			: this(local, remote, port, DefaultMulticastTimeToLive)
		{

		}

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="address">The ip address of the local or remote end point</param>
		/// <param name="port">the port</param>
		internal OscSocket(IPAddress address, int port)
		{
			if (address.AddressFamily != AddressFamily.InterNetwork &&
				address.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(String.Format(Strings.Socket_UnsupportedAddressFamily, address.AddressFamily), "address");
			}

			bool useIpV6 = address.AddressFamily == AddressFamily.InterNetworkV6;

			m_Port = port;

			if (this.OscSocketType == Osc.OscSocketType.Send)
			{
				m_LocalAddress = useIpV6 ? IPAddress.IPv6Any : IPAddress.Any;
				m_RemoteAddress = address;
			}
			else
			{
				m_LocalAddress = address;
				m_RemoteAddress = useIpV6 ? IPAddress.IPv6Any : IPAddress.Any;
			}

			m_LocalEndPoint = new IPEndPoint(LocalAddress, Port);
			m_RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);

			m_IsMulticastEndPoint = IsMulticastAddress(m_RemoteAddress);

			m_SocketFlags = System.Net.Sockets.SocketFlags.None;
		}

		/// <summary>
		/// Create a new socket for any local IP address and a port
		/// </summary>
		/// <param name="port">the port to bind to</param>
		internal OscSocket(int port)
		{
			m_Port = port;
			m_LocalAddress = IPAddress.Any;
			m_RemoteAddress = IPAddress.Any;

			m_LocalEndPoint = new IPEndPoint(LocalAddress, Port);
			m_RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);

			m_SocketFlags = System.Net.Sockets.SocketFlags.None;

			m_IsMulticastEndPoint = false;
		}

		#endregion

		#region Connect

		/// <summary>
		/// Connect the socket
		/// </summary>
		public void Connect()
		{
			lock (m_Lock)
			{
				if (m_State != OscSocketState.NotConnected &&
					m_State != OscSocketState.Closed)
				{
					throw new Exception(Strings.Socket_AlreadyOpenOrNotClosed);
				}

				bool useIpV6 = m_LocalAddress.AddressFamily == AddressFamily.InterNetworkV6;

				// create the instance of the socket
				m_Socket = new Socket(useIpV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				m_Socket.Blocking = false;

				if (RemoteAddress == IPAddress.Broadcast)
				{
					m_Socket.EnableBroadcast = true;
					m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				}

				m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 16);
				m_Socket.ExclusiveAddressUse = false;

				m_Socket.Bind(m_LocalEndPoint);

				if (OscSocketType == Osc.OscSocketType.Receive)
				{
					if (IsMulticastEndPoint == true)
					{
						if (useIpV6 == true)
						{
							m_Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(m_RemoteAddress));
						}
						else
						{
							m_Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(m_RemoteAddress));
						}
					}
				}
				else
				{
					if (IsMulticastEndPoint == true)
					{
						if (useIpV6 == true)
						{
							m_Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, TimeToLive);
						}
						else
						{
							m_Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TimeToLive);
						}
					}

					m_Socket.Connect(m_RemoteEndPoint);
				}

				m_State = OscSocketState.Connected;

				OnConnect();
			}
		}

		#endregion

		/// <summary>
		/// Called when the socket is connected
		/// </summary>
		protected abstract void OnConnect();

		/// <summary>
		/// Called when the socket is closing
		/// </summary>
		protected abstract void OnClosing();

		#region Close

		/// <summary>
		/// Closes the socket and releases all resources associated with it
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		#endregion

		#region Dispose

		/// <summary>
		/// Closes the socket and releases all resources associated with it
		/// </summary>
		public void Dispose()
		{
			lock (m_Lock)
			{
				if (m_State == OscSocketState.Connected)
				{
					m_State = OscSocketState.Closing;

					OnClosing();

					m_Socket.Close();

					m_Socket = null;

					m_State = OscSocketState.Closed;
				}
			}
		}

		#endregion
	}
}

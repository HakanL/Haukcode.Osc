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

		const byte MulticastAddressMask = 0xF0;
		const byte MulticastAddressValue = 0xE0;

		readonly object syncLock = new object();

		Socket socket;
		readonly int port;
		
		int localPort;
		bool useDynamicLocalPort;

		readonly IPAddress remoteAddress;
		readonly IPEndPoint remoteEndPoint;

		readonly IPAddress localAddress;
		readonly IPEndPoint localEndPoint;

		readonly bool isMulticastEndPoint;
		readonly int timeToLive;
		readonly bool useIpV6;

		OscSocketState state = OscSocketState.NotConnected;
		readonly System.Net.Sockets.SocketFlags socketFlags;		

		#endregion

		#region Properties

		/// <summary>
		/// Network port
		/// </summary>
		public int Port { get { return port; } }

		/// <summary>
		/// Local network port, this is the result of dynamic port allocation is UseDynamicLocalPort is true
		/// </summary>
		public int LocalPort { get { return localPort; } } 

		/// <summary>
		/// True of the socket should use dynamic local port assignment
		/// </summary>
		public bool UseDynamicLocalPort { get { return useDynamicLocalPort; } }		

		/// <summary>
		/// Local IP address
		/// </summary>
		public IPAddress LocalAddress { get { return localAddress; } }

		/// <summary>
		/// Local Ip end point 
		/// </summary>
		public IPEndPoint LocalEndPoint { get { return localEndPoint; } }

		/// <summary>
		/// Remote IP address
		/// </summary>
		public IPAddress RemoteAddress { get { return remoteAddress; } }

		/// <summary>
		/// Remote Ip end point 
		/// </summary>
		public IPEndPoint RemoteEndPoint { get { return remoteEndPoint; } }

		/// <summary>
		/// Is the remote origin a multicast address
		/// </summary>
		public bool IsMulticastEndPoint { get { return isMulticastEndPoint; } }

		/// <summary>
		/// Time to live for multicast packets
		/// </summary>
		public int TimeToLive { get { return timeToLive; } }

		/// <summary>
		/// If true this socket uses IPv6 
		/// </summary>
		public bool UseIPv6 { get { return useIpV6; } } 

		/// <summary>
		/// The current state of the socket
		/// </summary>
		public OscSocketState State { get { return state; } protected set { state = value; } }

		/// <summary>
		/// The socket type
		/// </summary>
		public abstract OscSocketType OscSocketType { get; }

		public OscCommunicationStatistics Statistics { get; set; }

		/// <summary>
		/// The instance of the socket 
		/// </summary>
		protected Socket Socket { get { return socket; } }

		/// <summary>
		/// Flags for the socket
		/// </summary>
		protected System.Net.Sockets.SocketFlags SocketFlags { get { return socketFlags; } }		

		#endregion

		#region Constructors

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="local">The ip address of the local end point</param>
		/// <param name="localPort">the local port, use 0 for dynamically assigned</param>
		/// <param name="remote">The ip address of the remote end point</param>
		/// <param name="remotePort">the remote port</param>
		/// <param name="timeToLive">TTL value to apply to packets</param>
		internal OscSocket(IPAddress local, int localPort, IPAddress remote, int remotePort, int timeToLive)
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
				throw new ArgumentException(Strings.Socket_AddressFamilyIncompatible, "remote");
			}

			CheckPortRange(remotePort, "remotePort", Strings.Socket_RemotePortOutOfRange, false);

			CheckPortRange(localPort, "localPort", Strings.Socket_LocalPortOutOfRange, true);

			// if the local port is 0 then we are to use dynamic port assignment
			useDynamicLocalPort = localPort == 0;

			localAddress = local;
			remoteAddress = remote;
			port = remotePort;
			this.localPort = localPort; 

			remoteEndPoint = new IPEndPoint(RemoteAddress, remotePort);
			localEndPoint = new IPEndPoint(LocalAddress, localPort);

			this.timeToLive = timeToLive;

			useIpV6 = localAddress.AddressFamily == AddressFamily.InterNetworkV6;

			isMulticastEndPoint = IsMulticastAddress(remoteAddress);

			socketFlags = System.Net.Sockets.SocketFlags.None;
		}

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="local">The ip address of the local end point</param>
		/// <param name="remote">The ip address of the remote end point</param>
		/// <param name="port">the port, use 0 for dynamically assigned (receiver only)</param>
		/// <param name="timeToLive">TTL value to apply to packets</param>
		internal OscSocket(IPAddress local, IPAddress remote, int port, int timeToLive)
			: this(local, port, remote, port, timeToLive)
		{

		}

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="local">The ip address of the local end point</param>
		/// <param name="remote">The ip address of the remote end point</param>
		/// <param name="port">the port, use 0 for dynamically assigned (receiver only)</param>
		internal OscSocket(IPAddress local, IPAddress remote, int port)
			: this(local, remote, port, DefaultMulticastTimeToLive)
		{

		}

		/// <summary>
		/// Create a new socket for an address and port
		/// </summary>
		/// <param name="address">The ip address of the local or remote end point</param>
		/// <param name="port">the port, use 0 for dynamically assigned (receiver only)</param>
		internal OscSocket(IPAddress address, int port)
		{
			if (address.AddressFamily != AddressFamily.InterNetwork &&
				address.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(String.Format(Strings.Socket_UnsupportedAddressFamily, address.AddressFamily), "address");
			}

			useIpV6 = address.AddressFamily == AddressFamily.InterNetworkV6;

			CheckPortRange(port, "port", Strings.Socket_PortOutOfRange, this.OscSocketType == Osc.OscSocketType.Receive);

			this.port = port;
			localPort = port; 			
			useDynamicLocalPort = port == 0;

			if (this.OscSocketType == Osc.OscSocketType.Send)
			{
				localAddress = useIpV6 ? IPAddress.IPv6Any : IPAddress.Any;
				remoteAddress = address;
			}
			else
			{
				localAddress = address;
				remoteAddress = useIpV6 ? IPAddress.IPv6Any : IPAddress.Any;
			}

			localEndPoint = new IPEndPoint(LocalAddress, Port);
			remoteEndPoint = new IPEndPoint(RemoteAddress, Port);

			isMulticastEndPoint = IsMulticastAddress(remoteAddress);

			socketFlags = System.Net.Sockets.SocketFlags.None;
		}

		/// <summary>
		/// Create a new socket for any local IP address and a port
		/// </summary>
		/// <param name="port">the port to bind to, use 0 for dynamically assigned (receiver only)</param>
		internal OscSocket(int port)
		{
			CheckPortRange(port, "port", Strings.Socket_PortOutOfRange, this.OscSocketType == Osc.OscSocketType.Receive);

			this.port = port;
			localPort = port;
			useDynamicLocalPort = port == 0;

			localAddress = IPAddress.Any;
			remoteAddress = IPAddress.Any;			

			localEndPoint = new IPEndPoint(LocalAddress, Port);
			remoteEndPoint = new IPEndPoint(RemoteAddress, Port);

			socketFlags = System.Net.Sockets.SocketFlags.None;
			
			useIpV6 = false;

			isMulticastEndPoint = false;
		}

		private void CheckPortRange(int port, string arg, string errorMessage, bool allowZero)
		{
			// if zero ports are allowed and the port is zero then just return
			if (allowZero == true && port == 0) 
			{
				return; 
			}

			// check the range of the port
			if (port < 1 || port > 65535)
			{
				throw new ArgumentOutOfRangeException(arg, errorMessage);
			}
		}

		#endregion

		#region Connect

		/// <summary>
		/// Connect the socket
		/// </summary>
		public void Connect()
		{
			lock (syncLock)
			{
				if (state != OscSocketState.NotConnected &&
					state != OscSocketState.Closed)
				{
					throw new OscSocketStateException(this, state, Strings.Socket_AlreadyOpenOrNotClosed);
				}				

				// create the instance of the socket
				socket = new Socket(useIpV6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				socket.Blocking = false;

				if (RemoteAddress == IPAddress.Broadcast)
				{
					socket.EnableBroadcast = true;
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
				}

				// allow the reuse of addresses
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 16);
				socket.ExclusiveAddressUse = false;

				if (useDynamicLocalPort == true)
				{
					IPEndPoint tempEndPoint = new IPEndPoint(localEndPoint.Address, 0);

					// bind the the temp local origin
					socket.Bind(tempEndPoint);

					// set the local port from the resolved socket port 
					switch (socket.LocalEndPoint.AddressFamily)
					{
						case AddressFamily.InterNetworkV6:
						case AddressFamily.InterNetwork:
							localPort = ((IPEndPoint)socket.LocalEndPoint).Port;
							localEndPoint.Port = localPort;						
							break;
						default:
							throw new InvalidOperationException(String.Format(Strings.Socket_UnsupportedAddressFamily, socket.LocalEndPoint.AddressFamily));
					}
				}
				else
				{
					// bind the local origin
					socket.Bind(localEndPoint);
				}

				if (OscSocketType == Osc.OscSocketType.Receive)
				{
					if (IsMulticastEndPoint == true)
					{
						if (useIpV6 == true)
						{
							// add to the membership of the IPv6 multicast origin
							socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(remoteAddress));
						}
						else
						{
							// add to the membership of the IP multicast origin
							socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(remoteAddress));
						}
					}
				}
				else
				{
					if (IsMulticastEndPoint == true)
					{
						if (useIpV6 == true)
						{
							// set the multicast TTL
							socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, TimeToLive);
						}
						else
						{
							// set the multicast TTL
							socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TimeToLive);
						}
					}

					// connect to the remote origin 
					socket.Connect(remoteEndPoint);
				}

				state = OscSocketState.Connected;

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
			lock (syncLock)
			{
				if (state == OscSocketState.Connected)
				{
					state = OscSocketState.Closing;

					OnClosing();					

					socket.Close();

					socket = null;

					state = OscSocketState.Closed;
				}
			}
		}

		#endregion
	}
}
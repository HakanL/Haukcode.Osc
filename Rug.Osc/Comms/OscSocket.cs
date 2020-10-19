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

    /// <summary>
    /// Base class for all osc UDP communication
    /// </summary>
    public abstract class OscSocket : IDisposable
    {
        /// <summary>
        /// Default time to live for multicast packets
        /// </summary>
        public const int DefaultMulticastTimeToLive = 8;

        /// <summary>
        /// Default maximum packet size
        /// </summary>
        public const int DefaultPacketSize = 2048;

        private const byte MulticastAddressMask = 0xF0;

        private const byte MulticastAddressValue = 0xE0;

        private readonly object syncLock = new object();

        /// <summary>
        /// Is the remote origin a multicast address
        /// </summary>
        public bool IsMulticastEndPoint { get; }

        /// <summary>
        /// Local IP address
        /// </summary>
        public IPAddress LocalAddress { get; }

        /// <summary>
        /// Local Ip end point
        /// </summary>
        public IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Local network port, this is the result of dynamic port allocation is UseDynamicLocalPort is true
        /// </summary>
        public int LocalPort { get; private set; }

        /// <summary>
        /// The socket type
        /// </summary>
        public abstract OscSocketType OscSocketType { get; }

        /// <summary>
        /// Network port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Remote IP address
        /// </summary>
        public IPAddress RemoteAddress { get; }

        /// <summary>
        /// Remote Ip end point
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// The current state of the socket
        /// </summary>
        public OscSocketState State { get; protected set; } = OscSocketState.NotConnected;

        public OscCommunicationStatistics Statistics { get; set; }

        /// <summary>
        /// Time to live for multicast packets
        /// </summary>
        public int TimeToLive { get; }

        /// <summary>
        /// True of the socket should use dynamic local port assignment
        /// </summary>
        public bool UseDynamicLocalPort { get; }

        /// <summary>
        /// If true this socket uses IPv6
        /// </summary>
        public bool UseIPv6 { get; }

        /// <summary>
        /// Flags for the socket
        /// </summary>
        protected System.Net.Sockets.SocketFlags SocketFlags { get; }

        /// <summary>
        /// The instance of the socket
        /// </summary>
        protected Socket Socket { get; private set; }

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
                throw new ArgumentException($@"Unsupported address family '{local.AddressFamily}'", nameof(local));
            }

            if (remote.AddressFamily != AddressFamily.InterNetwork &&
                remote.AddressFamily != AddressFamily.InterNetworkV6)
            {
                throw new ArgumentException($@"Unsupported address family '{local.AddressFamily}'", nameof(remote));
            }

            if (local.AddressFamily != remote.AddressFamily)
            {
                throw new ArgumentException("Both local and remote must belong to the same address family", nameof(remote));
            }

            CheckPortRange(remotePort, nameof(remotePort), "The valid range for remote port numbers is 1 to 65535", false);

            CheckPortRange(localPort, nameof(localPort), "The valid range for local port numbers is 1 to 65535 or 0 for a dynamically assigned port", true);

            // if the local port is 0 then we are to use dynamic port assignment
            UseDynamicLocalPort = localPort == 0;

            LocalAddress = local;
            RemoteAddress = remote;
            Port = remotePort;
            LocalPort = localPort;

            RemoteEndPoint = new IPEndPoint(RemoteAddress, remotePort);
            LocalEndPoint = new IPEndPoint(LocalAddress, localPort);

            TimeToLive = timeToLive;

            UseIPv6 = LocalAddress.AddressFamily == AddressFamily.InterNetworkV6;

            IsMulticastEndPoint = IsMulticastAddress(RemoteAddress);

            SocketFlags = System.Net.Sockets.SocketFlags.None;
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
                throw new ArgumentException($@"Unsupported address family '{address.AddressFamily}'", nameof(address));
            }

            UseIPv6 = address.AddressFamily == AddressFamily.InterNetworkV6;

            CheckPortRange(port, nameof(port), "The valid range for port numbers is 1 to 65535", OscSocketType == Osc.OscSocketType.Receive);

            Port = port;
            LocalPort = port;
            UseDynamicLocalPort = port == 0;

            if (OscSocketType == Osc.OscSocketType.Send)
            {
                LocalAddress = UseIPv6 ? IPAddress.IPv6Any : IPAddress.Any;
                RemoteAddress = address;
            }
            else
            {
                LocalAddress = address;
                RemoteAddress = UseIPv6 ? IPAddress.IPv6Any : IPAddress.Any;
            }

            LocalEndPoint = new IPEndPoint(LocalAddress, Port);
            RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);

            IsMulticastEndPoint = IsMulticastAddress(RemoteAddress);

            SocketFlags = SocketFlags.None;
        }

        /// <summary>
        /// Create a new socket for any local IP address and a port
        /// </summary>
        /// <param name="port">the port to bind to, use 0 for dynamically assigned (receiver only)</param>
        internal OscSocket(int port)
        {
            CheckPortRange(port, nameof(port), "The valid range for port numbers is 1 to 65535", this.OscSocketType == Osc.OscSocketType.Receive);

            this.Port = port;
            LocalPort = port;
            UseDynamicLocalPort = port == 0;

            LocalAddress = IPAddress.Any;
            RemoteAddress = IPAddress.Any;

            LocalEndPoint = new IPEndPoint(LocalAddress, Port);
            RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);

            SocketFlags = SocketFlags.None;

            UseIPv6 = false;

            IsMulticastEndPoint = false;
        }

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

        /// <summary>
        /// Closes the socket and releases all resources associated with it
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Connect the socket
        /// </summary>
        public void Connect()
        {
            lock (syncLock)
            {
                if (State != OscSocketState.NotConnected &&
                    State != OscSocketState.Closed)
                {
                    throw new OscSocketStateException(this, State, "The socket is already open or is not fully closed");
                }

                // create the instance of the socket
                Socket = new Socket(UseIPv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                {
                    Blocking = false
                };

                if (Equals(RemoteAddress, IPAddress.Broadcast))
                {
                    Socket.EnableBroadcast = true;
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                }

                // allow the reuse of addresses
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 16);
                Socket.ExclusiveAddressUse = false;

                if (UseDynamicLocalPort == true)
                {
                    IPEndPoint tempEndPoint = new IPEndPoint(LocalEndPoint.Address, 0);

                    // bind the the temp local origin
                    Socket.Bind(tempEndPoint);

                    // set the local port from the resolved socket port
                    switch (Socket.LocalEndPoint.AddressFamily)
                    {
                        case AddressFamily.InterNetworkV6:
                        case AddressFamily.InterNetwork:
                            LocalPort = ((IPEndPoint)Socket.LocalEndPoint).Port;
                            LocalEndPoint.Port = LocalPort;
                            break;

                        default:
                            throw new InvalidOperationException($@"Unsupported address family '{Socket.LocalEndPoint.AddressFamily}'");
                    }
                }
                else
                {
                    // bind the local origin
                    Socket.Bind(LocalEndPoint);
                }

                if (OscSocketType == Osc.OscSocketType.Receive)
                {
                    if (IsMulticastEndPoint == true)
                    {
                        if (UseIPv6 == true)
                        {
                            // add to the membership of the IPv6 multicast origin
                            Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new IPv6MulticastOption(RemoteAddress));
                        }
                        else
                        {
                            // add to the membership of the IP multicast origin
                            Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(RemoteAddress));
                        }
                    }
                }
                else
                {
                    if (IsMulticastEndPoint == true)
                    {
                        if (UseIPv6 == true)
                        {
                            // set the multicast TTL
                            Socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.MulticastTimeToLive, TimeToLive);
                        }
                        else
                        {
                            // set the multicast TTL
                            Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TimeToLive);
                        }
                    }

                    // connect to the remote origin
                    Socket.Connect(RemoteEndPoint);
                }

                State = OscSocketState.Connected;

                OnConnect();
            }
        }

        /// <summary>
        /// Closes the socket and releases all resources associated with it
        /// </summary>
        public void Dispose()
        {
            lock (syncLock)
            {
                if (State != OscSocketState.Connected)
                {
                    return;
                }

                State = OscSocketState.Closing;

                OnClosing();

                Socket.Close();

                Socket = null;

                State = OscSocketState.Closed;
            }
        }

        /// <summary>
        /// Called when the socket is closing
        /// </summary>
        protected abstract void OnClosing();

        /// <summary>
        /// Called when the socket is connected
        /// </summary>
        protected abstract void OnConnect();

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
    }
}
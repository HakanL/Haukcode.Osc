﻿/* 
 * Rug.Osc 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2013 Phill Tew. All rights reserved.
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

        #region Private Members

        private object m_Lock = new object(); 

        private Socket m_Socket; 
        private int m_Port;

        private IPAddress m_RemoteAddress;
		private IPEndPoint m_RemoteEndPoint; 

		private IPAddress m_LocalAddress;
		private IPEndPoint m_LocalEndPoint;

		private OscSocketState m_State = OscSocketState.NotConnected;
        private System.Net.Sockets.SocketFlags m_SocketFlags;

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
		internal OscSocket(IPAddress local, IPAddress remote, int port)
		{
			m_LocalAddress = local;
			m_RemoteAddress = remote;
			m_Port = port;

			m_RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);
			m_LocalEndPoint = new IPEndPoint(LocalAddress, Port);
		}

        /// <summary>
        /// Create a new socket for an address and port
        /// </summary>
        /// <param name="address">The ip address of the local or remote end point</param>
        /// <param name="port">the port</param>
        internal OscSocket(IPAddress address, int port)
        {
            m_Port = port;

			if (this.OscSocketType == Osc.OscSocketType.Send)
			{
				m_LocalAddress = IPAddress.Any;
				m_RemoteAddress = address;
			}
			else
			{
				m_LocalAddress = address;
				m_RemoteAddress = IPAddress.Any;
			}

			m_LocalEndPoint = new IPEndPoint(LocalAddress, Port); 
			m_RemoteEndPoint = new IPEndPoint(RemoteAddress, Port);
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

                // create the instance of the socket
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                m_Socket.Blocking = false;

                m_SocketFlags = System.Net.Sockets.SocketFlags.None;

                if (RemoteAddress == IPAddress.Broadcast)
                {
                    m_Socket.EnableBroadcast = true;
                }

                if (OscSocketType == Osc.OscSocketType.Receive)
                {
                    m_Socket.Bind(m_LocalEndPoint);
                }
                else
                {
					if (m_LocalAddress != IPAddress.Any)
					{
						m_Socket.Bind(m_LocalEndPoint);
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

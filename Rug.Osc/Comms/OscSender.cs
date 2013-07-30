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

		private readonly object m_Lock = new object();
		private readonly AutoResetEvent m_QueueEmpty = new AutoResetEvent(true);

		private readonly byte[] m_Bytes;

		private readonly OscPacket[] m_SendQueue;
        private int m_WriteIndex = 0;
        private int m_ReadIndex = 0;
        private int m_Count = 0;

        #endregion

        #region Properties

		public override OscSocketType OscSocketType
		{
			get { return Osc.OscSocketType.Send; }
		}

        /// <summary>
        /// The next queue index to write messages to 
        /// </summary>
        private int NextWriteIndex
        {
            get
            {
                int index = m_WriteIndex + 1;

                if (index >= m_SendQueue.Length)
                {
                    index -= m_SendQueue.Length; 
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
                int index = m_ReadIndex + 1;

                if (index >= m_SendQueue.Length)
                {
                    index -= m_SendQueue.Length;
                }

                return index;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new Osc UDP sender. Note the underlying soket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        public OscSender(IPAddress address, int port)
            : this(address, port, DefaultMessageBufferSize, DefaultPacketSize) 
        {
        }

		/// <summary>
		/// Create a new Osc UDP sender. Note the underlying soket will not be connected untill Connect is called
		/// </summary>
		/// <param name="local">the ip address to send from</param>
		/// <param name="remote">the ip address to send to</param>
		/// <param name="port">the port to send to</param>
		public OscSender(IPAddress local, IPAddress remote, int port)
			: this(local, remote, port, DefaultMessageBufferSize, DefaultPacketSize)
		{
		}

        /// <summary>
		/// Create a new Osc UDP sender. Note the underlying soket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
            : this(IPAddress.Any, address, port, messageBufferSize, maxPacketSize)
        {
        }

		/// <summary>
		/// Create a new Osc UDP sender. Note the underlying soket will not be connected untill Connect is called
		/// </summary>
		/// <param name="local">the ip address to send from</param>
		/// <param name="remote">the ip address to send to</param>
		/// <param name="port">the port to send to</param>
		/// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
		/// <param name="maxPacketSize">the maximum packet size of any message</param>
		public OscSender(IPAddress local, IPAddress remote, int port, int messageBufferSize, int maxPacketSize)
			: base(local, remote, port)
		{
			m_Bytes = new byte[maxPacketSize];
			m_SendQueue = new OscPacket[messageBufferSize];
		}

        #endregion 

        #region Protected Overrides

        protected override void OnConnect()
        {
            
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
                lock (m_Lock)
                {
                    m_QueueEmpty.Reset();

                    if (m_Count >= m_SendQueue.Length)
                    {
                        return;
                    }
                    
                    m_SendQueue[m_WriteIndex] = message;

                    m_WriteIndex = NextWriteIndex; 
                    m_Count++;

                    if (m_Count == 1)
                    {
                        int size = message.Write(m_Bytes);

                        Socket.BeginSend(m_Bytes, 0, size, SocketFlags, Send_Callback, message);
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
            m_QueueEmpty.WaitOne();
        }

        #endregion

        #region Private Methods

        void Send_Callback(IAsyncResult ar)
        {
            lock (m_Lock)
            {
                try
                {
                    SocketError error;

                    Socket.EndSend(ar, out error);

					if (m_SendQueue[m_ReadIndex] != ar.AsyncState as OscPacket)
                    {
                        Debug.WriteLine("Objects do not match at index " + m_ReadIndex);
                    }

                    m_Count--;
                    m_ReadIndex = NextReadIndex;

                    if (m_Count > 0 && State == OscSocketState.Connected)
                    {
						OscPacket packet = m_SendQueue[m_ReadIndex];

						int size = packet.Write(m_Bytes);

						Socket.BeginSend(m_Bytes, 0, size, SocketFlags, Send_Callback, packet);
                    }
                    else
                    {
                        m_QueueEmpty.Set();
                    }
                }
                catch
                {
                    m_QueueEmpty.Set(); 
                }
            }
        }

        #endregion
    }
}

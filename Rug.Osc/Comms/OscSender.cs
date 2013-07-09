/* 
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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Rug.Osc
{
    public class OscSender : OscSocket
    {
        public const int DefaultMessageBufferSize = 600;

        #region Private Members

        private object m_Lock = new object();
        private AutoResetEvent m_QueueEmpty = new AutoResetEvent(true);

        private byte[] m_Bytes;

        private OscMessage[] m_SendQueue;
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
        /// Create a new Osc sender
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        public OscSender(IPAddress address, int port)
            : this(address, port, DefaultMessageBufferSize, DefaultPacketSize) 
        {
        }

        /// <summary>
        /// Create a new Osc sender
        /// </summary>
        /// <param name="address">the ip address to send to</param>
        /// <param name="port">the port to send to</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSender(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
            : base(address, port)
        {
            m_Bytes = new byte[maxPacketSize];
            m_SendQueue = new OscMessage[messageBufferSize];
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
        public void Send(OscMessage message)
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

                    if (m_SendQueue[m_ReadIndex] != ar.AsyncState as OscMessage)
                    {
                        Debug.WriteLine("Objects do not match at index " + m_ReadIndex);
                    }

                    m_Count--;
                    m_ReadIndex = NextReadIndex;

                    if (m_Count > 0 && State == OscSocketState.Connected)
                    {
                        OscMessage message = m_SendQueue[m_ReadIndex];

                        int size = message.Write(m_Bytes);

                        Socket.BeginSend(m_Bytes, 0, size, SocketFlags, Send_Callback, message);
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

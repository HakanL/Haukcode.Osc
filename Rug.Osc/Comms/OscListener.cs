using System;
using System.Net;
using System.Threading;
 
namespace Rug.Osc
{
	public class OscListener : IDisposable
	{
		private readonly OscAddressManager m_Listener = new OscAddressManager();
		private readonly OscReceiver m_Receiver;
		private Thread m_Thread;

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
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="port">the port to listen on</param>
		public OscListener(IPAddress address, int port)
        {
			m_Receiver = new OscReceiver(address, port);
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
		}

        /// <summary>
		/// Create a new Osc UDP listener. Note the underlying socket will not be connected untill Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
		public OscListener(int port)
        {
			m_Receiver = new OscReceiver(port);
		}

        #endregion

		public void Connect()
		{
			m_Receiver.Connect();
			
			m_Thread = new Thread(new ThreadStart(ListenLoop));

			m_Thread.Start();
		}

		public void Close()
		{
			m_Receiver.Close(); 			
		}

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

		private void ListenLoop()
		{
			try
			{
				while (m_Receiver.State != OscSocketState.Closed)
				{
					// if we are in a state to recieve
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

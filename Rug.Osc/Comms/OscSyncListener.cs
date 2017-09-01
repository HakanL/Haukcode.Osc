using System;
using System.Net;

namespace Rug.Osc
{
    /// <summary>
    /// Preforms common functions needed when listening for osc messages
    /// </summary>
    public class OscSyncListener : IDisposable
    {
        public readonly OscAddressManager OscAddressManager = new OscAddressManager();
        public readonly OscReceiver OscReceiver;

        /// <summary>
        /// This event will be raised whenever an unknown address is encountered
        /// </summary>
        public event EventHandler<UnknownAddressEventArgs> UnknownAddress;

        public event OscPacketEvent PacketReceived;

        public event OscPacketEvent PacketProcessed;

        #region Constructors

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local IP address to listen to</param>
        /// <param name="multicast">a multi-cast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSyncListener(IPAddress address, IPAddress multicast, int port, int messageBufferSize, int maxPacketSize)
        {
            OscReceiver = new OscReceiver(address, multicast, port, messageBufferSize, maxPacketSize);

            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local IP address to listen to</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSyncListener(IPAddress address, int port, int messageBufferSize, int maxPacketSize)
        {
            OscReceiver = new OscReceiver(address, port, messageBufferSize, maxPacketSize);

            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local IP address to listen to</param>
        /// <param name="port">the port to listen on</param>
        public OscSyncListener(IPAddress address, int port)
        {
            OscReceiver = new OscReceiver(address, port);

            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="address">the local ip address to listen to</param>
        /// <param name="multicast">a multicast address to join</param>
        /// <param name="port">the port to listen on, use 0 for dynamically assigned</param>
        public OscSyncListener(IPAddress address, IPAddress multicast, int port)
        {
            OscReceiver = new OscReceiver(address, multicast, port);

            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        /// <param name="messageBufferSize">the number of messages that should be cached before messages get dropped</param>
        /// <param name="maxPacketSize">the maximum packet size of any message</param>
        public OscSyncListener(int port, int messageBufferSize, int maxPacketSize)
        {
            OscReceiver = new OscReceiver(port, messageBufferSize, maxPacketSize);

            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

        /// <summary>
        /// Create a new Osc UDP listener. Note the underlying socket will not be connected until Connect is called
        /// </summary>
        /// <param name="port">the port to listen on</param>
        public OscSyncListener(int port)
        {
            OscReceiver = new OscReceiver(port);

            OscAddressManager.UnknownAddress += new EventHandler<UnknownAddressEventArgs>(OnUnknownAddress);
        }

        #endregion Constructors

        /// <summary>
        /// Connect the receiver and start listening
        /// </summary>
        public void Connect()
        {
            OscReceiver.Connect();
        }

        /// <summary>
        /// Close the receiver
        /// </summary>
        public void Close()
        {
            OscReceiver.Close();
        }

        /// <summary>
        /// Dispose of all resources
        /// </summary>
        public void Dispose()
        {
            OscReceiver.Dispose();
            OscAddressManager.Dispose();
        }

        /// <summary>
        /// Attach an event listener on to the given address
        /// </summary>
        /// <param name="address">the address of the contianer</param>
        /// <param name="event">the event to attach</param>
        public void Attach(string address, OscMessageEvent @event)
        {
            OscAddressManager.Attach(address, @event);
        }

        /// <summary>
        /// Detach an event listener
        /// </summary>
        /// <param name="address">the address of the container</param>
        /// <param name="event">the event to remove</param>
        public void Detach(string address, OscMessageEvent @event)
        {
            OscAddressManager.Detach(address, @event);
        }

        private void OnUnknownAddress(object sender, UnknownAddressEventArgs e)
        {
            UnknownAddress?.Invoke(this, e);
        }

        public bool TryReceive()
        {
            // if we are in a state to receive
            if (OscReceiver.State == OscSocketState.Connected)
            {
                OscPacket packet;

                // try and get the next message
                bool packetReceived = OscReceiver.TryReceive(out packet);

                if (packetReceived == false)
                {
                    return false;
                }

                PacketReceived?.Invoke(packet);

                if (packet.Error == OscPacketError.None)
                {
                    OscAddressManager.Invoke(packet);
                }

                PacketProcessed?.Invoke(packet);

                return true;
            }

            return false;
        }
    }
}
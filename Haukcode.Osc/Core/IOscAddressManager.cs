using System;
using System.Collections.Generic;

namespace Haukcode.Osc
{
    /// <summary>
    /// Manages osc address event listening
    /// </summary>
    public interface IOscAddressManager : IEnumerable<OscAddress>, IDisposable
    {
        OscCommunicationStatistics Statistics { get; set; }

        /// <summary>
        /// This event will be raised whenever an unknown address is encountered
        /// </summary>
        event EventHandler<UnknownAddressEventArgs> UnknownAddress;

        /// <summary>
        /// Attach an event listener on to the given address
        /// </summary>
        /// <param name="address">the address of the container</param>
        /// <param name="event">the event to attach</param>
        void Attach(string address, OscMessageEvent @event);

        /// <summary>
        /// Detach an event listener
        /// </summary>
        /// <param name="address">the address of the container</param>
        /// <param name="event">the event to remove</param>
        void Detach(string address, OscMessageEvent @event);

        /// <summary>
        /// Disposes of any resources and releases all events
        /// </summary>
        void Dispose();

        /// <summary>
        /// Invoke a osc packet
        /// </summary>
        /// <param name="packet">the packet</param>
        /// <returns>true if any thing was invoked</returns>
        bool Invoke(OscPacket packet);

        /// <summary>
        /// Invoke any event that matches the address on the message
        /// </summary>
        /// <param name="message">the message argument</param>
        /// <returns>true if there was a listener to invoke otherwise false</returns>
        bool Invoke(OscMessage message);

        /// <summary>
        /// Invoke all the messages within a bundle
        /// </summary>
        /// <param name="bundle">an osc bundle of messages</param>
        /// <returns>true if there was a listener to invoke for any message in the otherwise false</returns>
        bool Invoke(OscBundle bundle);

        bool Contains(OscAddress oscAddress);

        bool Contains(string oscAddress);

        bool ContainsLiteral(string oscAddress);

        bool ContainsPattern(OscAddress oscAddress);
    }
}
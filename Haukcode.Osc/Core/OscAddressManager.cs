/*
 * Haukcode.Osc
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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Haukcode.Osc
{
    public delegate void OscMessageEvent(OscMessage message);

    /// <summary>
    /// Manages osc address event listening
    /// </summary>
    public sealed class OscAddressManager : IOscAddressManager, IEnumerable<OscAddress>
    {
        /// <summary>
        /// Lookup of all literal addresses to listeners
        /// </summary>
        private readonly ConcurrentDictionary<string, OscLiteralEvent> literalAddresses = new ConcurrentDictionary<string, OscLiteralEvent>();

        /// <summary>
        /// Lookup of all pattern address to filters
        /// </summary>
        private readonly ConcurrentDictionary<OscAddress, OscPatternEvent> patternAddresses = new ConcurrentDictionary<OscAddress, OscPatternEvent>();

        /// <summary>
        /// This event will be raised whenever an unknown address is encountered
        /// </summary>
        public event EventHandler<UnknownAddressEventArgs> UnknownAddress;

        public OscCommunicationStatistics Statistics { get; set; }

        /// <summary>
        /// Attach an event listener on to the given address
        /// </summary>
        /// <param name="address">the address of the container</param>
        /// <param name="event">the event to attach</param>
        public void Attach(string address, OscMessageEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            // if the address is a literal then add it to the literal lookup
            if (OscAddress.IsValidAddressLiteral(address) == true)
            {
                OscLiteralEvent container = literalAddresses.GetOrAdd(address, func => new OscLiteralEvent(address));

                // attach the event
                container.Event += @event;
            }
            // if the address is a pattern add it to the pattern lookup
            else if (OscAddress.IsValidAddressPattern(address) == true)
            {
                OscAddress oscAddress = new OscAddress(address);

                // add it to the lookup
                OscPatternEvent container = patternAddresses.GetOrAdd(oscAddress, func => new OscPatternEvent(oscAddress));

                // attach the event
                container.Event += @event;
            }
            else
            {
                throw new ArgumentException($"Invalid container address '{address}'", nameof(address));
            }
        }

        public bool Contains(OscAddress oscAddress)
        {
            return patternAddresses.ContainsKey(oscAddress) || literalAddresses.ContainsKey(oscAddress.ToString());
        }

        public bool Contains(string oscAddress)
        {
            return Contains(new OscAddress(oscAddress));
        }

        public bool ContainsLiteral(string oscAddress)
        {
            return literalAddresses.ContainsKey(oscAddress);
        }

        public bool ContainsPattern(OscAddress oscAddress)
        {
            return patternAddresses.ContainsKey(oscAddress);
        }

        /// <summary>
        /// Detach an event listener
        /// </summary>
        /// <param name="address">the address of the container</param>
        /// <param name="event">the event to remove</param>
        public void Detach(string address, OscMessageEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            if (OscAddress.IsValidAddressLiteral(address) == true)
            {
                if (literalAddresses.TryGetValue(address, out OscLiteralEvent container) == false)
                {
                    // no container was found so abort
                    return;
                }

                // unregiser the event
                container.Event -= @event;

                // if the container is now empty remove it from the lookup
                if (container.IsNull == true)
                {
                    literalAddresses.TryRemove(container.Address, out container);
                }
            }
            else if (OscAddress.IsValidAddressPattern(address) == true)
            {
                OscAddress oscAddress = new OscAddress(address);

                if (patternAddresses.TryGetValue(oscAddress, out OscPatternEvent container) == false)
                {
                    // no container was found so abort
                    return;
                }

                // unregiser the event
                container.Event -= @event;

                // if the container is now empty remove it from the lookup
                if (container.IsNull == true)
                {
                    patternAddresses.TryRemove(container.Address, out container);
                }
            }
            else
            {
                throw new ArgumentException($"Invalid container address '{address}'", nameof(address));
            }
        }

        /// <summary>
        /// Disposes of any resources and releases all events
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, OscLiteralEvent> value in literalAddresses)
            {
                value.Value.Clear();
            }

            literalAddresses.Clear();

            foreach (KeyValuePair<OscAddress, OscPatternEvent> value in patternAddresses)
            {
                value.Value.Clear();
            }

            patternAddresses.Clear();
        }

        public IEnumerator<OscAddress> GetEnumerator()
        {
            return GetAllAddresses().GetEnumerator();
        }

        /// <summary>
        /// Invoke a osc packet
        /// </summary>
        /// <param name="packet">the packet</param>
        /// <returns>true if any thing was invoked</returns>
        public bool Invoke(OscPacket packet)
        {
            Statistics?.PacketsReceived.Increment(1);

            switch (packet)
            {
                case OscMessage _:
                    return Invoke((OscMessage)packet);
                case OscBundle _:
                    return Invoke((OscBundle)packet);
            }

            throw new Exception($"Unknown osc packet type '{packet}'");
        }

        /// <summary>
        /// Invoke all the messages within a bundle
        /// </summary>
        /// <param name="bundle">an osc bundle of messages</param>
        /// <returns>true if there was a listener to invoke for any message in the otherwise false</returns>
        public bool Invoke(OscBundle bundle)
        {
            bool result = false;

            Statistics?.BundlesReceived.Increment(1);

            foreach (OscPacket packet in bundle)
            {
                if (packet.Error != OscPacketError.None)
                {
                    continue;
                }

                switch (packet)
                {
                    case OscMessage _:
                        result |= Invoke(packet as OscMessage);
                        break;
                    case OscBundle _:
                        result |= Invoke(packet as OscBundle);
                        break;
                    default:
                        throw new Exception($"Unknown osc packet type '{packet}'");
                }
            }

            return result;
        }

        /// <summary>
        /// Invoke any event that matches the address on the message
        /// </summary>
        /// <param name="message">the message argument</param>
        /// <returns>true if there was a listener to invoke otherwise false</returns>
        public bool Invoke(OscMessage message)
        {
            bool invoked = false;
            OscAddress oscAddress = null;

            Statistics?.MessagesReceived.Increment(1);

            do
            {
                if (OscAddress.IsValidAddressLiteral(message.Address) == true)
                {
                    if (literalAddresses.TryGetValue(message.Address, out OscLiteralEvent container) == true)
                    {
                        container.Invoke(message);

                        invoked = true;
                    }
                }
                else
                {
                    oscAddress = new OscAddress(message.Address);

                    foreach (KeyValuePair<string, OscLiteralEvent> value in literalAddresses)
                    {
                        if (oscAddress.Match(value.Key) != true)
                        {
                            continue;
                        }

                        value.Value.Invoke(message);
                        invoked = true;
                    }
                }

                if (patternAddresses.Count > 0)
                {
                    if (oscAddress == null)
                    {
                        oscAddress = new OscAddress(message.Address);
                    }

                    foreach (KeyValuePair<OscAddress, OscPatternEvent> value in patternAddresses)
                    {
                        if (oscAddress.Match(value.Key) == false)
                        {
                            continue;
                        }

                        value.Value.Invoke(message);
                        invoked = true;
                    }
                }
            }
            while (invoked == false && OnUnknownAddress(message.Address, message) == true);

            return invoked;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetAllAddresses() as IEnumerable).GetEnumerator();
        }

        private List<OscAddress> GetAllAddresses()
        {
            List<OscAddress> addresses = new List<OscAddress>();

            addresses.AddRange(patternAddresses.Keys);

            addresses.AddRange(literalAddresses.Keys.Select(address => new OscAddress(address)));

            return addresses;
        }

        private bool OnUnknownAddress(string address, OscPacket packet)
        {
            if (UnknownAddress != null)
            {
                UnknownAddressEventArgs arg = new UnknownAddressEventArgs(this, address, packet);

                UnknownAddress(this, arg);

                return arg.Retry;
            }
            else
            {
                return false;
            }
        }
    }
}
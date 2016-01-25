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
using System.Collections.Generic;

namespace Rug.Osc
{
	public delegate void OscMessageEvent(OscMessage message);

	/// <summary>
	/// Manages osc address event listening
	/// </summary>
	public sealed class OscAddressManager : IDisposable
	{
		readonly object syncLock = new object(); 

		#region Private Members

		/// <summary>
		/// Lookup of all literal addresses to listeners 
		/// </summary>
		readonly Dictionary<string, OscLiteralEvent> literalAddresses = new Dictionary<string, OscLiteralEvent>();

		/// <summary>
		/// Lookup of all pattern address to filters
		/// </summary>
		readonly Dictionary<OscAddress, OscPatternEvent> patternAddresses = new Dictionary<OscAddress, OscPatternEvent>();

		IOscTimeProvider timeProvider;
		
		OscBundleInvokeMode bundleInvokeMode; 

		#endregion

		public OscCommunicationStatistics Statistics { get; set; }

		/// <summary>
		/// Osc time provider, used for filtering bundles by time, if null then the DefaultTimeProvider is used 
		/// </summary>
		public IOscTimeProvider TimeProvider { get { return timeProvider; } set { timeProvider = value; } }

		/// <summary>
		/// Bundle invoke mode, the default is OscBundleInvokeMode.InvokeAllBundlesImmediately
		/// </summary>
		public OscBundleInvokeMode BundleInvokeMode { get { return bundleInvokeMode; } set { bundleInvokeMode = value; } }

		/// <summary>
		/// This event will be raised whenever an unknown address is encountered
		/// </summary>
		public event EventHandler<UnknownAddressEventArgs> UnknownAddress;

		/// <summary>
		/// Create a osc address manager
		/// </summary>
		public OscAddressManager()
		{
			BundleInvokeMode = OscBundleInvokeMode.InvokeAllBundlesImmediately;
		}

		#region Attach

		/// <summary>
		/// Attach an event listener on to the given address
		/// </summary>
		/// <param name="address">the address of the contianer</param>
		/// <param name="event">the event to attach</param>
		public void Attach(string address, OscMessageEvent @event)
		{
			if (@event == null)
			{
				throw new ArgumentNullException("event"); 
			}

			// if the address is a literal then add it to the literal lookup
			if (OscAddress.IsValidAddressLiteral(address) == true)
			{			
				OscLiteralEvent container;

				lock (syncLock)
				{
					if (literalAddresses.TryGetValue(address, out container) == false)
					{
						// no container was found so create one 
						container = new OscLiteralEvent(address);

						// add it to the lookup 
						literalAddresses.Add(address, container);
					}
				}

				// attach the event
				container.Event += @event;
			}
			// if the address is a pattern add it to the pattern lookup 
			else if (OscAddress.IsValidAddressPattern(address) == true)
			{			
				OscPatternEvent container;
				OscAddress oscAddress = new OscAddress(address);

				lock (syncLock)
				{
					if (patternAddresses.TryGetValue(oscAddress, out container) == false)
					{
						// no container was found so create one 
						container = new OscPatternEvent(oscAddress);

						// add it to the lookup 
						patternAddresses.Add(oscAddress, container);
					}
				}

				// attach the event
				container.Event += @event;
			}
			else
			{
				throw new ArgumentException(String.Format(Strings.Container_IsValidContainerAddress, address), "address");
			}
		}

		#endregion

		#region Detach

		/// <summary>
		/// Detach an event listener 
		/// </summary>
		/// <param name="address">the address of the container</param>
		/// <param name="event">the event to remove</param>
		public void Detach(string address, OscMessageEvent @event)
		{
			if (@event == null)
			{
				throw new ArgumentNullException("event");
			}

			if (OscAddress.IsValidAddressLiteral(address) == true)
			{
				OscLiteralEvent container;

				lock (syncLock)
				{
					if (literalAddresses.TryGetValue(address, out container) == false)
					{
						// no container was found so abort
						return;
					}
				}
				// unregiser the event 
				container.Event -= @event;

				// if the container is now empty remove it from the lookup
				if (container.IsNull == true)
				{
					literalAddresses.Remove(container.Address);
				}
			}
			else if (OscAddress.IsValidAddressPattern(address) == true)
			{
				OscPatternEvent container;
				OscAddress oscAddress = new OscAddress(address);

				lock (syncLock)
				{
					if (patternAddresses.TryGetValue(oscAddress, out container) == false)
					{
						// no container was found so abort
						return;
					}
				}

				// unregiser the event 
				container.Event -= @event;

				// if the container is now empty remove it from the lookup
				if (container.IsNull == true)
				{
					patternAddresses.Remove(container.Address);
				}
			}
			else
			{
				throw new ArgumentException(String.Format(Strings.Container_IsValidContainerAddress, address), "address");
			}
		}

		#endregion

		#region Should Invoke

		/// <summary>
		/// Determin if the packet should be invoked
		/// </summary>
		/// <param name="packet">A packet</param>
		/// <returns>The appropriate action that should be taken with the packet</returns>
		public OscPacketInvokeAction ShouldInvoke(OscPacket packet)
		{
			if (packet.Error != OscPacketError.None)
			{
				return OscPacketInvokeAction.HasError; 
			}

			if (packet is OscMessage)
			{
				return OscPacketInvokeAction.Invoke; 
			}

			if (packet is OscBundle)
			{
				OscBundle bundle = packet as OscBundle;

				if (BundleInvokeMode == OscBundleInvokeMode.NeverInvoke)
				{
					return OscPacketInvokeAction.DontInvoke;
				}
				else if (BundleInvokeMode != OscBundleInvokeMode.InvokeAllBundlesImmediately)
				{
					double delay;

					IOscTimeProvider provider = TimeProvider;

					if (TimeProvider == null)
					{
						provider = DefaultTimeProvider.Instance;
					}

					delay = provider.DifferenceInSeconds(bundle.Timestamp);

					if ((BundleInvokeMode & OscBundleInvokeMode.InvokeEarlyBundlesImmediately) !=
						OscBundleInvokeMode.InvokeEarlyBundlesImmediately)
					{
						if (delay > 0 && provider.IsWithinTimeFrame(bundle.Timestamp) == false)
						{
							if ((BundleInvokeMode & OscBundleInvokeMode.PosponeEarlyBundles) !=
								OscBundleInvokeMode.PosponeEarlyBundles)
							{
								return OscPacketInvokeAction.Pospone;
							}
							else
							{
								return OscPacketInvokeAction.DontInvoke;
							}
						}
					}

					if ((BundleInvokeMode & OscBundleInvokeMode.InvokeLateBundlesImmediately) !=
						OscBundleInvokeMode.InvokeLateBundlesImmediately)
					{
						if (delay < 0 && provider.IsWithinTimeFrame(bundle.Timestamp) == false)
						{
							return OscPacketInvokeAction.DontInvoke;
						}
					}

					if ((BundleInvokeMode & OscBundleInvokeMode.InvokeOnTimeBundles) !=
						OscBundleInvokeMode.InvokeOnTimeBundles)
					{
						if (provider.IsWithinTimeFrame(bundle.Timestamp) == true)
						{
							return OscPacketInvokeAction.DontInvoke;
						}
					}
				}

				return OscPacketInvokeAction.Invoke;
			}
			else
			{
				return OscPacketInvokeAction.DontInvoke;
			}
		}

		#endregion 

		#region Invoke

		/// <summary>
		/// Invoke a osc packet 
		/// </summary>
		/// <param name="packet">the packet</param>
		/// <returns>true if any thing was invoked</returns>
		public bool Invoke(OscPacket packet)
		{
			if (Statistics != null)
			{
				Statistics.PacketsReceived.Increment(1);
			}

			if (packet is OscMessage)
			{
				return Invoke(packet as OscMessage); 
			}
			else if (packet is OscBundle)
			{
				return Invoke(packet as OscBundle); 
			}
			else
			{
				throw new Exception(String.Format(Strings.Listener_UnknownOscPacketType, packet.ToString())); 
			}
		}

		/// <summary>
		/// Invoke all the messages within a bundle
		/// </summary>
		/// <param name="bundle">an osc bundle of messages</param>
		/// <returns>true if there was a listener to invoke for any message in the otherwise false</returns>
		public bool Invoke(OscBundle bundle)
		{			
			bool result = false;

			if (Statistics != null)
			{
				Statistics.BundlesReceived.Increment(1);
			}

			foreach (OscPacket packet in bundle)
			{
				if (packet.Error != OscPacketError.None)
				{
					continue;
				}

				if (packet is OscMessage)
				{
					result |= Invoke(packet as OscMessage);
				}
				else if (packet is OscBundle)
				{
					result |= Invoke(packet as OscBundle);
				}
				else
				{
					throw new Exception(String.Format(Strings.Listener_UnknownOscPacketType, packet.ToString()));
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

			List<OscLiteralEvent> shouldInvoke = new List<OscLiteralEvent>();
			List<OscPatternEvent> shouldInvoke_Filter = new List<OscPatternEvent>();

			if (Statistics != null)
			{
				Statistics.MessagesReceived.Increment(1);
			}

			do
			{
				lock (syncLock)
				{

					if (OscAddress.IsValidAddressLiteral(message.Address) == true)
					{
						OscLiteralEvent container;

						if (literalAddresses.TryGetValue(message.Address, out container) == true)
						{
							//container.Invoke(message);
							shouldInvoke.Add(container);
							invoked = true;
						}
					}
					else
					{
						oscAddress = new OscAddress(message.Address);

						foreach (KeyValuePair<string, OscLiteralEvent> value in literalAddresses)
						{
							if (oscAddress.Match(value.Key) == true)
							{
								//value.Value.Invoke(message);
								shouldInvoke.Add(value.Value);
								invoked = true;
							}
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
							if (oscAddress.Match(value.Key) == true)
							{
								//value.Value.Invoke(message);
								shouldInvoke_Filter.Add(value.Value);
								invoked = true;
							}
						}
					}
				}
			}
			while (invoked == false && OnUnknownAddress(message.Address, message) == true);

			foreach (OscLiteralEvent @event in shouldInvoke)
			{
				@event.Invoke(message); 
			}

			foreach (OscPatternEvent @event in shouldInvoke_Filter)
			{
				@event.Invoke(message);
			}

			return invoked; 
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

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes of any resources and releases all events 
		/// </summary>
		public void Dispose()
		{
			lock (syncLock)
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
		}

		#endregion

	}
}

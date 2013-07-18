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
using System.Collections.Generic;

namespace Rug.Osc
{
	public delegate void OscMessageEvent(OscMessage message);

	#region Osc Container

	internal class OscListener
	{
		/// <summary>
		/// The literal address of the event
		/// </summary>
		public readonly string Address;

		public event OscMessageEvent Event; 

		public bool IsNull { get { return Event == null; } }

		internal OscListener(string address)
		{
			Address = address; 
		}

		/// <summary>
		/// Invoke the event
		/// </summary>
		/// <param name="message">message that caused the event</param>
		public void Invoke(OscMessage message) 
		{
			if (Event != null)
			{
				Event(message);
			}
		}

		/// <summary>
		/// Nullify the event
		/// </summary>
		public void Clear() 
		{
			Event = null; 
		} 
	}

	#endregion

	#region Osc Container

	internal class OscFilter
	{
		/// <summary>
		/// The literal address of the event
		/// </summary>
		public readonly OscAddress Address;

		public event OscMessageEvent Event;

		public bool IsNull { get { return Event == null; } }

		internal OscFilter(OscAddress address)
		{
			Address = address;
		}

		/// <summary>
		/// Invoke the event
		/// </summary>
		/// <param name="message">message that caused the event</param>
		public void Invoke(OscMessage message)
		{
			if (Event != null)
			{
				Event(message);
			}
		}

		/// <summary>
		/// Nullify the event
		/// </summary>
		public void Clear()
		{
			Event = null;
		}
	}

	#endregion

	[Flags]
	public enum OscBundleInvokeMode : int 
	{
		NeverInvoke = 0,

		InvokeOnTimeBundles = 1, 

		InvokeLateBundlesImmediately = 2,

		InvokeEarlyBundlesImmediately = 4,

		InvokeAllBundlesImmediately = InvokeOnTimeBundles | InvokeLateBundlesImmediately | InvokeEarlyBundlesImmediately,
	}

	#region Osc Listener Manager

	/// <summary>
	/// Manages osc address event listening
	/// </summary>
	public class OscListenerManager : IDisposable
	{
		private object m_Lock = new object(); 

		#region Private Members

		/// <summary>
		/// Lookup of all literal addresses to listeners 
		/// </summary>
		private Dictionary<string, OscListener> m_LiteralAddresses = new Dictionary<string, OscListener>();

		/// <summary>
		/// Lookup of all pattern address to filters
		/// </summary>
		private Dictionary<OscAddress, OscFilter> m_PatternAddresses = new Dictionary<OscAddress, OscFilter>();

		#endregion

		/// <summary>
		/// Osc time provider, used for filtering bundles by time, if null then the DefaultTimeProvider is used 
		/// </summary>
		public IOscTimeProvider TimeProvider { get; set; }

		/// <summary>
		/// Bundle invoke mode, the default is OscBundleInvokeMode.InvokeAllBundlesImmediately
		/// </summary>
		public OscBundleInvokeMode BundleInvokeMode { get; set; }

		public OscListenerManager()
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
				OscListener container;

				lock (m_Lock)
				{
					if (m_LiteralAddresses.TryGetValue(address, out container) == false)
					{
						// no container was found so create one 
						container = new OscListener(address);

						// add it to the lookup 
						m_LiteralAddresses.Add(address, container);
					}
				}

				// attach the event
				container.Event += @event;
			}
			// if the address is a pattern add it to the pattern lookup 
			else if (OscAddress.IsValidAddressPattern(address) == true)
			{			
				OscFilter container;
				OscAddress oscAddress = new OscAddress(address);

				lock (m_Lock)
				{
					if (m_PatternAddresses.TryGetValue(oscAddress, out container) == false)
					{
						// no container was found so create one 
						container = new OscFilter(oscAddress);

						// add it to the lookup 
						m_PatternAddresses.Add(oscAddress, container);
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
				OscListener container;

				lock (m_Lock)
				{
					if (m_LiteralAddresses.TryGetValue(address, out container) == false)
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
					m_LiteralAddresses.Remove(container.Address);
				}
			}
			else if (OscAddress.IsValidAddressPattern(address) == true)
			{
				OscFilter container;
				OscAddress oscAddress = new OscAddress(address);

				lock (m_Lock)
				{
					if (m_PatternAddresses.TryGetValue(oscAddress, out container) == false)
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
					m_PatternAddresses.Remove(container.Address);
				}
			}
			else
			{
				throw new ArgumentException(String.Format(Strings.Container_IsValidContainerAddress, address), "address");
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
		/// <remarks>
		/// TODO WRITE NOTES ON TimeProvider and BundleInvokeMode
		/// </remarks>
		/// <param name="bundle">an osc bundle of messages</param>
		/// <returns>true if the bundle passed through the timing filter and there was a listener to invoke otherwise false</returns>
		public bool Invoke(OscBundle bundle)
		{
			if (BundleInvokeMode == OscBundleInvokeMode.NeverInvoke)
			{
				return false; 
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
						return false; 
					}
				}

				if ((BundleInvokeMode & OscBundleInvokeMode.InvokeLateBundlesImmediately) !=
					OscBundleInvokeMode.InvokeLateBundlesImmediately)
				{
					if (delay < 0 && provider.IsWithinTimeFrame(bundle.Timestamp) == false)
					{
						return false;
					}
				}

				if ((BundleInvokeMode & OscBundleInvokeMode.InvokeOnTimeBundles) != 
					OscBundleInvokeMode.InvokeOnTimeBundles)
				{
					if (provider.IsWithinTimeFrame(bundle.Timestamp) == true)
					{
						return false;
					}
				}
			}

			bool result = false;

			foreach (OscMessage message in bundle)
			{
				if (message.Error != OscPacketError.None)
				{
					continue;
				}

				result |= Invoke(message); 
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

			if (OscAddress.IsValidAddressLiteral(message.Address) == true)
			{
				OscListener container;

				lock (m_Lock)
				{
					if (m_LiteralAddresses.TryGetValue(message.Address, out container) == true)
					{
						container.Invoke(message);
						invoked = true;
					}
				}
			}
			else
			{
				oscAddress = new OscAddress(message.Address);

				lock (m_Lock)
				{
					foreach (KeyValuePair<string, OscListener> value in m_LiteralAddresses)
					{
						if (oscAddress.Match(value.Key) == true)
						{
							value.Value.Invoke(message);
							invoked = true;
						}
					}
				}
			}

			if (m_PatternAddresses.Count > 0)
			{
				if (oscAddress == null)
				{
					oscAddress = new OscAddress(message.Address);
				}

				lock (m_Lock)
				{
					foreach (KeyValuePair<OscAddress, OscFilter> value in m_PatternAddresses)
					{
						if (oscAddress.Match(value.Key) == true)
						{
							value.Value.Invoke(message);
							invoked = true;
						}
					}
				}
			}

			return invoked; 
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes of any resources and releases all events 
		/// </summary>
		public void Dispose()
		{
			lock (m_Lock)
			{
				foreach (KeyValuePair<string, OscListener> value in m_LiteralAddresses)
				{
					value.Value.Clear();
				}

				m_LiteralAddresses.Clear();
			}
		}

		#endregion
	}

	#endregion
}
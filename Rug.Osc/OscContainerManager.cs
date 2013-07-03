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

	#region Osc Container Manager

	/// <summary>
	/// Manages osc address event listening
	/// </summary>
	public class OscContainerManager : IDisposable
	{
		#region Private Members

		/// <summary>
		/// Lookup of all addresses to listeners 
		/// </summary>
		private Dictionary<string, OscListener> m_Lookup = new Dictionary<string, OscListener>();

		#endregion

		#region Register

		/// <summary>
		/// Attach an event to the container at the given address
		/// </summary>
		/// <param name="address">the address of the contianer</param>
		/// <param name="event">the event to attach</param>
		public void Register(string address, OscMessageEvent @event)
		{
			if (@event == null)
			{
				throw new ArgumentNullException("event"); 
			}

			if (OscAddress.IsValidAddressLiteral(address) != true)
			{
				throw new ArgumentException(String.Format(Strings.Container_IsValidContainerAddress, address), "address");
			}

			OscListener container;

			if (m_Lookup.TryGetValue(address, out container) == false)
			{
				// no container was found so create one 
				container = new OscListener(address);

				// add it to the lookup 
				m_Lookup.Add(address, container); 
			}

			// attach the event
			container.Event += @event; 
		}

		#endregion

		#region Unregister

		/// <summary>
		/// Unregiser an event from a container address
		/// </summary>
		/// <param name="address">the address of the container</param>
		/// <param name="event">the event to remove</param>
		public void Unregister(string address, OscMessageEvent @event)
		{
			if (@event == null)
			{
				throw new ArgumentNullException("event");
			}

			if (OscAddress.IsValidAddressLiteral(address) != true)
			{
				throw new ArgumentException(String.Format(Strings.Container_IsValidContainerAddress, address), "address");
			}

			OscListener container;

			if (m_Lookup.TryGetValue(address, out container) == false)
			{
				// no container was found so abort
				return; 
			}

			// unregiser the event 
			container.Event -= @event;

			// if the container is now empty remove it from the lookup
			if (container.IsNull == true)
			{
				m_Lookup.Remove(container.Address); 
			}
		}

		#endregion

		#region Process

		/// <summary>
		/// Invoke any event that matches the address on the message
		/// </summary>
		/// <param name="message"></param>
		public void Process(OscMessage message)
		{			
			if (OscAddress.IsValidAddressLiteral(message.Address) == true)
			{
				OscListener container;

				if (m_Lookup.TryGetValue(message.Address, out container) == true)
				{
					container.Invoke(message);
				}
			}
			else
			{
				OscAddress address = new OscAddress(message.Address);

				foreach (KeyValuePair<string, OscListener> value in m_Lookup)
				{
					if (address.Match(value.Key) == true)
					{
						value.Value.Invoke(message);
					}
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			foreach (KeyValuePair<string, OscListener> value in m_Lookup)
			{
				value.Value.Clear(); 
			}

			m_Lookup.Clear(); 
		}

		#endregion
	}

	#endregion
}

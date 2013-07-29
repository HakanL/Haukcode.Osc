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


namespace Rug.Osc
{
	internal struct OscFilter
	{
		/// <summary>
		/// The pattern address of the event
		/// </summary>
		public readonly OscAddress Address;

		public event OscMessageEvent Event;

		public bool IsNull { get { return Event == null; } }

		internal OscFilter(OscAddress address)
		{
			Address = address;
			Event = null;
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
}

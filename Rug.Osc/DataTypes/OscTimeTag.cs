﻿/* 
 * Rug.Osc 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2013 Phill Tew. All rights reserved.
 * 
 * Based on code by Kas http://stackoverflow.com/questions/5206857/convert-ntp-timestamp-to-utc
 */

using System;
using System.Globalization;

namespace Rug.Osc
{
	/// <summary>
	/// Osc time tag
	/// </summary>
	public struct OscTimeTag
	{
		public static readonly DateTime BaseDate = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// Ntp Timestamp value
		/// </summary>
		public ulong Value;

		/// <summary>
		/// Build a osc timetag from a Ntp 64 bit integer 
		/// </summary>
		/// <param name="value">the 64 bit integer containing the time stamp</param>
		public OscTimeTag(ulong value)
		{
			Value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is OscTimeTag)
			{
				return Value.Equals(((OscTimeTag)obj).Value);
			}
			else
			{
				return Value.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override string ToString()
		{
			return ToDataTime().ToString("dd-MM-yyyy HH:mm:ss.ffff");
		}

		/// <summary>
		/// Get the equivient datetime value from the osc timetag 
		/// </summary>
		/// <returns>the equivilent value as a datetime</returns>
		public DateTime ToDataTime()
		{
			// Kas: http://stackoverflow.com/questions/5206857/convert-ntp-timestamp-to-utc

			uint seconds = (uint)((Value & 0xFFFFFFFF00000000) >> 32);

			uint fraction = (uint)(Value & 0xFFFFFFFF);

			double milliseconds = ((double)fraction / (double)UInt32.MaxValue) * 1000;
			
			DateTime datetime = BaseDate.AddSeconds(seconds).AddMilliseconds(milliseconds);

			return datetime;
		}

		/// <summary>
		/// Get a Osc timstamp from a datetime value
		/// </summary>
		/// <param name="datetime">datetime value</param>
		/// <returns>the equivilent value as an osc timetag</returns>
		public static OscTimeTag FromDataTime(DateTime datetime)
		{
			TimeSpan span = datetime.Subtract(BaseDate);

			double seconds = span.TotalSeconds;

			uint seconds_UInt = (uint)seconds;

			double milliseconds = span.TotalMilliseconds - ((double)seconds_UInt * 1000);

			double fraction = (milliseconds / 1000) * (double)UInt32.MaxValue;

			return new OscTimeTag(((ulong)(seconds_UInt & 0xFFFFFFFF) << 32) | ((ulong)fraction & 0xFFFFFFFF));
		}

		/// <summary>
		/// Parse a osc time tag from datetime string
		/// </summary>
		/// <param name="str">string to parse</param>
		/// <param name="provider">format provider</param>
		/// <returns>the parsed time tag</returns>
		public static OscTimeTag Parse(string str, IFormatProvider provider)
		{
			string[] formats = new string[] 
			{	
				"dd-MM-yy", 
				"dd-MM-yyyy",
 				"HH:mm",
				"HH:mm:ss",
				"HH:mm:ss.ffff",
				"dd-MM-yyyy HH:mm:ss",
				"dd-MM-yyyy HH:mm",
				"dd-MM-yyyy HH:mm:ss.ffff" 
			}; 

			DateTime datetime = DateTime.ParseExact(str, formats, provider, DateTimeStyles.None);

			return FromDataTime(datetime); 
		}

		/// <summary>
		/// Parse a osc time tag from datetime string
		/// </summary>
		/// <param name="str">string to parse</param>
		/// <returns>the parsed time tag</returns>
		public static OscTimeTag Parse(string str)
		{
			return Parse(str, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Try to parse a osc time tag from datetime string
		/// </summary>
		/// <param name="str">string to parse</param>
		/// <param name="provider">format provider</param>
		/// <param name="value">the parsed time tag</param>
		/// <returns>true if parsed else false</returns>
		public static bool TryParse(string str, IFormatProvider provider, out OscTimeTag value)
		{
			try
			{
				value = Parse(str, provider);

				return true;
			}
			catch
			{
				value = default(OscTimeTag);

				return false;
			}
		}

		/// <summary>
		/// Try to parse a osc time tag from datetime string
		/// </summary>
		/// <param name="str">string to parse</param>
		/// <param name="value">the parsed time tag</param>
		/// <returns>true if parsed else false</returns>
		public static bool TryParse(string str, out OscTimeTag value)
		{
			try
			{
				value = Parse(str, CultureInfo.InvariantCulture);

				return true;
			}
			catch
			{
				value = default(OscTimeTag);

				return false;
			}
		}		
	}
}

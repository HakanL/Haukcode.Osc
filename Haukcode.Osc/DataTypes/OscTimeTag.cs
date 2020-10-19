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
 * Based on code by Kas http://stackoverflow.com/questions/5206857/convert-ntp-timestamp-to-utc
 */

using System;
using System.Globalization;

namespace Haukcode.Osc
{
    /// <summary>
    /// OSC time tag.
    /// </summary>
    public struct OscTimeTag
    {
        /// <summary>
        /// The minimum date for any OSC time tag.
        /// </summary>
        public static readonly DateTime BaseDate = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Gets a OscTimeTag object that is set to the current date and time on this computer, expressed as the local time.
        /// </summary>
        public static OscTimeTag Now { get { return FromDataTime(DateTime.Now); } }

        /// <summary>
        /// Gets a OscTimeTag object that is set to the current date and time on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public static OscTimeTag UtcNow { get { return FromDataTime(DateTime.UtcNow); } }

        /// <summary>
        /// Time tag value represented by a 64 bit fixed point number. The first 32 bits specify the number of seconds since midnight on January 1, 1900, and the last 32 bits specify fractional parts of a second to a precision of about 200 picoseconds. This is the representation used by Internet NTP timestamps.
        /// </summary>
        public ulong Value;

        /// <summary>
        /// Gets the number of seconds since midnight on January 1, 1900. This is the first 32 bits of the 64 bit fixed point OSC time tag value.
        /// </summary>
		public uint Seconds { get { return (uint)((Value & 0xFFFFFFFF00000000) >> 32); } }

        /// <summary>
        /// Gets the fractional parts of a second. This is the 32 bits of the 64 bit fixed point OSC time tag value.
        /// </summary>
		public uint Fraction { get { return (uint)(Value & 0xFFFFFFFF); } }

        /// <summary>
        /// Gets the number of seconds including fractional parts since midnight on January 1, 1900.
        /// </summary>
        public decimal SecondsDecimal
        {
            get
            {
                return ((decimal)Seconds + (decimal)((double)Fraction / (double)uint.MaxValue));
            }
        }

        /// <summary>
        /// Build a OSC time tag from a NTP 64 bit integer.
        /// </summary>
        /// <param name="value">The 64 bit integer containing the time stamp.</param>
        public OscTimeTag(ulong value)
        {
            Value = value;
        }

        /// <summary>
        /// Does this OSC time tag equal another object.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns>True if the objects are the same.</returns>
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

        /// <summary>
        /// Gets a hashcode for this OSC time tag.
        /// </summary>
        /// <returns>A hashcode.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Get a string of this OSC time tag in the format "dd-MM-yyyy HH:mm:ss.ffffZ".
        /// </summary>
        /// <returns>The string value of this OSC time tag.</returns>
        public override string ToString()
        {
            return ToDataTime().ToString("dd-MM-yyyy HH:mm:ss.ffffZ");
        }

        #region To Date Time

        /// <summary>
        /// Get the equivalent date-time value from the OSC time tag.
        /// </summary>
        /// <returns>the equivalent value as a date-time</returns>
        public DateTime ToDataTime()
        {
            // Kas: http://stackoverflow.com/questions/5206857/convert-ntp-timestamp-to-utc

            uint seconds = Seconds;

            uint fraction = Fraction;

            double milliseconds = ((double)fraction / (double)UInt32.MaxValue) * 1000;

            DateTime datetime = BaseDate.AddSeconds(seconds).AddMilliseconds(milliseconds);

            return datetime;
        }

        #endregion To Date Time

        #region From Data Time

        /// <summary>
        /// Get a Osc times tamp from a date-time value.
        /// </summary>
        /// <param name="datetime">Date-time value.</param>
        /// <returns>The equivalent value as an osc time tag.</returns>
        public static OscTimeTag FromDataTime(DateTime datetime)
        {
            TimeSpan span = datetime.Subtract(BaseDate);

            double seconds = span.TotalSeconds;

            uint seconds_UInt = (uint)seconds;

            double milliseconds = span.TotalMilliseconds - ((double)seconds_UInt * 1000);

            double fraction = (milliseconds / 1000) * (double)UInt32.MaxValue;

            return new OscTimeTag(((ulong)(seconds_UInt & 0xFFFFFFFF) << 32) | ((ulong)fraction & 0xFFFFFFFF));
        }

        #endregion From Data Time

        #region Parse

        /// <summary>
        /// Parse a OSC time tag from date-time string.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <param name="provider">Format provider</param>
        /// <returns>The parsed time tag.</returns>
        public static OscTimeTag Parse(string str, IFormatProvider provider)
        {
            DateTimeStyles style = DateTimeStyles.AdjustToUniversal;

            if (str.Trim().EndsWith("Z") == true)
            {
                style = DateTimeStyles.AssumeUniversal;

                str = str.Trim().TrimEnd('Z');
            }

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

            DateTime datetime;
            ulong value_UInt64;

            if (DateTime.TryParseExact(str, formats, provider, style, out datetime) == true)
            {
                return FromDataTime(datetime);
            }
            else if (str.StartsWith("0x") == true &&
                     ulong.TryParse(str.Substring(2), NumberStyles.HexNumber, provider, out value_UInt64) == true)
            {
                return new OscTimeTag(value_UInt64);
            }
            else if (ulong.TryParse(str, NumberStyles.Integer, provider, out value_UInt64) == true)
            {
                return new OscTimeTag(value_UInt64);
            }
            else
            {
                throw new Exception($"Invalid osc-timetag string \'{str}\'");
            }
        }

        /// <summary>
        /// Parse a OSC time tag from date-time string.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>The parsed time tag.</returns>
        public static OscTimeTag Parse(string str)
        {
            return Parse(str, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Try to parse a OSC time tag from date-time string.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <param name="provider">Format provider.</param>
        /// <param name="value">The parsed time tag.</param>
        /// <returns>True if parsed else false.</returns>
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
        /// Try to parse a OSC time tag from date-time string.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <param name="value">The parsed time tag.</param>
        /// <returns>True if parsed else false.</returns>
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

        #endregion Parse
    }
}
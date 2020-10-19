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

namespace Rug.Osc
{
    /// <summary>
    /// Default implementation of IOscTimeProvider
    /// </summary>
    [Obsolete]
    public sealed class DefaultTimeProvider : IOscTimeProvider
    {
        /// <summary>
        /// Immutable instance
        /// </summary>
        public static readonly DefaultTimeProvider Instance = new DefaultTimeProvider();

        /// <summary>
        /// Frame size in seconds
        /// </summary>
        public double FrameSizeInSeconds { get; set; }

        /// <summary>
        /// Get the current time
        /// </summary>
        public OscTimeTag Now => OscTimeTag.FromDataTime(DateTime.UtcNow);

        /// <summary>
        /// Create a new instance of DefaultTimeProvider
        /// </summary>
        public DefaultTimeProvider() { FrameSizeInSeconds = 1; }

        /// <summary>
        /// Is the supplied time within the current frame according to this time provider
        /// </summary>
        /// <param name="time">the time to check</param>
        /// <returns>true if within the frame else false</returns>
        public bool IsWithinTimeFrame(OscTimeTag time)
        {
            double offset = DifferenceInSeconds(time);

            return offset > -FrameSizeInSeconds &&
                   offset < FrameSizeInSeconds;
        }

        /// <summary>
        /// Get the difference in seconds between the current time and the suppied time
        /// </summary>
        /// <param name="time">the time to compair</param>
        /// <returns>the difference in seconds between the current time and the suppied time</returns>
        public double DifferenceInSeconds(OscTimeTag time)
        {
            TimeSpan span = DateTime.UtcNow.Subtract(time.ToDataTime());

            return span.TotalSeconds;
        }
    }
}
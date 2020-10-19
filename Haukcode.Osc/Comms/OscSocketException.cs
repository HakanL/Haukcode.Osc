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

namespace Haukcode.Osc
{
    /// <summary>
    /// Exception that relates explicitly to osc socket instance
    /// </summary>
    public class OscSocketException : Exception
    {
        /// <summary>
        /// The socket that threw the exception
        /// </summary>
        public OscSocket Socket { get; }

        /// <summary>
        /// Creates a new osc socket exception
        /// </summary>
        /// <param name="socket">The socket that this exception relates to</param>
        /// <param name="message">A message string</param>
        public OscSocketException(OscSocket socket, string message)
            : base(message)
        {
            this.Socket = socket;
        }

        /// <summary>
        /// Creates a new osc socket exception
        /// </summary>
        /// <param name="socket">The socket that this exception relates to</param>
        /// <param name="message">A message string</param>
        /// <param name="innerException">An inner exception</param>
        public OscSocketException(OscSocket socket, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Socket = socket;
        }
    }
}
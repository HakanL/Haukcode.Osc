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
    /// Exception thrown when a osc socket is in an incorrect state
    /// </summary>
    public class OscSocketStateException : OscSocketException
    {
        private OscSocketState state;

        /// <summary>
        /// The state the socket was in when the exception was thrown
        /// </summary>
        public OscSocketState State { get { return state; } }

        /// <summary>
        /// Creates a new osc socket state exception
        /// </summary>
        /// <param name="socket">The socket that this exception relates to</param>
        /// <param name="state">The state the socket was in when the exception was thrown</param>
        /// <param name="message">A message string</param>
        public OscSocketStateException(OscSocket socket, OscSocketState state, string message)
            : base(socket, message)
        {
            this.state = state;
        }

        /// <summary>
        /// Creates a new osc socket state exception
        /// </summary>
        /// <param name="socket">The socket that this exception relates to</param>
        /// <param name="state">The state the socket was in when the exception was thrown</param>
        /// <param name="message">A message string</param>
        /// <param name="innerException">An inner exception</param>
        public OscSocketStateException(OscSocket socket, OscSocketState state, string message, Exception innerException)
            : base(socket, message, innerException)
        {
            this.state = state;
        }
    }
}
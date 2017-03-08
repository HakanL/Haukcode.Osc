﻿/*
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

namespace Rug.Osc
{
    internal class OscLiteralEvent
    {
        /// <summary>
        /// The literal address of the event
        /// </summary>
        public readonly string Address;

        public event OscMessageEvent Event;

        public bool IsNull { get { return Event == null; } }

        internal OscLiteralEvent(string address)
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
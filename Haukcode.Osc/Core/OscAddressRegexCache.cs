﻿/*
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haukcode.Osc
{
    /// <summary>
    /// Regex cache is an optimisation for regexes for address patterns. Caching is enabled by default.
    /// </summary>
    /// <remarks>
    /// This mechanism assumes that the same addresses will be used multiple times
    /// and that there will be a finite number of unique addresses parsed over the course
    /// of the execution of the program.
    ///
    /// If there are to be many unique addresses used of the course of the execution of
    /// the program then it maybe desirable to disable caching.
    /// </remarks>
    public static class OscAddressRegexCache
    {
        private static readonly ConcurrentDictionary<string, Regex> Lookup = new ConcurrentDictionary<string, Regex>();

        /// <summary>
        /// Enable regex caching for the entire program (Enabled by default)
        /// </summary>
        public static bool Enabled { get; set; }

        /// <summary>
        /// The number of cached regex(s)
        /// </summary>
        public static int Count => Lookup.Count;

        static OscAddressRegexCache()
        {
            // enable caching by default
            Enabled = true;
        }

        /// <summary>
        /// Clear the entire cache
        /// </summary>
        public static void Clear()
        {
            Lookup.Clear();
        }

        /// <summary>
        /// Acquire a regex, either by creating it if no cached one can be found or retrieving the cached one.
        /// </summary>
        /// <param name="regex">regex pattern</param>
        /// <returns>a regex created from or retrieved for the pattern</returns>
        public static Regex Aquire(string regex)
        {
            return Enabled == false ?
                // if caching is disabled then just return a new regex
                new Regex(regex, RegexOptions.None) :
                // else see if we have one cached
                Lookup.GetOrAdd(regex,
                    // create a new one, we can compile it as it will probably be resued
                    func => new Regex(regex, RegexOptions.Compiled)
                );
        }
    }
}
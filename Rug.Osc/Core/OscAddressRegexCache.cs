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

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Rug.Osc
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
		private static readonly object m_Lock = new object();

		private static readonly Dictionary<string, Regex> m_Lookup = new Dictionary<string, Regex>(); 

		/// <summary>
		/// Enable regex caching for the entire program (Enabled by default)
		/// </summary>
		public static bool Enabled { get; set; }

		/// <summary>
		/// The number of cached regex(s) 
		/// </summary>
		public static int Count { get { return m_Lookup.Count; } } 

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
			lock (m_Lock)
			{
				m_Lookup.Clear(); 
			}
		}

		/// <summary>
		/// Aquire a regex, either by creating it if no cached one can be found or retrieving a cached one  
		/// </summary>
		/// <param name="regex">regex pattern</param>
		/// <returns>a regex created from or retrieved for the pattern</returns>
		public static Regex Aquire(string regex)
		{
			// if caching is disabled then just return a new regex 
			if (Enabled == false)
			{
				// do not compile!
				return new Regex(regex, RegexOptions.None); 
			}

			lock (m_Lock)
			{
				Regex result;

				// see if we have one cached
				if (m_Lookup.TryGetValue(regex, out result) == true)
				{
					return result;
				}

				// create a new one, we can compile it as it will probably be resued
				result = new Regex(regex, RegexOptions.Compiled);

				// add it to the lookup 
				m_Lookup.Add(regex, result);

				// return the new regex
				return result; 
			}
		}
	}
}

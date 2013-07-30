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
using System.Text;
using System.Text.RegularExpressions;

namespace Rug.Osc
{
	/// <summary>
	/// Encompasses a single part of an osc address
	/// </summary>
	public struct OscAddressPart
	{
		#region Regex Char Escape Helpers

		private static readonly Regex CharMatcher = new Regex(@"[\.\$\^\{\[\(\|\)\*\+\?\\]", RegexOptions.Compiled);

		private static string EscapeString(string str)
		{
			return CharMatcher.Replace(str, match =>
			{
				switch (match.Value)
				{
					case ".":
						return @"\.";
					case "$":
						return @"\$";
					case "^":
						return @"\^";
					case "{":
						return @"\{";
					case "[":
						return @"\[";
					case "(":
						return @"\(";
					case "|":
						return @"\|";
					case ")":
						return @"\)";
					case "*":
						return @"\*";
					case "+":
						return @"\+";
					case "?":
						return @"\?";
					case "\\":
						return @"\\";
					default: throw new Exception(Strings.OscAddress_UnexpectedMatch);
				}
			});
		}

		private static string EscapeChar(char c)
		{
			return EscapeString(c.ToString());
		}

		#endregion

		#region Public Fields

		/// <summary>
		/// The address part type 
		/// </summary>
		public readonly OscAddressPartType Type;

		/// <summary>
		/// The original string value of this part
		/// </summary>
		public readonly string Value;

		/// <summary>
		/// How the string was interpreted (only used for testing) 
		/// </summary>
		internal readonly string Interpreted;

		/// <summary>
		/// The regex representation of this part
		/// </summary>
		public readonly string PartRegex;

		#endregion

		#region Constructor

		/// <summary>
		/// Create a address part
		/// </summary>
		/// <param name="type">the type of part</param>
		/// <param name="value">the original string value</param>
		/// <param name="interpreted">the representation of the original value as interpreted by the parser</param>
		/// <param name="partRegex">the part as a regex expression</param>
		private OscAddressPart(OscAddressPartType type, string value, string interpreted, string partRegex)
		{
			Type = type;
			Value = value;
			Interpreted = interpreted;
			PartRegex = partRegex;
		}

		#endregion

		#region Factory Methods

		/// <summary>
		/// Create a address separator part '/' 
		/// </summary>
		/// <returns>the part</returns>
		internal static OscAddressPart AddressSeparator()
		{
			return new OscAddressPart(OscAddressPartType.AddressSeparator, "/", "/", "/");
		}

		/// <summary>
		/// Create a address wildcard part "//" 
		/// </summary>
		/// <returns>the part</returns>
		internal static OscAddressPart AddressWildcard()
		{
			return new OscAddressPart(OscAddressPartType.AddressWildcard, "//", "//", "/");
		}

		/// <summary>
		/// Create a literal address part
		/// </summary>
		/// <param name="value">the literal</param>
		/// <returns>the part</returns>
		internal static OscAddressPart Literal(string value)
		{
			return new OscAddressPart(OscAddressPartType.Literal, value, value, "(" + EscapeString(value) + ")");
		}

		/// <summary>
		/// Create a part for a wildcard part
		/// </summary>
		/// <param name="value">the original string</param>
		/// <returns>the part</returns>
		internal static OscAddressPart Wildcard(string value)
		{
			string regex = value;

			// reduce the complexity 
			while (regex.Contains("*?") == true)
			{
				regex = regex.Replace("*?", "*");
			}

			// reduce needless complexity 
			while (regex.Contains("**") == true)
			{
				regex = regex.Replace("**", "*");
			}


			StringBuilder sb = new StringBuilder();

			// replace with wildcard regex
			foreach (char c in regex)
			{
				if (c == '*')
				{
					sb.Append(@"([^\s#\*,/\?\[\]\{}]*)");
				}
				else if (c == '?')
				{
					sb.Append(@"([^\s#\*,/\?\[\]\{}])");
				}
			}

			return new OscAddressPart(OscAddressPartType.Wildcard, value, value, sb.ToString());
		}


		/// <summary>
		/// Character span e.g. [a-e] 
		/// </summary>
		/// <param name="value">the original string</param>
		/// <returns>the part</returns>
		internal static OscAddressPart CharSpan(string value)
		{
			bool isNot = false;
			int index = 1;

			if (value[index] == '!')
			{
				isNot = true;
				index++;
			}

			char low = value[index++];
			index++;
			char high = value[index++];

			string regex = String.Format("[{0}{1}-{2}]+", isNot ? "^" : String.Empty, EscapeChar(low), EscapeChar(high));
			string rebuild = String.Format("[{0}{1}-{2}]", isNot ? "!" : String.Empty, low, high);

			return new OscAddressPart(OscAddressPartType.CharSpan, value, rebuild, regex);
		}

		/// <summary>
		/// Character list e.g. [abcde]
		/// </summary>
		/// <param name="value">the original string</param>
		/// <returns>the part</returns>
		internal static OscAddressPart CharList(string value)
		{
			bool isNot = false;
			int index = 1;

			if (value[index] == '!')
			{
				isNot = true;
				index++;
			}

			string list = value.Substring(index, (value.Length - 1) - index);

			string regex = String.Format("[{0}{1}]+", isNot ? "^" : String.Empty, EscapeString(list));
			string rebuild = String.Format("[{0}{1}]", isNot ? "!" : String.Empty, list);

			return new OscAddressPart(OscAddressPartType.CharList, value, rebuild, regex);
		}

		/// <summary>
		/// Literal list e.g. {thing1,THING1}
		/// </summary>
		/// <param name="value">the original string</param>
		/// <returns>the part</returns>
		internal static OscAddressPart List(string value)
		{
			string[] list = value.Substring(1, value.Length - 2).Split(',');

			StringBuilder regSb = new StringBuilder();
			StringBuilder listSb = new StringBuilder();

			bool first = true;

			foreach (string str in list)
			{
				if (first == false)
				{
					regSb.Append("|");
					listSb.Append(",");
				}
				else
				{
					first = false;
				}

				regSb.Append("(" + EscapeString(str) + ")");
				listSb.Append(str);
			}

			return new OscAddressPart(OscAddressPartType.List, value, "{" + listSb.ToString() + "}", "(" + regSb.ToString() + ")");
		}

		#endregion
	}
}

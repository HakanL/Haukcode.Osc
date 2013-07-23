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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Rug.Osc
{
	#region Osc Address Type

	internal enum OscAddressType 
	{
		Literal, 
		Pattern 
	}

	#endregion

	#region Osc Address Part Type

	/// <summary>
	/// Type of address part
	/// </summary>
	public enum OscAddressPartType
	{
		/// <summary>
		/// Address seperator char i.e. '/'  
		/// </summary>
		AddressSeparator,

		/// <summary>
		/// Address wildcared i.e. '//'
		/// </summary>
		AddressWildcard,

		/// <summary>
		/// Any string literal i.e [^\s#\*,/\?\[\]\{}]+ 
		/// </summary>
		Literal,

		/// <summary>
		/// Either single char or anylength wildcard i.e '?' or '*'
		/// </summary>
		Wildcard,

		/// <summary>
		/// Char span e.g. [a-z]+
		/// </summary>
		CharSpan,

		/// <summary>
		/// List of literal matches
		/// </summary>
		List,

		/// <summary>
		/// List of posible char matches e.g. [abcdefg]+
		/// </summary>
		CharList,
	}

	#endregion 

	#region Osc Address Part

	/// <summary>
	/// Encompasses a single part of an osc address
	/// </summary>
	public struct OscAddressPart
	{
		#region Regex Char Escape Helpers

		private static Regex CharMatcher = new Regex(@"[\.\$\^\{\[\(\|\)\*\+\?\\]", RegexOptions.Compiled); 

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
	
	#endregion

	#region Osc Address

	/// <summary>
	/// Encompasses an entire osc address
	/// </summary>
	public class OscAddress
	{
		#region Private Static Members

		private static readonly Regex LiteralAddressValidator = new Regex(@"^/[^\s#\*,/\?\[\]\{}]+((/[^\s#\*,/\?\[\]\{}]+)*)$", RegexOptions.Compiled);

		private static readonly Regex PatternAddressValidator = new Regex(@"^(//|/)[^\s#/]+((/[^\s#/]+)*)$", RegexOptions.Compiled);

		private static readonly Regex PatternAddressPartValidator = new Regex(
			@"^((
			  (?<Literal>([^\s#\*,/\?\[\]\{}]+)) |
			  (?<Wildcard>([\*\?]+)) |	
			  (?<CharSpan>(\[(!?)[^\s#\*,/\?\[\]\{}-]-[^\s#\*,/\?\[\]\{}-]\])) |
			  (?<CharList>(\[(!?)[^\s#\*,/\?\[\]\{}]+\])) |
			  (?<List>{([^\s#\*/\?\,[\]\{}]+)((,[^\s#\*/\?\,[\]\{}]+)*)})
			  )+)$",
			RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private static readonly Regex PatternAddressPartExtractor = new Regex(
			@"
			  (?<Literal>([^\s#\*,/\?\[\]\{}]+)) |
			  (?<Wildcard>([\*\?]+)) |	
			  (?<CharSpan>(\[(!?)[^\s#\*,/\?\[\]\{}-]-[^\s#\*,/\?\[\]\{}-]\])) |
			  (?<CharList>(\[(!?)[^\s#\*,/\?\[\]\{}]+\])) |
			  (?<List>{([^\s#\*/\?\,[\]\{}]+)((,[^\s#\*/\?\,[\]\{}]+)*)})
			  ",
			RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

		private static readonly char[] AddressSeperatorChar = new char[] { '/' };

		#endregion

		#region Private Members

		private string m_OrigialString;
		private OscAddressPart[] m_Parts;
		private OscAddressType m_Type;
		private Regex m_Regex; 

		#endregion 

		#region Public Properties

		/// <summary>
		/// The string used to create the address 
		/// </summary>
		public string OrigialString { get { return m_OrigialString; } } 

		/// <summary>
		/// The number of parts in the address
		/// </summary>
		public int Count { get { return m_Parts.Length; } } 

		/// <summary>
		/// Address parts
		/// </summary>
		/// <param name="index">the index of the part</param>
		/// <returns>the address part at the given index</returns>
		public OscAddressPart this[int index] { get { return m_Parts[index]; } }

		/// <summary>
		/// Is this address a literal
		/// </summary>
		public bool IsLiteral { get { return m_Type == OscAddressType.Literal; } } 

		#endregion

		#region Constructor

		/// <summary>
		/// Create an osc address from a string, must follow the rules set out in http://opensoundcontrol.org/spec-1_0 and http://opensoundcontrol.org/spec-1_1
		/// </summary>
		/// <param name="address">the address string</param>
		public OscAddress(string address)
		{
			// Ensure address is valid
			if (IsValidAddressPattern(address) == false)
			{
				throw new ArgumentException(String.Format(Strings.OscAddress_NotAValidOscAddress, address), "address"); 
			}

			// stash the original string
			m_OrigialString = address;

			// is this address non-literal (an address pattern)
			bool nonLiteral = false;
			bool skipNextSeparator = false; 

			// create a list for the parsed parts
			List<OscAddressPart> addressParts = new List<OscAddressPart>();

			if (address.StartsWith("//") == true)
			{
				// add the wildcard
				addressParts.Add(OscAddressPart.AddressWildcard());
				
				// strip off the "//" from the address 
				address = address.Substring(2);

				// this address in not a literal
				nonLiteral = true;

				// do not add a Separator before the next token 
				skipNextSeparator = true; 					 
			}

			// the the bits of the path, split by the '/' char
			string[] parts = address.Split(AddressSeperatorChar, StringSplitOptions.RemoveEmptyEntries);

			// loop through all the parts
			foreach (string part in parts)
			{
				if (skipNextSeparator == false)
				{
					// add a separator
					addressParts.Add(OscAddressPart.AddressSeparator());
				}
				else
				{
					// we dont want to skip the next one
					skipNextSeparator = false; 
				}

				// get the matches within the part 
				MatchCollection matches = PatternAddressPartExtractor.Matches(part);								
				
				// loop through all matches
				foreach (Match match in matches) 
				{
					if (match.Groups["Literal"].Success == true)
					{
						addressParts.Add(OscAddressPart.Literal(match.Groups["Literal"].Value));
					}
					else if (match.Groups["Wildcard"].Success == true)
					{
						addressParts.Add(OscAddressPart.Wildcard(match.Groups["Wildcard"].Value));
						nonLiteral = true;
					}
					else if (match.Groups["CharSpan"].Success == true)
					{
						addressParts.Add(OscAddressPart.CharSpan(match.Groups["CharSpan"].Value));
						nonLiteral = true;
					}
					else if (match.Groups["CharList"].Success == true)
					{
						addressParts.Add(OscAddressPart.CharList(match.Groups["CharList"].Value));
						nonLiteral = true;
					}
					else if (match.Groups["List"].Success == true)
					{
						addressParts.Add(OscAddressPart.List(match.Groups["List"].Value));
						nonLiteral = true;
					}
					else
					{
						throw new Exception(String.Format(Strings.OscAddress_UnknownAddressPart, match.Value)); 
					}
				}
			}

			// set the type
			m_Type = nonLiteral ? OscAddressType.Pattern : OscAddressType.Literal;

			// set the parts array
			m_Parts = addressParts.ToArray();

			// build the regex if one is needed
			if (m_Type != OscAddressType.Literal)
			{
				BuildRegex(); 
			}
		}

		private void BuildRegex()
		{
			StringBuilder regex = new StringBuilder();

			if (m_Parts[0].Type == OscAddressPartType.AddressWildcard)
			{
				// dont care where the start is 
				regex.Append("(");
			}
			else
			{
				// match the start of the string 
				regex.Append("^(");
			}

			foreach (OscAddressPart part in m_Parts)
			{
				// match the part
				regex.Append(part.PartRegex);
			}

			// match the end of the string
			regex.Append(")$");

			// aquire the regex
			m_Regex = OscAddressRegexCache.Aquire(regex.ToString()); // new Regex(regex.ToString(), RegexOptions.None);
		}

		#endregion

		#region Match

		/// <summary>
		/// Match this address against an address string
		/// </summary>
		/// <param name="address">the address string to match against</param>
		/// <returns>true if the addresses match, otherwise false</returns>
		public bool Match(string address) 
		{
			// if this address in a literal 
			if (m_Type == OscAddressType.Literal)
			{
				// if the original string is the same then we are good
				return m_OrigialString.Equals(address); 
			}
			else
			{
				// use the pattern regex to determin a match
				return m_Regex.IsMatch(address);
			}
		}

		/// <summary>
		/// Match this address against another
		/// </summary>
		/// <param name="address">the address to match against</param>
		/// <returns>true if the addresses match, otherwise false</returns>
		public bool Match(OscAddress address) 
		{
			// if both addresses are literals then we can match on original string 			 
			if (m_Type == OscAddressType.Literal && 
				address.m_Type == OscAddressType.Literal)
			{
				return m_OrigialString.Equals(address.m_OrigialString);
			}
			// if this address is a literal then use the others regex 
			else if (m_Type == OscAddressType.Literal)
			{
				return address.m_Regex.IsMatch(m_OrigialString);
			}
			// if the other is a literal use this ones regex 
			else if (address.m_Type == OscAddressType.Literal)
			{
				return m_Regex.IsMatch(address.m_OrigialString);
			}
			// if both are patterns then we just match on pattern original strings
			else
			{
				return m_OrigialString.Equals(address.m_OrigialString);
			}
		}

		#endregion

		#region To String

		public override string ToString()
		{
 			 return m_OrigialString;
		}

		/// <summary>
		/// Only used for testing
		/// </summary>
		/// <returns>a string that would produce the same address pattern but not a copy of the original string</returns>
		internal string ToString_Rebuild()
		{
			StringBuilder sb = new StringBuilder(); 			

			foreach (OscAddressPart part in m_Parts) 
			{
				sb.Append(part.Interpreted);
			}
			
 			return sb.ToString();
		}

		#endregion

		#region Standard Overrides

		public override bool Equals(object obj)
		{
 			 return m_OrigialString.Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return m_OrigialString.GetHashCode();
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Is the supplied address a valid literal address (no wildcards or lists) 
		/// </summary>
		/// <param name="address">the address to check</param>
		/// <returns>true if the address is valid</returns>
		public static bool IsValidAddressLiteral(string address)
		{
			if (Helper.IsNullOrWhiteSpace(address) == true)
			{
				return false;
			}

			return LiteralAddressValidator.IsMatch(address); 
		}

		/// <summary>
		/// Is the supplied address a valid address pattern (may include wildcards and lists) 
		/// </summary>
		/// <param name="addressPattern">the address pattern to check</param>
		/// <returns>true if the address pattern is valid</returns>
		public static bool IsValidAddressPattern(string addressPattern)
		{
			if (Helper.IsNullOrWhiteSpace(addressPattern) == true) 
			{
				return false; 
			}

			if (PatternAddressValidator.IsMatch(addressPattern) == false)
			{
				return false; 
			}

			// is this address a liternal address? 
			if (IsValidAddressLiteral(addressPattern) == true) 
			{
				return true; 
			}

			string[] parts = addressPattern.Split(AddressSeperatorChar, StringSplitOptions.RemoveEmptyEntries); 			

			bool isMatch = true;

			// scan for wild chars and lists
			foreach (string part in parts)
			{
				isMatch &= PatternAddressPartValidator.IsMatch(part); 
			}

			return isMatch; 
 		}

		/// <summary>
		/// Does a address match a address pattern
		/// </summary>
		/// <param name="addressPattern">address pattern (may include wildcards and lists)</param>
		/// <param name="address">literal address</param>
		/// <returns>true if the addess matches the pattern</returns>
		public static bool IsMatch(string addressPattern, string address)
		{
			if (IsValidAddressLiteral(address) == false) 
			{
				return false; 
			}
			
			// are they both literals 
			if (IsValidAddressLiteral(addressPattern) == true)
			{
				// preform a string match 
				return addressPattern.Equals(address); 
			}

			if (IsValidAddressPattern(addressPattern) == false)
			{
				return false;
			}

			// create a new pattern for the match 
			OscAddress pattern = new OscAddress(addressPattern);

			// return the result
			return pattern.Match(address); 		
		}

		#endregion
	}

	#endregion
}

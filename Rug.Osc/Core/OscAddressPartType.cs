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

namespace Rug.Osc
{
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
}

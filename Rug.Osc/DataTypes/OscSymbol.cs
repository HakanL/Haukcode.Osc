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
	/// Osc symbol 
	/// </summary>
	public struct OscSymbol
	{
		/// <summary>
		/// The string value of the symbol
		/// </summary>
		public string Value;

		/// <summary>
		/// Create a new symbol
		/// </summary>
		/// <param name="value">literal string value</param>
		public OscSymbol(string value)
		{
			Value = value; 
		}

		public override string ToString()
		{
			return Value;
		}

		public override bool Equals(object obj)
		{
			if (obj is OscSymbol)
			{
				return Value.Equals(((OscSymbol)obj).Value);
			}
			else
			{
				return Value.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}

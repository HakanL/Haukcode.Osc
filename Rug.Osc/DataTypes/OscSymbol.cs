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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Rug.Osc
{
	/// <summary>
	/// Osc symbol 
	/// </summary>
	[Serializable]
	public struct OscSymbol : ISerializable
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

		public OscSymbol(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new System.ArgumentNullException("info");
			}

			Value = (string)info.GetValue("Value", typeof(string));
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

		#region ISerializable Members

		[SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new System.ArgumentNullException("info");
			}

			info.AddValue("Value", Value);
		}

		#endregion
	}
}

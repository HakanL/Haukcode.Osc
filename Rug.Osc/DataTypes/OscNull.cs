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
	/// Osc Null Singleton
	/// </summary>
	[Serializable]
	public sealed class OscNull : ISerializable
	{
		public static readonly OscNull Value = new OscNull();

		private OscNull() { }

		public override string ToString()
		{
			return "null";
		}

		public static bool IsNull(string str)
		{
			bool isTrue = false;

			isTrue |= "Null".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			isTrue |= "Nil".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			return isTrue;
		}

		#region ISerializable Members

		[SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof(OscNullSerializationHelper));
		}

		#endregion
	}


	[Serializable]
	//[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	internal sealed class OscNullSerializationHelper : IObjectReference
	{
		// GetRealObject is called after this object is deserialized.
		public Object GetRealObject(StreamingContext context)
		{
			// When deserialiing this object, return a reference to 
			// the Singleton object instead.
			return OscNull.Value;
		}
	}
}

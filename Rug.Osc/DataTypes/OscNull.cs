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
	/// Osc Null Singleton
	/// </summary>
	public sealed class OscNull
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
	}
}

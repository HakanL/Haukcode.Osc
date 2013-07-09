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
	/// Osc Infinitum Singleton
	/// </summary>
	public sealed class OscInfinitum
	{
		public static readonly OscInfinitum Value = new OscInfinitum();

		private OscInfinitum() { }

		public override string ToString()
		{
			return "inf";
		}

		public static bool IsInfinitum(string str)
		{
			bool isTrue = false;

			isTrue |= "Infinitum".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			isTrue |= "Inf".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			return isTrue; 
		}
	}
}

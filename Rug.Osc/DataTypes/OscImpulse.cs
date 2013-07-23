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
	/// Osc Impulse Singleton
	/// </summary>
	public sealed class OscImpulse
	{
		public static readonly OscImpulse Value = new OscImpulse();

		private OscImpulse() { }

		public override string ToString()
		{
			return "bang";
		}

		/// <summary>
		/// Matches the string against "Impulse", "Bang", "Infinitum", "Inf" the comparison is StringComparison.InvariantCultureIgnoreCase
		/// </summary>
		/// <param name="str">string to check</param>
		/// <returns>true if the string matches any of the recognised impulse strings else false</returns>
		public static bool IsImpulse(string str)
		{
			bool isTrue = false;

			isTrue |= "Infinitum".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			isTrue |= "Inf".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			isTrue |= "Bang".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			isTrue |= "Impulse".Equals(str, System.StringComparison.InvariantCultureIgnoreCase);

			return isTrue; 
		}
	}
}

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

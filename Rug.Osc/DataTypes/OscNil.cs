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
	/// Osc Nil Singleton
	/// </summary>
	public sealed class OscNil
	{
		public static readonly OscNil Value = new OscNil();

		private OscNil() { }

		public override string ToString()
		{
			return "nil";
		}
	}
}

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

namespace Rug.Osc.Profile.Addresses
{
	class StaticObjects
	{
		static StaticObjects()
		{
			#region Parsed_Addresses

			Parsed_Addresses = new OscAddress[StaticObjects.Good_AddressPatterns.Length];

			for (int i = 0; i < Parsed_Addresses.Length; i++)
			{
				Parsed_Addresses[i] = new OscAddress(StaticObjects.Good_AddressPatterns[i]);
			}

			#endregion

			#region Parsed_AddressPatternMatches

			Parsed_AddressPatternMatches = new OscAddress[StaticObjects.Good_AddressPatternMatches.Length];

			for (int i = 0; i < Parsed_AddressPatternMatches.Length; i++)
			{
				Parsed_AddressPatternMatches[i] = new OscAddress(StaticObjects.Good_AddressPatternMatches[i]);
			}

			#endregion

			All_Addresses = new string[StaticObjects.Good_AddressPatterns.Length + StaticObjects.Good_AddressPatternMatches.Length];

			for (int i = 0; i < Good_AddressPatterns.Length; i++)
			{
				All_Addresses[i] = StaticObjects.Good_AddressPatterns[i];
			}

			for (int i = 0; i < Good_AddressPatternMatches.Length; i++)
			{
				All_Addresses[StaticObjects.Good_AddressPatterns.Length + i] = StaticObjects.Good_AddressPatternMatches[i];
			}

			Parsed_Messages = new OscMessage[All_Addresses.Length];

			for (int i = 0; i < All_Addresses.Length; i++)
			{
				Parsed_Messages[i] = new OscMessage(StaticObjects.All_Addresses[i]);
			}

			Mananger = new OscListenerManager();

			for (int i = 0; i < Good_AddressPatterns.Length; i++)
			{
				Mananger.Attach(StaticObjects.Good_AddressPatterns[i], DummyMethod);
			}
		}

		#region Addresses

		internal static OscAddress[] Parsed_Addresses;
		internal static OscAddress[] Parsed_AddressPatternMatches; 

		internal static string[] Good_AddressPatterns = new string[] 
		{
			"/container_A", 
			"/container_A/method_A", 
			"/0/1/2/3/4", 
			"/container_A/[0-9]", 
			"/container_A/[!0-9]", 
			"/container_A/[abc]", 
			"/container_A/[!abc]", 
			"/container_A/*g", 
			"/container_A/?tr?ng", 
			"/container_A/str?*", 
			"/container_A/str*?", 
			"/container_A/str**", 
			"/container_A/f*ing", 
			"/container_A/f?ing", 
			"/container_A/f?*s", 
			"/container_A/{method_A,method_B}", 
			"/container_A/method_{A,B}", 
			"/container_A/[method]_[A-Z]", 
			"/container_A/[!string]_[0-9]", 
			"/container_A/{method,container}_[A-Z]", 
			"//{method,container}_[A-Z]", 
		};

		internal static string[] Good_AddressPatternMatches = new string[] 
		{
			"/container_A", 
			"/container_A/method_A", 
			"/0/1/2/3/4", 
			"/container_A/3", 
			"/container_A/A", 
			"/container_A/ab", 
			"/container_A/string", 
			"/container_A/string", 
			"/container_A/string", 
			"/container_A/string", 
			"/container_A/string", 
			"/container_A/string", 
			"/container_A/falsethinging", 
			"/container_A/fking", 
			"/container_A/fals", 
			"/container_A/method_B", 
			"/container_A/method_B", 
			"/container_A/method_B", 
			"/container_A/me_hod_3", 
			"/container_A/method_B", 
			"/container_A/container_B/container_C/method_B", 
		};

		internal static string[] All_Addresses; 

		#endregion

		#region Messages

		internal static OscMessage[] Parsed_Messages;

		internal static byte[] MessageBody_Array_Ints2 = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'[', (byte)'i', (byte)'i', (byte)'i', (byte)']', (byte)'i', 0,

				// value array 
				0x1A, 0x2A, 0x3A, 0x4A,
				0x5A, 0x6A, 0x7A, 0x8A,
				0x9A, 0xAA, 0xBA, 0xCA,

				// value
				0x1A, 0x2A, 0x3A, 0x4A,
			};

		#endregion 

		#region Osc Listener Managers

		internal static readonly OscListenerManager Mananger;

		internal static void DummyMethod(OscMessage message) { } 

		#endregion
	}
}

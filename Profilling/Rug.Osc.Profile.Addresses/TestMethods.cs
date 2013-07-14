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
	internal static class TestMethods
	{
		public static void InstallTests()
		{
			TestManager.Tests.Add("Create Addresses", CreateAddresses);
			TestManager.Tests.Add("Addresses Match (String)", AddressesMatchString);
			TestManager.Tests.Add("Addresses Match (Object)", AddressesMatchObject);
			TestManager.Tests.Add("Create Message", MessageTest); 
			TestManager.Tests.Add("Read Message", ReadMessageTest);
			TestManager.Tests.Add("Invoke", InvokeTest); 
		}

		public static void CreateAddresses(int count)
		{
			int length = StaticObjects.Good_AddressPatterns.Length; 

			for (int i = 0; i < count; i++)
			{
				OscAddress address = new OscAddress(StaticObjects.Good_AddressPatterns[i % length]);		
			}
		}

		public static void AddressesMatchString(int count)
		{
			int length = StaticObjects.Good_AddressPatterns.Length;

			for (int i = 0; i < count; i++)
			{
				StaticObjects.Parsed_Addresses[i % length].Match(StaticObjects.Good_AddressPatternMatches[i % length]);
			}
		}

		public static void AddressesMatchObject(int count)
		{
			int length = StaticObjects.Good_AddressPatterns.Length;

			for (int i = 0; i < count; i++)
			{
				StaticObjects.Parsed_Addresses[i % length].Match(StaticObjects.Parsed_AddressPatternMatches[i % length]);
			}
		}

		public static void MessageTest(int count)
		{
			int length = StaticObjects.All_Addresses.Length;

			for (int i = 0; i < count; i++)
			{
				OscMessage message = new OscMessage(StaticObjects.All_Addresses[i % length], 
				new object[] 
				{ 
					unchecked((int)0x1A2A3A4A),
					unchecked((int)0x5A6A7A8A),
					unchecked((int)0x9AAABACA),
				},
				unchecked((int)0x1A2A3A4A));
			}
		}

		public static void ReadMessageTest(int count)
		{
			for (int i = 0; i < count; i++)
			{
				OscMessage message = OscMessage.Read(StaticObjects.MessageBody_Array_Ints2, StaticObjects.MessageBody_Array_Ints2.Length);
			}
		}		

		public static void InvokeTest(int count)
		{
			int length = StaticObjects.Parsed_Messages.Length;

			for (int i = 0; i < count; i++)
			{
				StaticObjects.Mananger.Invoke(StaticObjects.Parsed_Messages[i % length]);
			}
		}	
	}
}

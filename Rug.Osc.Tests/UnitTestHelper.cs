using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rug.Osc.Tests
{
	static class UnitTestHelper
	{
		#region Message Test Data

		#region Message Single Arg (Int)

		internal static byte[] MessageBody_Int = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'i', 0, 0, 
				// value
				0, 0, 0, 0x2A
			};

		internal static OscMessage Message_Int()
		{
			return new OscMessage("/test", 42);
		}

		#endregion

		#region Message Single Arg (Float)

		internal static byte[] MessageBody_Float = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'f', 0, 0, 
				// value				
				0x41, 0xCA, 00, 00
			};

		internal static OscMessage Message_Float()
		{
			return new OscMessage("/test", 25.25f);
		}

		#endregion

		#region Message Double Arg (Float)

		internal static byte[] MessageBody_Float2 = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'f', (byte)'f', 0, 
				// value 1				
				0x41, 0xCA, 00, 00,
				// value 2				
				0x41, 0xCA, 00, 00
			};

		internal static OscMessage Message_Float2()
		{
			return new OscMessage("/test", 25.25f, 25.25f);
		}

		#endregion

		#region Message Tripple Arg (Float)

		internal static byte[] MessageBody_Float3 = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'f', (byte)'f', (byte)'f', 0, 0, 0, 0,
				// value 1				
				0x41, 0xCA, 00, 00,
				// value 2				
				0x41, 0xCA, 00, 00,
				// value 3				
				0x41, 0xCA, 00, 00
			};

		internal static OscMessage Message_Float3()
		{
			return new OscMessage("/test", 25.25f, 25.25f, 25.25f);
		}

		#endregion

		#region Message Single Arg (String)

		internal static byte[] MessageBody_String = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'s', 0, 0, 
				// value				
				(byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)'!', 0, 0
			};

		internal static OscMessage Message_String()
		{
			return new OscMessage("/test", "hello!");
		}

		#endregion

		#region Message Single Arg (Blob)

		internal static byte[] MessageBody_Blob = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'b', 0, 0, 
				// length 
				0, 0, 0, 3,
				// value				
				3, 2, 1, 0
			};

		internal static OscMessage Message_Blob()
		{
			return new OscMessage("/test", new byte[] { 3, 2, 1 });
		}

		#endregion

		#region Badly Formed Messages

		internal static byte[] BadlyFormedMessage_PacketLength = new byte[] 
			{ 
				0
			};

		internal static byte[] BadlyFormedMessage_Address1 = new byte[] 
			{ 
				0, 0, 0, 0
			};

		internal static byte[] BadlyFormedMessage_Address2 = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s'
			};

		internal static byte[] BadlyFormedMessage_MissingComma = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				0, 0, 0, 0
			};

		internal static byte[] BadlyFormedMessage_MissingTypeTag = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'i', (byte)'i', (byte)'i'
			};

		internal static byte[] BadlyFormedMessage_MissingArgs = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'i', 0, 0
			};

		internal static byte[] BadlyFormedMessage_UnknownArguemntType = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'g', 0, 0,

				0, 0, 0, 0x2A
			};

		internal static byte[] BadlyFormedMessage_ErrorParsingBlob = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'b', 0, 0,

				0, 0, 0, 3
			};

		internal static byte[] BadlyFormedMessage_ErrorParsingBlob2 = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'i', (byte)'b', 0,

				0, 0, 0, 0x2A
			};

		internal static byte[] BadlyFormedMessage_ErrorParsingString = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'s', 0, 0,

				(byte)'t', (byte)'e', (byte)'s', (byte)'t',
			};

		internal static byte[] BadlyFormedMessage_ErrorParsingString2 = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'i', (byte)'s', 0,

				0, 0, 0, 0x2A
			};

		internal static byte[] BadlyFormedMessage_ErrorParsingInt = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'i', (byte)'i', 0,

				0, 0, 0, 0x2A
			};

		internal static byte[] BadlyFormedMessage_ErrorParsingFloat = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'i', (byte)'f', 0,

				0, 0, 0, 0x2A
			};

		#endregion

		#endregion

		#region Are Equal

		internal static void AreEqual(OscMessage expected, OscMessage actual)
		{
			Assert.AreEqual(expected.Error, actual.Error, "Error states do not match");
			Assert.AreEqual(expected.ErrorMessage, actual.ErrorMessage, "Error messages do not match");
			Assert.AreEqual(expected.Address, actual.Address, "Message addresses do not match");
			Assert.AreEqual(expected.Arguments.Length, actual.Arguments.Length, "Number of arguments do not match");

			for (int i = 0; i < actual.Arguments.Length; i++)
			{
				Assert.AreEqual(expected.Arguments[i].GetType(), actual.Arguments[i].GetType(), "Argument types at index {0} do not match", i);

				if (expected.Arguments[i] is byte[])
				{
					byte[] expectedArg = (byte[])expected.Arguments[i];
					byte[] actualArg = (byte[])actual.Arguments[i];

					AreEqual(expectedArg, actualArg);
				}
				else
				{
					Assert.AreEqual(expected.Arguments[i], actual.Arguments[i], "Arguments at index {0} do not match", i);
				}
			}
		}

		internal static void AreEqual(byte[] expected, byte[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length, "Array lengths do not match");

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], "Bytes at index {0} do not match", i);
			}
		}

		internal static void AreEqual(byte[] expected, byte[] actual, long actualLength)
		{
			Assert.AreEqual(expected.Length, actualLength, "Array lengths do not match");

			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], "Bytes at index {0} do not match", i);
			}
		}

		internal static void AreEqual(OscMessage target, string address, int sizeInBytes, params object[] values)
		{
			Assert.AreEqual(target.Error, OscMessageError.None, "Error state is not None");
			Assert.AreEqual(target.ErrorMessage, String.Empty, "Error message is not empty");
			Assert.AreEqual(target.Address, address, "Addresses do not match");
			Assert.IsNotNull(target.Arguments, "Arguments are null");

			if (values.Length == 0)
			{
				Assert.IsTrue(target.IsEmpty, "Arguments are not empty");
			}
			else
			{
				Assert.IsFalse(target.IsEmpty, "Arguments are empty");

				Assert.AreEqual(target.Arguments.Length, values.Length, "Does not have {0} argument", values.Length);

				for (int i = 0; i < values.Length; i++)
				{
					Assert.AreEqual(target.Arguments[i], values[i], "Argument at index {0} value does not match", i);
				}
			}

			Assert.AreEqual(target.MessageSize, sizeInBytes, "Message size is not correct");
		}

		#endregion


		#region Address Test Data 

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
		};

		internal static string[] Bad_AddressPatterns = new string[] 
		{
			"/", 
			"//1", 
			"/0/1/", 
			"/ /1/", 
			"/container A/1/", 
			"/container_A/[0-9]]", 
			"/container_A/[[!0-9]", 
			"/container_A/{{method_A,method_B}", 
			"/container_A/{method_A,method_B}}", 			
		};

		#endregion
	}
}

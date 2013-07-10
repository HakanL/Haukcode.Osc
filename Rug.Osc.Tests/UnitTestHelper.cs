using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

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

		internal static string MessageString_Int = "/test, 42";

		#endregion

		#region Message Single Arg (Long)

		internal static byte[] MessageBody_Long = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'h', 0, 0, 
				// value
				0xA1, 0xC2, 0xE3, 0xF4, 0xA5, 0xC6, 0xE7, 0xF8 
			};

		internal static OscMessage Message_Long()
		{
			return new OscMessage("/test", unchecked((long)0xA1C2E3F4A5C6E7F8));
		}

		internal static string MessageString_Long = "/test, 0xA1C2E3F4A5C6E7F8";

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

		internal static string MessageString_Float = "/test, 25.25";

		#endregion

		#region Message Single Arg (Double)

		internal static byte[] MessageBody_Double = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'d', 0, 0, 
				// value				
				0x40, 0x28, 0xd8, 0xc7, 0xe2, 0x82, 0x40, 0xb8 
			};

		internal static OscMessage Message_Double()
		{
			return new OscMessage("/test", 12.4234);
		}

		internal static string MessageString_Double = "/test, 12.4234d";

		#endregion

		#region Message Single Arg (TimeTag)

		internal static byte[] MessageBody_TimeTag = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'t', 0, 0, 
				// value				
				0x13, 0xC1, 0xDA, 0x49, 0xE6, 0xB5, 0x0B, 0x0F
			};

		internal static OscMessage Message_TimeTag()
		{
			return new OscMessage("/test", new OscTimeTag(0x13C1DA49E6B50B0F));
		}

		#endregion

		#region Message Single Arg (Char)

		internal static byte[] MessageBody_Char = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'c', 0, 0, 
				// value				
				(byte)'p', 0x00, 0x00, 0x00
			};

		internal static OscMessage Message_Char()
		{
			return new OscMessage("/test", (byte)'p');
		}

		internal static string MessageString_Char = "/test, p";

		#endregion

		#region Message Single Arg (Color)

		internal static byte[] MessageBody_Color_Red = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'r', 0, 0, 
				// value				
				0xFF, 0x00, 0x00, 0xFF
			};

		internal static OscMessage Message_Color_Red()
		{
			return new OscMessage("/test", Color.FromArgb(255, 255, 0, 0));
		}

		internal static byte[] MessageBody_Color_Green = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'r', 0, 0, 
				// value				
				0x00, 0xFF, 0x00, 0xFF
			};

		internal static OscMessage Message_Color_Green()
		{
			return new OscMessage("/test", Color.FromArgb(255, 0, 255, 0));
		}

		internal static byte[] MessageBody_Color_Blue = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'r', 0, 0, 
				// value				
				0x00, 0x00, 0xFF, 0xFF
			};

		internal static OscMessage Message_Color_Blue()
		{
			return new OscMessage("/test", Color.FromArgb(255, 0, 0, 255));
		}

		internal static byte[] MessageBody_Color_Transparent = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'r', 0, 0, 
				// value				
				0x00, 0x00,0x00, 0x00
			};

		internal static OscMessage Message_Color_Transparent()
		{
			return new OscMessage("/test", Color.FromArgb(0, 0, 0, 0));
		}

		#endregion

		#region Message Single Arg (Midi)

		internal static byte[] MessageBody_Midi = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'m', 0, 0, 
				// value				
				0x03, 0xF3, 0x56, 0x26
			};

		internal static OscMessage Message_Midi()
		{
			return new OscMessage("/test", new OscMidiMessage(3, OscMidiSystemMessageType.SongSelect, 0x1356));
		}

		internal static string MessageString_Midi = "/test, { Midi: 3, SongSelect, 86, 38 }";

		#endregion

		#region Message Single Arg (True)

		internal static byte[] MessageBody_True = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'T', 0, 0, 				
			};

		internal static OscMessage Message_True()
		{
			return new OscMessage("/test", true);
		}

		internal static string MessageString_True = "/test, true";

		#endregion

		#region Message Single Arg (False)

		internal static byte[] MessageBody_False = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'F', 0, 0, 				
			};

		internal static OscMessage Message_False()
		{
			return new OscMessage("/test", false);
		}

		internal static string MessageString_False = "/test, false";

		#endregion

		#region Message Single Arg (Nil)

		internal static byte[] MessageBody_Nil = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'N', 0, 0, 				
			};

		internal static OscMessage Message_Nil()
		{
			return new OscMessage("/test", OscNull.Value);
		}

		internal static string MessageString_Nil = "/test, nil";

		#endregion

		#region Message Single Arg (Infinitum)

		internal static byte[] MessageBody_Infinitum = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'I', 0, 0, 				
			};

		internal static OscMessage Message_Infinitum()
		{
			return new OscMessage("/test", OscImpulse.Value);
		}

		internal static string MessageString_Infinitum = "/test, inf";

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

		internal static string MessageString_Float2 = "/test, 25.25f, 25.25f";

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

		internal static string MessageString_Float3 = "/test, 25.25f, 25.25f, 25.25f";

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

		internal static string MessageString_String = "/test, \"hello!\"";

		#endregion

		#region Message Single Arg (Symbol)

		internal static byte[] MessageBody_Symbol = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'S', 0, 0, 
				// value				
				(byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o', (byte)'!', 0, 0
			};

		internal static OscMessage Message_Symbol()
		{
			return new OscMessage("/test", new OscSymbol("hello!"));
		}

		internal static string MessageString_Symbol = "/test, hello!";

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

		#region Message Array Arg (Ints)

		internal static byte[] MessageBody_Array_Ints = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'i', (byte)'[', (byte)'i', (byte)'i', (byte)'i', (byte)']', 0,
				// value
				0x1A, 0x2A, 0x3A, 0x4A,

				// value array 
				0x1A, 0x2A, 0x3A, 0x4A,
				0x5A, 0x6A, 0x7A, 0x8A,
				0x9A, 0xAA, 0xBA, 0xCA,
			};

		internal static OscMessage Message_Array_Ints()
		{
			return new OscMessage("/test", unchecked((int)0x1A2A3A4A), 
				new object[] 
				{ 
					unchecked((int)0x1A2A3A4A),
					unchecked((int)0x5A6A7A8A),
					unchecked((int)0x9AAABACA),
				});
		}

		internal static string MessageString_Array_Ints = "/test, 0x1A2A3A4A, [0x1A2A3A4A, 0x5A6A7A8A, 0x9AAABACA]";

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

		internal static OscMessage Message_Array_Ints2()
		{
			return new OscMessage("/test", 
				new object[] 
				{ 
					unchecked((int)0x1A2A3A4A),
					unchecked((int)0x5A6A7A8A),
					unchecked((int)0x9AAABACA),
				},
				unchecked((int)0x1A2A3A4A));
		}

		internal static string MessageString_Array_Ints2 = "/test, [0x1A2A3A4A, 0x5A6A7A8A, 0x9AAABACA], 0x1A2A3A4A";

		#endregion

		#region Message Nested Array Arg (Ints)

		internal static byte[] MessageBody_Array_NestedInts = new byte[] 
			{ 
				// Address 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0, 
				// Typetag
				(byte)',', (byte)'i', (byte)'[', (byte)'i', (byte)'i', (byte)'[', (byte)'i', (byte)'i', (byte)'i', (byte)']', (byte)'i', (byte)']', 0, 0, 0, 0,
				// value
				0x1A, 0x2A, 0x3A, 0x4A,

				// value array 
				0x1A, 0x2A, 0x3A, 0x4A,
				0x5A, 0x6A, 0x7A, 0x8A,

				// nested value array 
				0x1A, 0x2A, 0x3A, 0x4A,
				0x5A, 0x6A, 0x7A, 0x8A,
				0x9A, 0xAA, 0xBA, 0xCA,

				0x9A, 0xAA, 0xBA, 0xCA,
			};

		internal static OscMessage Message_Array_NestedInts()
		{
			return new OscMessage("/test", unchecked((int)0x1A2A3A4A),
				new object[] 
				{ 
					unchecked((int)0x1A2A3A4A),
					unchecked((int)0x5A6A7A8A),
					
					new object[] 
					{ 
						unchecked((int)0x1A2A3A4A),
						unchecked((int)0x5A6A7A8A),
						unchecked((int)0x9AAABACA),
					},

					unchecked((int)0x9AAABACA),
					
				});
		}

		internal static string MessageString_Array_NestedInts = "/test, 0x1A2A3A4A, [0x1A2A3A4A, 0x5A6A7A8A, [0x1A2A3A4A, 0x5A6A7A8A, 0x9AAABACA], 0x9AAABACA]";

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

		internal static byte[] BadlyFormedMessage_ErrorParsingDouble = new byte[] 
			{ 
				(byte)'/', (byte)'t', (byte)'e', (byte)'s', (byte)'t', 0, 0, 0,

				(byte)',', (byte)'d', 0, 0,

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

			AreEqual(expected.Arguments, actual.Arguments);
		}

		internal static void AreEqual(object[] expected, object[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length, "Number of arguments do not match");

			for (int i = 0; i < actual.Length; i++)
			{
				Assert.AreEqual(expected[i].GetType(), actual[i].GetType(), "Argument types at index {0} do not match", i);

				if (expected[i] is object[])
				{
					object[] expectedArg = (object[])expected[i];
					object[] actualArg = (object[])actual[i];

					AreEqual(expectedArg, actualArg);
				}
				else if (expected[i] is byte[])
				{
					byte[] expectedArg = (byte[])expected[i];
					byte[] actualArg = (byte[])actual[i];

					AreEqual(expectedArg, actualArg);
				}
				else if (expected[i] is Color)
				{
					Color expectedArg = (Color)expected[i];
					Color actualArg = (Color)actual[i];

					Assert.AreEqual(expectedArg.R, actualArg.R, "Color arguments at index {0} Red componets do not match", i);
					Assert.AreEqual(expectedArg.G, actualArg.G, "Color arguments at index {0} Green componets do not match", i);
					Assert.AreEqual(expectedArg.B, actualArg.B, "Color arguments at index {0} Blue componets do not match", i);
					Assert.AreEqual(expectedArg.A, actualArg.A, "Color arguments at index {0} Alpha componets do not match", i);
				}
				else
				{
					Assert.AreEqual(expected[i], actual[i], "Arguments at index {0} do not match", i);
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

		internal static string[] Bad_AddressPatterns = new string[] 
		{
			"/", 
			"/0//1", 
			"/0/1/", 
			"/ /1/", 
			"///1/2", 
			"/container A/1/", 
			"/container_A/[0-9]]", 
			"/container_A/[[!0-9]", 
			"/container_A/{{method_A,method_B}", 
			"/container_A/{method_A,method_B}}", 
		};

		#endregion
	}
}

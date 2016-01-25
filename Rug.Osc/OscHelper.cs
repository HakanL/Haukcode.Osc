﻿/* 
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
using System.Globalization;
using System.Text;

namespace Rug.Osc
{
	public static class OscHelper
	{
		public static string ArgumentsToString(object[] args)
		{
			return ArgumentsToString(args, CultureInfo.InvariantCulture);
		}

		public static string ArgumentsToString(object[] args, IFormatProvider provider)
		{
			if (args == null || args.Length == 0)
			{
				return string.Empty;
			}

			StringBuilder sb = new StringBuilder();

			OscMessage.ArgumentsToString(sb, provider, args);

			return sb.ToString();
		}

		/// <summary>
		/// Parse a single argument
		/// </summary>
		/// <param name="str">string contain the argument to parse</param>
		/// <returns>the parsed argument</returns>
		public static object ParseArgument(string str)
		{
			return OscMessage.ParseArgument(str, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parse a single argument
		/// </summary>
		/// <param name="str">string contain the argument to parse</param>
		/// <param name="provider">format provider to use</param>
		/// <returns>the parsed argument</returns>
		public static object ParseArgument(string str, IFormatProvider provider)
		{
			return OscMessage.ParseArgument(str, provider);
		}

		static byte[] GetBytes(string str)
		{
			return System.Text.Encoding.UTF8.GetBytes(str);

			//byte[] bytes = new byte[str.Length * sizeof(char)];
			//System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			//return bytes;
		}

		static string GetString(byte[] bytes)
		{
			return System.Text.Encoding.UTF8.GetString(bytes);

			/* 
			char[] chars = new char[bytes.Length / sizeof(char)];
			System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
			return new string(chars);
			*/
		}

		public static string EscapeString(string str)
		{
			return Escape(GetBytes(str));
		}

		public static string UnescapeString(string str)
		{
			return GetString(Unescape(str));
		}

		/// <summary>
		/// Turn a byte array into a readable, escaped string
		/// </summary>
		/// <param name="bytes">bytes</param>
		/// <returns>a string</returns>
		public static string Escape(byte[] bytes)
		{
			// the result is maximum of bytes length * 4 
			char[] chars = new char[bytes.Length * 4];

			int j = 0;

			for (int i = 0; i < bytes.Length; i++)
			{
				byte b = bytes[i];
				char c = (char)b;

				if (c > '~')
				{
					//chars[j++] = '�';
					chars[j++] = '\\';
					chars[j++] = 'x';
					chars[j++] = ((b & 240) >> 4).ToString("X")[0];
					chars[j++] = (b & 15).ToString("X")[0];
				}
				else if (c >= ' ')
				{
					chars[j++] = c;
				}
				else
				{
					switch (c)
					{
						case '\0':
							chars[j++] = '\\';
							chars[j++] = '0';
							break;

						case '\a':
							chars[j++] = '\\';
							chars[j++] = 'a';
							break;

						case '\b':
							chars[j++] = '\\';
							chars[j++] = 'b';
							break;

						case '\f':
							chars[j++] = '\\';
							chars[j++] = 'f';
							break;

						case '\n':
							chars[j++] = '\\';
							chars[j++] = 'n';
							break;

						case '\r':
							chars[j++] = '\\';
							chars[j++] = 'r';
							break;

						case '\t':
							chars[j++] = '\\';
							chars[j++] = 't';
							break;

						case '\v':
							chars[j++] = '\\';
							chars[j++] = 'v';
							break;

						case '\\':
							chars[j++] = '\\';
							chars[j++] = '\\';
							break;

						default:
							chars[j++] = '\\';
							chars[j++] = 'x';
							chars[j++] = ((b & 240) >> 4).ToString("X")[0];
							chars[j++] = (b & 15).ToString("X")[0];
							break;
					}
				}
			}

			return new string(chars, 0, j);
		}

        public static string ToStringBlob(byte[] bytes)
        {
            // if the default is to be Base64 encoded
            // return "64x" + System.Convert.ToBase64String(bytes); 

            StringBuilder sb = new StringBuilder((bytes.Length * 2) + 2);

            sb.Append("0x");

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static bool IsValidEscape(string str)
		{
			bool isEscaped = false;
			bool parseHexNext = false;
			int parseHexCount = 0;

			// first we count the number of chars we will be returning
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];

				if (parseHexNext == true)
				{
					parseHexCount++;

					if (Uri.IsHexDigit(c) == false)
					{
						return false;
					}

					if (parseHexCount == 2)
					{
						parseHexNext = false;
						parseHexCount = 0;
					}
				}
				// if we are not in  an escape sequence and the char is a escape char 
				else if (isEscaped == false && c == '\\')
				{
					// escape 
					isEscaped = true;
				}
				// else if we are escaped 
				else if (isEscaped == true)
				{
					// reset escape state 
					isEscaped = false;

					// check the char against the set of known escape chars 
					switch (char.ToLower(c))
					{
						case '0':
						case 'a':
						case 'b':
						case 'f':
						case 'n':
						case 'r':
						case 't':
						case 'v':
						case '\\':
							// do not increment count
							break;
						case 'x':
							// do not increment count
							parseHexNext = true;
							parseHexCount = 0;
							break;
						default:
							// this is not a valid escape sequence 
							// return false 
							return false;
					}
				}
				else
				{
					// normal char increment count
				}
			}

			if (parseHexNext == true)
			{
				return false;
			}

			return isEscaped == false;
		}

		/// <summary>
		/// Turn a readable string into a byte array 
		/// </summary>
		/// <param name="str">a string, optionally with escape sequences in it</param>
		/// <returns>a byte array</returns>
		public static byte[] Unescape(string str)
		{
			int count = 0;
			bool isEscaped = false;
			bool parseHexNext = false;
			int parseHexCount = 0;

			// Uri.HexEscape(
			// first we count the number of chars we will be returning
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];

				if (parseHexNext == true)
				{
					parseHexCount++;

					if (Uri.IsHexDigit(c) == false)
					{
						throw new Exception(string.Format(Strings.Escape_InvalidEscapeSequence_InvalidHexDigit, i, c));
					}

					if (parseHexCount == 2)
					{
						parseHexNext = false;
						parseHexCount = 0;
					}
				}
				// if we are not in  an escape sequence and the char is a escape char 
				else if (isEscaped == false && c == '\\')
				{
					// escape 
					isEscaped = true;

					// increment count 
					count++;
				}
				// else if we are escaped 
				else if (isEscaped == true)
				{
					// reset escape state 
					isEscaped = false;

					// check the char against the set of known escape chars 
					switch (char.ToLower(c))
					{
						case '0':
						case 'a':
						case 'b':
						case 'f':
						case 'n':
						case 'r':
						case 't':
						case 'v':
						case '\\':
							// do not increment count
							break;
						case 'x':
							// do not increment count
							parseHexNext = true;
							parseHexCount = 0;
							break;
						default:
							// this is not a valid escape sequence 
							throw new Exception(string.Format(Strings.Escape_InvalidEscapeSequence, i - 1));
					}
				}
				else
				{
					// normal char increment count
					count++;
				}
			}

			if (parseHexNext == true)
			{
				throw new Exception(string.Format(Strings.Escape_InvalidEscapeSequence_MissingHexValue, str.Length - 1));
			}

			if (isEscaped == true)
			{
				throw new Exception(string.Format(Strings.Escape_InvalidEscapeSequence, str.Length - 1));
			}

			// reset the escape state
			isEscaped = false;
			parseHexNext = false;
			parseHexCount = 0;

			// create a byte array for the result
			byte[] chars = new byte[count];

			int j = 0;

			// actually populate the array 
			for (int i = 0; i < str.Length; i++)
			{
				char c = (char)str[i];

				// if we are not in  an escape sequence and the char is a escape char 
				if (isEscaped == false && c == '\\')
				{
					// escape 
					isEscaped = true;
				}
				// else if we are escaped 
				else if (isEscaped == true)
				{
					// reset escape state
					isEscaped = false;

					// check the char against the set of known escape chars 
					switch (char.ToLower(str[i]))
					{
						case '0':
							chars[j++] = (byte)'\0';
							break;

						case 'a':
							chars[j++] = (byte)'\a';
							break;

						case 'b':
							chars[j++] = (byte)'\b';
							break;

						case 'f':
							chars[j++] = (byte)'\f';
							break;

						case 'n':
							chars[j++] = (byte)'\n';
							break;

						case 'r':
							chars[j++] = (byte)'\r';
							break;

						case 't':
							chars[j++] = (byte)'\t';
							break;

						case 'v':
							chars[j++] = (byte)'\v';
							break;

						case '\\':
							chars[j++] = (byte)'\\';
							break;

						case 'x':
							//string temp = "" + str[++i] + str[++i]; 
							//chars[j++] = byte.Parse(temp, NumberStyles.HexNumber); //;
							chars[j++] = (byte)((Uri.FromHex(str[++i]) << 4) | Uri.FromHex(str[++i]));
							break;

						default:
							// this is not a valid escape sequence 
							break;
					}
				}
				else
				{
					// normal char
					chars[j++] = (byte)c;
				}
			}

			return chars;
		}
	}
}

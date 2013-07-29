/* 
 * Rug.Osc 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2013 Phill Tew. All rights reserved.
 * 
 * Based on orginal code by Simon Mourier codefluententities.com
 * http://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value
 */

using System;
using System.Globalization;

namespace Rug.Osc
{
	/// <summary>
	/// Enum Helper class orginal code by Simon Mourier codefluententities.com
	/// http://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value
	/// </summary>
	internal static class EnumHelper
	{
		#region Private Static Members

		private static readonly char[] m_EnumSeperators = new char[] { ',', ';', '+', '|', ' ' };

		#endregion

		#region Parse

		/// <summary>
		/// Converts the string representation of an enum to its Enum equivalent value. A return value indicates whether the operation succeeded.
		/// This method does not rely on Enum.Parse and therefore will never raise any first or second chance exception.
		/// </summary>
		/// <typeparam name="T">The enum target type</typeparam>
		/// <param name="input">The input text. May be null.</param>
		/// <param name="value">When this method returns, contains Enum equivalent value to the enum contained in input, if the conversion succeeded.</param>
		/// <returns>true if s was converted successfully; otherwise, false.</returns>
		public static bool TryParse<T>(string input, out T value)
		{
			return TryParse<T>(input, false, out value); 
		}

		/// <summary>
		/// Converts the string representation of an enum to its Enum equivalent value. A return value indicates whether the operation succeeded.
		/// This method does not rely on Enum.Parse and therefore will never raise any first or second chance exception.
		/// </summary>
		/// <typeparam name="T">The enum target type</typeparam>
		/// <param name="input">The input text. May be null.</param>
		/// <param name="ignoreCase">ignore the case of the input string</param>
		/// <param name="value">When this method returns, contains Enum equivalent value to the enum contained in input, if the conversion succeeded.</param>
		/// <returns>true if s was converted successfully; otherwise, false.</returns>
		public static bool TryParse<T>(string input, bool ignoreCase, out T value)
		{
			object output;

			bool result = TryParse(typeof(T), input, ignoreCase, out output);

			if (result == true)
			{
				value = (T)output; 
			}
			else
			{
				value = default(T);
			}

			return result; 
		}
	
		/// <summary>
		/// Converts the string representation of an enum to its Enum equivalent value. A return value indicates whether the operation succeeded.
		/// This method does not rely on Enum.Parse and therefore will never raise any first or second chance exception.
		/// </summary>
		/// <param name="type">The enum target type. May not be null.</param>
		/// <param name="input">The input text. May be null.</param>
		/// <param name="value">When this method returns, contains Enum equivalent value to the enum contained in input, if the conversion succeeded.</param>
		/// <returns>
		/// true if s was converted successfully; otherwise, false.
		/// </returns>
		public static bool TryParse(Type type, string input, out object value)
		{
			return TryParse(type, input, false, out value); 
		}

		/// <summary>
		/// Converts the string representation of an enum to its Enum equivalent value. A return value indicates whether the operation succeeded.
		/// This method does not rely on Enum.Parse and therefore will never raise any first or second chance exception.
		/// </summary>
		/// <param name="type">The enum target type. May not be null.</param>
		/// <param name="input">The input text. May be null.</param>
		/// <param name="ignoreCase">ignore the case of the input string</param>
		/// <param name="value">When this method returns, contains Enum equivalent value to the enum contained in input, if the conversion succeeded.</param>
		/// <returns>
		/// true if s was converted successfully; otherwise, false.
		/// </returns>
		public static bool TryParse(Type type, string input, bool ignoreCase, out object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (!type.IsEnum)
			{
				throw new ArgumentException(null, "type");
			}

			if (input == null)
			{
				value = Activator.CreateInstance(type);
				return false;
			}

			input = input.Trim();
			if (input.Length == 0)
			{
				value = Activator.CreateInstance(type);
				return false;
			}

			string[] names = Enum.GetNames(type);
			if (names.Length == 0)
			{
				value = Activator.CreateInstance(type);
				return false;
			}

			Type underlyingType = Enum.GetUnderlyingType(type);
			Array values = Enum.GetValues(type);

			// some enums like System.CodeDom.MemberAttributes *are* flags but are not declared with Flags...
			if ((!type.IsDefined(typeof(FlagsAttribute), true)) && (input.IndexOfAny(m_EnumSeperators) < 0))
			{
				return ToObject(type, underlyingType, names, values, input, ignoreCase, out value);
			}

			// multi value enum
			string[] tokens = input.Split(m_EnumSeperators, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length == 0)
			{
				value = Activator.CreateInstance(type);
				return false;
			}

			ulong ul = 0;

			foreach (string tok in tokens)
			{
				string token = tok.Trim(); // NOTE: we don't consider empty tokens as errors	
				if (token.Length == 0)
				{
					continue;
				}

				object tokenValue;
				if (!ToObject(type, underlyingType, names, values, token, ignoreCase, out tokenValue))
				{
					value = Activator.CreateInstance(type);
					return false;
				}

				ulong tokenUl;
				switch (Convert.GetTypeCode(tokenValue))
				{
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.Int64:
					case TypeCode.SByte:
						tokenUl = (ulong)Convert.ToInt64(tokenValue, CultureInfo.InvariantCulture);
						break;
					default:
						tokenUl = Convert.ToUInt64(tokenValue, CultureInfo.InvariantCulture);
						break;
				}

				ul |= tokenUl;
			}

			value = Enum.ToObject(type, ul);

			return true;
		}

		#endregion 

		#region To Object

		private static object ToObject(Type underlyingType, string input)
		{
			if (underlyingType == typeof(int))
			{
				int s;
				if (int.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(uint))
			{
				uint s;
				if (uint.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(ulong))
			{
				ulong s;
				if (ulong.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(long))
			{
				long s;
				if (long.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(short))
			{
				short s;
				if (short.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(ushort))
			{
				ushort s;
				if (ushort.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(byte))
			{
				byte s;
				if (byte.TryParse(input, out s))
				{
					return s;
				}
			}

			if (underlyingType == typeof(sbyte))
			{
				sbyte s;
				if (sbyte.TryParse(input, out s))
				{
					return s;
				}
			}

			return null;
		}

		private static bool ToObject(Type type, Type underlyingType, string[] names, Array values, string input, bool ignoreCase, out object value)
		{
			for (int i = 0; i < names.Length; i++)
			{
				StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

				if (string.Compare(names[i], input, comparison) == 0)
				{
					value = values.GetValue(i);
					return true;
				}		
			}

			if ((char.IsDigit(input[0]) || (input[0] == '-')) || (input[0] == '+'))
			{
				object obj = ToObject(underlyingType, input);

				if (obj == null)
				{
					value = Activator.CreateInstance(type);
					return false;
				}

				value = obj;
				return true;
			}

			value = Activator.CreateInstance(type);
			return false;
		}

		#endregion
	}
}

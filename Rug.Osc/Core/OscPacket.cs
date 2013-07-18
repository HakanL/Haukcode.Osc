using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc
{
	/// <summary>
	/// Base class for all osc packets
	/// </summary>
	public abstract class OscPacket
	{
		/// <summary>
		/// The size of the packet in bytes
		/// </summary>
		public abstract int SizeInBytes { get; }

		/// <summary>
		/// If anything other than OscPacketError.None then an error occured while the packet was being parsed
		/// </summary>
		public abstract OscPacketError Error { get; }

		/// <summary>
		/// The descriptive string associated with Error
		/// </summary>
		public abstract string ErrorMessage { get; } 

		/// <summary>
		/// Get an array of bytes containing the entire packet
		/// </summary>
		/// <returns></returns>
		public abstract byte[] ToByteArray();

		#region Write

		/// <summary>
		/// Write the packet into a byte array
		/// </summary>
		/// <param name="data">the destination for the packet</param>
		/// <returns>the length of the packet in bytes</returns>
		public abstract int Write(byte[] data);

		/// <summary>
		/// Write the packet into a byte array
		/// </summary>
		/// <param name="data">the destination for the packet</param>
		/// <param name="index">the offset within the array where writing should begin</param>
		/// <returns>the length of the packet in bytes</returns>
		public abstract int Write(byte[] data, int index);

		#endregion

		#region Read

		/// <summary>
		/// Read the osc packet from a byte array
		/// </summary>
		/// <param name="bytes">array to read from</param>
		/// <param name="count">the number of bytes in the packet</param>
		/// <returns>the packet</returns>
		public static OscPacket Read(byte[] bytes, int count)
		{
			return Read(bytes, 0, count);
		}

		/// <summary>
		/// Read the osc packet from a byte array
		/// </summary>
		/// <param name="bytes">array to read from</param>
		/// <param name="index">the offset within the array where reading should begin</param>
		/// <param name="count">the number of bytes in the packet</param>
		/// <returns>the packet</returns>
		public static OscPacket Read(byte[] bytes, int index, int count)
		{
			if (OscBundle.IsBundle(bytes, index, count) == true)
			{
				return OscBundle.Read(bytes, index, count); 
			}
			else
			{
				return OscMessage.Read(bytes, index, count); 
			}
		}
		
		#endregion

		public static OscPacket Parse(string str)
		{
			if (str.Trim().StartsWith(OscBundle.BundleIdent) == true)
			{
				return OscBundle.Parse(str);
			}
			else
			{
				return OscMessage.Parse(str);
			}
		}

		public static bool TryParse(string str, out OscPacket packet)
		{
			try
			{
				packet = Parse(str); 

				return true; 
			}
			catch
			{
				packet = default(OscPacket);

				return false; 
			}
		}


		#region Scan Forward

		/// <summary>
		/// Scan for array start and end control chars
		/// </summary>
		/// <param name="str">the string to scan</param>
		/// <param name="controlChar">the index of the starting control char</param>
		/// <returns>the index of the end char</returns>
		protected static int ScanForward_Array(string str, int controlChar)
		{
			return ScanForward(str, controlChar, '[', ']', Strings.Parser_MissingArrayEndChar);
		}

		/// <summary>
		/// Scan for object start and end control chars
		/// </summary>
		/// <param name="str">the string to scan</param>
		/// <param name="controlChar">the index of the starting control char</param>
		/// <returns>the index of the end char</returns>
		protected static int ScanForward_Object(string str, int controlChar)
		{
			return ScanForward(str, controlChar, '{', '}', Strings.Parser_MissingObjectEndChar);
		}

		/// <summary>
		/// Scan for start and end control chars
		/// </summary>
		/// <param name="str">the string to scan</param>
		/// <param name="controlChar">the index of the starting control char</param>
		/// <param name="startChar">start control char</param>
		/// <param name="endChar">end control char</param>
		/// <param name="errorString">string to use in the case of an error</param>
		/// <returns>the index of the end char</returns>
		protected static int ScanForward(string str, int controlChar, char startChar, char endChar, string errorString)
		{
			bool found = false;

			int count = 0;

			int index = controlChar + 1;

			bool insideString = false;

			while (index < str.Length)
			{
				if (str[index] == '"')
				{
					insideString = !insideString;
				}
				else
				{
					if (insideString == false)
					{
						if (str[index] == startChar)
						{
							count++;
						}
						else if (str[index] == endChar)
						{
							if (count == 0)
							{
								found = true;

								break;
							}

							count--;
						}
					}
				}

				index++;
			}

			if (insideString == true)
			{
				throw new Exception(Strings.Parser_MissingStringEndChar);
			}

			if (count > 0)
			{
				throw new Exception(errorString);
			}

			if (found == false)
			{
				throw new Exception(errorString);
			}

			return index;
		}

		#endregion
	}
}

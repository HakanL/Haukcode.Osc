using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace Rug.Osc
{
	/// <summary>
	/// Bundle of osc messages
	/// </summary>
	public sealed class OscBundle : OscPacket, IEnumerable<OscMessage>
	{
		#region Private Members

		internal const string BundleIdent = "#bundle";
		internal const int BundleHeaderSizeInBytes = 16; 

		private OscTimeTag m_Timestamp;
		private OscMessage[] m_Messages;

		private OscPacketError m_Error;
		private string m_ErrorMessage;

		#endregion

		#region Public Properties

		/// <summary>
		/// If anything other than OscPacketError.None then an error occured while the packet was being parsed
		/// </summary>
		public override OscPacketError Error { get { return m_Error; } }

		/// <summary>
		/// The descriptive string associated with Error
		/// </summary>
		public override string ErrorMessage { get { return m_ErrorMessage; } } 

		/// <summary>
		/// Osc timestamp associated with this bundle
		/// </summary>
		public OscTimeTag Timestamp { get { return m_Timestamp; } }

		/// <summary>
		/// Access bundle messages by index 
		/// </summary>
		/// <param name="index">the index of the message</param>
		/// <returns>message at the supplied index</returns>
		public OscMessage this[int index] { get { return m_Messages[index]; } }

		/// <summary>
		/// The number of messages in the bundle
		/// </summary>
		public int Count { get { return m_Messages.Length; } }

		/// <summary>
		/// The size of the packet in bytes
		/// </summary>
		public override int SizeInBytes
		{
			get
			{
				int size = BundleHeaderSizeInBytes;

				foreach (OscMessage message in this)
				{
					size += 4 + message.SizeInBytes; 
				}

				return size; 
			}
		}

		#endregion

		/// <summary>
		/// Create a bundle of messages
		/// </summary>
		/// <param name="timestamp">timestamp</param>
		/// <param name="messages">messages to bundle</param>
		public OscBundle(OscTimeTag timestamp, params OscMessage[] messages)
		{
			m_Timestamp = timestamp;
			m_Messages = messages;
		}

		/// <summary>
		/// Create a bundle of messages
		/// </summary>
		/// <param name="timestamp">timestamp</param>
		/// <param name="messages">messages to bundle</param>
		public OscBundle(ulong timestamp, params OscMessage[] messages)
		{
			m_Timestamp = new OscTimeTag(timestamp); 
			m_Messages = messages;
		}

		/// <summary>
		/// Create a bundle of messages
		/// </summary>
		/// <param name="timestamp">timestamp</param>
		/// <param name="messages">messages to bundle</param>
		public OscBundle(DateTime timestamp, params OscMessage[] messages)
		{
			m_Timestamp = OscTimeTag.FromDataTime(timestamp);
			m_Messages = messages;
		}

		private OscBundle() { }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(BundleIdent);
			sb.Append(", ");
			sb.Append(Timestamp.ToString()); 

			foreach (OscMessage message in this)
			{
				sb.Append(", { "); 
				sb.Append(message.ToString());
				sb.Append(" }");
			}

			return sb.ToString();
		}

		#region Enumerable

		public IEnumerator<OscMessage> GetEnumerator()
		{
			return (m_Messages as IEnumerable<OscMessage>).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (m_Messages as System.Collections.IEnumerable).GetEnumerator();
		}

		#endregion

		#region To Array

		public OscMessage[] ToArray()
		{
			return m_Messages; 
		}

		#endregion

		#region To Byte Array

		/// <summary>
		/// Creates a byte array that contains the osc message
		/// </summary>
		/// <returns></returns>
		public override byte[] ToByteArray()
		{
			byte[] data = new byte[SizeInBytes];

			Write(data);

			return data;
		}

		#endregion

		#region Write
		
		/// <summary>
		/// Write the bundle into a byte array 
        /// </summary>
		/// <param name="data">an array ouf bytes to write the bundle into</param>
        /// <returns>the number of bytes in the message</returns>
		public override int Write(byte[] data)
		{
			return Write(data, 0); 
		}

		/// <summary>
		/// Write the bundle into a byte array 
        /// </summary>
        /// <param name="data">an array ouf bytes to write the bundle into</param>
		/// <param name="index">the index within the array where writing should begin</param>
        /// <returns>the number of bytes in the message</returns>
		public override int Write(byte[] data, int index)
		{
			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				stream.Position = index;

				writer.Write(Encoding.UTF8.GetBytes(BundleIdent));
				writer.Write((byte)0); 

				Helper.Write(writer, m_Timestamp);

				foreach (OscMessage message in this)
				{
					Helper.Write(writer, message.SizeInBytes);

					stream.Position += message.Write(data, (int)stream.Position);
				}

				return (int)stream.Position - index;
			}
		}

		#endregion

		#region Is Bundle

		/// <summary>
		/// Does the array contain a bundle packet? 
		/// </summary>
		/// <param name="bytes">the array that contains a packet</param>
		/// <param name="index">the offset within the array where the packet starts</param>
		/// <param name="count">the number of bytes in the packet</param>
		/// <returns>true if the packet contains a valid bundle header</returns>
		public static bool IsBundle(byte[] bytes, int index, int count)
		{
			if (count < BundleHeaderSizeInBytes)
			{
				return false; 
			}

			string ident = Encoding.UTF8.GetString(bytes, index, BundleIdent.Length);
			
			if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
			{
				return false; 
			}

			return true; 
		}

		#endregion

		#region Read

		/// <summary>
		/// Read a OscBundle from a array of bytes
		/// </summary>
		/// <param name="bytes">the array that countains the bundle</param>
		/// <param name="count">the number of bytes in the bundle</param>
		/// <returns>the bundle</returns>
		public static new OscBundle Read(byte[] bytes, int count)
		{
			return Read(bytes, 0, count);
		}

		/// <summary>
		/// Read a OscBundle from a array of bytes
		/// </summary>
		/// <param name="bytes">the array that countains the bundle</param>
		/// <param name="index">the offset within the array where reading should begin</param>
		/// <param name="count">the number of bytes in the bundle</param>
		/// <returns>the bundle</returns>
		public static new OscBundle Read(byte[] bytes, int index, int count)
		{
			OscBundle bundle = new OscBundle();

			List<OscMessage> messages = new List<OscMessage>(); 

			using (MemoryStream stream = new MemoryStream(bytes, index, count))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				if (stream.Length < BundleHeaderSizeInBytes)
				{
					// this is an error 
					bundle.m_Error = OscPacketError.MissingBundleIdent;
					bundle.m_ErrorMessage = Strings.Bundle_MissingIdent;

					bundle.m_Messages = new OscMessage[0]; 

					return bundle;
				}

				string ident = Encoding.UTF8.GetString(bytes, index, BundleIdent.Length);

				if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
				{
					// this is an error 
					bundle.m_Error = OscPacketError.InvalidBundleIdent;
					bundle.m_ErrorMessage = String.Format(Strings.Bundle_InvalidIdent, ident);

					bundle.m_Messages = new OscMessage[0]; 

					return bundle;
				}

				stream.Position = BundleIdent.Length + 1;

				bundle.m_Timestamp = Helper.ReadOscTimeTag(reader);

				while (stream.Position < stream.Length)
				{
					if (stream.Position + 4 > stream.Length)
					{
						// this is an error
						bundle.m_Error = OscPacketError.InvalidBundleMessageHeader;
						bundle.m_ErrorMessage = Strings.Bundle_InvalidMessageHeader;

						bundle.m_Messages = new OscMessage[0]; 

						return bundle;
					}

					int messageLength = Helper.ReadInt32(reader);

					messages.Add(OscMessage.Read(bytes, index + (int)stream.Position, messageLength));

					stream.Position += messageLength; 
				}

				bundle.m_Messages = messages.ToArray(); 
			}

			return bundle;
		}

		#endregion

		#region Parse

		/// <summary>
		/// Try to parse a bundle from a string using the InvariantCulture
		/// </summary>
		/// <param name="str">the bundle as a string</param>
		/// <param name="bundle">the parsed bundle</param>
		/// <returns>true if the bundle could be parsed else false</returns>
		public static bool TryParse(string str, out OscBundle bundle)
		{
			try
			{
				bundle = Parse(str, CultureInfo.InvariantCulture);

				return true;
			}
			catch
			{
				bundle = null;

				return false;
			}
		}

		/// <summary>
		/// Try to parse a bundle from a string using a supplied format provider
		/// </summary>
		/// <param name="str">the bundle as a string</param>
		/// <param name="provider">the format provider to use</param>
		/// <param name="bundle">the parsed bundle</param>
		/// <returns>true if the bundle could be parsed else false</returns>
		public static bool TryParse(string str, IFormatProvider provider, out OscBundle bundle)
		{
			try
			{
				bundle = Parse(str, provider);

				return true;
			}
			catch
			{
				bundle = null;

				return false;
			}
		}

		/// <summary>
		/// Parse a bundle from a string using the InvariantCulture
		/// </summary>
		/// <param name="str">a string containing a bundle</param>
		/// <returns>the parsed bundle</returns>
		public static new OscBundle Parse(string str)
		{
			return Parse(str, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// parse a bundle from a string using a supplied format provider
		/// </summary>
		/// <param name="str">a string containing a bundle</param>
		/// <param name="provider">the format provider to use</param>
		/// <returns>the parsed bundle</returns>
		public static OscBundle Parse(string str, IFormatProvider provider)
		{
			if (Helper.IsNullOrWhiteSpace(str) == true)
			{
				throw new ArgumentNullException("str");
			}

			int start = 0; 

			int end = str.IndexOf(',', start);

			if (end <= start)
			{
				throw new Exception(String.Format(Strings.Bundle_InvalidIdent, ""));
			}

			string ident = str.Substring(start, end - start).Trim();

			if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
			{
				throw new Exception(String.Format(Strings.Bundle_InvalidIdent, ident));
			}

			start = end + 1; 

			end = str.IndexOf(',', start);

			if (end <= start)
			{
				throw new Exception(String.Format(Strings.Bundle_InvalidTimestamp, ""));
			}

			string timeStampStr = str.Substring(start, end - start);

			OscTimeTag timeStamp = OscTimeTag.Parse(timeStampStr.Trim(), provider);

			start = end + 1;

			end = str.IndexOf('{', start);

			if (end < 0)
			{
				end = str.Length; 			
			}

			string gap = str.Substring(start, end - start);

			if (Helper.IsNullOrWhiteSpace(gap) == false)
			{
				throw new Exception(String.Format(Strings.Bundle_MissingOpenBracket, gap));
			}

			start = end; 

			List<OscMessage> messages = new List<OscMessage>();

			while (start > 0 && start < str.Length)
			{
				end = ScanForward_Object(str, start);

				messages.Add(OscMessage.Parse(str.Substring(start + 1, end - (start + 1)).Trim(), provider));

				start = end + 1; 

				end = str.IndexOf('{', start);

				if (end < 0)
				{
					end = str.Length;
				}

				gap = str.Substring(start, end - start).Trim();

				if (gap.Equals(",") == false && Helper.IsNullOrWhiteSpace(gap) == false)
				{
					throw new Exception(String.Format(Strings.Bundle_MissingOpenBracket, gap));
				}

				start = end; 
			}

			return new OscBundle(timeStamp, messages.ToArray()); 
		}

		#endregion
	}
}

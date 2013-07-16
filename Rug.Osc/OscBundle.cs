using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rug.Osc
{
	public sealed class OscBundle : OscPacket, IEnumerable<OscMessage>
	{
		private const string BundleIdent = "#bundle";
		private const int BundleHeaderSizeInBytes = 16; 

		private OscTimeTag m_Timestamp;
		private OscMessage[] m_Messages;

		private OscPacketError m_Error;
		private string m_ErrorMessage; 

		public override OscPacketError Error { get { return m_Error; } }

		public override string ErrorMessage { get { return m_ErrorMessage; } } 

		public OscTimeTag Timestamp { get { return m_Timestamp; } }

		public OscMessage this[int index] { get { return m_Messages[index]; } }

		public int Count { get { return m_Messages.Length; } }

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

		public OscBundle(OscTimeTag timestamp, params OscMessage[] messages)
		{
			m_Timestamp = timestamp; 
			m_Messages = messages;
		}

		private OscBundle()
		{
		}

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

		public IEnumerator<OscMessage> GetEnumerator()
		{
			return (m_Messages as IEnumerable<OscMessage>).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (m_Messages as System.Collections.IEnumerable).GetEnumerator();
		}

		public OscMessage[] ToArray()
		{
			return m_Messages; 
		}

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

		public static OscBundle Read(byte[] bytes, int index, int count)
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
					bundle.m_ErrorMessage = Strings.Bundle_MissingBundleIdent;

					bundle.m_Messages = new OscMessage[0]; 

					return bundle;
				}

				string ident = Encoding.UTF8.GetString(bytes, index, BundleIdent.Length);

				if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
				{
					// this is an error 
					bundle.m_Error = OscPacketError.InvalidBundleIdent;
					bundle.m_ErrorMessage = String.Format(Strings.Bundle_InvalidBundleIdent, ident);

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
						bundle.m_ErrorMessage = Strings.Bundle_InvalidBundleMessageHeader;

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
	}
}

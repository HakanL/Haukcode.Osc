using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rug.Osc
{
	/// <summary>
	/// Reads osc packets from a stream
	/// </summary>
	public sealed class OscReader : IDisposable
	{
		#region Private Members

		private Stream m_Stream;
		private OscPacketFormat m_Format;
		private BinaryReader m_BinaryReader;
		private StreamReader m_StringReader;

		#endregion 

		#region Properties

		/// <summary>
		/// Exposes access to the underlying stream of the OscReader.
		/// </summary>
		public Stream BaseStream { get { return m_Stream; } }

		/// <summary>
		/// Packet format 
		/// </summary>
		public OscPacketFormat Format { get { return m_Format; } }

		/// <summary>
		/// Gets a value that indicates whether the current stream position is at the end of the stream.
		/// </summary>
		public bool EndOfStream 
		{ 
			get 
			{
				if (BaseStream.CanRead == false)
				{
					return false;
				}

				if (m_Format == OscPacketFormat.Binary)
				{
					return BaseStream.Position < BaseStream.Length; 
				}
				else
				{
					return m_StringReader.EndOfStream;
				}				
			} 
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the OscReader class based on the supplied stream. 
		/// </summary>
		/// <param name="stream">a stream</param>
		/// <param name="mode">the format of the packets in the stream</param>
		public OscReader(Stream stream, OscPacketFormat mode)
		{
			m_Stream = stream;
			m_Format = mode;
		
			if (m_Format == OscPacketFormat.Binary)
			{
				m_BinaryReader = new BinaryReader(m_Stream);
			}
			else
			{
				m_StringReader = new StreamReader(m_Stream);
			}
		}

		#endregion

		#region Read

		/// <summary>
		/// Read a single packet from the stream at the current position
		/// </summary>
		/// <returns>An osc packet</returns>
		public OscPacket Read()
		{
			if (Format == OscPacketFormat.Binary)
			{				
				int length = Helper.ReadInt32(m_BinaryReader);

				byte[] bytes = new byte[length]; 

				m_BinaryReader.Read(bytes, 0, length);

				return OscPacket.Read(bytes, length); 
			}
			else
			{
				string line = m_StringReader.ReadLine();
				OscPacket packet;

				if (OscPacket.TryParse(line, out packet) == false)
				{
					return OscMessage.ParseError; 
				}

				return packet; 
			}
		}

		#endregion 

		#region Close

		/// <summary>
		/// Closes the current reader and the underlying stream.
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		/// <summary>
		/// Disposes the current reader and the underlying stream.
		/// </summary>
		public void Dispose()
		{
			if (m_Format == OscPacketFormat.Binary)
			{
				m_BinaryReader.Close();
			}
			else
			{
				m_StringReader.Close();
			}
		}
	
		#endregion
	}
}

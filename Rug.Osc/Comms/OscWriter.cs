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

using System;
using System.IO;

namespace Rug.Osc
{
	/// <summary>
	/// Writes osc packets to a stream
	/// </summary>
	public sealed class OscWriter : IDisposable
	{
		#region Private Memebers

		private readonly Stream m_Stream;
		private readonly BinaryWriter m_BinaryWriter;
		private readonly StreamWriter m_StringWriter;

		private readonly OscPacketFormat m_Format;

		#endregion

		#region Properties

		/// <summary>
		/// Exposes access to the underlying stream of the OscWriter.
		/// </summary>
		public Stream BaseStream { get { return m_Stream; } }

		/// <summary>
		/// Packet format 
		/// </summary>
		public OscPacketFormat Format { get { return m_Format; } }

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the OscWriter class based on the supplied stream. 
		/// </summary>
		/// <param name="stream">a stream</param>
		/// <param name="format">packet format</param>
		public OscWriter(Stream stream, OscPacketFormat format)
		{
			m_Stream = stream;
			m_Format = format;

			if (m_Format == OscPacketFormat.Binary)
			{
				m_BinaryWriter = new BinaryWriter(m_Stream);
			}
			else
			{
				m_StringWriter = new StreamWriter(m_Stream); 
			}
		}

		#endregion 

		#region Write

		/// <summary>
		/// Writes a single packet to the stream at the current position.
		/// </summary>
		/// <param name="packet">A osc packet</param>
		public void Write(OscPacket packet)
		{
			if (Format == OscPacketFormat.Binary)
			{
				byte[] bytes = packet.ToByteArray();

				// write the length
				Helper.Write(m_BinaryWriter, bytes.Length);

				// write the packet
				m_BinaryWriter.Write(bytes); 
			}
			else
			{
				// write as a string
				m_StringWriter.WriteLine(packet.ToString()); 
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
				m_BinaryWriter.Close();
			}
			else
			{
				m_StringWriter.Close();
			}
		}

		#endregion
	}
}

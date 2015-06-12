/* 
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Rug.Osc.Slip
{
	public class SlipPacketReader
	{
		private byte[] m_Buffer;
		private int m_Index;

		public int BufferSize { get { return m_Buffer.Length; } }

		public SlipPacketReader(int bufferSize)
		{
			m_Buffer = new byte[bufferSize]; 
		}

		public void Clear()
		{
			m_Index = 0; 
		}

		public int ProcessBytes(byte[] bytes, int index, int count, ref byte[] packetBytes, out int packetLength)
		{
			packetLength = 0; 

			for (int i = 0; i < count; i++)
			{
				byte @byte = bytes[index + i];
				
				// Add byte to buffer
				m_Buffer[m_Index] = @byte;

				// Increment index with overflow
				if (++m_Index >= m_Buffer.Length)
				{
					m_Index = 0;
				}

				// Decode packet if END byte
				if (@byte == (byte)SlipBytes.End) 
				{
					m_Index = 0;

					packetLength = ProcessPacket(ref packetBytes);
					
					return i + 1; 
				}
			}

			return count; 
		}

		private int ProcessPacket(ref byte[] packet)
		{
			int i = 0;

			int packetLength = 0;
			packet = new byte[m_Buffer.Length];

			while (m_Buffer[i] != (byte)SlipBytes.End)
			{
				if (m_Buffer[i] == (byte)SlipBytes.Escape)
				{
					switch (m_Buffer[++i])
					{
						case (byte)SlipBytes.EscapeEnd:
							packet[packetLength++] = (byte)SlipBytes.End;
							break;
						case (byte)SlipBytes.EscapeEscape:
							packet[packetLength++] = (byte)SlipBytes.Escape;
							break;
						default:
							return 0; // error: unexpected byte value
					}
				}
				else
				{
					packet[packetLength++] = m_Buffer[i];
				}

				if (packetLength > packet.Length)
				{
					return 0; // error: decoded packet too large
				}

				i++;
			}

			return packetLength;
		}
	}
}

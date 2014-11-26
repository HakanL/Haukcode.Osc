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

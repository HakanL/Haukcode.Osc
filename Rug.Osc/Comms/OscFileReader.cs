using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Rug.Osc
{
	public class OscFileReader : IDisposable, IOscPacketReceiver
	{
		private FileStream m_File;
		private OscReader m_Reader;

		public event OscPacketEvent PacketRecived;

		public OscCommunicationStatistics Statistics { get; set; } 

		public bool EndOfStream { get { return m_Reader.EndOfStream; } }

		public long Position { get { return m_Reader.BaseStream.Position; } }

		public long Length { get { return m_Reader.BaseStream.Length; } }

		public OscFileReader(string filePath, OscPacketFormat format)
		{
			m_File = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			m_Reader = new OscReader(m_File, format);
		}

		public void ReadToEnd()
		{
			while (EndOfStream == false)
			{
				Read(); 
			}
		}

		public OscPacket Read()
		{
			long position = m_Reader.BaseStream.Position; 
			
			OscPacket packet = m_Reader.Read();

			long newPosition = m_Reader.BaseStream.Position;
            
			if (Statistics != null)
			{
				Statistics.BytesReceived.Increment((int)(newPosition - position));
			}

			if (Statistics != null && packet.Error != OscPacketError.None)
			{
				Statistics.ReceiveErrors.Increment(1);
			}

			if (PacketRecived != null)
			{
				PacketRecived(packet);
			}

			return packet; 
		}

		public void Dispose()
		{
			m_Reader.Dispose();
			m_File.Close();
			m_File.Dispose(); 
		}
	}
}

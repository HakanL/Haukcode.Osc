using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rug.Osc
{
	public class OscFileWriter : IDisposable, IOscPacketSender
	{
		private FileStream m_File;
		private OscWriter m_Writer;

		public OscCommunicationStatistics Statistics { get; set; }

		public OscFileWriter(string filePath, FileMode fileMode, OscPacketFormat format)
		{
			m_File = new FileStream(filePath, fileMode, FileAccess.Write); 			
			m_Writer = new OscWriter(m_File, format); 
		}

		public void Send(OscPacket packet)
		{
			if (Statistics != null)
			{
				packet.IncrementSendStatistics(Statistics);
			}

			m_Writer.Write(packet);
			m_File.Flush(); 
		}

		public void Dispose()
		{
			m_Writer.Dispose();
			m_File.Close();
			m_File.Dispose(); 
		}
	}
}

using System;
using System.IO;

namespace Rug.Osc
{
    public class OscFileWriter : IDisposable, IOscPacketSender
	{
		private readonly FileStream file;
		private readonly OscWriter writer;

		public OscCommunicationStatistics Statistics { get; set; }

		public OscFileWriter(string filePath, FileMode fileMode, OscPacketFormat format)
		{
			file = new FileStream(filePath, fileMode, FileAccess.Write); 			
			writer = new OscWriter(file, format); 
		}

		public void Send(OscPacket packet)
		{
			if (Statistics != null)
			{
				packet.IncrementSendStatistics(Statistics);
			}

			writer.Write(packet);
			file.Flush(); 
		}

		public void Dispose()
		{
			writer.Dispose();
			file.Close();
			file.Dispose(); 
		}
	}
}

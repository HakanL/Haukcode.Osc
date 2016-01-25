﻿using System;
using System.IO;

namespace Rug.Osc
{
    public class OscFileReader : IDisposable, IOscPacketReceiver
	{
		FileStream file;
		OscReader reader;

		public event OscPacketEvent PacketRecived;

		public OscCommunicationStatistics Statistics { get; set; } 

		public bool EndOfStream { get { return reader.EndOfStream; } }

		public long Position { get { return reader.BaseStream.Position; } }

		public long Length { get { return reader.BaseStream.Length; } }

		public OscFileReader(string filePath, OscPacketFormat format)
		{
			file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			reader = new OscReader(file, format);
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
			long position = reader.BaseStream.Position; 
			
			OscPacket packet = reader.Read();

			long newPosition = reader.BaseStream.Position;
            
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
			reader.Dispose();
			file.Close();
			file.Dispose(); 
		}
	}
}

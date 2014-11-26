using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Rug.Osc.Slip
{
	public class SlipPacketWriter
	{
		public SlipPacketWriter() { }

		public byte[] Write(byte[] bytes, int index, int count)
		{
			using (MemoryStream stream = new MemoryStream(count))
			{
				for (int i = 0; i < count; i++)
				{
					switch (bytes[index + i])
					{
						case (byte)SlipBytes.End:
							stream.WriteByte((byte)SlipBytes.Escape);
							stream.WriteByte((byte)SlipBytes.EscapeEnd);
							break;
						case (byte)SlipBytes.Escape:
							stream.WriteByte((byte)SlipBytes.Escape);
							stream.WriteByte((byte)SlipBytes.EscapeEscape);
							break;
						default:
							stream.WriteByte(bytes[index + i]);
							break;
					}
				}

				stream.WriteByte((byte)SlipBytes.End);

				return stream.ToArray();
			}
		}
	}
}

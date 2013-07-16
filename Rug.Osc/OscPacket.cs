using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc
{
	public abstract class OscPacket
	{
		public abstract int SizeInBytes { get; }

		public abstract OscPacketError Error { get; }

		public abstract string ErrorMessage { get; } 

		public abstract byte[] ToByteArray();

		public abstract int Write(byte[] data);

		public abstract int Write(byte[] data, int index);

		public static OscPacket Read(byte[] bytes, int count)
		{
			return Read(bytes, 0, count);
		}

		public static OscPacket Read(byte[] bytes, int index, int count)
		{
			if (OscBundle.IsBundle(bytes, index, count) == true)
			{
				return OscBundle.Read(bytes, index, count); 
			}
			else
			{
				return OscMessage.Read(bytes, index, count); 
			}
		}
	}
}

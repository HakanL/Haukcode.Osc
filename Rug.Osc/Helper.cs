using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rug.Osc
{
	internal static class Helper
	{
		private static byte[] m_Padding = new byte[] { 0, 0, 0, 0 }; 

		internal static void Write(System.IO.BinaryWriter writer, int value)
		{
			byte[] valueBytes = BitConverter.GetBytes(value);

			writer.Write(valueBytes[3]);
			writer.Write(valueBytes[2]);
			writer.Write(valueBytes[1]);
			writer.Write(valueBytes[0]);
		}

		internal static void Write(System.IO.BinaryWriter writer, float value)
		{
			byte[] valueBytes = BitConverter.GetBytes(value);

			writer.Write(valueBytes[3]);
			writer.Write(valueBytes[2]);
			writer.Write(valueBytes[1]);
			writer.Write(valueBytes[0]);
		}

		internal static void WritePadding(System.IO.BinaryWriter writer, long position)
		{
			int nullCount = 4 - (int)(position % 4);

			if (nullCount < 4)
			{
				writer.Write(m_Padding, 0, nullCount); 
			}
		}
	}
}

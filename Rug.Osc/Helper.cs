using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Rug.Osc
{
	internal static class Helper
	{
		#region Private Static Members

		private static byte[] m_Padding = new byte[] { 0, 0, 0, 0 };

		#endregion

		#region Is Null Or White Space

		public static bool IsNullOrWhiteSpace(string str) 
		{
			if (str == null)
			{
				return true; 
			}

			if (String.IsNullOrEmpty(str.Trim()) == true)
			{
				return true; 
			}

			return false; 
		}

		#endregion

		#region UInt to Float Conversion Helper

		/// <summary>
		/// UInt to Float Conversion Helper 
		/// http://stackoverflow.com/questions/8037645/cast-float-to-int-without-any-conversion
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatAndUIntUnion
		{
			[FieldOffset(0)]
			public uint UInt32Bits;
			[FieldOffset(0)]
			public float FloatValue;
		}

		#endregion

		#region Byte

		internal static void Write(System.IO.BinaryWriter writer, byte value)
		{
			writer.Write(value);
			writer.Write((byte)0);
			writer.Write((byte)0);
			writer.Write((byte)0);
		}

		internal static byte ReadByte(BinaryReader reader)
		{
			byte value = reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte(); 

			return value;
		}

		#endregion

		#region Int 32

		internal static void Write(System.IO.BinaryWriter writer, int value)
		{
			uint allBits = unchecked((uint)value);

			Write(writer, allBits);								
		}

		internal static int ReadInt32(System.IO.BinaryReader reader)
		{
			uint value = ReadUInt32(reader);

			return unchecked((int)value);
		}

		#endregion

		#region UInt 32

		internal static void Write(System.IO.BinaryWriter writer, uint value)
		{
			value = unchecked((value & 0xFF000000) >> 24 |
							   (value & 0x00FF0000) >> 8 |
							   (value & 0x0000FF00) << 8 |
							   (value & 0x000000FF) << 24);

			writer.Write(value); 
		}

		internal static uint ReadUInt32(System.IO.BinaryReader reader)
		{
			uint value = reader.ReadUInt32();
			value = unchecked((value & 0xFF000000) >> 24 |
							   (value & 0x00FF0000) >> 8 |
							   (value & 0x0000FF00) << 8 |
							   (value & 0x000000FF) << 24);

			return value;
		}

		#endregion

		#region Single (float)

		internal static void Write(System.IO.BinaryWriter writer, float value)
		{
			FloatAndUIntUnion v = default(FloatAndUIntUnion);

			v.FloatValue = value; 

			Write(writer, v.UInt32Bits); 
		}

		internal static float ReadSingle(System.IO.BinaryReader reader)
		{
			FloatAndUIntUnion v = default(FloatAndUIntUnion);

			v.UInt32Bits = ReadUInt32(reader);

			return v.FloatValue; 
		}

		#endregion

		#region Int 64

		internal static void Write(System.IO.BinaryWriter writer, long value)
		{
			ulong allBits = unchecked((ulong)value);

			Write(writer, allBits);
		}

		internal static long ReadInt64(System.IO.BinaryReader reader)
		{
			ulong value = ReadUInt64(reader);

			return unchecked((long)value);
		}

		#endregion

		#region Uint 64

		internal static void Write(System.IO.BinaryWriter writer, ulong value)
		{
			value = unchecked((value & 0xFF00000000000000) >> 56 |
							   (value & 0x00FF000000000000) >> 40 |
							   (value & 0x0000FF0000000000) >> 24 |
							   (value & 0x000000FF00000000) >> 8 |
							   (value & 0x00000000FF000000) << 8 |
							   (value & 0x0000000000FF0000) << 24 |
							   (value & 0x000000000000FF00) << 40 |
							   (value & 0x00000000000000FF) << 56);

			writer.Write(value); 
		}

		internal static ulong ReadUInt64(System.IO.BinaryReader reader)
		{
			ulong value = reader.ReadUInt64();
			value = unchecked((value & 0xFF00000000000000) >> 56 |
							   (value & 0x00FF000000000000) >> 40 |
							   (value & 0x0000FF0000000000) >> 24 |
							   (value & 0x000000FF00000000) >> 8 |
							   (value & 0x00000000FF000000) << 8 |
							   (value & 0x0000000000FF0000) << 24 |
							   (value & 0x000000000000FF00) << 40 |
							   (value & 0x00000000000000FF) << 56);

			return value;
		}

		#endregion

		#region Double

		internal static void Write(System.IO.BinaryWriter writer, double value)
		{
			long setofBits = BitConverter.DoubleToInt64Bits(value);

			ulong allBits = unchecked((ulong)setofBits);

			Write(writer, allBits);
		}

		internal static double ReadDouble(System.IO.BinaryReader reader)
		{
			ulong value = ReadUInt64(reader);

			return BitConverter.Int64BitsToDouble(unchecked((long)value));
		}

		#endregion

		#region Color

		internal static void Write(BinaryWriter writer, Color value)
		{
			uint intValue = unchecked((uint)(
						((byte)value.R << 24) |
						((byte)value.G << 16) |
						((byte)value.B << 8) |
						((byte)value.A << 0)));

			Write(writer, intValue);
		}

		internal static Color ReadColor(System.IO.BinaryReader reader)
		{			
			uint value = ReadUInt32(reader);

			byte a, r, g, b;

			r = (byte)((value & 0xFF000000) >> 24);
			g = (byte)((value & 0x00FF0000) >> 16);
			b = (byte)((value & 0x0000FF00) >> 8);
			a = (byte)(value & 0x000000FF);

			return Color.FromArgb(a, r, g, b);
		}

		#endregion

		#region OscTimeTag

		internal static void Write(BinaryWriter writer, OscTimeTag value)
		{
			Write(writer, value.Value); 
		}

		internal static OscTimeTag ReadOscTimeTag(System.IO.BinaryReader reader)
		{
			ulong value = ReadUInt64(reader);

			return new OscTimeTag(value);
		}

		#endregion

		#region OscMidiMessage

		internal static void Write(BinaryWriter writer, OscMidiMessage value)
		{
			Write(writer, value.FullMessage); 
		}

		internal static OscMidiMessage ReadOscMidiMessage(System.IO.BinaryReader reader)
		{
			uint value = ReadUInt32(reader);

			return new OscMidiMessage(value);
		}

		#endregion

		#region Padding

		internal static void WritePadding(System.IO.BinaryWriter writer, long position)
		{
			int nullCount = 4 - (int)(position % 4);

			if (nullCount < 4)
			{
				writer.Write(m_Padding, 0, nullCount); 
			}
		}

		internal static bool SkipPadding(Stream stream)
		{
			if (stream.Position % 4 != 0)
			{
				long newPosition = stream.Position + (4 - (stream.Position % 4));

				// this shouldn't happen and means we're decoding rubbish
				if (newPosition > stream.Length)
				{
					return false;
				}

				stream.Position = newPosition;
			}

			return true;
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc
{
	public struct OscColor
	{
		private int Value;

		public byte R
		{
			get
			{
				return (byte)((this.Value >> 0x10) & 0xff);
			}
		}

		public byte G
		{
			get
			{
				return (byte)((this.Value >> 8) & 0xff);
			}
		}

		public byte B
		{
			get
			{
				return (byte)(this.Value & 0xff);
			}
		}

		public byte A
		{
			get
			{
				return (byte)((this.Value >> 0x18) & 0xff);
			}
		}

		public OscColor(int value) 
		{
			Value = value; 
		}

		private static int MakeArgb(byte alpha, byte red, byte green, byte blue)
		{
			return unchecked((int)(((uint)((((red << 0x10) | (green << 8)) | blue) | (alpha << 0x18))) & 0xffffffff));
		}

		public static OscColor FromArgb(int argb)
		{
			return new OscColor(unchecked(argb & ((int)0xffffffff)));
		}
		
		private static void CheckByte(int value, string name)
		{
			if ((value < 0) || (value > 0xff))
			{
				// throw new ArgumentException(System.Drawing.SR.GetString("InvalidEx2BoundArgument", new object[] { name, value, 0, 0xff }));
				throw new Exception(); 
			}
		}

		public static OscColor FromArgb(int alpha, int red, int green, int blue)
		{
			CheckByte(alpha, "alpha");
			CheckByte(red, "red");
			CheckByte(green, "green");
			CheckByte(blue, "blue");
			return new OscColor(MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue));
		}
	}
}

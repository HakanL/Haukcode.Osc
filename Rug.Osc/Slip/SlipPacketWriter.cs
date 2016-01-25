/* 
 * Rug.Osc 
 * 
 * Copyright (C) 2013 Phill Tew (peatew@gmail.com)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 * 
 */

using System.IO;

namespace Rug.Osc.Slip
{
    public static class SlipPacketWriter
	{
		//public SlipPacketWriter() { }

		public static byte[] Write(byte[] bytes, int index, int count)
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

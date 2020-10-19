/*
 * Haukcode.Osc
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

namespace Haukcode.Osc.Slip
{
    public class SlipPacketReader
    {
        private byte[] buffer;
        private int index;

        public int BufferSize => buffer.Length;

        public SlipPacketReader(int bufferSize)
        {
            buffer = new byte[bufferSize];
        }

        public void Clear()
        {
            index = 0;
        }

        public int ProcessBytes(byte[] bytes, int index, int count, ref byte[] packetBytes, out int packetLength)
        {
            packetLength = 0;

            for (int i = 0; i < count; i++)
            {
                byte @byte = bytes[index + i];

                // Add byte to buffer
                buffer[this.index] = @byte;

                // Increment index with overflow
                if (++this.index >= buffer.Length)
                {
                    this.index = 0;
                }

                // Decode packet if END byte
                if (@byte == (byte)SlipBytes.End)
                {
                    this.index = 0;

                    packetLength = ProcessPacket(ref packetBytes);

                    return i + 1;
                }
            }

            return count;
        }

        private int ProcessPacket(ref byte[] packet)
        {
            int i = 0;

            int packetLength = 0;
            packet = new byte[buffer.Length];

            while (buffer[i] != (byte)SlipBytes.End)
            {
                if (buffer[i] == (byte)SlipBytes.Escape)
                {
                    switch (buffer[++i])
                    {
                        case (byte)SlipBytes.EscapeEnd:
                            packet[packetLength++] = (byte)SlipBytes.End;
                            break;

                        case (byte)SlipBytes.EscapeEscape:
                            packet[packetLength++] = (byte)SlipBytes.Escape;
                            break;

                        default:
                            return 0; // error: unexpected byte value
                    }
                }
                else
                {
                    packet[packetLength++] = buffer[i];
                }

                if (packetLength > packet.Length)
                {
                    return 0; // error: decoded packet too large
                }

                i++;
            }

            return packetLength;
        }
    }
}
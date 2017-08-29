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

using System;
using System.IO;
using System.Text;

namespace Rug.Osc
{
    /// <summary>
    /// Reads osc packets from a stream
    /// </summary>
    public sealed class OscReader : IDisposable
    {
        private readonly BinaryReader binaryReader;
        private readonly byte[] slipByteCache;
        private readonly Slip.SlipPacketReader slipReader;
        private readonly StreamReader stringReader;
        private int slipByteCount;
        private int slipByteIndex;

        /// <summary>
        /// Exposes access to the underlying stream of the OscReader.
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        /// Gets a value that indicates whether the current stream position is at the end of the stream.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                if (BaseStream.CanRead == false)
                {
                    return true;
                }

                switch (Format)
                {
                    case OscPacketFormat.Binary:
                        return BaseStream.Position >= BaseStream.Length;

                    case OscPacketFormat.Slip:
                        return BaseStream.Position >= BaseStream.Length && slipByteIndex >= slipByteCount;

                    case OscPacketFormat.String:
                        return stringReader.EndOfStream;

                    default:
                        throw new Exception($@"Invalid OSC stream format ""{Format}"".");
                }
            }
        }

        /// <summary>
        /// Packet format
        /// </summary>
        public OscPacketFormat Format { get; }

        /// <summary>
        /// Initializes a new instance of the OscReader class based on the supplied stream.
        /// </summary>
        /// <param name="stream">a stream</param>
        /// <param name="mode">the format of the packets in the stream</param>
        public OscReader(Stream stream, OscPacketFormat mode)
        {
            this.BaseStream = stream;
            Format = mode;

            switch (Format)
            {
                case OscPacketFormat.Binary:
                    binaryReader = new BinaryReader(this.BaseStream);
                    break;

                case OscPacketFormat.Slip:
                    binaryReader = new BinaryReader(this.BaseStream);
                    slipReader = new Slip.SlipPacketReader(1024);

                    slipByteCache = new byte[1024];
                    slipByteIndex = 0;
                    slipByteCount = 0;
                    break;

                case OscPacketFormat.String:
                    stringReader = new StreamReader(this.BaseStream);
                    break;

                default:
                    throw new Exception($@"Invalid OSC stream format ""{Format}"".");
            }
        }

        /// <summary>
        /// Closes the current reader and the underlying stream.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes the current reader and the underlying stream.
        /// </summary>
        public void Dispose()
        {
            binaryReader?.Close();
            stringReader?.Close();
        }

        /// <summary>
        /// Read a single packet from the stream at the current position
        /// </summary>
        /// <returns>An osc packet</returns>
        public OscPacket Read()
        {
            switch (Format)
            {
                case OscPacketFormat.Binary:
                    int length = Helper.ReadInt32(binaryReader);

                    byte[] bytes = new byte[length];

                    binaryReader.Read(bytes, 0, length);

                    return OscPacket.Read(bytes, length);

                case OscPacketFormat.Slip:
                    byte[] packetBytes = new byte[slipReader.BufferSize];
                    int packetLength = 0;

                    do
                    {
                        if (slipByteCount - slipByteIndex == 0)
                        {
                            slipByteIndex = 0;

                            slipByteCount = binaryReader.Read(slipByteCache, 0, slipByteCache.Length);
                        }

                        slipByteIndex += slipReader.ProcessBytes(slipByteCache, slipByteIndex, slipByteCount - slipByteIndex, ref packetBytes, out packetLength);
                    }
                    while (packetLength <= 0 && EndOfStream == false);

                    return packetLength > 0 ? OscPacket.Read(packetBytes, packetLength) : OscMessage.ParseError;

                case OscPacketFormat.String:
                    string line = stringReader.ReadLine();
                    OscPacket packet;

                    if (OscPacket.TryParse(line, out packet) == false)
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine(line);

                        while (EndOfStream == false)
                        {
                            sb.Append(stringReader.ReadLine());

                            if (OscPacket.TryParse(sb.ToString(), out packet) == true)
                            {
                                return packet;
                            }

                            sb.AppendLine();
                        }

                        return OscMessage.ParseError;
                    }

                    return packet;

                default:
                    throw new Exception($@"Invalid OSC stream format ""{Format}"".");
            }
        }
    }
}
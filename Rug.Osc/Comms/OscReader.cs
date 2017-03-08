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
        #region Private Members

        private readonly Stream stream;
        private readonly OscPacketFormat format;
        private readonly BinaryReader binaryReader;
        private readonly StreamReader stringReader;
        private readonly Slip.SlipPacketReader slipReader;

        private readonly byte[] slipByteCache;
        private int slipByteIndex;
        private int slipByteCount;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Exposes access to the underlying stream of the OscReader.
        /// </summary>
        public Stream BaseStream { get { return stream; } }

        /// <summary>
        /// Packet format
        /// </summary>
        public OscPacketFormat Format { get { return format; } }

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

                if (format == OscPacketFormat.Binary)
                {
                    return BaseStream.Position >= BaseStream.Length;
                }
                else if (format == OscPacketFormat.Slip)
                {
                    return BaseStream.Position >= BaseStream.Length && slipByteIndex >= slipByteCount;
                }
                else
                {
                    return stringReader.EndOfStream;
                }
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the OscReader class based on the supplied stream.
        /// </summary>
        /// <param name="stream">a stream</param>
        /// <param name="mode">the format of the packets in the stream</param>
        public OscReader(Stream stream, OscPacketFormat mode)
        {
            this.stream = stream;
            format = mode;

            if (format == OscPacketFormat.Binary)
            {
                binaryReader = new BinaryReader(this.stream);
            }
            else if (format == OscPacketFormat.Slip)
            {
                binaryReader = new BinaryReader(this.stream);
                slipReader = new Slip.SlipPacketReader(1024);

                slipByteCache = new byte[1024];
                slipByteIndex = 0;
                slipByteCount = 0;
            }
            else
            {
                stringReader = new StreamReader(this.stream);
            }
        }

        #endregion Constructor

        #region Read

        /// <summary>
        /// Read a single packet from the stream at the current position
        /// </summary>
        /// <returns>An osc packet</returns>
        public OscPacket Read()
        {
            if (Format == OscPacketFormat.Binary)
            {
                int length = Helper.ReadInt32(binaryReader);

                byte[] bytes = new byte[length];

                binaryReader.Read(bytes, 0, length);

                return OscPacket.Read(bytes, length);
            }
            else if (Format == OscPacketFormat.Slip)
            {
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

                if (packetLength > 0)
                {
                    return OscPacket.Read(packetBytes, packetLength);
                }
                else
                {
                    return OscMessage.ParseError;
                }
            }
            else
            {
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
            }
        }

        #endregion Read

        #region Close

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
            if (format != OscPacketFormat.String)
            {
                binaryReader.Close();
            }
            else
            {
                stringReader.Close();
            }
        }

        #endregion Close
    }
}
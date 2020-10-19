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

namespace Rug.Osc
{
    /// <summary>
    /// Writes osc packets to a stream
    /// </summary>
    public sealed class OscWriter : IDisposable
    {
        private readonly BinaryWriter binaryWriter;
        private readonly StreamWriter stringWriter;

        /// <summary>
        /// Exposes access to the underlying stream of the OscWriter.
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        /// Packet format
        /// </summary>
        public OscPacketFormat Format { get; }

        /// <summary>
        /// Initializes a new instance of the OscWriter class based on the supplied stream.
        /// </summary>
        /// <param name="stream">a stream</param>
        /// <param name="format">packet format</param>
        public OscWriter(Stream stream, OscPacketFormat format)
        {
            this.BaseStream = stream;
            this.Format = format;

            if (this.Format != OscPacketFormat.String)
            {
                binaryWriter = new BinaryWriter(this.BaseStream);
            }
            else
            {
                stringWriter = new StreamWriter(this.BaseStream);
            }
        }

        /// <summary>
        /// Writes a single packet to the stream at the current position.
        /// </summary>
        /// <param name="packet">A osc packet</param>
        public void Write(OscPacket packet)
        {
            switch (Format)
            {
                case OscPacketFormat.Binary:
                    byte[] bytes = packet.ToByteArray();

                    // write the length
                    Helper.Write(binaryWriter, bytes.Length);

                    // write the packet
                    binaryWriter.Write(bytes);
                    break;
                case OscPacketFormat.Slip:
                    byte[] preSlipBytes = packet.ToByteArray();
                    byte[] slipBytes = Slip.SlipPacketWriter.Write(preSlipBytes, 0, preSlipBytes.Length);

                    // write the packet
                    binaryWriter.Write(slipBytes);
                    break;
                case OscPacketFormat.String:
                    stringWriter.WriteLine(packet.ToString());
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
            if (Format != OscPacketFormat.String)
            {
                binaryWriter.Close();
            }
            else
            {
                stringWriter.Close();
            }
        }
    }
}
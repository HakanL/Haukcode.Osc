﻿/*
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Haukcode.Osc
{
    /// <summary>
    /// Bundle of osc messages
    /// </summary>
    public sealed class OscBundle : OscPacket, IEnumerable<OscPacket>
    {
        internal const int BundleHeaderSizeInBytes = 16;
        internal const string BundleIdent = "#bundle";
        private OscPacketError error;
        private string errorMessage;
        private bool hasHashCode = false;
        private int hashCode;
        private OscPacket[] packets;

        /// <summary>
        /// The number of messages in the bundle
        /// </summary>
        public int Count => packets.Length;

        /// <summary>
        /// If anything other than OscPacketError.None then an error occured while the packet was being parsed
        /// </summary>
        public override OscPacketError Error => error;

        /// <summary>
        /// The descriptive string associated with Error
        /// </summary>
        public override string ErrorMessage => errorMessage;

        /// <summary>
        /// The size of the packet in bytes
        /// </summary>
        public override int SizeInBytes => BundleHeaderSizeInBytes + this.Sum(message => 4 + message.SizeInBytes);

        /// <summary>
        /// Osc timestamp associated with this bundle
        /// </summary>
        public OscTimeTag Timestamp { get; private set; }

        /// <summary>
        /// Access bundle messages by index
        /// </summary>
        /// <param name="index">the index of the message</param>
        /// <returns>message at the supplied index</returns>
        public OscPacket this[int index] => packets[index];

        /// <summary>
        /// Create a bundle of messages
        /// </summary>
        /// <param name="timestamp">timestamp</param>
        /// <param name="messages">messages to bundle</param>
        public OscBundle(OscTimeTag timestamp, params OscPacket[] messages)
        {
            Origin = Helper.EmptyEndPoint;

            this.Timestamp = timestamp;
            this.packets = messages;
        }

        /// <summary>
        /// Create a bundle of messages
        /// </summary>
        /// <param name="timestamp">timestamp</param>
        /// <param name="messages">messages to bundle</param>
        public OscBundle(ulong timestamp, params OscPacket[] messages)
        {
            Origin = Helper.EmptyEndPoint;

            this.Timestamp = new OscTimeTag(timestamp);
            this.packets = messages;
        }

        /// <summary>
        /// Create a bundle of messages
        /// </summary>
        /// <param name="timestamp">timestamp</param>
        /// <param name="messages">messages to bundle</param>
        public OscBundle(DateTime timestamp, params OscPacket[] messages)
        {
            Origin = Helper.EmptyEndPoint;

            this.Timestamp = OscTimeTag.FromDataTime(timestamp);
            this.packets = messages;
        }

        /// <summary>
        /// Create a bundle of messages
        /// </summary>
        /// <param name="origin">the origin of the osc bundle</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="messages">messages to bundle</param>
        public OscBundle(IPEndPoint origin, OscTimeTag timestamp, params OscPacket[] messages)
        {
            Origin = origin;

            this.Timestamp = timestamp;
            this.packets = messages;
        }

        /// <summary>
        /// Create a bundle of messages
        /// </summary>
        /// <param name="origin">the origin of the osc bundle</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="messages">messages to bundle</param>
        public OscBundle(IPEndPoint origin, ulong timestamp, params OscPacket[] messages)
        {
            Origin = origin;

            this.Timestamp = new OscTimeTag(timestamp);
            this.packets = messages;
        }

        /// <summary>
        /// Create a bundle of messages
        /// </summary>
        /// <param name="origin">the origin of the osc bundle</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="messages">messages to bundle</param>
        public OscBundle(IPEndPoint origin, DateTime timestamp, params OscPacket[] messages)
        {
            Origin = origin;

            this.Timestamp = OscTimeTag.FromDataTime(timestamp);
            this.packets = messages;
        }

        private OscBundle()
        {
        }

        /// <summary>
        /// Does the array contain a bundle packet?
        /// </summary>
        /// <param name="bytes">the array that contains a packet</param>
        /// <param name="index">the offset within the array where the packet starts</param>
        /// <param name="count">the number of bytes in the packet</param>
        /// <returns>true if the packet contains a valid bundle header</returns>
        public static bool IsBundle(byte[] bytes, int index, int count)
        {
            if (count < BundleHeaderSizeInBytes)
            {
                return false;
            }

            string ident = Encoding.UTF8.GetString(bytes, index, BundleIdent.Length);

            if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
            {
                return false;
            }

            return true;
        }

        public static bool operator !=(OscBundle bundle1, OscBundle bundle2)
        {
            return bundle1.Equals(bundle2) == false;
        }

        public static bool operator ==(OscBundle bundle1, OscBundle bundle2)
        {
            return bundle1.Equals(bundle2) == true;
        }

        /// <summary>
        /// Parse a bundle from a string using the InvariantCulture
        /// </summary>
        /// <param name="str">a string containing a bundle</param>
        /// <returns>the parsed bundle</returns>
        public new static OscBundle Parse(string str)
        {
            return Parse(str, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// parse a bundle from a string using a supplied format provider
        /// </summary>
        /// <param name="str">a string containing a bundle</param>
        /// <param name="provider">the format provider to use</param>
        /// <returns>the parsed bundle</returns>
        public new static OscBundle Parse(string str, IFormatProvider provider)
        {
            if (Helper.IsNullOrWhiteSpace(str) == true)
            {
                throw new ArgumentNullException(nameof(str));
            }

            int start = 0;

            int end = str.IndexOf(',', start);

            if (end <= start)
            {
                throw new Exception($"Invalid bundle ident '{""}'");
            }

            string ident = str.Substring(start, end - start).Trim();

            if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
            {
                throw new Exception($"Invalid bundle ident '{ident}'");
            }

            start = end + 1;

            end = str.IndexOf(',', start);

            if (end < 0)
            {
                end = str.Length;
            }

            //if (end <= start)
            //{
            //    throw new Exception($"Invalid bundle timestamp '{""}'");
            //}

            string timeStampStr = str.Substring(start, end - start);

            OscTimeTag timeStamp = OscTimeTag.Parse(timeStampStr.Trim(), provider);

            start = end + 1;

            if (start >= str.Length)
            {
                return new OscBundle(timeStamp);
            }

            end = str.IndexOf('{', start);

            if (end < 0)
            {
                end = str.Length;
            }

            string gap = str.Substring(start, end - start);

            if (Helper.IsNullOrWhiteSpace(gap) == false)
            {
                throw new Exception($"Missing '{{'. Found '{gap}'");
            }

            start = end;

            List<OscPacket> messages = new List<OscPacket>();

            while (start > 0 && start < str.Length)
            {
                end = ScanForward_Object(str, start);

                messages.Add(OscPacket.Parse(str.Substring(start + 1, end - (start + 1)).Trim(), provider));

                start = end + 1;

                end = str.IndexOf('{', start);

                if (end < 0)
                {
                    end = str.Length;
                }

                gap = str.Substring(start, end - start).Trim();

                if (gap.Equals(",") == false && Helper.IsNullOrWhiteSpace(gap) == false)
                {
                    throw new Exception($"Missing '{{'. Found '{gap}'");
                }

                start = end;
            }

            return new OscBundle(timeStamp, messages.ToArray());
        }

        /// <summary>
        /// Read a OscBundle from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that countains the bundle</param>
        /// <param name="count">the number of bytes in the bundle</param>
        /// <returns>the bundle</returns>
        public new static OscBundle Read(byte[] bytes, int count)
        {
            return Read(bytes, 0, count, Helper.EmptyEndPoint);
        }

        /// <summary>
        /// Read a OscBundle from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that countains the bundle</param>
        /// <param name="index">the offset within the array where reading should begin</param>
        /// <param name="count">the number of bytes in the bundle</param>
        /// <returns>the bundle</returns>
        public new static OscBundle Read(byte[] bytes, int index, int count)
        {
            return Read(bytes, index, count, Helper.EmptyEndPoint);
        }

        /// <summary>
        /// Read a OscBundle from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that contains the bundle</param>
        /// <param name="index">the offset within the array where reading should begin</param>
        /// <param name="count">the number of bytes in the bundle</param>
        /// <param name="origin">the origin that is the origin of this bundle</param>
        /// <returns>the bundle</returns>
        public new static OscBundle Read(byte[] bytes, int index, int count, IPEndPoint origin)
        {
            OscBundle bundle = new OscBundle();

            List<OscPacket> messages = new List<OscPacket>();

            using (MemoryStream stream = new MemoryStream(bytes, index, count))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                bundle.Origin = origin;

                if (stream.Length < BundleHeaderSizeInBytes)
                {
                    // this is an error
                    bundle.error = OscPacketError.MissingBundleIdent;
                    bundle.errorMessage = "Missing bundle ident";

                    bundle.packets = new OscPacket[0];

                    return bundle;
                }

                string ident = Encoding.UTF8.GetString(bytes, index, BundleIdent.Length);

                if (BundleIdent.Equals(ident, System.StringComparison.InvariantCulture) == false)
                {
                    // this is an error
                    bundle.error = OscPacketError.InvalidBundleIdent;
                    bundle.errorMessage = $"Invalid bundle ident '{ident}'";

                    bundle.packets = new OscPacket[0];

                    return bundle;
                }

                stream.Position = BundleIdent.Length + 1;

                bundle.Timestamp = Helper.ReadOscTimeTag(reader);

                while (stream.Position < stream.Length)
                {
                    if (stream.Position + 4 > stream.Length)
                    {
                        // this is an error
                        bundle.error = OscPacketError.InvalidBundleMessageHeader;
                        bundle.errorMessage = "Invalid bundle message header";

                        bundle.packets = new OscPacket[0];

                        return bundle;
                    }

                    int messageLength = Helper.ReadInt32(reader);

                    if (stream.Position + messageLength > stream.Length ||
                        messageLength < 0 ||
                        messageLength % 4 != 0)
                    {
                        // this is an error
                        bundle.error = OscPacketError.InvalidBundleMessageLength;
                        bundle.errorMessage = "Invalid bundle message length";

                        bundle.packets = new OscPacket[0];

                        return bundle;
                    }

                    messages.Add(OscPacket.Read(bytes, index + (int)stream.Position, messageLength, origin, bundle.Timestamp));

                    stream.Position += messageLength;
                }

                bundle.packets = messages.ToArray();
            }

            return bundle;
        }

        /// <summary>
        /// Try to parse a bundle from a string using the InvariantCulture
        /// </summary>
        /// <param name="str">the bundle as a string</param>
        /// <param name="bundle">the parsed bundle</param>
        /// <returns>true if the bundle could be parsed else false</returns>
        public static bool TryParse(string str, out OscBundle bundle)
        {
            try
            {
                bundle = Parse(str, CultureInfo.InvariantCulture);

                return true;
            }
            catch
            {
                bundle = null;

                return false;
            }
        }

        /// <summary>
        /// Try to parse a bundle from a string using a supplied format provider
        /// </summary>
        /// <param name="str">the bundle as a string</param>
        /// <param name="provider">the format provider to use</param>
        /// <param name="bundle">the parsed bundle</param>
        /// <returns>true if the bundle could be parsed else false</returns>
        public static bool TryParse(string str, IFormatProvider provider, out OscBundle bundle)
        {
            try
            {
                bundle = Parse(str, provider);

                return true;
            }
            catch
            {
                bundle = null;

                return false;
            }
        }

        /// <summary>
        /// Does a deep comparison of the suppied object and this instance
        /// </summary>
        /// <param name="obj">An object</param>
        /// <returns>true if the objects are equivalent</returns>
        public override bool Equals(object obj)
        {
            // if the object is the same instance then return true
            if (IsSameInstance(obj) == true)
            {
                return true;
            }
            // if the object is a bundle
            else if (obj is OscBundle)
            {
                return BundlesAreEqual(obj as OscBundle, this);
            }
            // if the object is a byte array
            else if (obj is byte[])
            {
                // check the bytes against the bytes of this bundle
                return BytesAreEqual(obj as byte[], this.ToByteArray());
            }
            // if the object is a string
            else if (obj is string)
            {
                // check the string
                return this.ToString().Equals(obj is string);
            }

            return false;
        }

        /// <summary>
        /// Enumerate all the osc packets contained in this bundle
        /// </summary>
        /// <returns>A IEnumerator of osc packets</returns>
        public IEnumerator<OscPacket> GetEnumerator()
        {
            return (packets as IEnumerable<OscPacket>).GetEnumerator();
        }

        /// <summary>
        /// Get the hash code for this object
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            // if no has code has been created
            if (hasHashCode == false)
            {
                // assign the hashcode from the string form (TODO: do something better?!)
                hashCode = this.ToString().GetHashCode();

                // indicate that a hashcode has been created
                hasHashCode = true;
            }

            // return the hashcode
            return hashCode;
        }

        public OscPacket[] ToArray()
        {
            return packets;
        }

        /// <summary>
        /// Creates a byte array that contains the osc message
        /// </summary>
        /// <returns></returns>
        public override byte[] ToByteArray()
        {
            byte[] data = new byte[SizeInBytes];

            Write(data);

            return data;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(BundleIdent);
            sb.Append(", ");
            sb.Append(Timestamp.ToString());

            foreach (OscPacket message in this)
            {
                sb.Append(", { ");
                sb.Append(message.ToString());
                sb.Append(" }");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Send the bundle into a byte array
        /// </summary>
        /// <param name="data">an array of bytes to write the bundle into</param>
        /// <returns>the number of bytes in the message</returns>
        public override int Write(byte[] data)
        {
            return Write(data, 0);
        }

        /// <summary>
        /// Send the bundle into a byte array
        /// </summary>
        /// <param name="data">an array ouf bytes to write the bundle into</param>
        /// <param name="index">the index within the array where writing should begin</param>
        /// <returns>the number of bytes in the message</returns>
        public override int Write(byte[] data, int index)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                stream.Position = index;

                writer.Write(Encoding.UTF8.GetBytes(BundleIdent));
                writer.Write((byte)0);

                Helper.Write(writer, Timestamp);

                foreach (OscPacket message in this)
                {
                    Helper.Write(writer, message.SizeInBytes);

                    stream.Position += message.Write(data, (int)stream.Position);
                }

                return (int)stream.Position - index;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (packets as System.Collections.IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Are 2 bundles equivalent
        /// </summary>
        /// <param name="bundle1">A bundle</param>
        /// <param name="bundle2">A bundle</param>
        /// <returns>true if the objects are equivalent</returns>
        private bool BundlesAreEqual(OscBundle bundle1, OscBundle bundle2)
        {
            // ensure the error codes are the same
            if (bundle1.Error != bundle2.Error)
            {
                return false;
            }

            // ensure the error messages are the same
            if (bundle1.ErrorMessage != bundle2.ErrorMessage)
            {
                return false;
            }

            // ensure the timestamps are the same
            if (bundle1.Timestamp.Value != bundle2.Timestamp.Value)
            {
                return false;
            }

            // ensure the packet arrays are equivalent
            return PacketArraysAreEqual(bundle1.ToArray(), bundle2.ToArray());
        }

        /// <summary>
        /// Are 2 packet arrays equivalent
        /// </summary>
        /// <param name="array1">A packet array</param>
        /// <param name="array2">A packet array</param>
        /// <returns>true if the packet arrays are equivalent</returns>
        private bool PacketArraysAreEqual(OscPacket[] array1, OscPacket[] array2)
        {
            // ensure they are the same length
            if (array1.Length != array2.Length)
            {
                return false;
            }

            // iterate through all the objects
            for (int i = 0; i < array2.Length; i++)
            {
                // are they the same?
                if (PacketsAreEqual(array1[i], array2[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Are 2 packets equivalent
        /// </summary>
        /// <param name="packet1">A packet</param>
        /// <param name="packet2">A packet</param>
        /// <returns>true if the packets are equivalent</returns>
        private bool PacketsAreEqual(OscPacket packet1, OscPacket packet2)
        {
            // ensure they are the same type
            if (packet1.GetType() != packet2.GetType())
            {
                return false;
            }

            // are the packets messages
            if (packet1 is OscMessage)
            {
                return (packet1 as OscMessage).Equals(packet2);
            }
            // are the packets bundles
            else if (packet1 is OscBundle)
            {
                return BundlesAreEqual(packet1 as OscBundle, packet2 as OscBundle);
            }
            // return false
            else
            {
                return false;
            }
        }
    }
}
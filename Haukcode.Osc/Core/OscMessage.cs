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
 * Based on Osc parsing code witten by Tom Mitchell teamaxe.co.uk 
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Haukcode.Osc
{
    /// <summary>
    /// Any osc message
    /// </summary>
    public sealed class OscMessage : OscPacket, IEnumerable<object>, ICloneable
	{
		public static readonly OscMessage ParseError;

		static OscMessage()
		{
			ParseError = new OscMessage();

			ParseError.error = OscPacketError.ErrorParsingPacket;
			ParseError.errorMessage = "An error occured while parsing an osc packet";
		}

		#region Private Members

		object[] arguments;

		string address;

		OscPacketError error = OscPacketError.None;
		string errorMessage = String.Empty;

		bool hasHashCode = false;
		int hashCode = 0;

		#endregion

		#region Public Properties

		/// <summary>
		/// The address of the message
		/// </summary>
		public string Address { get { return address; } }

		/// <summary>
		/// IS the argument list empty
		/// </summary>
		public bool IsEmpty { get { return arguments.Length == 0; } }

		/// <summary>
		/// Number of arguments in the message 
		/// </summary>
		public int Count { get { return arguments.Length; } }

		/// <summary>
		/// Access message arguments by index 
		/// </summary>
		/// <param name="index">the index of the message</param>
		/// <returns>message at the supplied index</returns>
		public object this[int index]
		{
			get { return arguments[index]; }
		}

		/// <summary>
		/// The error associated with the message
		/// </summary>
		public override OscPacketError Error { get { return error; } }

		/// <summary>
		///  Error message 
		/// </summary>
		public override string ErrorMessage { get { return errorMessage; } }

		/// <summary>
		/// Optional time tag, will be non-null if this message was extracted from a bundle
		/// </summary>
		public OscTimeTag? TimeTag { get; set; } 

		#region Packet Size

		/// <summary>
		/// The size of the message in bytes
		/// </summary>
		public override int SizeInBytes
		{
			get
			{
				int size = 0;

				#region Address

				// should never happen 
				if (address == String.Empty)
				{
					return size;
				}

				// address + terminator 
				size += address.Length + 1;

				// padding 
				int nullCount = 4 - (size % 4);

				if (nullCount < 4)
				{
					size += nullCount;
				}

				#endregion

				#region Zero Arguments

				if (arguments.Length == 0)
				{
					// return the size plus the comma and padding
					return size + 4;
				}

				#endregion

				#region Type Tag

				// comma 
				size++;

				size += SizeOfObjectArray_TypeTag(arguments);

				// terminator
				size++;

				// padding
				nullCount = 4 - (size % 4);

				if (nullCount < 4)
				{
					size += nullCount;
				}

				#endregion

				#region Arguments

				size += SizeOfObjectArray(arguments);

				#endregion

				return size;
			}
		}

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Construct a osc message
		/// </summary>
		/// <param name="address">An osc address that is the destination for this message</param>
		/// <param name="args">Object array of OSC argument values. The type tag string will be created automatically according to each argument type</param>
		/// <example>OscMessage message = new OscMessage("/test/test", 1, 2, 3);</example>
		public OscMessage(string address, params object[] args)
		{
			Origin = Helper.EmptyEndPoint;

			this.address = address;
			arguments = args;

			if (Helper.IsNullOrWhiteSpace(this.address) == true)
			{
				throw new ArgumentNullException(nameof(address));
			}

			if (OscAddress.IsValidAddressPattern(address) == false)
			{
				throw new ArgumentException($"The address '{address}' is not a valid osc address", nameof(address));
			}

			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			CheckArguments(arguments);
		}

        /// <summary>
        /// Construct a OSC message
        /// </summary>
        /// <param name="origin">the origin of the OSC message</param>
        /// <param name="address">An OSC address that is the destination for this message</param>		
        /// <param name="args">Object array of OSC argument values. The type tag string will be created automatically according to each argument type</param>
        /// <example>OscMessage message = new OscMessage("/test/test", 1, 2, 3);</example>
        public OscMessage(IPEndPoint origin, string address, params object[] args)
		{
			Origin = origin;
			this.address = address;
			arguments = args;

			if (Helper.IsNullOrWhiteSpace(this.address) == true)
			{
				throw new ArgumentNullException(nameof(address));
			}

			if (OscAddress.IsValidAddressPattern(address) == false)
			{
				throw new ArgumentException($"The address '{address}' is not a valid osc address", nameof(address));
			}

			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			CheckArguments(arguments);
		}

		private OscMessage()
		{

		}

		#region Check Arguments

		private void CheckArguments(object[] args)
		{
			foreach (object obj in args)
			{
				if (obj == null)
				{
					throw new ArgumentNullException(nameof(args));
				}

				if (obj is object[])
				{
					CheckArguments(obj as object[]);
				}
				else if (
					!(obj is int) &&
					!(obj is long) &&
					!(obj is float) &&
					!(obj is double) &&
					!(obj is string) &&
					!(obj is bool) &&
					!(obj is OscNull) &&
					!(obj is OscColor) &&
					!(obj is OscSymbol) &&
					!(obj is OscTimeTag) &&
					!(obj is OscMidiMessage) &&
					!(obj is OscImpulse) &&
					!(obj is byte) &&
					!(obj is byte[]))
				{
					throw new ArgumentException("Argument is of an invalid type.", nameof(args));
				}
			}
		}

		#endregion

		#endregion

		#region Get Argument Size

		/// <summary>
		/// Calculate the size of the type tag of an object array 
		/// </summary>
		/// <param name="args">the array</param>
		/// <returns>the size of the type tag for the array</returns>
		private int SizeOfObjectArray_TypeTag(object[] args)
		{
			int size = 0;

			// typetag
			foreach (object obj in args)
			{
				if (obj is object[])
				{
					size += SizeOfObjectArray_TypeTag(obj as object[]);
					size += 2; // for the [ ] 
				}
				else
				{
					size++;
				}
			}

			return size;
		}

		/// <summary>
		/// Calculate the size of the an object array in bytes
		/// </summary>
		/// <param name="args">the array</param>
		/// <returns>the size of the array in bytes</returns>
		private int SizeOfObjectArray(object[] args)
		{
			int size = 0;
			int nullCount = 0;

			foreach (object obj in args)
			{
				if (obj is object[])
				{
					size += SizeOfObjectArray(obj as object[]);
				}
				else if (
					(obj is int) ||
					(obj is float) ||
					(obj is byte) ||
					(obj is OscMidiMessage) ||
					(obj is OscColor))
				{
					size += 4;
				}
				else if (
					(obj is long) ||
					(obj is double) ||
					(obj is OscTimeTag))
				{
					size += 8;
				}
				else if (
					(obj is string) ||
					(obj is OscSymbol))
				{
					string value = obj.ToString();

					// string and terminator
					size += value.Length + 1;

					// padding 
					nullCount = 4 - (int)(size % 4);

					if (nullCount < 4)
					{
						size += nullCount;
					}
				}
				else if (obj is byte[])
				{
					byte[] value = (byte[])obj;

					// length integer 
					size += 4;

					// content 
					size += value.Length;

					// padding 
					nullCount = 4 - (int)(size % 4);

					if (nullCount < 4)
					{
						size += nullCount;
					}
				}
				else if (
					(obj is bool) ||
					(obj is OscNull) ||
					(obj is OscImpulse))
				{
					size += 0;
				}
			}

			return size;
		}

		#endregion

		#region IEnumerable<object> Members

		public IEnumerator<object> GetEnumerator()
		{
			return (arguments as IEnumerable<object>).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return arguments.GetEnumerator();
		}

		#endregion

		#region To Array

		/// <summary>
		/// Get the arguments as an array 
		/// </summary>
		/// <returns>arguments array</returns>
		public object[] ToArray()
		{
			return arguments;
		}

		#endregion

		#region To Byte Array

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

		#endregion

		#region Send

		/// <summary>
		/// Send the message body into a byte array 
		/// </summary>
		/// <param name="data">an array ouf bytes to write the message body into</param>
		/// <returns>the number of bytes in the message</returns>
		public override int Write(byte[] data)
		{
			return Write(data, 0);
		}

		/// <summary>
		/// Send the message body into a byte array 
		/// </summary>
		/// <param name="data">an array of bytes to write the message body into</param>
		/// <param name="index">the index within the array where writing should begin</param>
		/// <returns>the number of bytes in the message</returns>
		public override int Write(byte[] data, int index)
		{
			// is the a address string empty? 
			if (Helper.IsNullOrWhiteSpace(address) == true)
			{
				throw new Exception("Address string may not be null or empty");
            }

			using (MemoryStream stream = new MemoryStream(data))
			using (BinaryWriter writer = new BinaryWriter(stream))
			{
				stream.Position = index;

				#region Address

				// write the address
				writer.Write(Encoding.UTF8.GetBytes(address));
				// write null terminator
				writer.Write((byte)0);

				// padding
				Helper.WritePadding(writer, stream.Position);

				#endregion

				#region Zero Arguments

				if (arguments.Length == 0)
				{
					// Send the comma 
					writer.Write((byte)',');

					// write null terminator
					writer.Write((byte)0);

					// padding
					Helper.WritePadding(writer, stream.Position);

					return (int)stream.Position - index;
				}

				#endregion


				#region Type Tag

				// Send the comma 
				writer.Write((byte)',');

				// iterate through arguments and write their types
				WriteTypeTag(writer, arguments);

				// write null terminator
				writer.Write((byte)0);

				// padding
				Helper.WritePadding(writer, stream.Position);

				#endregion

				#region Send Argument Values

				WriteValues(writer, stream, arguments);

				#endregion

				return (int)stream.Position - index;
			}
		}

		#region Send Type Tag

		private void WriteTypeTag(BinaryWriter writer, object[] args)
		{
            foreach (object obj in args)
			{
				if (obj is object[])
				{
					writer.Write((byte)'[');

					WriteTypeTag(writer, obj as object[]);

					writer.Write((byte)']');
				}
				else if (obj is int)
				{
					writer.Write((byte)'i');
				}
				else if (obj is long)
				{
					writer.Write((byte)'h');
				}
				else if (obj is float)
				{
					writer.Write((byte)'f');
				}
				else if (obj is double)
				{
					writer.Write((byte)'d');
				}
				else if (obj is byte)
				{
					writer.Write((byte)'c');
				}
				else if (obj is OscColor)
				{
					writer.Write((byte)'r');
				}
				else if (obj is OscTimeTag)
				{
					writer.Write((byte)'t');
				}
				else if (obj is OscMidiMessage)
				{
					writer.Write((byte)'m');
				}
				else if (obj is bool)
				{
					bool value = (bool)obj;

					if (value == true)
					{
						writer.Write((byte)'T');
					}
					else
					{
						writer.Write((byte)'F');
					}
				}
				else if (obj is OscNull)
				{
					writer.Write((byte)'N');
				}
				else if (obj is OscImpulse)
				{
					writer.Write((byte)'I');
				}
				else if (obj is string)
				{
					writer.Write((byte)'s');
				}
				else if (obj is OscSymbol)
				{
					writer.Write((byte)'S');
				}
				else if (obj is byte[])
				{
					writer.Write((byte)'b');
				}
				else
				{
					throw new Exception($"Unsupported arguemnt type '{obj.GetType().ToString()}'");
				}
			}
		}

		#endregion

		#region Send Values

		private void WriteValues(BinaryWriter writer, Stream stream, object[] args)
		{
			foreach (object obj in args)
			{
				if (obj is object[])
				{
					// write object array
					WriteValues(writer, stream, obj as object[]);
				}
				else if (obj is int)
				{
					int value = (int)obj;

					// write the integer
					Helper.Write(writer, value);
				}
				else if (obj is long)
				{
					long value = (long)obj;

					// write the long
					Helper.Write(writer, value);
				}
				else if (obj is float)
				{
					float value = (float)obj;

					// write the float
					Helper.Write(writer, value);
				}
				else if (obj is double)
				{
					double value = (double)obj;

					// write the double
					Helper.Write(writer, value);
				}
				else if (obj is byte)
				{
					byte value = (byte)obj;

					// write the byte
					Helper.Write(writer, value);
				}
				else if (obj is OscColor)
				{
					OscColor value = (OscColor)obj;

					// write the Color
					Helper.Write(writer, value);
				}
				else if (obj is OscTimeTag)
				{
					OscTimeTag value = (OscTimeTag)obj;

					// write the OscTimeTag
					Helper.Write(writer, value);
				}
				else if (obj is OscMidiMessage)
				{
					OscMidiMessage value = (OscMidiMessage)obj;

					// write the OscMidiMessage
					Helper.Write(writer, value);
				}
				else if ((obj is string) ||
						 (obj is OscSymbol))
				{
					string value = obj.ToString();

					// write string 
					writer.Write(Encoding.UTF8.GetBytes(value));
					// write null terminator 
					writer.Write((byte)0);

					// padding
					Helper.WritePadding(writer, stream.Position);
				}
				else if (obj is byte[])
				{
					byte[] value = (byte[])obj;

					// write length 
					Helper.Write(writer, value.Length);

					// write bytes 
					writer.Write(value);

					// padding
					Helper.WritePadding(writer, stream.Position);
				}
			}
		}

		#endregion

		#endregion

		#region Read

		/// <summary>
		/// Read a OscMessage from a array of bytes
		/// </summary>
		/// <param name="bytes">the array that contains the message</param>
		/// <param name="count">the number of bytes in the message</param>
		/// <returns>the parsed OSC message or an empty message if their was an error while parsing</returns>
		public static new OscMessage Read(byte[] bytes, int count)
		{
			return Read(bytes, 0, count, Helper.EmptyEndPoint);
		}

        /// <summary>
        /// Read a OscMessage from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that contains the message</param>
        /// <param name="index">the offset within the array where reading should begin</param>
        /// <param name="count">the number of bytes in the message</param>
        /// <returns>the parsed OSC message or an empty message if their was an error while parsing</returns>
        public static new OscMessage Read(byte[] bytes, int index, int count)
		{
			return Read(bytes, index, count, Helper.EmptyEndPoint);
		}

        /// <summary>
        /// Read a OscMessage from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that contains the message</param>
        /// <param name="index">the offset within the array where reading should begin</param>
        /// <param name="count">the number of bytes in the message</param>
        /// <param name="origin">the origin of the packet</param>
        /// <returns>the parsed OSC message or an empty message if their was an error while parsing</returns>
        public static new OscMessage Read(byte[] bytes, int index, int count, IPEndPoint origin)
		{
			return Read(bytes, index, count, origin, null);
		}

        /// <summary>
        /// Read a OscMessage from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that contains the message</param>
        /// <param name="index">the offset within the array where reading should begin</param>
        /// <param name="count">the number of bytes in the message</param>
        /// <param name="origin">the origin of the packet</param>
        /// <returns>the parsed OSC message or an empty message if their was an error while parsing</returns>
        public static new OscMessage Read(byte[] bytes, int index, int count, IPEndPoint origin, OscTimeTag? timeTag)
		{
			OscMessage msg = new OscMessage();

			msg.Origin = origin;
			msg.TimeTag = timeTag; 

			using (MemoryStream stream = new MemoryStream(bytes, index, count))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				#region Check the length of the whole message is correct

				// check for valid length.
				if (stream.Length % 4 != 0)
				{
					// this is an error! 
					msg.address = String.Empty;
					msg.arguments = new object[0];

					msg.error = OscPacketError.InvalidSegmentLength;
					msg.errorMessage = "The packet length is not the correct size";

					return msg;
				}

				#endregion

				#region Parse Address

				long start = stream.Position;
				bool failed = true;

				// scan forward and look for the end of the address string 
				while (stream.Position < stream.Length)
				{
					if (stream.ReadByte() == 0)
					{
						failed = false;
						break;
					}
				}

				if (failed == true)
				{
					// this shouldn't happen and means we're decoding rubbish
					msg.address = String.Empty;
					msg.arguments = new object[0];

					msg.error = OscPacketError.MissingAddress;
					msg.errorMessage = "Address terminator could not be found";

					return msg;
				}

				#region Empty Address String

				// check for an empty string
				if ((int)(stream.Position - start) - 1 == 0)
				{
					msg.address = String.Empty;
					msg.arguments = new object[0];

					msg.error = OscPacketError.MissingAddress;
					msg.errorMessage = "Address was empty";

					return msg;
				}

				#endregion

				// read the string 
				msg.address = Encoding.UTF8.GetString(bytes, index + (int)start, (int)(stream.Position - start) - 1);

				#region Padding

				// Advance to the typetag
				if (Helper.SkipPadding(stream) == false)
				{
					msg.arguments = new object[0];

					msg.error = OscPacketError.InvalidSegmentLength;
					msg.errorMessage = "Unexpected end of message";

					return msg;
				}

				#endregion

				#endregion

				#region Zero Arguments

				if (stream.Position == stream.Length)
				{
					msg.arguments = new object[0];

					return msg;
				}

				#endregion

				#region Parse Type Tag

				// check that the next char is a comma                
				if ((char)reader.ReadByte() != ',')
				{
					msg.arguments = new object[0];

					msg.error = OscPacketError.MissingComma;
					msg.errorMessage = "No comma found";

					return msg;
				}

				// mark the start of the type tag
				int typeTag_Start = (int)stream.Position;
				int typeTag_Count = 0;
				int typeTag_Inset = 0;
				failed = true;

				// scan forward and look for the end of the typetag string 
				while (stream.Position < stream.Length)
				{
					char @char = (char)stream.ReadByte();

					if (@char == 0)
					{
						failed = false;
						break;
					}

					if (typeTag_Inset == 0)
					{
						typeTag_Count++;
					}

					if (@char == '[')
					{
						typeTag_Inset++;
					}
					else if (@char == ']')
					{
						typeTag_Inset--;
					}

					if (typeTag_Inset < 0)
					{
						msg.arguments = new object[0];

						msg.error = OscPacketError.MalformedTypeTag;
						msg.errorMessage = "Malformed type tag";

						return msg;
					}
				}

				if (failed == true)
				{
					// this shouldn't happen and means we're decoding rubbish
					msg.arguments = new object[0];

					msg.error = OscPacketError.MissingTypeTag;
					msg.errorMessage = "Type tag terminator could not be found";

					return msg;
				}

				// allocate the arguments array 
				msg.arguments = new object[typeTag_Count];

				// Advance to the arguments
				if (Helper.SkipPadding(stream) == false)
				{
					msg.arguments = new object[0];
					msg.error = OscPacketError.InvalidSegmentLength;
					msg.errorMessage = "Unexpected end of message";

					return msg;
				}

				#endregion

				if (ReadArguments(msg, bytes, index, stream, reader, ref typeTag_Start, typeTag_Count, msg.arguments) == false)
				{
					msg.arguments = new object[0];
				}

				return msg;
			}
		}

		#region Read Arguments

		private static bool ReadArguments(OscMessage msg, byte[] bytes, int offset, MemoryStream stream, BinaryReader reader, ref int tagIndex, int count, object[] args)
		{
			for (int i = 0; i < count; i++)
			{
				// get the type tag char
				char type = (char)bytes[offset + tagIndex++];

				switch (type)
				{
					case 'b':
						#region Blob
						if (Read_Blob(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 's':
						#region String
						if (Read_String(msg, bytes, offset, stream, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'S':
						#region Symbol
						if (Read_Symbol(msg, bytes, offset, stream, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'i':
						#region Int 32
						if (Read_Int32(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'h':
						#region Int 64
						if (Read_Int64(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'f':
						#region Float
						if (Read_Float(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'd':
						#region Double
						if (Read_Double(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 't':
						#region Osc Time Tag
						if (Read_OscTimeTag(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'c':
						#region Char
						if (Read_Char(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'r':
						#region Color
						if (Read_Color(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'm':
						#region Midi Message
						if (Read_MidiMessage(msg, stream, reader, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					case 'T':
						#region True
						args[i] = true;
						#endregion
						break;
					case 'F':
						#region False
						args[i] = false;
						#endregion
						break;
					case 'N':
						#region Nill
						args[i] = OscNull.Value;
						#endregion
						break;
					case 'I':
						#region Infinitum
						args[i] = OscImpulse.Value;
						#endregion
						break;
					case '[':
						#region Array
						if (Read_Array(msg, bytes, offset, stream, reader, ref tagIndex, args, i) == false)
						{
							return false;
						}
						#endregion
						break;
					default:
						// Unknown argument type
						msg.error = OscPacketError.UnknownArguemntType;
						msg.errorMessage = $@"Unknown argument type '{type}' on argument '{i}'";

						return false;
				}
			}

			return true;
		}

		private static bool Read_Blob(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 4 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingBlob;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			uint length = Helper.ReadUInt32(reader);

			// this shouldn't happen and means we're decoding rubbish
			if (length > 0 && stream.Position + length > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingBlob;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			args[i] = reader.ReadBytes((int)length);

			// Advance pass the padding
			if (Helper.SkipPadding(stream) == false)
			{
				msg.error = OscPacketError.ErrorParsingBlob;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			return true;
		}

		private static bool Read_String(OscMessage msg, byte[] bytes, int offset, MemoryStream stream, object[] args, int i)
		{
			long stringStart = stream.Position;
			bool failed = true;

			// scan forward and look for the end of the string 
			while (stream.Position < stream.Length)
			{
				if (stream.ReadByte() == 0)
				{
					failed = false;
					break;
				}
			}

			if (failed == true)
			{
				msg.error = OscPacketError.ErrorParsingString;
				msg.errorMessage = $@"Terminator could not be found while parsing argument '{i}'";

				return false;
			}

			args[i] = Encoding.UTF8.GetString(bytes, offset + (int)stringStart, (int)(stream.Position - stringStart) - 1);

			// Advance pass the padding
			if (Helper.SkipPadding(stream) == false)
			{
				msg.error = OscPacketError.ErrorParsingString;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			return true;
		}

		private static bool Read_Symbol(OscMessage msg, byte[] bytes, int offset, MemoryStream stream, object[] args, int i)
		{
			long stringStart = stream.Position;
			bool failed = true;

			// scan forward and look for the end of the string 
			while (stream.Position < stream.Length)
			{
				if (stream.ReadByte() == 0)
				{
					failed = false;
					break;
				}
			}

			if (failed == true)
			{
				msg.error = OscPacketError.ErrorParsingSymbol;
				msg.errorMessage = $@"Terminator could not be found while parsing argument '{i}'";

				return false;
			}

			args[i] = new OscSymbol(Encoding.UTF8.GetString(bytes, offset + (int)stringStart, (int)(stream.Position - stringStart) - 1));

			// Advance pass the padding
			if (Helper.SkipPadding(stream) == false)
			{
				msg.error = OscPacketError.ErrorParsingSymbol;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			return true;
		}

		private static bool Read_Int32(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 4 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingInt32;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			int value = Helper.ReadInt32(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_Int64(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 8 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingInt64;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			long value = Helper.ReadInt64(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_Float(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 4 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingSingle;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			float value = Helper.ReadSingle(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_Double(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 8 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingDouble;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			double value = Helper.ReadDouble(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_OscTimeTag(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 8 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingOscTimeTag;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			OscTimeTag value = Helper.ReadOscTimeTag(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_Char(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 4 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingChar;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			byte value = Helper.ReadByte(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_Color(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 4 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingColor;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			OscColor value = Helper.ReadColor(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_MidiMessage(OscMessage msg, MemoryStream stream, BinaryReader reader, object[] args, int i)
		{
			if (stream.Position + 4 > stream.Length)
			{
				msg.error = OscPacketError.ErrorParsingMidiMessage;
				msg.errorMessage = $"Unexpected end of message while parsing argument '{i}'";

				return false;
			}

			OscMidiMessage value = Helper.ReadOscMidiMessage(reader);

			args[i] = value;

			return true;
		}

		private static bool Read_Array(OscMessage msg, byte[] bytes, int offset, MemoryStream stream, BinaryReader reader, ref int tagIndex, object[] args, int i)
		{
			// mark the start of the type tag
			int typeTag_Count = 0;
			int typeTag_Inset = 0;

			int typeTag_Char = tagIndex;

			// scan forward and look for the end of the typetag string 
			while (true)
			{
				char @char = (char)bytes[offset + typeTag_Char++];

				if (@char == ']' && typeTag_Inset == 0)
				{
					break;
				}

				if (typeTag_Inset == 0)
				{
					typeTag_Count++;
				}

				if (@char == '[')
				{
					typeTag_Inset++;
				}
				else if (@char == ']')
				{
					typeTag_Inset--;
				}

				if (typeTag_Inset < 0)
				{
					msg.error = OscPacketError.MalformedTypeTag;
					msg.errorMessage = "Malformed type tag";

					return false;
				}
			}

			// allocate the arguments array 
			object[] array = new object[typeTag_Count];

			if (ReadArguments(msg, bytes, offset, stream, reader, ref tagIndex, typeTag_Count, array) == false)
			{
				return false;
			}

			args[i] = array;

			// skip the ']'
			tagIndex++;

			return true;
		}

		#endregion

		#endregion

		#region To String

		public override string ToString()
		{
			return ToString(CultureInfo.InvariantCulture);
		}

		public string ToString(IFormatProvider provider)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Address);

			if (IsEmpty == true)
			{
				return sb.ToString();
			}

			sb.Append(", ");

			ArgumentsToString(sb, provider, arguments);

			return sb.ToString();
		}

		internal static void ArgumentsToString(StringBuilder sb, IFormatProvider provider, object[] args)
		{
			bool first = true;

			foreach (object obj in args)
			{
				if (first == false)
				{
					sb.Append(", ");
				}
				else
				{
					first = false;
				}

				if (obj is object[])
				{
					sb.Append('[');

					ArgumentsToString(sb, provider, obj as object[]);

					sb.Append(']');
				}
				else if (obj is int)
				{
					sb.Append(((int)obj).ToString(provider));
				}
				else if (obj is long)
				{
					sb.Append(((long)obj).ToString(provider) + "L");
				}
				else if (obj is float)
				{
				    float value = (float)obj;

				    if (float.IsInfinity(value) == true || float.IsNaN(value) == true)
				    {
                        sb.Append(((float)obj).ToString(provider));
                    }
                    else
				    {
				        sb.Append(((float) obj).ToString(provider) + "f");
				    }
				}
				else if (obj is double)
				{
					sb.Append(((double)obj).ToString(provider) + "d");
				}
				else if (obj is byte)
				{
					sb.Append($"'{(char)(byte)obj}'");
				}
				else if (obj is OscColor)
				{
					sb.Append($"{{ Color: {Helper.ToStringColor((OscColor)obj)} }}");
				}
				else if (obj is OscTimeTag)
				{
					sb.Append($"{{ Time: {(OscTimeTag)obj} }}");
				}
				else if (obj is OscMidiMessage)
				{
					sb.Append($"{{ Midi: {(OscMidiMessage)obj} }}");
				}
				else if (obj is bool)
				{
					sb.Append(((bool)obj).ToString());
				}
				else if (obj is OscNull)
				{
					sb.Append((OscNull)obj);
				}
				else if (obj is OscImpulse)
				{
					sb.Append((OscImpulse)obj);
				}
				else if (obj is string)
				{
					sb.Append($@"""{OscHelper.EscapeString(obj.ToString())}""");
				}
				else if (obj is OscSymbol)
				{
					sb.Append(OscHelper.EscapeString(obj.ToString()));
				}
				else if (obj is byte[])
				{
					sb.Append($"{{ Blob: {OscHelper.ToStringBlob(obj as byte[])} }}");
				}
				else
				{
					throw new Exception($"Unsupported arguemnt type '{obj.GetType()}'");
				}
			}
		}

		#endregion

		#region Equals

		/// <summary>
		/// Is the supplied object exactly the same instance as this object
		/// </summary>
		/// <param name="obj">an object</param>
		/// <returns>returns true if </returns>
		public override bool IsSameInstance(object obj)
		{
			return base.IsSameInstance(obj);
		}

		/// <summary>
		/// Does a deep comparison of the supplied object and this instance
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
			// if the object is a message 
			else if (obj is OscMessage)
			{
				return MessagesAreEqual(obj as OscMessage, this);
			}
			// if the object is a byte array
			else if (obj is byte[])
			{
				// check the bytes against the bytes of this message
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
		/// Are 2 messages equivalent
		/// </summary>
		/// <param name="message1">A message</param>
		/// <param name="message2">A message</param>
		/// <returns>true if the objects are equivalent</returns>
		private bool MessagesAreEqual(OscMessage message1, OscMessage message2)
		{
			// ensure the error codes are the same
			if (message1.Error != message2.Error)
			{
				return false;
			}

			// ensure the error messages are the same
			if (message1.ErrorMessage != message2.ErrorMessage)
			{
				return false;
			}

			// ensure the address is the same
			if (message1.Address != message2.Address)
			{
				return false;
			}

			// ensure the argument arrays are the same
			return ArgumentsAreEqual(message1.ToArray(), message2.ToArray());
		}

		/// <summary>
		/// Are the contents of 2 argument arrays the equivalent
		/// </summary>
		/// <param name="array1">An array containing argument objects</param>
		/// <param name="array2">An array containing argument objects</param>
		/// <returns>true if the object arrays are equivalent</returns>
		private bool ArgumentsAreEqual(object[] array1, object[] array2)
		{
			// ensure the arrays the same length
			if (array1.Length != array2.Length)
			{
				return false;
			}

			// iterate through the arrays
			for (int i = 0; i < array1.Length; i++)
			{
				// ensure the objects at index i of the same type? 
				if (array1[i].GetType() != array2[i].GetType())
				{
					return false;
				}

				// is the argument an object array
				if (array1[i] is object[])
				{
					object[] expectedArg = (object[])array1[i];
					object[] actualArg = (object[])array2[i];

					// ensure the argument object arrays are the same
					if (ArgumentsAreEqual(expectedArg, actualArg) == false)
					{
						return false;
					}
				}
				// is the argument an byte array
				else if (array1[i] is byte[])
				{
					byte[] expectedArg = (byte[])array1[i];
					byte[] actualArg = (byte[])array2[i];

					// ensure the byte arrays are the same
					if (BytesAreEqual(expectedArg, actualArg) == false)
					{
						return false;
					}
				}
				// is the argument a color
				else if (array1[i] is OscColor)
				{
					OscColor expectedArg = (OscColor)array1[i];
					OscColor actualArg = (OscColor)array2[i];

					// check the RGBA values
					if (expectedArg.R != actualArg.R ||
						expectedArg.G != actualArg.G ||
						expectedArg.B != actualArg.B ||
						expectedArg.A != actualArg.A)
					{
						return false;
					}
				}
				// anything else
				else
				{
					// just check the value
					if (array1[i].Equals(array2[i]) == false)
					{
						return false;
					}
				}
			}

			// were good
			return true;
		}

		#endregion

		#region Hash Code

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

		#endregion

		#region Parse

		/// <summary>
		/// Try to parse a message from a string using the InvariantCulture
		/// </summary>
		/// <param name="str">the message as a string</param>
		/// <param name="message">the parsed message</param>
		/// <returns>true if the message could be parsed else false</returns>
		public static bool TryParse(string str, out OscMessage message)
		{
			try
			{
				message = Parse(str, CultureInfo.InvariantCulture);

				return true;
			}
			catch
			{
				message = null;

				return false;
			}
		}

		/// <summary>
		/// Try to parse a message from a string using a supplied format provider
		/// </summary>
		/// <param name="str">the message as a string</param>
		/// <param name="provider">the format provider to use</param>
		/// <param name="message">the parsed message</param>
		/// <returns>true if the message could be parsed else false</returns>
		public static bool TryParse(string str, IFormatProvider provider, out OscMessage message)
		{
			try
			{
				message = Parse(str, provider);

				return true;
			}
			catch
			{
				message = null;

				return false;
			}
		}

		/// <summary>
		/// Parse a message from a string using the InvariantCulture
		/// </summary>
		/// <param name="str">a string containing a message</param>
		/// <returns>the parsed message</returns>
		public new static OscMessage Parse(string str)
		{
			return Parse(str, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// parse a message from a string using a supplied format provider
		/// </summary>
		/// <param name="str">a string containing a message</param>
		/// <param name="provider">the format provider to use</param>
		/// <returns>the parsed message</returns>
		public new static OscMessage Parse(string str, IFormatProvider provider)
		{
			if (Helper.IsNullOrWhiteSpace(str) == true)
			{
				throw new ArgumentNullException("str");
			}

			int index = str.IndexOf(',');

			if (index <= 0)
			{
				// could be an argument less message				
				index = str.Length;
			}

			string address = str.Substring(0, index).Trim();

			if (Helper.IsNullOrWhiteSpace(address) == true)
			{
				throw new Exception("Address was empty");
			}

			if (OscAddress.IsValidAddressPattern(address) == false)
			{
				throw new Exception("Invalid address");
			}

			List<object> arguments = new List<object>();

			// parse arguments
			ParseArguments(str, arguments, index + 1, provider);

			return new OscMessage(address, arguments.ToArray());
		}

		#region Parse Arguments

		/// <summary>
		/// Parse arguments
		/// </summary>
		/// <param name="str">string to parse</param>
		/// <param name="arguments">the list to put the parsed arguments into</param>
		/// <param name="index">the current index within the string</param>
		/// <param name="provider">the format to use</param>
		private static void ParseArguments(string str, List<object> arguments, int index, IFormatProvider provider)
		{
			while (true)
			{
				if (index >= str.Length)
				{
					return;
				}

				// scan forward for the first control char ',', '[', '{'
				int controlChar = str.IndexOfAny(new char[] { ',', '[', '{' }, index);

				if (controlChar == -1)
				{
					// no control char found 
					arguments.Add(ParseArgument(str.Substring(index, str.Length - index), provider));

					return;
				}
				else
				{
					char c = str[controlChar];

					if (c == ',')
					{
						arguments.Add(ParseArgument(str.Substring(index, controlChar - index), provider));

						index = controlChar + 1;
					}
					else if (c == '[')
					{
						int end = ScanForward_Array(str, controlChar);

						List<object> array = new List<object>();

						ParseArguments(str.Substring(controlChar + 1, end - (controlChar + 1)), array, 0, provider);

						arguments.Add(array.ToArray());

						end++;

						if (end >= str.Length)
						{
							return;
						}

						if (str[end] != ',')
						{
							controlChar = str.IndexOfAny(new char[] { ',' }, end);

							if (controlChar == -1)
							{
								return;
							}

							if (Helper.IsNullOrWhiteSpace(str.Substring(end, controlChar - end)) == false)
							{
								throw new Exception($@"Malformed array '{str.Substring(index, controlChar - end)}'");
							}

							index = controlChar;
						}
						else
						{
							index = end + 1;
						}
					}
					else if (c == '{')
					{
						int end = ScanForward_Object(str, controlChar);

						arguments.Add(ParseObject(str.Substring(controlChar + 1, end - (controlChar + 1)), provider));

						end++;

						if (end >= str.Length)
						{
							return;
						}

						if (str[end] != ',')
						{
							controlChar = str.IndexOfAny(new char[] { ',' }, end);

							if (controlChar == -1)
							{
								return;
							}

							if (Helper.IsNullOrWhiteSpace(str.Substring(end, controlChar - end)) == false)
							{
								throw new Exception($@"Malformed object '{str.Substring(index, controlChar - end)}'");
							}

							index = controlChar;
						}
						else
						{
							index = end + 1;
						}
					}
				}
			}
		}

		#endregion

		#region Parse Argument

		/// <summary>
		/// Parse a single argument
		/// </summary>
		/// <param name="str">string contain the argument to parse</param>
		/// <param name="provider">format provider to use</param>
		/// <returns>the parsed argument</returns>
		internal static object ParseArgument(string str, IFormatProvider provider)
		{
			int value_Int32;
			long value_Int64;
			float value_Float;
			double value_Double;
			bool value_Bool;

			string argString = str.Trim();

			if (argString.Length == 0)
			{
				throw new Exception("Argument is empty");
			}

			// try to parse a hex value
			if (argString.Length > 2 && argString.StartsWith("0x") == true)
			{
				string hexString = argString.Substring(2);

				// parse a int32
				if (hexString.Length <= 8)
				{
					uint value_UInt32;
					if (uint.TryParse(hexString, NumberStyles.HexNumber, provider, out value_UInt32) == true)
					{
						return unchecked((int)value_UInt32);
					}
				}
				// parse a int64
				else
				{
					ulong value_UInt64;
					if (ulong.TryParse(hexString, NumberStyles.HexNumber, provider, out value_UInt64) == true)
					{
						return unchecked((long)value_UInt64);
					}
				}
			}

			// parse int64
			if (argString.EndsWith("L") == true)
			{
				if (long.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Integer, provider, out value_Int64) == true)
				{
					return value_Int64;
				}
			}

			// parse int32
			if (int.TryParse(argString, NumberStyles.Integer, provider, out value_Int32) == true)
			{
				return value_Int32;
			}

			// parse int64
			if (long.TryParse(argString, NumberStyles.Integer, provider, out value_Int64) == true)
			{
				return value_Int64;
			}

			// parse double
			if (argString.EndsWith("d") == true)
			{
				if (double.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Float, provider, out value_Double) == true)
				{
					return value_Double;
				}
			}

			// parse float
			if (argString.EndsWith("f") == true)
			{
				if (float.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Float, provider, out value_Float) == true)
				{
					return value_Float;
				}
			}

		    if (argString.Equals(float.PositiveInfinity.ToString(provider)) == true)
		    {
		        return float.PositiveInfinity; 
		    }

            if (argString.Equals(float.NegativeInfinity.ToString(provider)) == true)
            {
                return float.NegativeInfinity;
            }

            if (argString.Equals(float.NaN.ToString(provider)) == true)
            {
                return float.NaN;
            }

            // parse float 
            if (float.TryParse(argString, NumberStyles.Float, provider, out value_Float) == true)
			{
				return value_Float;
			}

			// parse double
			if (double.TryParse(argString, NumberStyles.Float, provider, out value_Double) == true)
			{
				return value_Double;
			}

			// parse bool
			if (bool.TryParse(argString, out value_Bool) == true)
			{
				return value_Bool;
			}

			// parse char
			if (argString.Length == 3 &&
				argString[0] == '\'' &&
				argString[2] == '\'')
			{
				char c = str.Trim()[1];

				return (byte)c;
			}

			// parse null 
			if (OscNull.IsNull(argString) == true)
			{
				return OscNull.Value;
			}

			// parse impulse/bang
			if (OscImpulse.IsImpulse(argString) == true)
			{
				return OscImpulse.Value;
			}

			// parse string
			if (argString[0] == '\"')
			{
				int end = argString.IndexOf('"', 1);

				if (end < argString.Length - 1)
				{
					// some kind of other value tacked on the end of a string! 
					throw new Exception($@"Malformed string argument '{argString}'");
				}

				return OscHelper.UnescapeString(argString.Substring(1, argString.Length - 2));
			}

			// if all else fails then its a symbol i guess (?!?) 
			return new OscSymbol(OscHelper.UnescapeString(argString));
		}

		#endregion

		#region  Parse Object

		/// <summary>
		/// Parse an object
		/// </summary>
		/// <param name="str">string contain the object to parse</param>
		/// <param name="provider">format provider to use</param>
		/// <returns>the parsed argument</returns>
		private static object ParseObject(string str, IFormatProvider provider)
		{
			string strTrimmed = str.Trim();

			int colon = strTrimmed.IndexOf(':');

			if (colon <= 0)
			{
				throw new Exception($@"Malformed object '{strTrimmed}', missing type name");
			}

			string name = strTrimmed.Substring(0, colon).Trim();
			string nameLower = name.ToLowerInvariant();

			if (name.Length == 0)
			{
				throw new Exception($@"Malformed object '{strTrimmed}', missing type name");
			}

			if (colon + 1 >= strTrimmed.Length)
			{
				throw new Exception($@"Malformed object '{strTrimmed}'");
			}

			switch (nameLower)
			{
				case "midi":
				case "m":
					return OscMidiMessage.Parse(strTrimmed.Substring(colon + 1).Trim(), provider);
				case "time":
				case "t":
					return OscTimeTag.Parse(strTrimmed.Substring(colon + 1).Trim(), provider);
				case "color":
				case "c":
					return Helper.ParseColor(strTrimmed.Substring(colon + 1).Trim(), provider);
				case "blob":
				case "b":
				case "data":
				case "d":
					return Helper.ParseBlob(strTrimmed.Substring(colon + 1).Trim(), provider);
				default:
					throw new Exception($@"Unknown object type '{name}'");
			}
		}

		#endregion

		#endregion

		#region Operators

		public static bool operator ==(OscMessage msg1, OscMessage msg2)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(msg1, msg2))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)msg1 == null) || ((object)msg2 == null))
			{
				return false;
			}

			return msg1.Equals(msg2) == true;
		}

		public static bool operator !=(OscMessage msg1, OscMessage msg2)
		{
			return msg1.Equals(msg2) == false;
		}

		#endregion

		public OscMessage Clone()
		{
			string address = this.address; 
			object[] args = arguments.Clone() as object[];

		    OscMessage message = new OscMessage(address, args)
		    {
		        Origin = Origin,
		        TimeTag = TimeTag
		    };
            
		    return message;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}		
	}
}

/* 
 * OscMessage based on Osc parsing code witten by Tom Mitchell teamaxe.co.uk 
 */

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Rug.Osc
{
	#region Osc Message Error

	public enum OscMessageError
	{
		None,

		InvalidSegmentLength,

		MissingAddress,
		MissingComma,
		MissingTypeTag,
		MalformedTypeTag,		

		ErrorParsingArgument,
		ErrorParsingBlob,
		ErrorParsingString,
		ErrorParsingSymbol,
		ErrorParsingInt32,
		ErrorParsingInt64,
		ErrorParsingSingle,
		ErrorParsingDouble,

		ErrorParsingColor,
		ErrorParsingChar,
		ErrorParsingMidiMessage,
		ErrorParsingOscTimeTag,

		UnknownArguemntType,


	}

	#endregion 

	#region Osc Message

	/// <summary>
    /// Any osc message
    /// </summary>
    public class OscMessage
    {
        #region Private Members
        
        private object[] m_Arguments;

        private string m_Address;
        
        private OscMessageError m_Error = OscMessageError.None;
        private string m_ErrorMessage = String.Empty;    

	    #endregion

        #region Public Properties
        
        /// <summary>
        /// The address of the message
        /// </summary>
        public string Address { get { return m_Address; } }

        /// <summary>
        /// IS the argument list empty
        /// </summary>
        public bool IsEmpty { get { return m_Arguments.Length == 0; } }

        /// <summary>
        /// Array of arguemnts
        /// </summary>
        public object[] Arguments { get { return m_Arguments; } }

        /// <summary>
        /// The error accosiated with the message
        /// </summary>
        public OscMessageError Error { get { return m_Error; } }

        /// <summary>
        ///  Error message 
        /// </summary>
        public string ErrorMessage { get { return m_ErrorMessage; } }

		#region Message Size

		/// <summary>
		/// The size of the message in bytes
		/// </summary>
        public int MessageSize
        {
            get
            {
                int size = 0;

				#region Address
				
				// should never happen 
                if (m_Address == String.Empty)
                {
                    return size;
				}

				// address + terminator 
                size += m_Address.Length + 1;

				// padding 
                int nullCount = 4 - (int)(size % 4);

                if (nullCount < 4)
                {
                    size += nullCount;
				}

				#endregion

				#region Zero Arguments

				if (m_Arguments.Length == 0)
				{
					return size;
				}

				#endregion

				#region Type Tag

				// comma 
                size++;

				size += SizeOfObjectArray_TypeTag(m_Arguments); 

				// terminator
				size++;

				// padding
                nullCount = 4 - (int)(size % 4);

                if (nullCount < 4)
                {
                    size += nullCount;
				}

				#endregion

				#region Arguments

				size += SizeOfObjectArray(m_Arguments); 		

				#endregion

				return size;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public OscMessage(string address, params object[] args)
        {
            m_Address = address; 
            m_Arguments = args;

			if (Helper.IsNullOrWhiteSpace(m_Address) == true)
			{
				throw new ArgumentNullException("address"); 
			}

			if (args == null)
			{
				throw new ArgumentNullException("args"); 
			}

			CheckArguments(m_Arguments); 
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
					throw new ArgumentNullException("args");
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
					!(obj is Color) &&
					!(obj is OscSymbol) &&
					!(obj is OscNil) &&
					!(obj is OscTimeTag) &&
					!(obj is OscMidiMessage) &&
					!(obj is OscInfinitum) &&
					!(obj is byte) &&
					!(obj is byte[]))
				{
					throw new ArgumentException("args");
				}
			}
		}

		#endregion
		
		#endregion

		#region Get Argument Size

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
					(obj is OscMidiMessage) ||
					(obj is byte) ||
					(obj is Color))
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
					(obj is OscNil) ||
					(obj is OscInfinitum))
				{
					size += 0;
				}
			}

			return size;
		}

		#endregion

		#region To Byte Array

		/// <summary>
		/// Creates a byte array that contains the osc message
		/// </summary>
		/// <returns></returns>
		public byte[] ToByteArray()
		{
			byte[] data = new byte[MessageSize];

			Write(data);

			return data;
		}

		#endregion

		#region Write

		/// <summary>
        /// Write the message body into a byte array 
        /// </summary>
        /// <param name="data">an array ouf bytes to write the message body into</param>
        /// <returns>the number of bytes in the message</returns>
        public int Write(byte[] data)
        {
            // is the a address string empty? 
			if (Helper.IsNullOrWhiteSpace(m_Address) == true)
            {
                throw new Exception(Strings.Address_NullOrEmpty); 
            }

            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {                
                #region Address

                // write the address
                writer.Write(Encoding.UTF8.GetBytes(m_Address));
                // write null terminator
                writer.Write((byte)0);

				// padding
				Helper.WritePadding(writer, stream.Position);

                #endregion 

				#region Zero Arguments

				if (m_Arguments.Length == 0)
				{
					return (int)stream.Position;
				}

				#endregion

				#region Type Tag

				// Write the comma 
                writer.Write((byte)',');

				// iterate through arguments and write their types
				WriteTypeTag(writer, m_Arguments); 
 
				// write null terminator
				writer.Write((byte)0);

				// padding
				Helper.WritePadding(writer, stream.Position); 
                
                #endregion

                #region Write Argument Values

				WriteValues(writer, stream, m_Arguments); 

                #endregion

                return (int)stream.Position;
            }
        }

		#region Write Type Tag

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
				else if (obj is Color)
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
				else if (obj is OscNil)
				{
					writer.Write((byte)'N');
				}
				else if (obj is OscInfinitum)
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
					throw new Exception(String.Format(Strings.Arguments_UnsupportedType, obj.GetType().ToString()));
				}
			}
		}

		#endregion

		#region Write Values

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
				else if (obj is Color)
				{
					Color value = (Color)obj;

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
        /// <param name="bytes">the array that countains the message</param>
        /// <param name="count">the number of bytes in the message</param>
        /// <returns>the parsed osc message or an empty message if their was an error while parsing</returns>
        public static OscMessage Read(byte[] bytes, int count)
        {
			OscMessage msg = new OscMessage(); 

            using (MemoryStream stream = new MemoryStream(bytes, 0, count)) 
            using (BinaryReader reader = new BinaryReader(stream)) 
            {
                #region Check the length of the whole message is correct

                // check for valid length.
                if (stream.Length % 4 != 0) 
                {
                    // this is an error! 
					msg.m_Address = String.Empty;
					msg.m_Arguments = new object[0];
                    
                    msg.m_Error = OscMessageError.InvalidSegmentLength;
                    msg.m_ErrorMessage = Strings.Parser_InvalidSegmentLength;

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
					msg.m_Address = String.Empty;
					msg.m_Arguments = new object[0];
                    
                    msg.m_Error = OscMessageError.MissingAddress;
                    msg.m_ErrorMessage = Strings.Parser_MissingAddressTerminator;

                    return msg;
				}

				#region Empty Address String

				// check for an empty string
                if ((int)(stream.Position - start) - 1 == 0)
                {
                    msg.m_Address = String.Empty;
                    msg.m_Arguments = new object[0];

                    msg.m_Error = OscMessageError.MissingAddress;
                    msg.m_ErrorMessage = Strings.Parser_MissingAddressEmpty;

					return msg;
				}

				#endregion

				// read the string 
				msg.m_Address = Encoding.UTF8.GetString(bytes, (int)start, (int)(stream.Position - start) - 1); 

                #region Padding

                // Advance to the typetag
				if (Helper.SkipPadding(stream) == false)
				{
					msg.m_Arguments = new object[0];

					msg.m_Error = OscMessageError.InvalidSegmentLength;
					msg.m_ErrorMessage = Strings.Parser_UnexpectedEndOfMessage;

					return msg; 
				}

                #endregion 

                #endregion 

				#region Zero Arguments

				if (stream.Position == stream.Length)
				{
					msg.m_Arguments = new object[0];

					return msg;
				}

				#endregion

                #region Parse Type Tag

                // check that the next char is a comma                
                if ((char)reader.ReadByte() != ',')
                {
                    msg.m_Arguments = new object[0];

                    msg.m_Error = OscMessageError.MissingComma;
                    msg.m_ErrorMessage = Strings.Parser_MissingComma;

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
						msg.m_Arguments = new object[0];

						msg.m_Error = OscMessageError.MalformedTypeTag;
						msg.m_ErrorMessage = Strings.Parser_MalformedTypeTag;

						return msg;
					}
                } 
   
                if (failed == true) 
                {
                    // this shouldn't happen and means we're decoding rubbish
					msg.m_Arguments = new object[0];

                    msg.m_Error = OscMessageError.MissingTypeTag;
                    msg.m_ErrorMessage = Strings.Parser_MissingTypeTag; 

                    return msg;
                }               				

                // alocate the arguments array 
                msg.m_Arguments = new object[typeTag_Count];                

                // Advance to the arguments
				if (Helper.SkipPadding(stream) == false)
				{
					msg.m_Arguments = new object[0];
					msg.m_Error = OscMessageError.InvalidSegmentLength;
					msg.m_ErrorMessage = Strings.Parser_UnexpectedEndOfMessage;

					return msg; 
				}

                #endregion

				if (ReadArguments(msg, bytes, stream, reader, ref typeTag_Start, typeTag_Count, msg.m_Arguments) == false)
				{
					msg.m_Arguments = new object[0];
				}

				return msg;                 
            }
        }

		#region Read Arguments

		private static bool ReadArguments(OscMessage msg, byte[] bytes, MemoryStream stream, BinaryReader reader, ref int tagIndex, int count, object[] args)
		{
			for (int i = 0; i < count; i++)
			{
				// get the type tag char
				char type = (char)bytes[tagIndex++];

				switch (type)
				{
					case 'b':
						#region Blob
						{
							if (stream.Position + 4 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingBlob;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							uint length = Helper.ReadUInt32(reader);

							// this shouldn't happen and means we're decoding rubbish
							if (length > 0 && stream.Position + length >= stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingBlob;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							args[i] = reader.ReadBytes((int)length);

							// Advance pass the padding
							if (Helper.SkipPadding(stream) == false)
							{
								msg.m_Error = OscMessageError.ErrorParsingBlob;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

						}
						#endregion
						break;
					case 's':
						#region String
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
								msg.m_Error = OscMessageError.ErrorParsingString;
								msg.m_ErrorMessage = String.Format(Strings.Parser_MissingArgumentTerminator, i);

								return false;
							}

							args[i] = Encoding.UTF8.GetString(bytes, (int)stringStart, (int)(stream.Position - stringStart) - 1);

							// Advance pass the padding
							if (Helper.SkipPadding(stream) == false)
							{
								msg.m_Error = OscMessageError.ErrorParsingString;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}
						}
						#endregion
						break;
					case 'S':
						#region Symbol
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
								msg.m_Error = OscMessageError.ErrorParsingSymbol;
								msg.m_ErrorMessage = String.Format(Strings.Parser_MissingArgumentTerminator, i);

								return false;
							}

							args[i] = new OscSymbol(Encoding.UTF8.GetString(bytes, (int)stringStart, (int)(stream.Position - stringStart) - 1));

							// Advance pass the padding
							if (Helper.SkipPadding(stream) == false)
							{
								msg.m_Error = OscMessageError.ErrorParsingSymbol;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}
						}
						#endregion
						break;
					case 'i':
						#region Int 32
						{
							if (stream.Position + 4 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingInt32;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							int value = Helper.ReadInt32(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 'h':
						#region Int 64
						{
							if (stream.Position + 8 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingInt64;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							long value = Helper.ReadInt64(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 'f':
						#region Float
						{
							if (stream.Position + 4 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingSingle;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							float value = Helper.ReadSingle(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 'd':
						#region Double
						{
							if (stream.Position + 8 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingDouble;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							double value = Helper.ReadDouble(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 't':
						#region Osc Time Tag
						{
							if (stream.Position + 8 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingOscTimeTag;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							OscTimeTag value = Helper.ReadOscTimeTag(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 'c':
						#region Char
						{
							if (stream.Position + 4 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingChar;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							byte value = Helper.ReadByte(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 'r':
						#region Color
						{
							if (stream.Position + 4 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingColor;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							Color value = Helper.ReadColor(reader);

							args[i] = value;
						}
						#endregion
						break;
					case 'm':
						#region Midi Message
						{
							if (stream.Position + 4 > stream.Length)
							{
								msg.m_Error = OscMessageError.ErrorParsingMidiMessage;
								msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

								return false;
							}

							OscMidiMessage value = Helper.ReadOscMidiMessage(reader);

							args[i] = value;
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
						args[i] = OscNil.Value;
						#endregion
						break;
					case 'I':
						#region Infinitum
						args[i] = OscInfinitum.Value;
						#endregion
						break;
					case '[':
						#region Array
						{
							// mark the start of the type tag
							int typeTag_Count = 0;
							int typeTag_Inset = 0;

							int typeTag_Char = tagIndex; 

							// scan forward and look for the end of the typetag string 
							while (true)
							{
								char @char = (char)bytes[typeTag_Char++];

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
									msg.m_Error = OscMessageError.MalformedTypeTag;
									msg.m_ErrorMessage = Strings.Parser_MalformedTypeTag;

									return false;
								}
							} 

							// alocate the arguments array 
							object[] array = new object[typeTag_Count];

							if (ReadArguments(msg, bytes, stream, reader, ref tagIndex, typeTag_Count, array) == false)
							{
								return false;
							}

							args[i] = array;

							// skip the ']'
							tagIndex++; 
						}
						#endregion
						break;
					default:
						// Unknown argument type
						msg.m_Error = OscMessageError.UnknownArguemntType;
						msg.m_ErrorMessage = String.Format(Strings.Parser_UnknownArgumentType, type, i);

						return false;
				}
			}			

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

			ArgumentsToString(sb, provider, m_Arguments);

			return sb.ToString(); 
		}

		private void ArgumentsToString(StringBuilder sb, IFormatProvider provider, object[] args)
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
					sb.Append(((long)obj).ToString(provider));
				}
				else if (obj is float)
				{
					sb.Append(((float)obj).ToString(provider));
				}
				else if (obj is double)
				{
					sb.Append(((double)obj).ToString(provider) + "d");
				}
				else if (obj is byte)
				{
					sb.Append((char)obj); 
				}
				else if (obj is Color)
				{
					//sb.Append(((Color)obj).ToString());
					sb.Append("Color!");
				}
				else if (obj is OscTimeTag)
				{
					//sb.Append(((OscTimeTag)obj).ToString());
					sb.Append("OscTimeTag!");
				}
				else if (obj is OscMidiMessage)
				{
					sb.Append("{ Midi: " + ((OscMidiMessage)obj).ToString() + " }");
				}
				else if (obj is bool)
				{
					sb.Append(((bool)obj).ToString());
				}
				else if (obj is OscNil)
				{
					sb.Append(((OscNil)obj).ToString());
				}
				else if (obj is OscInfinitum)
				{
					sb.Append(((OscInfinitum)obj).ToString());
				}
				else if (obj is string)
				{
					sb.Append("\"" + obj.ToString() + "\"");
				}
				else if (obj is OscSymbol)
				{
					sb.Append(obj.ToString());
				}
				else if (obj is byte[])
				{
					sb.Append("BYTES!");
				}
				else
				{
					throw new Exception(String.Format(Strings.Arguments_UnsupportedType, obj.GetType().ToString()));
				}
			}
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
		public static OscMessage Parse(string str)
		{
			return Parse(str, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// parse a message from a string using a supplied format provider
		/// </summary>
		/// <param name="str">a string containing a message</param>
		/// <param name="provider">the format provider to use</param>
		/// <returns>the parsed message</returns>
		public static OscMessage Parse(string str, IFormatProvider provider)
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
				throw new Exception(Strings.Parser_MissingAddressEmpty);
			}

			if (OscAddress.IsValidAddressPattern(address) == false)
			{
				throw new Exception(Strings.Parser_InvalidAddress);
			}

			List<object> arguments = new List<object>();

			// parse arguments
			ParseArguments(str, arguments, index + 1, provider);

			return new OscMessage(address, arguments.ToArray());
		}

		#region Parse Arguments

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
								throw new Exception(String.Format(Strings.Parser_MalformedArrayArgument, str.Substring(index, controlChar - end)));
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
								throw new Exception(String.Format(Strings.Parser_MalformedObjectArgument, str.Substring(index, controlChar - end)));
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

		#region Scan Forward

		private static int ScanForward_Array(string str, int controlChar)
		{
			return ScanForward(str, controlChar, '[', ']', Strings.Parser_MissingArrayEndChar);
		}

		private static int ScanForward_Object(string str, int controlChar)
		{
			return ScanForward(str, controlChar, '{', '}', Strings.Parser_MissingObjectEndChar);
		}

		private static int ScanForward(string str, int controlChar, char startChar, char endChar, string errorString)
		{
			bool found = false; 

			int count = 0;

			int index = controlChar + 1;

			bool insideString = false;

			while (index < str.Length)
			{
				if (str[index] == '"')
				{
					insideString = !insideString;
				}
				else
				{
					if (insideString == false)
					{
						if (str[index] == startChar)
						{
							count++;
						}
						else if (str[index] == endChar)
						{
							if (count == 0)
							{
								found = true; 

								break;
							}

							count--;
						}
					}
				}

				index++;
			}


			if (insideString == true)
			{
				throw new Exception(Strings.Parser_MissingStringEndChar);
			}

			if (count > 0)
			{
				throw new Exception(errorString);
			}

			if (found == false)
			{
				throw new Exception(errorString);
			}

			return index;
		}

		#endregion

		#region Parse Argument

		private static object ParseArgument(string str, IFormatProvider provider)
		{
			int value_Int32;
			long value_Int64;
			float value_Float;
			double value_Double;
			bool value_Bool; 

			string argString = str.Trim();

			if (argString.Length == 0)
			{
				throw new Exception(Strings.Parser_ArgumentEmpty);
			}

			if (argString.Length > 2 && argString.StartsWith("0x") == true)
			{
				string hexString = argString.Substring(2);

				if (hexString.Length <= 8)
				{
					uint value_UInt32; 
					if (uint.TryParse(hexString, NumberStyles.HexNumber, provider, out value_UInt32) == true)
					{
						return unchecked((int)value_UInt32);
					}
				}
				else
				{
					ulong value_UInt64;
					if (ulong.TryParse(hexString, NumberStyles.HexNumber, provider, out value_UInt64) == true)
					{
						return unchecked((long)value_UInt64);
					}
				}
			}

			if (int.TryParse(argString, NumberStyles.Integer, provider, out value_Int32) == true)
			{
				return value_Int32;
			}

			if (long.TryParse(argString, NumberStyles.Integer, provider, out value_Int64) == true)
			{
				return value_Int64;
			}

			if (argString.EndsWith("d") == true)
			{
				if (double.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Float, provider, out value_Double) == true)
				{
					return value_Double;
				}
			}

			if (argString.EndsWith("f") == true)
			{
				if (float.TryParse(argString.Substring(0, argString.Length - 1), NumberStyles.Float, provider, out value_Float) == true)
				{
					return value_Float;
				}
			}

			if (float.TryParse(argString, NumberStyles.Float, provider, out value_Float) == true)
			{
				return value_Float;
			}

			if (double.TryParse(argString, NumberStyles.Float, provider, out value_Double) == true)
			{
				return value_Double;
			}

			if (bool.TryParse(argString, out value_Bool) == true)
			{
				return value_Bool; 
			}

			if (argString.Length == 1)
			{
				char c = str.Trim()[0];

				return (byte)c;
			}

			if (argString.Equals(OscNil.Value.ToString(), StringComparison.InvariantCultureIgnoreCase) == true)
			{
				return OscNil.Value;
			}

			if (OscInfinitum.IsInfinitum(argString) == true)
			{
				return OscInfinitum.Value;
			}

			if (argString[0] == '\"')
			{
				int end = argString.IndexOf('"', 1);

				if (end < argString.Length - 1)
				{
					// some kind of other value tacked on the end of a string! 
					throw new Exception(String.Format(Strings.Parser_MalformedStringArgument, argString)); 
				}

				return argString.Substring(1, argString.Length - 2); 
			}

			return new OscSymbol(argString); 
		}

		#endregion

		#region  Parse Object

		private static object ParseObject(string str, IFormatProvider provider)
		{
			string strTrimmed = str.Trim();

			int colon = strTrimmed.IndexOf(':');

			if (colon <= 0)
			{
				throw new Exception(String.Format(Strings.Parser_MalformedObjectArgument_MissingType, strTrimmed));
			}

			string name = strTrimmed.Substring(0, colon).Trim();
			string nameLower = name.ToLowerInvariant(); 

			if (name.Length == 0)
			{
				throw new Exception(String.Format(Strings.Parser_MalformedObjectArgument_MissingType, strTrimmed));
			}

			if (colon + 1 >= strTrimmed.Length)
			{
				throw new Exception(String.Format(Strings.Parser_MalformedObjectArgument, strTrimmed));
			}

			switch (nameLower)
			{
				case "midi":
					return OscMidiMessage.Parse(strTrimmed.Substring(colon + 1).Trim(), provider); 
				default:
					throw new Exception(String.Format(Strings.Parser_UnknownObjectType, name)); 
			}
		}

		#endregion

		#endregion
	}

	#endregion
}

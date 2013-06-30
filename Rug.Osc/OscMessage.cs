/* 
 * OscMessage based on Osc parsing code witten by Tom Mitchell teamaxe.co.uk 
 */

using System;
using System.IO;
using System.Text;

namespace Rug.Osc
{
	#region Osc Message Error

	public enum OscMessageError
    {
        None,

        InvalidSegmentLength,

        MissingAddress,
        MissingTypeTag,
        MissingComma,

        ErrorParsingArgument,
        ErrorParsingBlob,
        ErrorParsingString,        
        ErrorParsingInt,
        ErrorParsingFloat,
        
        UnknownArguemntType,
    }

	#endregion 

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

				// typetag
                foreach (object obj in m_Arguments)
                {
                    if ((obj is int) ||
                        (obj is float) ||
                        (obj is string) ||
                        (obj is byte[]))
                    {
                        size++;
                    }
                    else
                    {
                        // this is an error!                          
                        return 0;
                    }
                }

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

				foreach (object obj in m_Arguments)
                {
                    if (obj is int)
                    {
                        size += 4;
                    }
                    else if (obj is float)
                    {
                        size += 4;
                    }
                    else if (obj is string)
                    {
                        string value = (string)obj;

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
				}

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

			if (String.IsNullOrWhiteSpace(m_Address) == true)
			{
				throw new ArgumentNullException("address"); 
			}

			if (args == null)
			{
				throw new ArgumentNullException("args"); 
			}

			foreach (object obj in m_Arguments)
			{
				if (obj == null)
				{
					throw new ArgumentNullException("args"); 
				}

				if (!(obj is int) &&
					!(obj is float) && 
					!(obj is string) && 
					!(obj is byte[])) 
				{
					throw new ArgumentException("args");
				}
			}
        }

		private OscMessage()
		{

		}
		
		#endregion 

        #region Get Datagram

        /// <summary>
        /// Write the message body into a byte array 
        /// </summary>
        /// <param name="data">an arraty ouf bytes that contains the message body</param>
        /// <returns>the number of bytes in the message</returns>
		public int GetDatagram(out byte[] data)
        {
            data = new byte[MessageSize];

            return GetDatagram(data); 
        }

        /// <summary>
        /// Write the message body into a byte array 
        /// </summary>
        /// <param name="data">an arraty ouf bytes to write the message body into</param>
        /// <returns>the number of bytes in the message</returns>
        public int GetDatagram(byte[] data)
        {
            // is the a address string empty? 
            if (String.IsNullOrWhiteSpace(m_Address) == true)
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

                // iterate through arguments and check their types
                foreach (object obj in m_Arguments)
                {
                    if (obj is int)
                    {
                        writer.Write((byte)'i');
                    }
                    else if (obj is float)
                    {
                        writer.Write((byte)'f');
                    }
                    else if (obj is string)
                    {
                        writer.Write((byte)'s');
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

				// write null terminator
				writer.Write((byte)0);

				// padding
				Helper.WritePadding(writer, stream.Position); 
                
                #endregion

                #region Write Argument Values

                foreach (object obj in m_Arguments)
                {
                    if (obj is int)
                    {
                        int value = (int)obj;

						// write the integer
						Helper.Write(writer, value);
                    }
                    else if (obj is float)
                    {
                        float value = (float)obj;

						// write the float
						Helper.Write(writer, value);
                    }
                    else if (obj is string)
                    {
                        string value = (string)obj;

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

                #endregion

                return (int)stream.Position;
            }
        }

        #endregion

        #region Parse

        /// <summary>
        /// Parse a OscMessage from a array of bytes
        /// </summary>
        /// <param name="bytes">the array that countains the message</param>
        /// <param name="count">the number of bytes in the message</param>
        /// <returns>the parsed osc message or an empty message if their was an error while parsing</returns>
        public static OscMessage Parse(byte[] bytes, int count)
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

                // check for an empty string
                if ((int)(stream.Position - start) - 1 == 0)
                {
                    msg.m_Address = String.Empty;
                    msg.m_Arguments = new object[0];

                    msg.m_Error = OscMessageError.MissingAddress;
                    msg.m_ErrorMessage = Strings.Parser_MissingAddressEmpty;

					return msg; 
                }

                // read the string 
				msg.m_Address = Encoding.UTF8.GetString(bytes, (int)start, (int)(stream.Position - start) - 1); 

                #region Padding

                // Advance to the typetag
                if (stream.Position % 4 != 0)
                {
                    long newPosition = stream.Position + (4 - (stream.Position % 4)); 

                    // this shouldn't happen and means we're decoding rubbish
                    if (newPosition >= stream.Length) 
                    {
						msg.m_Arguments = new object[0];

                        msg.m_Error = OscMessageError.InvalidSegmentLength;
                        msg.m_ErrorMessage = Strings.Parser_UnexpectedEndOfMessage; 

                        return msg; 
                    }         
                    
                    stream.Position = newPosition; 
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
                failed = true; 

                // scan forward and look for the end of the typetag string 
                while (stream.Position < stream.Length) 
                { 
                    if (stream.ReadByte() == 0) 
                    {
                        failed = false;
						break; 
                    }
					
					typeTag_Count++;
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
                if (stream.Position % 4 != 0)
                {
                    long newPosition = stream.Position + (4 - (stream.Position % 4)); 

                    // this shouldn't happen and means we're decoding rubbish
                    if (newPosition >= stream.Length) 
                    {
                        msg.m_Arguments = new object[0];
                        msg.m_Error = OscMessageError.InvalidSegmentLength;
                        msg.m_ErrorMessage = Strings.Parser_UnexpectedEndOfMessage;  

                        return msg; 
                    }         
           
                    stream.Position = newPosition;
                }

                #endregion

                #region Parse Arguments

                for (int i = 0; i < typeTag_Count; i++) 
                {
					// get the type tag char
                    char type = (char)bytes[typeTag_Start + i]; 

                    switch (type)
	                {
                        case 'b': 
                            // blob
                            {
								if (stream.Position + 4 > stream.Length)
								{
									msg.m_Arguments = new object[0];

									msg.m_Error = OscMessageError.ErrorParsingBlob;
									msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

									return msg;
								}

                                uint length = reader.ReadUInt32(); 
                                length = unchecked((length & 0xFF000000) >> 24 | 
                                                   (length & 0x00FF0000) >> 8 | 
                                                   (length & 0x0000FF00) << 8 |
                                                   (length & 0x000000FF) << 24);

								// this shouldn't happen and means we're decoding rubbish
								if (length > 0 && stream.Position + length >= stream.Length)
								{
									msg.m_Arguments = new object[0];

									msg.m_Error = OscMessageError.ErrorParsingBlob;
									msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

									return msg;
								}  

								msg.m_Arguments[i] = reader.ReadBytes((int)length); 

                                // Advance pass the padding
                                if (stream.Position % 4 != 0)
                                {
                                    long newPosition = stream.Position + (4 - (stream.Position % 4)); 

                                    // this shouldn't happen and means we're decoding rubbish
                                    if (newPosition > stream.Length) 
                                    {
										msg.m_Arguments = new object[0];

                                        msg.m_Error = OscMessageError.ErrorParsingBlob;
                                        msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i); 

                                        return msg; 
                                    }         
           
                                    stream.Position = newPosition; 
                                }
                            }
                            break; 
                        case 's': 
                            // string 
                            {
                                long stringStart = stream.Position; 
                                failed = true; 

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
                                    // this shouldn't happen and means we're decoding rubbish
									msg.m_Arguments = new object[0];

                                    msg.m_Error = OscMessageError.ErrorParsingString;
                                    msg.m_ErrorMessage = String.Format(Strings.Parser_MissingArgumentTerminator, i);
                                    
                                    return msg; 
                                }

								msg.m_Arguments[i] = Encoding.UTF8.GetString(bytes, (int)stringStart, (int)(stream.Position - stringStart) - 1);

                                // Advance pass the padding
                                if (stream.Position % 4 != 0)
                                {
                                    long newPosition = stream.Position + (4 - (stream.Position % 4));

                                    // this shouldn't happen and means we're decoding rubbish
                                    if (newPosition > stream.Length)
                                    {
										msg.m_Arguments = new object[0];

                                        msg.m_Error = OscMessageError.ErrorParsingString;
                                        msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

                                        return msg;
                                    }

                                    stream.Position = newPosition;
                                }
                            }
                            break; 
                        case 'i': 
                            // int
                            {
                                if (stream.Position + 4 > stream.Length)
                                {
                                    msg.m_Arguments = new object[0];

                                    msg.m_Error = OscMessageError.ErrorParsingInt;
                                    msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

                                    return msg;
                                }

                                uint value = reader.ReadUInt32(); 
                                value = unchecked((value & 0xFF000000) >> 24 | 
                                                   (value & 0x00FF0000) >> 8 | 
                                                   (value & 0x0000FF00) << 8 |
                                                   (value & 0x000000FF) << 24);

								msg.m_Arguments[i] = (int)value; 
                            }
                            break; 
                        case 'f': 
                            // float
                            {
                                if (stream.Position + 4 > stream.Length)
                                {
                                    msg.m_Arguments = new object[0];

                                    msg.m_Error = OscMessageError.ErrorParsingFloat;
                                    msg.m_ErrorMessage = String.Format(Strings.Parser_ArgumentUnexpectedEndOfMessage, i);

                                    return msg;
                                }

                                uint value = reader.ReadUInt32(); 
                                value = unchecked((value & 0xFF000000) >> 24 | 
                                                   (value & 0x00FF0000) >> 8 | 
                                                   (value & 0x0000FF00) << 8 |
                                                   (value & 0x000000FF) << 24);
								
								msg.m_Arguments[i] = BitConverter.ToSingle(BitConverter.GetBytes((int)value), 0);                                 
                            }
                            break; 
		                default:
							// Unknown argument type
                            msg.m_Arguments = new object[0];

                            msg.m_Error = OscMessageError.UnknownArguemntType;
                            msg.m_ErrorMessage = String.Format(Strings.Parser_UnknownArgumentType, type, i);

                            return msg;
	                }
                }

                #endregion
            }
                           
            return msg;
        }

        #endregion 
    }
}

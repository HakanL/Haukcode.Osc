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

namespace Rug.Osc
{
	#region Osc Packet Error

	/// <summary>
	/// All errors that can occur while parsing or reading osc packets, messages and bundles
	/// </summary>
	public enum OscPacketError
	{
		/// <summary>
		/// No error
		/// </summary>
		None,

		/// <summary>
		/// An invalid number or bytes has been read
		/// </summary>
		InvalidSegmentLength,


		/// <summary>
		/// The address string is empty
		/// </summary>
		MissingAddress,

		/// <summary>
		/// Missing comma after the address string
		/// </summary>
		MissingComma,

		/// <summary>
		/// Missing type-tag 
		/// </summary>
		MissingTypeTag,

		/// <summary>
		/// Invalid type-tag 
		/// </summary>
		MalformedTypeTag,


		/// <summary>
		/// Error parsing arguemnt
		/// </summary>
		ErrorParsingArgument,

		/// <summary>
		/// Error parsing blob argument
		/// </summary>
		ErrorParsingBlob,

		/// <summary>
		/// Error parsing string argument
		/// </summary>
		ErrorParsingString,

		/// <summary>
		/// Error parsing symbol argument
		/// </summary>
		ErrorParsingSymbol,

		/// <summary>
		/// Error parsing int argument
		/// </summary>
		ErrorParsingInt32,
		
		/// <summary>
		/// Error parsing long argument
		/// </summary>
		ErrorParsingInt64,

		/// <summary>
		/// Error parsing float argument
		/// </summary>
		ErrorParsingSingle,

		/// <summary>
		/// Error parsing double argument
		/// </summary>
		ErrorParsingDouble,


		/// <summary>
		/// Error parsing osc-color argument
		/// </summary>
		ErrorParsingColor,

		/// <summary>
		/// Error parsing char argument
		/// </summary>
		ErrorParsingChar,

		/// <summary>
		/// Error parsing midi message argument
		/// </summary>
		ErrorParsingMidiMessage,

		/// <summary>
		/// Error parsing midi message argument
		/// </summary>
		ErrorParsingOscTimeTag,

		
		/// <summary>
		/// The type of an argument is unsupported
		/// </summary>
		UnknownArguemntType,


		/// <summary>
		/// Bundle with missing ident
		/// </summary>
		MissingBundleIdent,

		/// <summary>
		/// Bundle with invalid ident
		/// </summary>
		InvalidBundleIdent,

		/// <summary>
		/// Invalid bundle message header
		/// </summary>
		InvalidBundleMessageHeader,

		/// <summary>
		/// An error occured while parsing a packet
		/// </summary>
		ErrorParsingPacket,

		/// <summary>
		/// Invalid bundle message length
		/// </summary>
		InvalidBundleMessageLength,
	}

	#endregion 
}

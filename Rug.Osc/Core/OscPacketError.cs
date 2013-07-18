﻿/* 
 * Rug.Osc 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2013 Phill Tew. All rights reserved.
 * 
 */

namespace Rug.Osc
{
	#region Osc Packet Error

	public enum OscPacketError
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

		MissingBundleIdent,
		InvalidBundleIdent,
		InvalidBundleMessageHeader,
		ErrorParsingPacket,
	}

	#endregion 
}

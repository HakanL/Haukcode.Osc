using System;
using System.Collections.Generic;
using System.Text;

namespace Rug.Osc.Slip
{
	public enum SlipBytes : byte 
	{
		End = 0xC0, 
		Escape = 0xDB, 
		EscapeEnd = 0xDC, 
		EscapeEscape = 0xDD, 
	}
}

using System.Runtime.InteropServices;
using System;

namespace Rug.Osc
{
	/* 
	Note Off 	 0x8# 	 note number 	 velocity 
	Note On 	 0x9# 	 note number 	 velocity 
	Poly Pressure 	 0xa# 	 note number 	 value 
	Control Change 	 0xb# 	 controller number 	 value 
	Program Change 	 0xc# 	 program number 	
	Channel Pressure 	 0xd# 	 value 	
	Pitch Bend 	 0xe# 	 0 	 bend amount 
	System Exclusive 	 0xf0 	 (sysex message) 	 0xf7 
	Time Code 	 0xf1 	 data 	
	Song Position 	 0xf2 	 0 	 position 
	Song Select 	 0xf3 	 song number 	
	Tune Request 	 0xf6 		
	Clock Tick 	 0xf8 		
	Start 	 0xfa 		
	Continue 	 0xfb 		
	Stop 	 0xfc 		
	Active Sense 	 0xfe 		
	System Reset 	 0xff
	*/ 

	public enum OscMidiMessageType : int
	{
		NoteOff = 0x80,
		NoteOn = 0x90,
		PolyPressure = 0xA0,
		ControlChange = 0xB0,
		ProgramChange = 0xC0,
		ChannelPressure = 0xD0,
		PitchBend = 0xE0,
		SystemExclusive = 0xF0,
	}

	public enum OscMidiSystemMessageType : int
	{
		SystemExclusive = 0x00,
		TimeCode = 0x01,
		SongPosition = 0x02,
		SongSelect = 0x03,
		TuneRequest = 0x06,
		ClockTick = 0x08,		
		Start = 0x0A,
		Continue = 0x0B,
		Stop = 0x0C,
		ActiveSense = 0x0E,
		SystemReset = 0x0F,
	}

	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct OscMidiMessage
	{
		#region Fields

		[FieldOffset(0)]
		public uint FullMessage;

		[FieldOffset(3)] 
		public byte PortID;

		[FieldOffset(2)]
		public byte StatusByte;

		[FieldOffset(1)]
		public byte Data1;

		[FieldOffset(0)]
		public byte Data2;

		#endregion

		#region Properties

		public OscMidiMessageType MessageType { get { return (OscMidiMessageType)(StatusByte & 0xF0); } }

		public OscMidiSystemMessageType SystemMessageType { get { return (OscMidiSystemMessageType)(StatusByte & 0x0F); } }

		public int Channel { get { return StatusByte & 0x0F; } }

		#endregion

		public OscMidiMessage(uint value)
		{
			PortID = 0;
			StatusByte = 0;
			Data1 = 0;
			Data2 = 0;

			FullMessage = value; 
		}

		public OscMidiMessage(byte portID, OscMidiMessageType type, byte channel, byte data1, byte data2)
		{
			if (channel >= 16)
			{
				throw new ArgumentOutOfRangeException("channel"); 
			}

			FullMessage = 0; 

			PortID = portID;
			StatusByte = (byte)((int)type | (int)channel);
			Data1 = data1;
			Data2 = data2;
		}

		public override bool Equals(object obj)
		{
			if (obj is OscMidiMessage)
			{
				return FullMessage.Equals(((OscMidiMessage)obj).FullMessage); 
			}
			else
			{
				return FullMessage.Equals(obj);
			}
		}

		public override int GetHashCode()
		{
			return FullMessage.GetHashCode();
		}

		public override string ToString()
		{
			return "0x" + FullMessage.ToString("X");
		}
	}
}

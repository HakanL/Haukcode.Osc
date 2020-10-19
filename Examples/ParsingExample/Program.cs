using System;
using Haukcode.Osc;
using System.Drawing;

namespace ParsingExample
{
	class Program
	{
		static void Main(string[] args)
		{
			// integer
			Console.WriteLine("Integer (Int32)"); 
			CheckPackets(new OscMessage("/foo", 42), "/foo, 42");
			CheckPackets(new OscMessage("/foo", 0x2A), "/foo, 0x2A");
			Console.WriteLine(); 

			// long 
			Console.WriteLine("Long (Int64)"); 
			CheckPackets(new OscMessage("/foo", 12334L), "/foo, 12334L");
			CheckPackets(new OscMessage("/foo", 0x13C1DA49E6B50B0F), "/foo, 0x13C1DA49E6B50B0F");
			Console.WriteLine(); 

			// float 
			Console.WriteLine("Float (Single)"); 
			CheckPackets(new OscMessage("/foo", 123.34f), "/foo, 123.34");
			CheckPackets(new OscMessage("/foo", 123.34f), "/foo, 123.34f");
			CheckPackets(new OscMessage("/foo", 123.45e+6f), "/foo, 123.45e+6");
			CheckPackets(new OscMessage("/foo", +500f), "/foo, +500f");
			CheckPackets(new OscMessage("/foo", 5e2f), "/foo, 5e2");
			CheckPackets(new OscMessage("/foo", 600.0f), "/foo, 600.");
			CheckPackets(new OscMessage("/foo", -.123f), "/foo, -.123");
			CheckPackets(new OscMessage("/foo", float.NegativeInfinity), "/foo, -Infinity");
			CheckPackets(new OscMessage("/foo", -1E-16f), "/foo, -1E-16");
			Console.WriteLine(); 

			// double 
			Console.WriteLine("Double"); 
			CheckPackets(new OscMessage("/foo", 123.34d), "/foo, 123.34d");
			Console.WriteLine(); 

			// string
			Console.WriteLine("String"); 
			CheckPackets(new OscMessage("/foo", "string"), "/foo, \"string\"");
			Console.WriteLine(); 

			// Symbol
			Console.WriteLine("Symbol");
			CheckPackets(new OscMessage("/foo", new OscSymbol("SymbolString")), "/foo, SymbolString");
			Console.WriteLine(); 

			// bool
			Console.WriteLine("Bool (Boolean)");
			CheckPackets(new OscMessage("/foo", true), "/foo, true");
			CheckPackets(new OscMessage("/foo", false), "/foo, false");
			Console.WriteLine(); 

			// Color
			Console.WriteLine("Color");
			CheckPackets(new OscMessage("/foo", OscColor.FromArgb(255, 255, 0, 0)), "/foo, { Color: 255, 255, 0, 0 }");
			CheckPackets(new OscMessage("/foo", OscColor.FromArgb(255, 255, 255, 0)), "/foo, { Color: 255, 255, 0, 255 }");
			Console.WriteLine(); 

			// Osc-Null
			Console.WriteLine("Osc-Null");
			CheckPackets(new OscMessage("/foo", OscNull.Value), "/foo, null");
			CheckPackets(new OscMessage("/foo", OscNull.Value), "/foo, nil");
			Console.WriteLine(); 

			// Osc-Timetag
			Console.WriteLine("Osc-Timetag");
			CheckPackets(new OscMessage("/foo", OscTimeTag.Parse("00:00:34.3532Z")), "/foo, { Time: 00:00:34.3532Z }");
			CheckPackets(new OscMessage("/foo", new OscTimeTag(0x13C1DA49E6B50B0F)), "/foo, { Time: 0x13C1DA49E6B50B0F }");			
			CheckPackets(new OscMessage("/foo", OscTimeTag.Parse("0")), "/foo, { Time: 1234 }");
			
			Console.WriteLine(); 

			// Osc-Impulse
			Console.WriteLine("Osc-Impulse / Infinitum");
			CheckPackets(new OscMessage("/foo", OscImpulse.Value), "/foo, infinitum");
			CheckPackets(new OscMessage("/foo", OscImpulse.Value), "/foo, impulse");
			CheckPackets(new OscMessage("/foo", OscImpulse.Value), "/foo, bang");
			Console.WriteLine(); 

			// char
			Console.WriteLine("Char (byte)");
			CheckPackets(new OscMessage("/foo", (byte)'Q'), "/foo, 'Q'");
			Console.WriteLine(); 

			// blob
			Console.WriteLine("Blob (byte array)");
			CheckPackets(new OscMessage("/foo", new byte[] { 1, 2, 3, 4 }), "/foo, { Blob: 1, 2, 3, 4 }");
			CheckPackets(new OscMessage("/foo", new byte[] { 1, 2, 3, 4 }), "/foo, { Blob: 0x01020304}");
			CheckPackets(new OscMessage("/foo", new byte[] { 1, 2, 3, 4 }), "/foo, { Blob: 64xAQIDBA==}");
			Console.WriteLine(); 

			// arrays 
			Console.WriteLine("Array");
			CheckPackets(new OscMessage("/foo", new object[] { new object[] { 1, 2, 3, 4 } }), "/foo, [ 1, 2, 3, 4 ]");
			Console.WriteLine(); 

			// bundles 
			Console.WriteLine("Bundles");
			CheckPackets(new OscBundle(OscTimeTag.Parse("23-08-2013 01:00:34.3530Z"), new OscMessage("/foo", 42)), "#bundle, 23-08-2013 01:00:34.3530Z, { /foo, 42 }");			
			Console.WriteLine(); 

			Console.WriteLine("Press any key to exit");
			Console.ReadKey(true);
		}

		static void CheckPackets(OscPacket packet1, string packetStr)
		{
			OscPacket packet2 = OscPacket.Parse(packetStr); 

			Console.WriteLine("Input String       : " + packetStr);
			Console.WriteLine("Parsed Packet      : " + packet2.ToString());
			Console.WriteLine("Packets are equal  : " + (packet1 == packet2).ToString());
			Console.WriteLine();
		}
	}
}

using System;
using System.IO.Ports;
using System.Threading;
using Rug.Osc.Slip;

namespace Rug.Osc
{
	public class OscSerial : IDisposable, IOscPacketReceiver, IOscPacketSender
	{
		private bool m_ShouldExit = false;
		private Thread m_Thread;
		private SerialPort m_SerialPort;
		private SlipPacketReader m_Reader;
		private SlipPacketWriter m_Writer;

		private byte[] m_PacketBytes;
		private byte[] m_ReadBuffer;

		public string PortName { get; private set; }
		public int BaudRate { get; private set; }
		public Parity Parity { get; private set; }
		public int DataBits { get; private set; }
		public StopBits StopBits { get; private set; }

		public bool RtsCtsEnabled { get; private set; } 

		public OscCommunicationStatistics Statistics { get; set; }

		public event OscPacketEvent PacketRecived;

		public OscSerial(string portName, int baudRate, 
			bool rtsCtsEnabled, 
			Parity parity, 
			int dataBits, StopBits stopBits, 			
			int slipBufferSize)
		{
			PortName = portName;
			BaudRate = baudRate;
			RtsCtsEnabled = rtsCtsEnabled;

			Parity = parity;
			DataBits = dataBits;
			StopBits = stopBits;

			m_Reader = new SlipPacketReader(slipBufferSize);
			m_Writer = new SlipPacketWriter();

			m_PacketBytes = new byte[m_Reader.BufferSize];
			m_ReadBuffer = new byte[m_Reader.BufferSize];
		}

		public void Connect()
		{
			Close();

			m_ShouldExit = false;

			// Open serial port
			m_SerialPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);

			//m_SerialPort.WriteTimeout = 100;

			if (RtsCtsEnabled == true)
			{
				// Only set these if hardware flow control is enabled 
				m_SerialPort.Handshake = Handshake.RequestToSend;								
			}

			m_SerialPort.DtrEnable = true;

			m_SerialPort.Open();

			m_Reader.Clear();

			if (Helper.IsRunningOnMono == false)
			{
				m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(m_SerialPort_DataReceived);
			}
			else
			{
				m_Thread = new Thread(ListenLoop);

				m_Thread.Start();
			}
		}

		public void Send(OscPacket packet)
		{
			if (m_SerialPort == null)
			{
				throw new Exception("Serial port is not connected");
			}

			byte[] packetBytes = packet.ToByteArray();
			byte[] slippedBytes = m_Writer.Write(packetBytes, 0, packetBytes.Length);

			if (Statistics != null)
			{
				packet.IncrementSendStatistics(Statistics);

				Statistics.BytesSent.Increment(packetBytes.Length);
			}

			m_SerialPort.Write(slippedBytes, 0, slippedBytes.Length);
		}

		void m_SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			if (e.EventType == SerialData.Chars)
			{
				try
				{
					ReceivedBytes(ref m_PacketBytes, ref m_ReadBuffer);
				}
				catch { }
			}
		}

		void ListenLoop()
		{
			while (m_ShouldExit == false)
			{
				if (m_SerialPort.BytesToRead == 0)
				{
					Thread.CurrentThread.Join(1);
					continue;
				}

				ReceivedBytes(ref m_PacketBytes, ref m_ReadBuffer);
			}
		}

		private void ReceivedBytes(ref byte[] packetBytes, ref byte[] readBuffer)
		{
			// Fetch bytes from serial port
			int bytesToRead = Math.Min(m_SerialPort.BytesToRead, readBuffer.Length);

			m_SerialPort.Read(readBuffer, 0, bytesToRead);

			if (Statistics != null)
			{
				Statistics.BytesReceived.Increment(bytesToRead);
			}

			int processed = 0;

			do
			{
				int packetLength;

				processed += m_Reader.ProcessBytes(readBuffer, processed, bytesToRead - processed, ref packetBytes, out packetLength);

				if (packetLength > 0)
				{
					OscPacket oscPacket = OscPacket.Read(packetBytes, packetLength);

					if (PacketRecived != null)
					{
						PacketRecived(oscPacket);
					}
				}
			}
			while (processed < bytesToRead);
		}

		public void Close()
		{
			m_ShouldExit = true;

			if (m_Thread != null)
			{
				m_Thread.Join(1000);

				throw new Exception("THIS IS A NON-ERRROR, SHOULD NOT HAPPEN IN WINDOWS!!!");
			}

			if (m_SerialPort != null &&
				m_SerialPort.IsOpen == true)
			{
				// we need to discard the contents of the the in and out buffer, 
				// otherwise this thread will hang indefinatly when hardware flow 
				// control prevents transmission.
				m_SerialPort.DiscardInBuffer();
				m_SerialPort.DiscardOutBuffer(); 
				
				// should be safe to close now 
				m_SerialPort.Close();
			}

			m_SerialPort = null;
		}

		public void Dispose()
		{
			Close();
		}
	}
}

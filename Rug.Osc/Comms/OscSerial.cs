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

using System;
using System.IO.Ports;
using System.Threading;
using Rug.Osc.Slip;

namespace Rug.Osc
{
    public class OscSerial : IDisposable, IOscPacketReceiver, IOscPacketSender
    {
        private bool shouldExit = false;
        private Thread thread;
        private SerialPort serialPort;
        private SlipPacketReader reader;

        private byte[] packetBytes;
        private byte[] readBuffer;

        public string PortName { get; }

        public int BaudRate { get; }

        public Parity Parity { get; }

        public int DataBits { get; }

        public StopBits StopBits { get; }

        public int WriteTimeout { get; }

        public bool RtsCtsEnabled { get; }

        public OscCommunicationStatistics Statistics { get; set; }

        public event OscPacketEvent PacketRecived;

        public OscSerial(string portName, int baudRate,
            bool rtsCtsEnabled,
            Parity parity,
            int dataBits, StopBits stopBits,
            int slipBufferSize, int writeTimeout = 1000)
        {
            PortName = portName;
            BaudRate = baudRate;
            RtsCtsEnabled = rtsCtsEnabled;

            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
            WriteTimeout = writeTimeout;

            reader = new SlipPacketReader(slipBufferSize);

            packetBytes = new byte[reader.BufferSize];
            readBuffer = new byte[reader.BufferSize];
        }

        public void Connect()
        {
            Close();

            shouldExit = false;

            // Open serial port
            serialPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);

            if (RtsCtsEnabled == true)
            {
                // Only set these if hardware flow control is enabled
                serialPort.Handshake = Handshake.RequestToSend;
            }

            // Set timeout to avoid infinite wait if RTS input is never un-asserted
            serialPort.WriteTimeout = WriteTimeout;

            // Always un-assert CTS output regardless of hardware flow control mode
            serialPort.DtrEnable = true;

            serialPort.Open();

            reader.Clear();

            if (Helper.IsRunningOnMono == false)
            {
                serialPort.DataReceived += SerialPort_DataReceived;
            }
            else
            {
                thread = new Thread(ListenLoop);

                thread.Start();
            }
        }

        public void Send(OscPacket packet)
        {
            if (serialPort == null)
            {
                throw new Exception("Serial port is not connected");
            }

            byte[] packetBytes = packet.ToByteArray();
            byte[] slippedBytes = SlipPacketWriter.Write(packetBytes, 0, packetBytes.Length);

            if (Statistics != null)
            {
                packet.IncrementSendStatistics(Statistics);

                Statistics.BytesSent.Increment(packetBytes.Length);
            }

            serialPort.Write(slippedBytes, 0, slippedBytes.Length);
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                try
                {
                    ReceivedBytes(ref packetBytes, ref readBuffer);
                }
                catch { }
            }
        }

        private void ListenLoop()
        {
            while (shouldExit == false)
            {
                try
                {
                    //if (serialPort.BytesToRead == 0)
                    //{
                    //    Thread.CurrentThread.Join(1);
                    //    continue;
                    //}

                    ReceivedBytes(ref packetBytes, ref readBuffer);
                }
                catch { }
            }
        }

        private void ReceivedBytes(ref byte[] packetBytes, ref byte[] readBuffer)
        {
            // Fetch bytes from serial port
            //int bytesToRead = Math.Min(serialPort.BytesToRead, readBuffer.Length); // Don't use BytesToRead as it is not mono compatible
            int bytesToRead = readBuffer.Length;

            bytesToRead = serialPort.Read(readBuffer, 0, bytesToRead);

            Statistics?.BytesReceived.Increment(bytesToRead);

            int processed = 0;

            do
            {
                int packetLength;

                processed += reader.ProcessBytes(readBuffer, processed, bytesToRead - processed, ref packetBytes, out packetLength);

                if (packetLength <= 0)
                {
                    continue;
                }

                OscPacket oscPacket = OscPacket.Read(packetBytes, packetLength);

                if (Statistics != null && oscPacket.Error != OscPacketError.None)
                {
                    Statistics.ReceiveErrors.Increment(1);
                }

                PacketRecived?.Invoke(oscPacket);
            }
            while (processed < bytesToRead);
        }

        public void Close()
        {
            shouldExit = true;

            if (serialPort != null &&
                serialPort.IsOpen == true)
            {
                if (Helper.IsRunningOnMono == false)
                {
                    serialPort.DataReceived -= SerialPort_DataReceived;
                }

                SerialPort port = serialPort;

                serialPort = null;

                // we need to discard the contents of the in and out buffer,
                // otherwise this thread will hang indefinitely when hardware flow
                // control prevents transmission.
                port.DiscardInBuffer();
                port.DiscardOutBuffer();

                // should be safe to close now
                port.Close();
            }

            if (thread != null)
            {
                thread.Join(1000);

                //throw new Exception("THIS IS A NON-ERRROR, SHOULD NOT HAPPEN IN WINDOWS!!!");
            }

            serialPort = null;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
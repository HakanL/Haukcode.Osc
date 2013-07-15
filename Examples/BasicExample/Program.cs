using System;
using System.Net;
using System.Threading;
using Rug.Osc;

namespace BasicExample
{
	class Program
	{
		static OscListenerManager m_Listener;
		static OscReceiver m_Receiver;
		static OscSender m_Sender;
		static Thread m_Thread; 

		static void Main(string[] args)
		{
			m_Listener = new OscListenerManager();

			m_Listener.Attach("/testA", TestMethodA);
			m_Listener.Attach("/testB", TestMethodB);

			m_Receiver = new OscReceiver(12345);

			m_Sender = new OscSender(IPAddress.Loopback, 12345); 
			
			m_Thread = new Thread(new ThreadStart(ListenLoop));

			Console.WriteLine("Connecting");
			m_Receiver.Connect();
			m_Sender.Connect();
			m_Thread.Start();

			Console.WriteLine();
			Console.WriteLine("Sending message to A");
			Console.WriteLine();

			m_Sender.Send(new OscMessage("/testA", "Hello from sender (test1)"));

			Thread.CurrentThread.Join(100);
			Console.WriteLine();
			Console.WriteLine("Press any key to send the next message");
			Console.ReadKey(true);



			Console.WriteLine();
			Console.WriteLine("Sending message to B");
			Console.WriteLine();

			m_Sender.Send(new OscMessage("/testB", "Hello from sender (test2)"));

			Thread.CurrentThread.Join(100);
			Console.WriteLine();
			Console.WriteLine("Press any key to send the next message");
			Console.ReadKey(true);



			Console.WriteLine();
			Console.WriteLine("Sending message to A and B");
			Console.WriteLine();

			m_Sender.Send(new OscMessage("/*", "Hello from sender (test3)"));
			
			Thread.CurrentThread.Join(100);
			Console.WriteLine();
			Console.WriteLine("Press any key to exit");
			Console.ReadKey(true);

			Console.WriteLine("Shutting down");
			m_Receiver.Close();
			m_Thread.Join();
			m_Sender.Close(); 
		}

		static void TestMethodA(OscMessage message)
		{
			Console.WriteLine("Test method A called!: " + message.Arguments[0].ToString()); 
		}

		static void TestMethodB(OscMessage message)
		{
			Console.WriteLine("Test method B called!: " + message.Arguments[0].ToString());
		}

		static void ListenLoop()
		{
			try
			{
				while (m_Receiver.State != OscSocketState.Closed)
				{
					// if we are in a state to recieve
					if (m_Receiver.State == OscSocketState.Connected)
					{
						// get the next message 
						// this will block until one arrives or the socket is closed
						OscMessage message = m_Receiver.Receive();

						if (message.Error == OscMessageError.None)
						{
							Console.WriteLine("Received message"); 
							m_Listener.Invoke(message);
						}
						else
						{
							Console.WriteLine("Error reading message, " + message.Error);
							Console.WriteLine(message.ErrorMessage);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// if the socket was connected when this happens
				// then tell the user
				if (m_Receiver.State == OscSocketState.Connected)
				{
					Console.WriteLine("Exception in listen loop");
					Console.WriteLine(ex.Message);
				}
			}
		}
	}
}

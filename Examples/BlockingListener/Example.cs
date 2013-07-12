using System;
using System.Net;
using System.Windows.Forms;
using Rug.Osc;
using System.Threading;

namespace BlockingListener
{
	public partial class Example : Form
	{
		// for messages
		delegate void StringEvent(string str); 
		
		// listen thread
		Thread m_Thread;

		OscReceiver m_Reciever;
		
		public Example()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Append a line to the output box
		/// </summary>
		/// <param name="line">the line to append</param>
		void AppendLine(string line)
		{
			m_Output.AppendText(line + Environment.NewLine);
			m_Output.Select(m_Output.TextLength, 0);
			m_Output.ScrollToCaret();
		}

		private void Connect_Click(object sender, EventArgs e)
		{
			// if there is already an instace dispose of it
			if (m_Reciever != null)
			{				
				// dispose of the reciever
				AppendLine("Disconnecting");
				m_Reciever.Dispose();
		
				// wait for the thread to exit (IMPORTANT!) 
				m_Thread.Join(); 

				m_Reciever = null;
			}

			// get the ip address from the address box 
			string addressString = m_AddressBox.Text;

			IPAddress ipAddress;

			// parse the ip address
			if (addressString.Trim().Equals("Any", StringComparison.InvariantCultureIgnoreCase) == true)
			{
				ipAddress = IPAddress.Any; 
			}
			else if (IPAddress.TryParse(addressString, out ipAddress) == false)
			{
				AppendLine(String.Format("Invalid IP address, {0}", addressString));

				return;
			}

			// create the reciever instance
			m_Reciever = new OscReceiver(ipAddress, (int)m_PortBox.Value);

			// tell the user
			AppendLine(String.Format("Listening on: {0}:{1}", ipAddress, (int)m_PortBox.Value));

			try
			{
				// connect to the socket 
				m_Reciever.Connect();
			}
			catch (Exception ex)
			{
				this.Invoke(new StringEvent(AppendLine), "Exception while connecting");
				this.Invoke(new StringEvent(AppendLine), ex.Message);

				m_Reciever.Dispose();
				m_Reciever = null;
				
				return;
			}

			// create the listen thread
			m_Thread = new Thread(ListenLoop);

			// start listening
			m_Thread.Start(); 
		}

		void ListenLoop()
		{
			try
			{
				while (m_Reciever.State != OscSocketState.Closed)
				{
					// if we are in a state to recieve
					if (m_Reciever.State == OscSocketState.Connected)
					{
						// get the next message 
						// this will block until one arrives or the socket is closed
						OscMessage message = m_Reciever.Receive();

						if (message.Error == OscMessageError.None)
						{
							this.Invoke(new StringEvent(AppendLine), message.ToString());
						}
						else
						{
							this.Invoke(new StringEvent(AppendLine), "Error reading message, " + message.Error);
							this.Invoke(new StringEvent(AppendLine), message.ErrorMessage);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// if the socket was connected when this happens
				// then tell the user
				if (m_Reciever.State == OscSocketState.Connected) 
				{
					this.Invoke(new StringEvent(AppendLine), "Exception in listen loop");
					this.Invoke(new StringEvent(AppendLine), ex.Message);
				}
			}
		}
	
		private void Clear_Click(object sender, EventArgs e)
		{
			// clear the output box
			m_Output.Clear(); 
		}

		private void Example_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_Reciever != null)
			{				
				// dispose of the reciever
				m_Reciever.Dispose();

				// wait for the thread to exit
				m_Thread.Join(); 

				m_Reciever = null; 
			}
		}
	}
}

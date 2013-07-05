using System;
using System.Net;
using System.Windows.Forms;
using Rug.Osc;

namespace PollingListener
{
	public partial class Example : Form
	{
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
				// disable the timer
				m_MessageCheckTimer.Enabled = false;

				// dispose of the reciever
				AppendLine("Disconnecting");
				m_Reciever.Dispose();
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

			// connect to the socket 
			m_Reciever.Connect();

			// enable the timer
			m_MessageCheckTimer.Enabled = true; 
		}

		private void Listen_Tick(object sender, EventArgs e)
		{
			// if we are in a state to recieve
			if (m_Reciever != null &&
				m_Reciever.State == OscSocketState.Connected)
			{				
				OscMessage message;

				// try and get the next message
				while (m_Reciever.TryReceive(out message) == true)
				{
					// write the message to the output
					AppendLine(message.ToString());				
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
				m_Reciever = null; 
			}
		}
	}
}

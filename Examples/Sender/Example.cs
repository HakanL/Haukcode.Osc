using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Rug.Osc;

namespace Sender
{
	public partial class Example : Form
	{
		// the sender instance
		OscSender m_Sender; 

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
			if (m_Sender != null)
			{
				AppendLine("Disconnecting"); 
				m_Sender.Dispose();
				m_Sender = null; 
			}

			// get the ip address from the address box 
			string addressString = m_AddressBox.Text; 

			IPAddress ipAddress; 

			// parse the ip address
			if (IPAddress.TryParse(addressString, out ipAddress) == false) 
			{
				AppendLine(String.Format("Invalid IP address, {0}", addressString)); 

				return; 
			}

			// create the sender instance
			m_Sender = new OscSender(ipAddress, (int)m_PortBox.Value);

			// tell the user
			AppendLine(String.Format("Connecting to: {0}:{1}", ipAddress, (int)m_PortBox.Value));

			// connect to the socket 
			m_Sender.Connect(); 
		}
		
		private void MessageBox_TextChanged(object sender, EventArgs e)
		{
			OscMessage msg;

			// try to parse the osc message
			if (OscMessage.TryParse(m_MessageBox.Text, out msg) == true)
			{
				// if it parsed ok then green 
				m_MessageBox.BackColor = Color.LightGreen;
			}
			else
			{
				// if there was a problem parsing the message then red (pink) 
				m_MessageBox.BackColor = Color.Pink;
			}
		}

		private void Send_Click(object sender, EventArgs e)
		{
			try
			{
				// parse the message
				OscMessage msg = OscMessage.Parse(m_MessageBox.Text);				
				
				// write the parsed message
				AppendLine(msg.ToString());

				// if the sender exists
				if (m_Sender != null)
				{
					// send 
					m_Sender.Send(msg);
				}
				else
				{
					AppendLine("Sender is not connected");
				}
			}
			catch (Exception ex)
			{
				// explicitly tell the user why the message failed to parse
				AppendLine("Error parsing message");
				AppendLine(ex.Message);
				AppendLine("");			
			}
		}

		private void Clear_Click(object sender, EventArgs e)
		{
			// clear the output box
			m_Output.Clear(); 
		}

		private void Example_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_Sender != null)
			{
				// dispose of the sender
				m_Sender.Dispose();
				m_Sender = null; 
			}
		}
	}
}

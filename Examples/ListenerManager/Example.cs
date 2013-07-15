using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rug.Osc;

namespace ListenerManager
{
	public partial class Example : Form
	{
		OscListenerManager m_Manager;
		DataTable m_Data = new DataTable();
		int m_MethodID = 1; 

		public Example()
		{
			InitializeComponent();
		}

		private void Example_Load(object sender, EventArgs e)
		{
			PopulateData();
			m_MessageBox.BackColor = Color.LightGreen;
			m_ListenerAddress.BackColor = Color.LightGreen;
		}

		void PopulateData()
		{
			m_Manager = new OscListenerManager();

			m_Data.Columns.Add("MethodID", typeof(int), null);
			m_Data.Columns.Add("Address", typeof(string), null);
			m_Data.Columns.Add("Count", typeof(int), null);

			for (int j = 0; j < 3; j++) 
			{
				for (int i = 0; i < 2; i++) 
				{
					string address = "/test/" + (char)(j + (int)'a') + "/thing_" + i;
					
					AddEvent(address); 
				}
			}

			AddEvent("/test/*/thing_[0-9]");
			AddEvent("//thing_[0-9]");

			m_DataView.DataSource = m_Data; 
		}

		private void AddEvent(string address)
		{			
			try 
			{
				int methodID = m_MethodID++; 

				m_Manager.Attach(address, new OscMessageEvent((OscMessage message) =>
				{
					DataRow[] row = m_Data.Select("MethodID='" + methodID + "'");

					if (row.Length > 0)
					{
						int count = (int)row[0]["Count"];

						count += 1;

						row[0]["Count"] = count;
					}
				}));

				m_Data.Rows.Add(methodID, address, 0);
			}
			catch (Exception ex)
			{
				// explicitly tell the user why the address failed to parse
				MessageBox.Show(ex.Message, "Error parsing osc address");
			}
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

				m_Manager.Invoke(msg); 
			}
			catch (Exception ex)
			{
				// explicitly tell the user why the message failed to parse
				MessageBox.Show(ex.Message, "Error parsing message");
			}
		}

		private void m_ListenerAddress_TextChanged(object sender, EventArgs e)
		{			
			// try to parse the osc address literal
			if (OscAddress.IsValidAddressPattern(m_ListenerAddress.Text) == true)
			{
				// if it parsed ok then green 
				m_ListenerAddress.BackColor = Color.LightGreen;
			}
			else
			{
				// if there was a problem parsing the address then red (pink) 
				m_ListenerAddress.BackColor = Color.Pink;
			}
		}

		private void m_Add_Click(object sender, EventArgs e)
		{
			AddEvent(m_ListenerAddress.Text);			
		}
	}
}

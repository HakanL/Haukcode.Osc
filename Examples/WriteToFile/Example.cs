using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Rug.Osc;

namespace WriteToFile
{
	public partial class Example : Form
	{
		private FileStream m_Stream;
		private OscWriter m_Writer;
		private bool m_IsFileOpen = false; 

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

		private void Example_Load(object sender, EventArgs e)
		{
			m_Format.Items.Clear();
			m_Format.Items.AddRange(Enum.GetNames(typeof(OscPacketFormat)));
			m_Format.SelectedIndex = 0;

			m_SaveFileDialog.Title = "Save osc packet file";
			m_SaveFileDialog.Filter = "All files (*.*)|*.*";

			m_Write.Enabled = false; 
		}

		private void m_SelectFile_Click(object sender, EventArgs e)
		{
			m_SaveFileDialog.ShowDialog();
		}

		private void m_SaveFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			m_FilePath.Text = m_SaveFileDialog.FileName;
		}

		private void m_Format_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void m_Go_Click(object sender, EventArgs e)
		{
			if (m_IsFileOpen == false)
			{
				try
				{
					AppendLine("Opening file: " + m_FilePath.Text);

					m_Stream = new FileStream(m_FilePath.Text, FileMode.Create, FileAccess.Write);
					m_Writer = new OscWriter(m_Stream, (OscPacketFormat)m_Format.SelectedIndex);
					m_Go.Text = "Close";

					m_SelectFile.Enabled = false;
					m_Write.Enabled = true;
					m_FilePath.ReadOnly = true;
					m_Format.Enabled = false;

					m_IsFileOpen = true;					 
				}
				catch (Exception ex)
				{
					AppendLine("Exception while opening");
					AppendLine(ex.Message);

					if (m_Writer != null)
					{
						m_Writer.Dispose();
						m_Writer = null;
					}

					if (m_Stream != null)
					{
						m_Stream.Dispose();
						m_Stream = null;
					}

					m_SelectFile.Enabled = true;
					m_Write.Enabled = false;
					m_FilePath.ReadOnly = false;
					m_Format.Enabled = true;
					
					m_Go.Text = "Open";

					m_IsFileOpen = false;
				}
			}
			else
			{
				try
				{
					AppendLine("Closing file");

					m_SelectFile.Enabled = false;
					m_Write.Enabled = false;
					m_FilePath.ReadOnly = false;
					m_Format.Enabled = true;
					m_IsFileOpen = false;
					m_Go.Text = "Open";

					m_Writer.Dispose();
					m_Stream.Dispose(); 
				}
				catch (Exception ex)
				{
					AppendLine("Exception while closing");
					AppendLine(ex.Message);
				}
			}
		}

		private void m_Message_TextChanged(object sender, EventArgs e)
		{
			OscPacket msg;

			// try to parse the osc message
			if (OscPacket.TryParse(m_Message.Text, out msg) == true)
			{
				// if it parsed ok then green 
				m_Message.BackColor = Color.LightGreen;
			}
			else
			{
				// if there was a problem parsing the message then red (pink) 
				m_Message.BackColor = Color.Pink;
			}
		}

		private void m_Clear_Click(object sender, EventArgs e)
		{
			m_Output.Clear();
		}

		private void m_Write_Click(object sender, EventArgs e)
		{
			OscPacket msg;

			// try to parse the osc message
			if (OscPacket.TryParse(m_Message.Text, out msg) == true)
			{
				AppendLine(msg.ToString());
				m_Writer.Write(msg); 
			}
		}
	}
}

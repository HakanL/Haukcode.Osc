using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Rug.Osc;

namespace ReadFromFile
{
	public partial class Example : Form
	{
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

			m_OpenFileDialog.Title = "Open osc packet file";
			m_OpenFileDialog.Filter = "All files (*.*)|*.*"; 
		}

		private void SelectFile_Click(object sender, EventArgs e)
		{
			m_OpenFileDialog.ShowDialog(); 
		}

		private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
		{
			m_FilePath.Text = m_OpenFileDialog.FileName; 
		}

		private void Format_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void Go_Click(object sender, EventArgs e)
		{
			try
			{
				using (FileStream stream = new FileStream(m_FilePath.Text, FileMode.Open, FileAccess.Read))
				using (OscReader reader = new OscReader(stream, (OscPacketFormat)m_Format.SelectedIndex))
				{
					while (reader.EndOfStream == false) 
					{
						AppendLine(reader.Read().ToString()); 
					}

					AppendLine("End of file"); 
				}				
			}
			catch (Exception ex)
			{
				AppendLine("Exception while reading");
				AppendLine(ex.Message);
			}
		}

		private void Clear_Click(object sender, EventArgs e)
		{
			m_Output.Clear(); 
		}
	}
}

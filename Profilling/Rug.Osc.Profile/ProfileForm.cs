/* 
 * Rug.Osc 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2013 Phill Tew. All rights reserved.
 * 
 */

using System;
using System.Windows.Forms;

namespace Rug.Osc.Profile
{
	public partial class ProfileForm : Form
	{
		public ProfileForm(string title)
		{
			InitializeComponent();

			this.Text = "Rug.Osc Profile Tool: " + title; 
		}

		public ProfileForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			LoadTests();
		}

		private void LoadTests()
		{
			foreach (string name in TestManager.Tests.Keys)
			{
				m_Data.Rows.Add(name, 10000, 0, 0, true);
			}
		}

		private void m_Go_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in m_Data.Rows)
			{
				string name = row.Cells[TestName.Name].Value.ToString();
				int numberOfTests = int.Parse(row.Cells[Tests.Name].Value.ToString());
				bool enabled = (bool)row.Cells[RunTest.Name].Value;

				if (enabled == true)
				{
					long timeStart = DateTime.Now.Ticks;

					ProfileTest test = TestManager.Tests[name];

					test(numberOfTests);

					long timeEnd = DateTime.Now.Ticks;

					double diff = (double)timeEnd - (double)timeStart;

					double av = diff / (double)numberOfTests;

					row.Cells[Average.Name].Value = (av / TimeSpan.TicksPerMillisecond).ToString("N6");
					row.Cells[Total.Name].Value = (diff / TimeSpan.TicksPerMillisecond).ToString("N6");
				}
				else
				{
					row.Cells[Average.Name].Value = "--";
					row.Cells[Total.Name].Value = "--"; 
				}
			}
		}
	}
}

namespace Rug.Osc.Profile
{
	partial class ProfileForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_Data = new System.Windows.Forms.DataGridView();
			this.TestName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Tests = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Total = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Average = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RunTest = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.m_Go = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.m_Data)).BeginInit();
			this.SuspendLayout();
			// 
			// m_Data
			// 
			this.m_Data.AllowUserToAddRows = false;
			this.m_Data.AllowUserToDeleteRows = false;
			this.m_Data.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Data.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_Data.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TestName,
            this.Tests,
            this.Total,
            this.Average,
            this.RunTest});
			this.m_Data.Location = new System.Drawing.Point(12, 12);
			this.m_Data.Name = "m_Data";
			this.m_Data.Size = new System.Drawing.Size(632, 254);
			this.m_Data.TabIndex = 0;
			// 
			// Name
			// 
			this.TestName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.TestName.HeaderText = "Name";
			this.TestName.Name = "TestName";
			this.TestName.ReadOnly = true;
			// 
			// Tests
			// 
			this.Tests.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Tests.HeaderText = "Tests";
			this.Tests.Name = "Tests";
			this.Tests.Width = 60;
			// 
			// Total
			// 
			this.Total.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Total.HeaderText = "Total (ms)";
			this.Total.Name = "Total";
			this.Total.ReadOnly = true;
			this.Total.Width = 80;
			// 
			// Average
			// 
			this.Average.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Average.HeaderText = "Av. (ms)";
			this.Average.Name = "Average";
			this.Average.ReadOnly = true;
			this.Average.Width = 80;
			// 
			// RunTest
			// 
			this.RunTest.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.RunTest.HeaderText = "";
			this.RunTest.Name = "RunTest";
			this.RunTest.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.RunTest.Width = 30;
			// 
			// m_Go
			// 
			this.m_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Go.Location = new System.Drawing.Point(569, 272);
			this.m_Go.Name = "m_Go";
			this.m_Go.Size = new System.Drawing.Size(75, 23);
			this.m_Go.TabIndex = 1;
			this.m_Go.Text = "GO";
			this.m_Go.UseVisualStyleBackColor = true;
			this.m_Go.Click += new System.EventHandler(this.m_Go_Click);
			// 
			// ProfileForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 307);
			this.Controls.Add(this.m_Go);
			this.Controls.Add(this.m_Data);
			this.MinimumSize = new System.Drawing.Size(478, 301);
			this.Name = "ProfileForm";
			this.Text = "Rug.Osc Profile Tool";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.m_Data)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView m_Data;
		private System.Windows.Forms.Button m_Go;
		private System.Windows.Forms.DataGridViewTextBoxColumn TestName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Tests;
		private System.Windows.Forms.DataGridViewTextBoxColumn Total;
		private System.Windows.Forms.DataGridViewTextBoxColumn Average;
		private System.Windows.Forms.DataGridViewCheckBoxColumn RunTest;
	}
}


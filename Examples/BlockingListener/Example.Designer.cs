namespace BlockingListener
{
	partial class Example
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
			this.m_PortBox = new System.Windows.Forms.NumericUpDown();
			this.m_AddressBox = new System.Windows.Forms.TextBox();
			this.m_Connect = new System.Windows.Forms.Button();
			this.m_Clear = new System.Windows.Forms.Button();
			this.m_Output = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.m_PortBox)).BeginInit();
			this.SuspendLayout();
			// 
			// m_PortBox
			// 
			this.m_PortBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_PortBox.Location = new System.Drawing.Point(455, 10);
			this.m_PortBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.m_PortBox.Name = "m_PortBox";
			this.m_PortBox.Size = new System.Drawing.Size(120, 20);
			this.m_PortBox.TabIndex = 9;
			this.m_PortBox.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			// 
			// m_AddressBox
			// 
			this.m_AddressBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_AddressBox.Location = new System.Drawing.Point(12, 10);
			this.m_AddressBox.Name = "m_AddressBox";
			this.m_AddressBox.Size = new System.Drawing.Size(437, 20);
			this.m_AddressBox.TabIndex = 8;
			this.m_AddressBox.Text = "Any";
			// 
			// m_Connect
			// 
			this.m_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Connect.Location = new System.Drawing.Point(581, 8);
			this.m_Connect.Name = "m_Connect";
			this.m_Connect.Size = new System.Drawing.Size(75, 23);
			this.m_Connect.TabIndex = 7;
			this.m_Connect.Text = "Connect";
			this.m_Connect.UseVisualStyleBackColor = true;
			this.m_Connect.Click += new System.EventHandler(this.Connect_Click);
			// 
			// m_Clear
			// 
			this.m_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Clear.Location = new System.Drawing.Point(12, 299);
			this.m_Clear.Name = "m_Clear";
			this.m_Clear.Size = new System.Drawing.Size(644, 23);
			this.m_Clear.TabIndex = 10;
			this.m_Clear.Text = "Clear";
			this.m_Clear.UseVisualStyleBackColor = true;
			this.m_Clear.Click += new System.EventHandler(this.Clear_Click);
			// 
			// m_Output
			// 
			this.m_Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Output.Location = new System.Drawing.Point(12, 36);
			this.m_Output.Name = "m_Output";
			this.m_Output.Size = new System.Drawing.Size(644, 257);
			this.m_Output.TabIndex = 11;
			this.m_Output.Text = "";
			// 
			// Example
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(668, 334);
			this.Controls.Add(this.m_Output);
			this.Controls.Add(this.m_Clear);
			this.Controls.Add(this.m_PortBox);
			this.Controls.Add(this.m_AddressBox);
			this.Controls.Add(this.m_Connect);
			this.Name = "Example";
			this.Text = "Blocking Listener Example";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Example_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.m_PortBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown m_PortBox;
		private System.Windows.Forms.TextBox m_AddressBox;
		private System.Windows.Forms.Button m_Connect;
		private System.Windows.Forms.Button m_Clear;
		private System.Windows.Forms.RichTextBox m_Output;
	}
}


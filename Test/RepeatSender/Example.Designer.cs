namespace RepeatSender
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
			this.components = new System.ComponentModel.Container();
			this.m_Send = new System.Windows.Forms.Button();
			this.m_MessageBox = new System.Windows.Forms.TextBox();
			this.m_Output = new System.Windows.Forms.RichTextBox();
			this.m_Clear = new System.Windows.Forms.Button();
			this.m_Connect = new System.Windows.Forms.Button();
			this.m_AddressBox = new System.Windows.Forms.TextBox();
			this.m_PortBox = new System.Windows.Forms.NumericUpDown();
			this.m_SendCount = new System.Windows.Forms.NumericUpDown();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.m_PortBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_SendCount)).BeginInit();
			this.SuspendLayout();
			// 
			// m_Send
			// 
			this.m_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Send.Location = new System.Drawing.Point(483, 39);
			this.m_Send.Name = "m_Send";
			this.m_Send.Size = new System.Drawing.Size(75, 23);
			this.m_Send.TabIndex = 0;
			this.m_Send.Text = "Send";
			this.m_Send.UseVisualStyleBackColor = true;
			this.m_Send.Click += new System.EventHandler(this.Send_Click);
			// 
			// m_MessageBox
			// 
			this.m_MessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_MessageBox.Location = new System.Drawing.Point(13, 41);
			this.m_MessageBox.Name = "m_MessageBox";
			this.m_MessageBox.Size = new System.Drawing.Size(398, 20);
			this.m_MessageBox.TabIndex = 1;
			this.m_MessageBox.Text = "/test, 1, 2, 3";
			this.m_MessageBox.TextChanged += new System.EventHandler(this.MessageBox_TextChanged);
			// 
			// m_Output
			// 
			this.m_Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Output.Location = new System.Drawing.Point(13, 67);
			this.m_Output.Name = "m_Output";
			this.m_Output.Size = new System.Drawing.Size(545, 260);
			this.m_Output.TabIndex = 2;
			this.m_Output.Text = "";
			// 
			// m_Clear
			// 
			this.m_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Clear.Location = new System.Drawing.Point(12, 333);
			this.m_Clear.Name = "m_Clear";
			this.m_Clear.Size = new System.Drawing.Size(546, 23);
			this.m_Clear.TabIndex = 3;
			this.m_Clear.Text = "Clear";
			this.m_Clear.UseVisualStyleBackColor = true;
			this.m_Clear.Click += new System.EventHandler(this.Clear_Click);
			// 
			// m_Connect
			// 
			this.m_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Connect.Location = new System.Drawing.Point(483, 10);
			this.m_Connect.Name = "m_Connect";
			this.m_Connect.Size = new System.Drawing.Size(75, 23);
			this.m_Connect.TabIndex = 4;
			this.m_Connect.Text = "Connect";
			this.m_Connect.UseVisualStyleBackColor = true;
			this.m_Connect.Click += new System.EventHandler(this.Connect_Click);
			// 
			// m_AddressBox
			// 
			this.m_AddressBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_AddressBox.Location = new System.Drawing.Point(13, 12);
			this.m_AddressBox.Name = "m_AddressBox";
			this.m_AddressBox.Size = new System.Drawing.Size(398, 20);
			this.m_AddressBox.TabIndex = 5;
			this.m_AddressBox.Text = "127.0.0.1";
			// 
			// m_PortBox
			// 
			this.m_PortBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_PortBox.Location = new System.Drawing.Point(417, 12);
			this.m_PortBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
			this.m_PortBox.Name = "m_PortBox";
			this.m_PortBox.Size = new System.Drawing.Size(60, 20);
			this.m_PortBox.TabIndex = 6;
			this.m_PortBox.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			// 
			// m_SendCount
			// 
			this.m_SendCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_SendCount.Location = new System.Drawing.Point(417, 41);
			this.m_SendCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.m_SendCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.m_SendCount.Name = "m_SendCount";
			this.m_SendCount.Size = new System.Drawing.Size(60, 20);
			this.m_SendCount.TabIndex = 7;
			this.m_SendCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// Example
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(570, 368);
			this.Controls.Add(this.m_SendCount);
			this.Controls.Add(this.m_PortBox);
			this.Controls.Add(this.m_AddressBox);
			this.Controls.Add(this.m_Connect);
			this.Controls.Add(this.m_Clear);
			this.Controls.Add(this.m_Output);
			this.Controls.Add(this.m_MessageBox);
			this.Controls.Add(this.m_Send);
			this.MinimumSize = new System.Drawing.Size(410, 273);
			this.Name = "Example";
			this.Text = "Osc Repeat Sender Example";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Example_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.m_PortBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_SendCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_Send;
		private System.Windows.Forms.TextBox m_MessageBox;
		private System.Windows.Forms.RichTextBox m_Output;
		private System.Windows.Forms.Button m_Clear;
		private System.Windows.Forms.Button m_Connect;
		private System.Windows.Forms.TextBox m_AddressBox;
		private System.Windows.Forms.NumericUpDown m_PortBox;
		private System.Windows.Forms.NumericUpDown m_SendCount;
		private System.Windows.Forms.Timer timer1;
	}
}


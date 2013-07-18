namespace WriteToFile
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
			this.m_Clear = new System.Windows.Forms.Button();
			this.m_SelectFile = new System.Windows.Forms.Button();
			this.m_Format = new System.Windows.Forms.ComboBox();
			this.m_Output = new System.Windows.Forms.RichTextBox();
			this.m_FilePath = new System.Windows.Forms.TextBox();
			this.m_Go = new System.Windows.Forms.Button();
			this.m_Message = new System.Windows.Forms.TextBox();
			this.m_Write = new System.Windows.Forms.Button();
			this.m_SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// m_Clear
			// 
			this.m_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Clear.Location = new System.Drawing.Point(12, 302);
			this.m_Clear.Name = "m_Clear";
			this.m_Clear.Size = new System.Drawing.Size(516, 23);
			this.m_Clear.TabIndex = 11;
			this.m_Clear.Text = "Clear";
			this.m_Clear.UseVisualStyleBackColor = true;
			this.m_Clear.Click += new System.EventHandler(this.m_Clear_Click);
			// 
			// m_SelectFile
			// 
			this.m_SelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_SelectFile.Location = new System.Drawing.Point(339, 12);
			this.m_SelectFile.Name = "m_SelectFile";
			this.m_SelectFile.Size = new System.Drawing.Size(29, 23);
			this.m_SelectFile.TabIndex = 10;
			this.m_SelectFile.Text = "...";
			this.m_SelectFile.UseVisualStyleBackColor = true;
			this.m_SelectFile.Click += new System.EventHandler(this.m_SelectFile_Click);
			// 
			// m_Format
			// 
			this.m_Format.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_Format.FormattingEnabled = true;
			this.m_Format.Location = new System.Drawing.Point(374, 13);
			this.m_Format.Name = "m_Format";
			this.m_Format.Size = new System.Drawing.Size(73, 21);
			this.m_Format.TabIndex = 9;
			this.m_Format.SelectedIndexChanged += new System.EventHandler(this.m_Format_SelectedIndexChanged);
			// 
			// m_Output
			// 
			this.m_Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Output.Location = new System.Drawing.Point(12, 65);
			this.m_Output.Name = "m_Output";
			this.m_Output.Size = new System.Drawing.Size(516, 231);
			this.m_Output.TabIndex = 8;
			this.m_Output.Text = "";
			// 
			// m_FilePath
			// 
			this.m_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_FilePath.Location = new System.Drawing.Point(12, 12);
			this.m_FilePath.Name = "m_FilePath";
			this.m_FilePath.Size = new System.Drawing.Size(321, 20);
			this.m_FilePath.TabIndex = 7;
			// 
			// m_Go
			// 
			this.m_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Go.Location = new System.Drawing.Point(453, 12);
			this.m_Go.Name = "m_Go";
			this.m_Go.Size = new System.Drawing.Size(75, 23);
			this.m_Go.TabIndex = 6;
			this.m_Go.Text = "Open";
			this.m_Go.UseVisualStyleBackColor = true;
			this.m_Go.Click += new System.EventHandler(this.m_Go_Click);
			// 
			// m_Message
			// 
			this.m_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Message.Location = new System.Drawing.Point(13, 39);
			this.m_Message.Name = "m_Message";
			this.m_Message.Size = new System.Drawing.Size(434, 20);
			this.m_Message.TabIndex = 12;
			this.m_Message.Text = "#bundle, 13:44:34.4532, { /test, 2,5,6,7 }, { /test, 2,5,6,7 }";
			this.m_Message.TextChanged += new System.EventHandler(this.m_Message_TextChanged);
			// 
			// m_Write
			// 
			this.m_Write.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Write.Location = new System.Drawing.Point(453, 37);
			this.m_Write.Name = "m_Write";
			this.m_Write.Size = new System.Drawing.Size(75, 23);
			this.m_Write.TabIndex = 13;
			this.m_Write.Text = "Write";
			this.m_Write.UseVisualStyleBackColor = true;
			this.m_Write.Click += new System.EventHandler(this.m_Write_Click);
			// 
			// m_SaveFileDialog
			// 
			this.m_SaveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.m_SaveFileDialog_FileOk);
			// 
			// Example
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(540, 337);
			this.Controls.Add(this.m_Write);
			this.Controls.Add(this.m_Message);
			this.Controls.Add(this.m_Clear);
			this.Controls.Add(this.m_SelectFile);
			this.Controls.Add(this.m_Format);
			this.Controls.Add(this.m_Output);
			this.Controls.Add(this.m_FilePath);
			this.Controls.Add(this.m_Go);
			this.MinimumSize = new System.Drawing.Size(373, 313);
			this.Name = "Example";
			this.Text = "Write To File Example";
			this.Load += new System.EventHandler(this.Example_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_Clear;
		private System.Windows.Forms.Button m_SelectFile;
		private System.Windows.Forms.ComboBox m_Format;
		private System.Windows.Forms.RichTextBox m_Output;
		private System.Windows.Forms.TextBox m_FilePath;
		private System.Windows.Forms.Button m_Go;
		private System.Windows.Forms.TextBox m_Message;
		private System.Windows.Forms.Button m_Write;
		private System.Windows.Forms.SaveFileDialog m_SaveFileDialog;
	}
}


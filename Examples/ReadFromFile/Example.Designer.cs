namespace ReadFromFile
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
			this.m_Go = new System.Windows.Forms.Button();
			this.m_FilePath = new System.Windows.Forms.TextBox();
			this.m_Output = new System.Windows.Forms.RichTextBox();
			this.m_Format = new System.Windows.Forms.ComboBox();
			this.m_SelectFile = new System.Windows.Forms.Button();
			this.m_OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.m_Clear = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_Go
			// 
			this.m_Go.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Go.Location = new System.Drawing.Point(453, 10);
			this.m_Go.Name = "m_Go";
			this.m_Go.Size = new System.Drawing.Size(75, 23);
			this.m_Go.TabIndex = 0;
			this.m_Go.Text = "Read";
			this.m_Go.UseVisualStyleBackColor = true;
			this.m_Go.Click += new System.EventHandler(this.m_Go_Click);
			// 
			// m_FilePath
			// 
			this.m_FilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_FilePath.Location = new System.Drawing.Point(13, 12);
			this.m_FilePath.Name = "m_FilePath";
			this.m_FilePath.Size = new System.Drawing.Size(320, 20);
			this.m_FilePath.TabIndex = 1;
			// 
			// m_Output
			// 
			this.m_Output.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Output.Location = new System.Drawing.Point(13, 40);
			this.m_Output.Name = "m_Output";
			this.m_Output.Size = new System.Drawing.Size(515, 256);
			this.m_Output.TabIndex = 2;
			this.m_Output.Text = "";
			// 
			// m_Format
			// 
			this.m_Format.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Format.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.m_Format.FormattingEnabled = true;
			this.m_Format.Location = new System.Drawing.Point(374, 11);
			this.m_Format.Name = "m_Format";
			this.m_Format.Size = new System.Drawing.Size(73, 21);
			this.m_Format.TabIndex = 3;
			this.m_Format.SelectedIndexChanged += new System.EventHandler(this.m_Format_SelectedIndexChanged);
			// 
			// m_SelectFile
			// 
			this.m_SelectFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_SelectFile.Location = new System.Drawing.Point(339, 10);
			this.m_SelectFile.Name = "m_SelectFile";
			this.m_SelectFile.Size = new System.Drawing.Size(29, 23);
			this.m_SelectFile.TabIndex = 4;
			this.m_SelectFile.Text = "...";
			this.m_SelectFile.UseVisualStyleBackColor = true;
			this.m_SelectFile.Click += new System.EventHandler(this.m_SelectFile_Click);
			// 
			// m_OpenFileDialog
			// 
			this.m_OpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.m_OpenFileDialog_FileOk);
			// 
			// m_Clear
			// 
			this.m_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Clear.Location = new System.Drawing.Point(13, 302);
			this.m_Clear.Name = "m_Clear";
			this.m_Clear.Size = new System.Drawing.Size(515, 23);
			this.m_Clear.TabIndex = 5;
			this.m_Clear.Text = "Clear";
			this.m_Clear.UseVisualStyleBackColor = true;
			this.m_Clear.Click += new System.EventHandler(this.m_Clear_Click);
			// 
			// Example
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(540, 337);
			this.Controls.Add(this.m_Clear);
			this.Controls.Add(this.m_SelectFile);
			this.Controls.Add(this.m_Format);
			this.Controls.Add(this.m_Output);
			this.Controls.Add(this.m_FilePath);
			this.Controls.Add(this.m_Go);
			this.MinimumSize = new System.Drawing.Size(373, 313);
			this.Name = "Example";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Read From File Example";
			this.Load += new System.EventHandler(this.Example_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_Go;
		private System.Windows.Forms.TextBox m_FilePath;
		private System.Windows.Forms.RichTextBox m_Output;
		private System.Windows.Forms.ComboBox m_Format;
		private System.Windows.Forms.Button m_SelectFile;
		private System.Windows.Forms.OpenFileDialog m_OpenFileDialog;
		private System.Windows.Forms.Button m_Clear;
	}
}


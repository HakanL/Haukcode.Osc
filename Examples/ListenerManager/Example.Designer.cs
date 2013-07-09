namespace ContainerManager
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
			this.m_MessageBox = new System.Windows.Forms.TextBox();
			this.m_Send = new System.Windows.Forms.Button();
			this.m_DataView = new System.Windows.Forms.DataGridView();
			this.m_Add = new System.Windows.Forms.Button();
			this.m_ListenerAddress = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.m_DataView)).BeginInit();
			this.SuspendLayout();
			// 
			// m_MessageBox
			// 
			this.m_MessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_MessageBox.Location = new System.Drawing.Point(12, 13);
			this.m_MessageBox.Name = "m_MessageBox";
			this.m_MessageBox.Size = new System.Drawing.Size(464, 20);
			this.m_MessageBox.TabIndex = 0;
			this.m_MessageBox.Text = "/test/*/thing_[0-9]";
			this.m_MessageBox.TextChanged += new System.EventHandler(this.MessageBox_TextChanged);
			// 
			// m_Send
			// 
			this.m_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Send.Location = new System.Drawing.Point(483, 11);
			this.m_Send.Name = "m_Send";
			this.m_Send.Size = new System.Drawing.Size(75, 23);
			this.m_Send.TabIndex = 1;
			this.m_Send.Text = "Parse";
			this.m_Send.UseVisualStyleBackColor = true;
			this.m_Send.Click += new System.EventHandler(this.Send_Click);
			// 
			// m_DataView
			// 
			this.m_DataView.AllowUserToAddRows = false;
			this.m_DataView.AllowUserToDeleteRows = false;
			this.m_DataView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_DataView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.m_DataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.m_DataView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.m_DataView.Location = new System.Drawing.Point(12, 40);
			this.m_DataView.Name = "m_DataView";
			this.m_DataView.RowTemplate.ReadOnly = true;
			this.m_DataView.Size = new System.Drawing.Size(546, 287);
			this.m_DataView.TabIndex = 2;
			// 
			// m_Add
			// 
			this.m_Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.m_Add.Location = new System.Drawing.Point(482, 333);
			this.m_Add.Name = "m_Add";
			this.m_Add.Size = new System.Drawing.Size(76, 23);
			this.m_Add.TabIndex = 4;
			this.m_Add.Text = "Add";
			this.m_Add.UseVisualStyleBackColor = true;
			this.m_Add.Click += new System.EventHandler(this.m_Add_Click);
			// 
			// m_ListenerAddress
			// 
			this.m_ListenerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_ListenerAddress.Location = new System.Drawing.Point(12, 335);
			this.m_ListenerAddress.Name = "m_ListenerAddress";
			this.m_ListenerAddress.Size = new System.Drawing.Size(464, 20);
			this.m_ListenerAddress.TabIndex = 5;
			this.m_ListenerAddress.Text = "/test/d/thing_0";
			this.m_ListenerAddress.TextChanged += new System.EventHandler(this.m_ListenerAddress_TextChanged);
			// 
			// Example
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(570, 368);
			this.Controls.Add(this.m_ListenerAddress);
			this.Controls.Add(this.m_Add);
			this.Controls.Add(this.m_DataView);
			this.Controls.Add(this.m_Send);
			this.Controls.Add(this.m_MessageBox);
			this.MinimumSize = new System.Drawing.Size(410, 273);
			this.Name = "Example";
			this.Text = "Listener Manager Example";
			this.Load += new System.EventHandler(this.Example_Load);
			((System.ComponentModel.ISupportInitialize)(this.m_DataView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox m_MessageBox;
		private System.Windows.Forms.Button m_Send;
		private System.Windows.Forms.DataGridView m_DataView;
		private System.Windows.Forms.Button m_Add;
		private System.Windows.Forms.TextBox m_ListenerAddress;
	}
}


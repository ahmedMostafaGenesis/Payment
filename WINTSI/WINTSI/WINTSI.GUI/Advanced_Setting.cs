using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ingenico.GUI
{

	public class Advanced_Setting : Form
	{
		public bool isCDEnabled;

		private IContainer components;

		private GroupBox groupBox1;

		private RadioButton DisableRbt;

		private RadioButton EnableRbt;

		private Button Cancel;

		private Button OK;

		public Advanced_Setting()
		{
			InitializeComponent();
		}

		private void OK_Click(object sender, EventArgs e)
		{
			if (EnableRbt.Checked)
			{
				isCDEnabled = true;
			}
			else
			{
				isCDEnabled = false;
			}

			Dispose();
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.DisableRbt = new System.Windows.Forms.RadioButton();
			this.EnableRbt = new System.Windows.Forms.RadioButton();
			this.Cancel = new System.Windows.Forms.Button();
			this.OK = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.groupBox1.Controls.Add(this.DisableRbt);
			this.groupBox1.Controls.Add(this.EnableRbt);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 94);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Cash Drawer";
			this.DisableRbt.AutoSize = true;
			this.DisableRbt.Checked = true;
			this.DisableRbt.Location = new System.Drawing.Point(19, 30);
			this.DisableRbt.Name = "DisableRbt";
			this.DisableRbt.Size = new System.Drawing.Size(60, 17);
			this.DisableRbt.TabIndex = 1;
			this.DisableRbt.TabStop = true;
			this.DisableRbt.Text = "Disable";
			this.DisableRbt.UseVisualStyleBackColor = true;
			this.EnableRbt.AutoSize = true;
			this.EnableRbt.Location = new System.Drawing.Point(19, 62);
			this.EnableRbt.Name = "EnableRbt";
			this.EnableRbt.Size = new System.Drawing.Size(58, 17);
			this.EnableRbt.TabIndex = 0;
			this.EnableRbt.Text = "Enable";
			this.EnableRbt.UseVisualStyleBackColor = true;
			this.Cancel.Location = new System.Drawing.Point(152, 119);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(60, 23);
			this.Cancel.TabIndex = 8;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(Cancel_Click);
			this.OK.Location = new System.Drawing.Point(86, 119);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(60, 23);
			this.OK.TabIndex = 9;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new System.EventHandler(OK_Click);
			base.AcceptButton = this.OK;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(225, 154);
			base.Controls.Add(this.Cancel);
			base.Controls.Add(this.OK);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "Advanced_Setting";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Advanced Setting";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
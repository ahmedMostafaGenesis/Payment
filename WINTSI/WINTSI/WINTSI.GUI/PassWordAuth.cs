using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ingenico.GUI
{



	public class PassWordAuth : Form
	{
		public bool isCorrectPassWord;

		private IContainer components;

		private TextBox Password;

		private Label EntManPW;

		private Button OK;

		private Button Cancel;

		private GroupBox groupBox1;

		public PassWordAuth()
		{
			InitializeComponent();
		}

		private void OK_Click(object sender, EventArgs e)
		{
			if (Password.Text.Equals("Ingenico2014"))
			{
				isCorrectPassWord = true;
				Dispose();
			}
			else
			{
				MessageBox.Show("Incorrect Password!", "Password Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Password.Text = "";
			}
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
			this.Password = new System.Windows.Forms.TextBox();
			this.EntManPW = new System.Windows.Forms.Label();
			this.OK = new System.Windows.Forms.Button();
			this.Cancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			this.Password.Location = new System.Drawing.Point(77, 29);
			this.Password.MaxLength = 24;
			this.Password.Name = "Password";
			this.Password.PasswordChar = '*';
			this.Password.Size = new System.Drawing.Size(134, 20);
			this.Password.TabIndex = 5;
			this.EntManPW.AutoSize = true;
			this.EntManPW.Location = new System.Drawing.Point(6, 33);
			this.EntManPW.Name = "EntManPW";
			this.EntManPW.Size = new System.Drawing.Size(53, 13);
			this.EntManPW.TabIndex = 4;
			this.EntManPW.Text = "Password";
			this.OK.Location = new System.Drawing.Point(85, 74);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(60, 23);
			this.OK.TabIndex = 7;
			this.OK.Text = "OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new System.EventHandler(OK_Click);
			this.Cancel.Location = new System.Drawing.Point(151, 74);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(60, 23);
			this.Cancel.TabIndex = 6;
			this.Cancel.Text = "Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(Cancel_Click);
			this.groupBox1.Controls.Add(this.Password);
			this.groupBox1.Controls.Add(this.Cancel);
			this.groupBox1.Controls.Add(this.EntManPW);
			this.groupBox1.Controls.Add(this.OK);
			this.groupBox1.Location = new System.Drawing.Point(8, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(222, 109);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sign In";
			base.AcceptButton = this.OK;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(239, 127);
			base.Controls.Add(this.groupBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PassWordAuth";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Authentication";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
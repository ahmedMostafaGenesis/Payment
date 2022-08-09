using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ingenico
{



	public class EditHtmlSourceForm : Form
	{
		private string m_htmlSource;

		private IContainer components;

		private RichTextBox HtmlSourceRichTextBox;

		private Button CloseButton;

		private Button SaveButton;

		public string HtmlSource
		{
			get { return m_htmlSource; }
			set { m_htmlSource = value; }
		}

		public EditHtmlSourceForm()
		{
			InitializeComponent();
		}

		private void EditHtmlSourceForm_Load(object sender, EventArgs e)
		{
			HtmlSourceRichTextBox.Text = m_htmlSource;
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			m_htmlSource = HtmlSourceRichTextBox.Text;
			base.DialogResult = DialogResult.OK;
			Close();
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
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
			this.HtmlSourceRichTextBox = new System.Windows.Forms.RichTextBox();
			this.CloseButton = new System.Windows.Forms.Button();
			this.SaveButton = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.HtmlSourceRichTextBox.DetectUrls = false;
			this.HtmlSourceRichTextBox.Location = new System.Drawing.Point(12, 12);
			this.HtmlSourceRichTextBox.Name = "HtmlSourceRichTextBox";
			this.HtmlSourceRichTextBox.Size = new System.Drawing.Size(481, 397);
			this.HtmlSourceRichTextBox.TabIndex = 0;
			this.HtmlSourceRichTextBox.Text = "";
			this.CloseButton.Location = new System.Drawing.Point(418, 415);
			this.CloseButton.Name = "CloseButton";
			this.CloseButton.Size = new System.Drawing.Size(75, 23);
			this.CloseButton.TabIndex = 1;
			this.CloseButton.Text = "Close";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(CloseButton_Click);
			this.SaveButton.Location = new System.Drawing.Point(337, 415);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(75, 23);
			this.SaveButton.TabIndex = 1;
			this.SaveButton.Text = "Save";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(SaveButton_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(505, 450);
			base.Controls.Add(this.SaveButton);
			base.Controls.Add(this.CloseButton);
			base.Controls.Add(this.HtmlSourceRichTextBox);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "EditHtmlSourceForm";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "HTML Source";
			base.Load += new System.EventHandler(EditHtmlSourceForm_Load);
			base.ResumeLayout(false);
		}
	}
}

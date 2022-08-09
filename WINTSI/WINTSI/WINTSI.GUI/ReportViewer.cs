using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Ingenico.GUI
{


	public class ReportViewer : Form
	{
		private string htmlPage;

		private IContainer components;

		private WebBrowser reportBrowser;

		public ReportViewer(string htmlPage)
		{
			InitializeComponent();
			this.htmlPage = htmlPage;
		}

		private void ReportViewer_Load(object sender, EventArgs e)
		{
			reportBrowser.DocumentText = htmlPage;
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
			this.reportBrowser = new System.Windows.Forms.WebBrowser();
			base.SuspendLayout();
			this.reportBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reportBrowser.Location = new System.Drawing.Point(0, 0);
			this.reportBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.reportBrowser.Name = "reportBrowser";
			this.reportBrowser.Size = new System.Drawing.Size(492, 666);
			this.reportBrowser.TabIndex = 0;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(492, 666);
			base.Controls.Add(this.reportBrowser);
			base.Name = "ReportViewer";
			base.ShowIcon = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ReportViewer";
			base.Load += new System.EventHandler(ReportViewer_Load);
			base.ResumeLayout(false);
		}
	}
}
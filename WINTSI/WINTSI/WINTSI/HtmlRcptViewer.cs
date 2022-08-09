using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Ingenico
{
	


public class HtmlRcptViewer : Form
{
	private WebBrowser merchantWebBrowser;

	private WebBrowser custWebBrowser;

	private WebBrowser formatRcptWebBrowser;

	private int merchIndex;

	private int custIndex;

	private readonly int rcptIndex;

	private const int MerchTabPageIndex = 0;

	private const int CustTabPageIndex = 1;

	private const int RcptTabPageIndex = 0;

	public string FormattedRcptName;

	private IContainer components;

	private TabControl tabControlReceipt;

	private TabPage tabPageMerchant;

	private TabPage tabPageCustomer;

	private TabPage tabPageFormattedRcpt;

	private Button saveButton;

	private Button viewSourceButton;

	private Button previousBtn;

	private Button nextBtn;

	public List<string> MerchantCopyHtml { get; set; }

	public List<string> CustomerCopyHtml { get; set; }

	public List<string> FormattedRcptCopyHtml { get; set; }

	public HtmlRcptViewer()
	{
		InitializeComponent();
	}

	private void PrepareHtml(ref WebBrowser htmlWebBrowser, List<string> listHtml, int htmlIndex, int selectedPageIndex)
	{
		htmlWebBrowser?.Hide();
		htmlWebBrowser = new WebBrowser();
		htmlWebBrowser.Size = new Size(435, 592);
		tabControlReceipt.TabPages[selectedPageIndex].Controls.Add(htmlWebBrowser);
		tabControlReceipt.SelectedIndex = selectedPageIndex;
		htmlWebBrowser.Show();
		htmlWebBrowser.DocumentText = listHtml[htmlIndex];
	}

	private void HtmlRcptViewer_Load(object sender, EventArgs e)
	{
		if (MerchantCopyHtml != null && MerchantCopyHtml.Count != 0)
		{
			PrepareHtml(ref merchantWebBrowser, MerchantCopyHtml, merchIndex, 0);
		}
		else
		{
			tabControlReceipt.TabPages.Remove(tabPageMerchant);
		}
		if (CustomerCopyHtml != null && CustomerCopyHtml.Count != 0)
		{
			if (tabControlReceipt.TabCount > 1)
			{
				PrepareHtml(ref custWebBrowser, CustomerCopyHtml, custIndex, 1);
			}
			else
			{
				PrepareHtml(ref custWebBrowser, CustomerCopyHtml, custIndex, 0);
			}
		}
		else
		{
			tabControlReceipt.TabPages.Remove(tabPageCustomer);
		}
		if (FormattedRcptCopyHtml != null && FormattedRcptCopyHtml.Count != 0)
		{
			tabControlReceipt.Controls.Add(tabPageFormattedRcpt);
			tabPageFormattedRcpt.Text = FormattedRcptName;
			nextBtn.Visible = false;
			previousBtn.Visible = false;
			PrepareHtml(ref formatRcptWebBrowser, FormattedRcptCopyHtml, rcptIndex, 0);
		}
		else
		{
			tabControlReceipt.TabPages.Remove(tabPageFormattedRcpt);
		}
	}

	private void SaveButton_Click(object sender, EventArgs e)
	{
		var saveFileDialog = new SaveFileDialog();
		saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		saveFileDialog.Title = "Save Receipt ...";
		saveFileDialog.Filter = "HTML files(*.html; *.HTML)| *.html; *.HTML";
		if (tabControlReceipt.SelectedTab == tabPageCustomer)
		{
			for (var i = 0; i < CustomerCopyHtml.Count; i++)
			{
				saveFileDialog.FileName = "CustomerReceipt_" + (i + 1);
				if (saveFileDialog.ShowDialog(this) != DialogResult.OK) continue;
				var fileName = saveFileDialog.FileName;
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
				var streamWriter = new StreamWriter(fileName);
				streamWriter.Write(CustomerCopyHtml[i]);
				streamWriter.Close();
			}
			return;
		}
		if (tabControlReceipt.SelectedTab == tabPageMerchant)
		{
			for (var j = 0; j < MerchantCopyHtml.Count; j++)
			{
				saveFileDialog.FileName = "MerchantReceipt_" + (j + 1);
				if (saveFileDialog.ShowDialog(this) != DialogResult.OK) continue;
				var fileName2 = saveFileDialog.FileName;
				if (File.Exists(fileName2))
				{
					File.Delete(fileName2);
				}
				var streamWriter2 = new StreamWriter(fileName2);
				streamWriter2.Write(MerchantCopyHtml[j]);
				streamWriter2.Close();
			}
			return;
		}
		saveFileDialog.FileName = "RqstFormatReceipt_1";
		if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;
		var fileName3 = saveFileDialog.FileName;
		if (File.Exists(fileName3))
		{
			File.Delete(fileName3);
		}
		var streamWriter3 = new StreamWriter(fileName3);
		streamWriter3.Write(FormattedRcptCopyHtml[0]);
		streamWriter3.Close();
	}

	private void CloseButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void ViewSourceButton_Click(object sender, EventArgs e)
	{
		var editHtmlSourceForm = new EditHtmlSourceForm();
		if (tabControlReceipt.SelectedTab == tabPageCustomer)
		{
			if (custWebBrowser != null)
			{
				editHtmlSourceForm.HtmlSource = custWebBrowser.DocumentText;
			}
		}
		else if (tabControlReceipt.SelectedTab == tabPageMerchant)
		{
			if (merchantWebBrowser != null)
			{
				editHtmlSourceForm.HtmlSource = merchantWebBrowser.DocumentText;
			}
		}
		else if (formatRcptWebBrowser != null)
		{
			editHtmlSourceForm.HtmlSource = formatRcptWebBrowser.DocumentText;
		}

		if (editHtmlSourceForm.HtmlSource == null || editHtmlSourceForm.ShowDialog(this) != DialogResult.OK) return;
		if (tabControlReceipt.SelectedTab == tabPageCustomer)
		{
			if (custWebBrowser == null) return;
			custWebBrowser.DocumentText = editHtmlSourceForm.HtmlSource;
			CustomerCopyHtml[custWebBrowser.TabIndex] = editHtmlSourceForm.HtmlSource;
		}
		else if (tabControlReceipt.SelectedTab == tabPageMerchant)
		{
			if (merchantWebBrowser == null) return;
			merchantWebBrowser.DocumentText = editHtmlSourceForm.HtmlSource;
			MerchantCopyHtml[merchantWebBrowser.TabIndex] = editHtmlSourceForm.HtmlSource;
		}
		else
		{
			if (formatRcptWebBrowser == null) return;
			formatRcptWebBrowser.DocumentText = editHtmlSourceForm.HtmlSource;
			FormattedRcptCopyHtml[formatRcptWebBrowser.TabIndex] = editHtmlSourceForm.HtmlSource;
		}
	}

	private void nextBtn_Click(object sender, EventArgs e)
	{
		if (tabControlReceipt.SelectedIndex == 0)
		{
			merchIndex++;
			if (merchIndex < MerchantCopyHtml.Count && MerchantCopyHtml[merchIndex] != null)
			{
				PrepareHtml(ref merchantWebBrowser, MerchantCopyHtml, merchIndex, 0);
			}
			else
			{
				merchIndex = MerchantCopyHtml.Count - 1;
			}
		}

		if (tabControlReceipt.SelectedIndex != 1) return;
		custIndex++;
		if (custIndex < CustomerCopyHtml.Count && CustomerCopyHtml[custIndex] != null)
		{
			PrepareHtml(ref custWebBrowser, CustomerCopyHtml, custIndex, 1);
		}
		else
		{
			custIndex = CustomerCopyHtml.Count - 1;
		}
	}

	private void PreviousBtn_Click(object sender, EventArgs e)
	{
		if (tabControlReceipt.SelectedIndex == 0 && merchIndex > 0)
		{
			merchIndex--;
			if (merchIndex < MerchantCopyHtml.Count && MerchantCopyHtml[merchIndex] != null)
			{
				PrepareHtml(ref merchantWebBrowser, MerchantCopyHtml, merchIndex, 0);
			}
		}
		if (tabControlReceipt.SelectedIndex == 1 && custIndex > 0)
		{
			custIndex--;
			if (custIndex < CustomerCopyHtml.Count && CustomerCopyHtml[custIndex] != null)
			{
				PrepareHtml(ref custWebBrowser, CustomerCopyHtml, custIndex, 1);
			}
		}
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
		tabControlReceipt = new TabControl();
		tabPageMerchant = new TabPage();
		tabPageCustomer = new TabPage();
		tabPageFormattedRcpt = new TabPage();
		saveButton = new Button();
		viewSourceButton = new Button();
		previousBtn = new Button();
		nextBtn = new Button();
		tabControlReceipt.SuspendLayout();
		SuspendLayout();
		tabControlReceipt.Controls.Add(tabPageMerchant);
		tabControlReceipt.Controls.Add(tabPageCustomer);
		tabControlReceipt.Location = new Point(13, 2);
		tabControlReceipt.Name = "tabControlReceipt";
		tabControlReceipt.SelectedIndex = 0;
		tabControlReceipt.Size = new Size(443, 618);
		tabControlReceipt.TabIndex = 0;
		tabPageMerchant.Location = new Point(4, 22);
		tabPageMerchant.Name = "tabPageMerchant";
		tabPageMerchant.Size = new Size(435, 592);
		tabPageMerchant.TabIndex = 0;
		tabPageMerchant.Text = "Merchant Copy";
		tabPageMerchant.UseVisualStyleBackColor = true;
		tabPageCustomer.Location = new Point(4, 22);
		tabPageCustomer.Name = "tabPageCustomer";
		tabPageCustomer.Size = new Size(435, 592);
		tabPageCustomer.TabIndex = 1;
		tabPageCustomer.Text = "Customer Copy";
		tabPageCustomer.UseVisualStyleBackColor = true;
		tabPageFormattedRcpt.Location = new Point(4, 22);
		tabPageFormattedRcpt.Name = "tabPageFormattedRcpt";
		tabPageFormattedRcpt.Size = new Size(435, 592);
		tabPageFormattedRcpt.TabIndex = 2;
		tabPageFormattedRcpt.UseVisualStyleBackColor = true;
		saveButton.Location = new Point(276, 626);
		saveButton.Name = "saveButton";
		saveButton.Size = new Size(75, 23);
		saveButton.TabIndex = 1;
		saveButton.Text = "Save";
		saveButton.UseVisualStyleBackColor = true;
		saveButton.Click += SaveButton_Click;
		viewSourceButton.Location = new Point(377, 626);
		viewSourceButton.Name = "viewSourceButton";
		viewSourceButton.Size = new Size(75, 23);
		viewSourceButton.TabIndex = 1;
		viewSourceButton.Text = "View source";
		viewSourceButton.UseVisualStyleBackColor = true;
		viewSourceButton.Click += ViewSourceButton_Click;
		previousBtn.Location = new Point(14, 626);
		previousBtn.Name = "previousBtn";
		previousBtn.Size = new Size(75, 23);
		previousBtn.TabIndex = 5;
		previousBtn.Text = "Previous";
		previousBtn.UseVisualStyleBackColor = true;
		previousBtn.Click += PreviousBtn_Click;
		nextBtn.Location = new Point(98, 626);
		nextBtn.Name = "nextBtn";
		nextBtn.Size = new Size(75, 23);
		nextBtn.TabIndex = 4;
		nextBtn.Text = "Next";
		nextBtn.UseVisualStyleBackColor = true;
		nextBtn.Click += nextBtn_Click;
		AcceptButton = saveButton;
		AutoScaleDimensions = new SizeF(6f, 13f);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(468, 661);
		Controls.Add(previousBtn);
		Controls.Add(nextBtn);
		Controls.Add(viewSourceButton);
		Controls.Add(saveButton);
		Controls.Add(tabControlReceipt);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "HtmlRcptViewer";
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;
		Text = "HTML Viewer";
		Load += HtmlRcptViewer_Load;
		tabControlReceipt.ResumeLayout(false);
		ResumeLayout(false);
	}
}
}
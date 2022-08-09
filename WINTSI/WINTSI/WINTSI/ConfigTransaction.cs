#define TRACE
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Ingenico
{
public class ConfigTransaction : Form
{
	private class allTags
	{
		public string name;

		public ColumnHeader element;

		public allTags(string name, ColumnHeader element)
		{
			this.name = name;
			this.element = element;
		}
	}

	private allTags[] allTheTags;

	private IContainer components;

	private Button cancel;

	private Button Add;

	private Button DeleteButton;

	public ListView CSNListView;

	private ColumnHeader columnHeader1;

	private ColumnHeader columnHeader2;

	private ColumnHeader columnHeader3;

	private Button Edit;

	private ColumnHeader columnHeader4;

	private ColumnHeader columnHeader5;

	private ColumnHeader columnHeader6;

	private ColumnHeader columnHeader7;

	private ColumnHeader columnHeader8;

	private ColumnHeader columnHeader9;

	private ColumnHeader columnHeader10;

	private ColumnHeader columnHeader11;

	private ColumnHeader columnHeader12;

	private ColumnHeader columnHeader13;

	private ColumnHeader columnHeader14;

	private ColumnHeader columnHeader15;

	private ColumnHeader columnHeader16;

	private ColumnHeader columnHeader17;

	private ColumnHeader columnHeader18;

	private ColumnHeader columnHeader19;

	private ColumnHeader columnHeader20;

	private ColumnHeader columnHeader21;

	private ColumnHeader columnHeader22;

	private ColumnHeader columnHeader23;

	private ColumnHeader columnHeader24;

	private ColumnHeader columnHeader25;

	private ColumnHeader columnHeader26;

	private ColumnHeader columnHeader27;

	private ColumnHeader columnHeader28;

	private ColumnHeader columnHeader29;

	private void allTagsInit()
	{
		allTheTags = new allTags[28];
		allTheTags[0] = new allTags("Amount", CSNListView.Columns[2]);
		allTheTags[1] = new allTags("TenderType", CSNListView.Columns[3]);
		allTheTags[2] = new allTags("ClerkId", CSNListView.Columns[4]);
		allTheTags[3] = new allTags("InvoiceNum", CSNListView.Columns[5]);
		allTheTags[4] = new allTags("Authorization", CSNListView.Columns[6]);
		allTheTags[5] = new allTags("OrigSeq", CSNListView.Columns[7]);
		allTheTags[6] = new allTags("OrigRef", CSNListView.Columns[8]);
		allTheTags[7] = new allTags("TraceNum", CSNListView.Columns[9]);
		allTheTags[8] = new allTags("ReprintType", CSNListView.Columns[10]);
		allTheTags[9] = new allTags("ParameterType", CSNListView.Columns[11]);
		allTheTags[10] = new allTags("CustRefNum", CSNListView.Columns[12]);
		allTheTags[11] = new allTags("ForcedUP", CSNListView.Columns[13]);
		allTheTags[12] = new allTags("DCC", CSNListView.Columns[14]);
		allTheTags[13] = new allTags("TranType", CSNListView.Columns[15]);
		allTheTags[14] = new allTags("RefNumber", CSNListView.Columns[16]);
		allTheTags[15] = new allTags("PAN", CSNListView.Columns[17]);
		allTheTags[16] = new allTags("SpecificData", CSNListView.Columns[18]);
		allTheTags[17] = new allTags("FormattedRcpt", CSNListView.Columns[19]);
		allTheTags[18] = new allTags("RcptName", CSNListView.Columns[20]);
		allTheTags[19] = new allTags("MerchURL", CSNListView.Columns[21]);
		allTheTags[20] = new allTags("MerchID", CSNListView.Columns[22]);
		allTheTags[21] = new allTags("FilterCateg", CSNListView.Columns[23]);
		allTheTags[22] = new allTags("EncryptReq", CSNListView.Columns[24]);
		allTheTags[23] = new allTags("VasMode", CSNListView.Columns[25]);
		allTheTags[24] = new allTags("MerchIndex", CSNListView.Columns[26]);
		allTheTags[25] = new allTags("EccKey", CSNListView.Columns[27]);
		allTheTags[26] = new allTags("FinalAmount", CSNListView.Columns[28]);
	}

	public ConfigTransaction()
	{
		InitializeComponent();
		allTagsInit();
	}

	private void cancel_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void Add_Click(object sender, EventArgs e)
	{
		new AddTransaction(null).ShowDialog(this);
		updateListView();
	}

	private void ConfigTransaction_Load(object sender, EventArgs e)
	{
		updateListView();
	}

	public void updateListView()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		XmlNode xmlNode = null;
		CSNListView.Items.Clear();
		try
		{
			while (xmlTextReader.Read())
			{
				if (!xmlTextReader.Name.Equals("TransactionType"))
				{
					continue;
				}
				ListViewItem listViewItem = new ListViewItem();
				xmlNode = xmlDocument.ReadNode(xmlTextReader);
				if (xmlNode.Attributes["Name"] == null)
				{
					continue;
				}
				listViewItem.Text = xmlNode.Attributes["Name"].Value;
				if (xmlNode.Attributes["Value"] == null)
				{
					continue;
				}
				listViewItem.SubItems.Add(xmlNode.Attributes["Value"].Value);
				string text = "";
				for (int i = 0; allTheTags[i] != null; i++)
				{
					if (xmlNode.Attributes[allTheTags[i].name] != null)
					{
						if (allTheTags[i].element.Width == 0)
						{
							allTheTags[i].element.Width = -2;
						}
						if (xmlNode.Attributes[allTheTags[i].name].Value == "ON")
						{
							text = "X";
						}
					}
					listViewItem.SubItems.Add(text);
					text = "";
				}
				CSNListView.Items.Add(listViewItem);
			}
		}
		catch
		{
			Trace.WriteLine("Problem updateListView");
		}
		xmlTextReader.Close();
		Edit.Enabled = false;
		DeleteButton.Enabled = false;
	}

	private void Edit_Click(object sender, EventArgs e)
	{
		new AddTransaction(CSNListView.SelectedItems[0].SubItems[0].Text).ShowDialog(this);
		updateListView();
	}

	private void CSNListView_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (CSNListView.SelectedItems.Count != 0)
		{
			Edit.Enabled = true;
			DeleteButton.Enabled = true;
		}
		else
		{
			Edit.Enabled = false;
			DeleteButton.Enabled = false;
		}
	}

	private void DeleteButton_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Are you sure you want to delete the selected Transaction Type?", "Delete Transaction Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.Load(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			XmlNode xmlNode = xmlDocument.SelectSingleNode("Config/TransactionType[@Name='" + CSNListView.SelectedItems[0].SubItems[0].Text + "']");
			if (xmlNode != null)
			{
				xmlNode.ParentNode.RemoveChild(xmlNode);
				xmlDocument.Save(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			}
			updateListView();
		}
		catch
		{
			Trace.WriteLine("Impossible to load cfg file");
		}
	}

	private void CSNListView_DoubleClick(object sender, EventArgs e)
	{
		if (CSNListView.SelectedItems.Count != 0)
		{
			new AddTransaction(CSNListView.SelectedItems[0].SubItems[0].Text).ShowDialog(this);
			updateListView();
		}
	}

	private void CSNListView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
	{
		e.Cancel = true;
		e.NewWidth = CSNListView.Columns[e.ColumnIndex].Width;
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
		this.cancel = new System.Windows.Forms.Button();
		this.Add = new System.Windows.Forms.Button();
		this.DeleteButton = new System.Windows.Forms.Button();
		this.CSNListView = new System.Windows.Forms.ListView();
		this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader14 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader15 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader16 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader17 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader18 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader19 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader20 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader21 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader22 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader23 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader24 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader25 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader26 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader27 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader28 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader29 = new System.Windows.Forms.ColumnHeader();
		this.Edit = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.cancel.Location = new System.Drawing.Point(692, 112);
		this.cancel.Name = "cancel";
		this.cancel.Size = new System.Drawing.Size(75, 23);
		this.cancel.TabIndex = 5;
		this.cancel.Text = "Cancel";
		this.cancel.UseVisualStyleBackColor = true;
		this.cancel.Click += new System.EventHandler(cancel_Click);
		this.Add.Location = new System.Drawing.Point(692, 18);
		this.Add.Name = "Add";
		this.Add.Size = new System.Drawing.Size(75, 23);
		this.Add.TabIndex = 2;
		this.Add.Text = "Add";
		this.Add.UseVisualStyleBackColor = true;
		this.Add.Click += new System.EventHandler(Add_Click);
		this.DeleteButton.Location = new System.Drawing.Point(692, 78);
		this.DeleteButton.Name = "DeleteButton";
		this.DeleteButton.Size = new System.Drawing.Size(75, 25);
		this.DeleteButton.TabIndex = 4;
		this.DeleteButton.Text = "Delete";
		this.DeleteButton.UseVisualStyleBackColor = true;
		this.DeleteButton.Click += new System.EventHandler(DeleteButton_Click);
		this.CSNListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[29]
		{
			this.columnHeader1, this.columnHeader2, this.columnHeader3, this.columnHeader4, this.columnHeader5, this.columnHeader6, this.columnHeader7, this.columnHeader8, this.columnHeader9, this.columnHeader10,
			this.columnHeader11, this.columnHeader12, this.columnHeader13, this.columnHeader14, this.columnHeader15, this.columnHeader16, this.columnHeader17, this.columnHeader18, this.columnHeader19, this.columnHeader20,
			this.columnHeader21, this.columnHeader22, this.columnHeader23, this.columnHeader24, this.columnHeader25, this.columnHeader26, this.columnHeader27, this.columnHeader28, this.columnHeader29
		});
		this.CSNListView.FullRowSelect = true;
		this.CSNListView.GridLines = true;
		this.CSNListView.HideSelection = false;
		this.CSNListView.Location = new System.Drawing.Point(12, 18);
		this.CSNListView.MultiSelect = false;
		this.CSNListView.Name = "CSNListView";
		this.CSNListView.Size = new System.Drawing.Size(674, 194);
		this.CSNListView.TabIndex = 15;
		this.CSNListView.UseCompatibleStateImageBehavior = false;
		this.CSNListView.View = System.Windows.Forms.View.Details;
		this.CSNListView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(CSNListView_ColumnWidthChanging);
		this.CSNListView.SelectedIndexChanged += new System.EventHandler(CSNListView_SelectedIndexChanged);
		this.CSNListView.DoubleClick += new System.EventHandler(CSNListView_DoubleClick);
		this.columnHeader1.Text = "Transaction Name";
		this.columnHeader1.Width = 120;
		this.columnHeader2.Text = "Value";
		this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader3.Text = "Amount";
		this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader3.Width = 0;
		this.columnHeader4.Text = "Tender";
		this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader4.Width = 0;
		this.columnHeader5.Text = "Clk Id";
		this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader5.Width = 0;
		this.columnHeader6.Text = "Invoice #";
		this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader6.Width = 0;
		this.columnHeader7.Text = "Auth";
		this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader7.Width = 0;
		this.columnHeader8.Text = "Orig Seq";
		this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader8.Width = 0;
		this.columnHeader9.Text = "Orig Ref";
		this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader9.Width = 0;
		this.columnHeader10.Text = "Trace #";
		this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader10.Width = 0;
		this.columnHeader11.Text = "Reprint";
		this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader11.Width = 0;
		this.columnHeader12.Text = "Parameter";
		this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader12.Width = 0;
		this.columnHeader13.Text = "Cust Ref #";
		this.columnHeader13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader13.Width = 0;
		this.columnHeader14.Text = "ForcedUp";
		this.columnHeader14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader14.Width = 0;
		this.columnHeader15.Text = "DCC Trans";
		this.columnHeader15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader15.Width = 0;
		this.columnHeader16.Text = "Orig TnxType";
		this.columnHeader16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader16.Width = 0;
		this.columnHeader17.Text = "Ref #";
		this.columnHeader17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader17.Width = 0;
		this.columnHeader18.Text = "PAN";
		this.columnHeader18.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader18.Width = 0;
		this.columnHeader19.Text = "Specific Data";
		this.columnHeader19.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader19.Width = 0;
		this.columnHeader20.DisplayIndex = 19;
		this.columnHeader20.Text = "Formatted Rcpt";
		this.columnHeader20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader20.Width = 0;
		this.columnHeader21.DisplayIndex = 20;
		this.columnHeader21.Text = "RcptName";
		this.columnHeader21.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader21.Width = 0;
		this.columnHeader22.DisplayIndex = 21;
		this.columnHeader22.Text = "Merch URL";
		this.columnHeader22.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader22.Width = 0;
		this.columnHeader23.DisplayIndex = 22;
		this.columnHeader23.Text = "Merch ID";
		this.columnHeader23.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader23.Width = 0;
		this.columnHeader24.DisplayIndex = 23;
		this.columnHeader24.Text = "Filter Category";
		this.columnHeader24.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader24.Width = 0;
		this.columnHeader25.DisplayIndex = 24;
		this.columnHeader25.Text = "Encrypt Req";
		this.columnHeader25.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader25.Width = 0;
		this.columnHeader26.DisplayIndex = 25;
		this.columnHeader26.Text = "Vas Mode";
		this.columnHeader26.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader26.Width = 0;
		this.columnHeader27.DisplayIndex = 26;
		this.columnHeader27.Text = "Merch Index";
		this.columnHeader27.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader27.Width = 0;
		this.columnHeader28.DisplayIndex = 27;
		this.columnHeader28.Text = "Vas Ecc Key";
		this.columnHeader28.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader28.Width = 0;
		this.Edit.Location = new System.Drawing.Point(692, 49);
		this.Edit.Name = "Edit";
		this.Edit.Size = new System.Drawing.Size(75, 23);
		this.Edit.TabIndex = 3;
		this.Edit.Text = "Edit";
		this.Edit.UseVisualStyleBackColor = true;
		this.Edit.Click += new System.EventHandler(Edit_Click);
		this.columnHeader29.DisplayIndex = 28;
		this.columnHeader29.Text = "Final Amount";
		this.columnHeader29.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
		this.columnHeader29.Width = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.ControlLight;
		base.ClientSize = new System.Drawing.Size(775, 224);
		base.Controls.Add(this.Edit);
		base.Controls.Add(this.CSNListView);
		base.Controls.Add(this.DeleteButton);
		base.Controls.Add(this.cancel);
		base.Controls.Add(this.Add);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ConfigTransaction";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Transaction Type Configuration";
		base.Load += new System.EventHandler(ConfigTransaction_Load);
		base.ResumeLayout(false);
	}
}
}
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
public class ConfigTender : Form
{
	private IContainer components;

	private Button cancel;

	private Button add;

	private Button deleteButton;

	public ListView CsnListView;

	private ColumnHeader columnHeader1;

	private ColumnHeader columnHeader2;

	private Button edit;

	public ConfigTender()
	{
		InitializeComponent();
	}

	private void cancel_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void Add_Click(object sender, EventArgs e)
	{
		new AddTender(null).ShowDialog(this);
		UpdateListView();
	}

	private void ConfigTender_Load(object sender, EventArgs e)
	{
		UpdateListView();
	}

	private void UpdateListView()
	{
		var xmlDocument = new XmlDocument();
		var xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		CsnListView.Items.Clear();
		try
		{
			while (xmlTextReader.Read())
			{
				if (!xmlTextReader.Name.Equals("TenderType"))
				{
					continue;
				}
				var listViewItem = new ListViewItem();
				var xmlNode = xmlDocument.ReadNode(xmlTextReader);
				if (!xmlNode.Attributes.Item(0).Name.Equals("Name")) continue;
				listViewItem.Text = xmlNode.Attributes.Item(0).Value;
				if (!xmlNode.Attributes.Item(1).Name.Equals("Value")) continue;
				listViewItem.SubItems.Add(xmlNode.Attributes.Item(1).Value);
				CsnListView.Items.Add(listViewItem);
			}
		}
		catch
		{
			Trace.WriteLine("Problem updateListView");
		}
		xmlTextReader.Close();
		edit.Enabled = false;
		deleteButton.Enabled = false;
	}

	private void Edit_Click(object sender, EventArgs e)
	{
		new AddTender(CsnListView.SelectedItems[0].SubItems[0].Text).ShowDialog(this);
		UpdateListView();
	}

	private void CSNListView_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (CsnListView.SelectedItems.Count != 0)
		{
			edit.Enabled = true;
			deleteButton.Enabled = true;
		}
		else
		{
			edit.Enabled = false;
			deleteButton.Enabled = false;
		}
	}

	private void DeleteButton_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Are you sure you want to delete the selected Tender Type?", "Delete Tender Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		var xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.Load(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			var xmlNode = xmlDocument.SelectSingleNode("Config/TenderType[@Name='" + CsnListView.SelectedItems[0].SubItems[0].Text + "']");
			if (xmlNode != null)
			{
				xmlNode.ParentNode?.RemoveChild(xmlNode);
				xmlDocument.Save("Cfg.xml");
			}
			UpdateListView();
		}
		catch
		{
			Trace.WriteLine("Impossible to load cfg file");
		}
	}

	private void CSNListView_DoubleClick(object sender, EventArgs e)
	{
		if (CsnListView.SelectedItems.Count == 0) return;
		new AddTender(CsnListView.SelectedItems[0].SubItems[0].Text).ShowDialog(this);
		UpdateListView();
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
		cancel = new Button();
		add = new Button();
		deleteButton = new Button();
		CsnListView = new ListView();
		columnHeader1 = new ColumnHeader();
		columnHeader2 = new ColumnHeader();
		edit = new Button();
		SuspendLayout();
		cancel.Location = new Point(302, 111);
		cancel.Name = "cancel";
		cancel.Size = new Size(75, 23);
		cancel.TabIndex = 5;
		cancel.Text = "Cancel";
		cancel.UseVisualStyleBackColor = true;
		cancel.Click += cancel_Click;
		add.Location = new Point(302, 19);
		add.Name = "add";
		add.Size = new Size(75, 23);
		add.TabIndex = 2;
		add.Text = "Add";
		add.UseVisualStyleBackColor = true;
		add.Click += Add_Click;
		deleteButton.Location = new Point(302, 79);
		deleteButton.Name = "deleteButton";
		deleteButton.Size = new Size(75, 25);
		deleteButton.TabIndex = 4;
		deleteButton.Text = "Delete";
		deleteButton.UseVisualStyleBackColor = true;
		deleteButton.Click += DeleteButton_Click;
		CsnListView.Columns.AddRange(new ColumnHeader[2] { columnHeader1, columnHeader2 });
		CsnListView.FullRowSelect = true;
		CsnListView.GridLines = true;
		CsnListView.HideSelection = false;
		CsnListView.Location = new Point(12, 19);
		CsnListView.MultiSelect = false;
		CsnListView.Name = "CsnListView";
		CsnListView.Size = new Size(284, 135);
		CsnListView.TabIndex = 15;
		CsnListView.UseCompatibleStateImageBehavior = false;
		CsnListView.View = View.Details;
		CsnListView.SelectedIndexChanged += CSNListView_SelectedIndexChanged;
		CsnListView.DoubleClick += CSNListView_DoubleClick;
		columnHeader1.Text = "Tender Name";
		columnHeader1.Width = 160;
		columnHeader2.Text = "Value";
		columnHeader2.TextAlign = HorizontalAlignment.Center;
		columnHeader2.Width = 113;
		edit.Location = new Point(302, 49);
		edit.Name = "edit";
		edit.Size = new Size(75, 23);
		edit.TabIndex = 3;
		edit.Text = "Edit";
		edit.UseVisualStyleBackColor = true;
		edit.Click += Edit_Click;
		AutoScaleDimensions = new SizeF(6f, 13f);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlLight;
		ClientSize = new Size(383, 166);
		Controls.Add(edit);
		Controls.Add(CsnListView);
		Controls.Add(deleteButton);
		Controls.Add(cancel);
		Controls.Add(add);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		MaximizeBox = false;
		MinimizeBox = false;
		Name = "ConfigTender";
		ShowIcon = false;
		ShowInTaskbar = false;
		StartPosition = FormStartPosition.CenterParent;
		Text = "Tender Type Configuration";
		Load += ConfigTender_Load;
		ResumeLayout(false);
	}
}
}
#define TRACE
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;


public class AddTender : Form
{
	private string TenderTypeName;

	private IContainer components;

	private Label label1;

	private Label label3;

	private Button OKButton;

	private TextBox TenderName;

	private TextBox TenderValue;

	private Button Cancel;

	public AddTender(string CName)
	{
		TenderTypeName = CName;
		InitializeComponent();
	}

	private void AddTender_Load(object sender, EventArgs e)
	{
		if (TenderTypeName != null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			XmlNode xmlNode = null;
			try
			{
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.Name.Equals("TenderType"))
					{
						new ListViewItem();
						xmlNode = xmlDocument.ReadNode(xmlTextReader);
						if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(0).Value.ToString().Equals(TenderTypeName))
						{
							TenderName.Text = TenderTypeName;
							TenderValue.Text = xmlNode.Attributes.Item(1).Value.ToString();
						}
					}
				}
			}
			catch
			{
				Trace.WriteLine("Problem AddTender_Load");
			}
			xmlTextReader.Close();
			Text = "Edit Tender Type";
			TenderName.ReadOnly = true;
		}
		else
		{
			Text = "Add Tender Type";
		}
	}

	private bool TenderTypeExist(string TenderName)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		XmlNode xmlNode = null;
		try
		{
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.Name.Equals("TenderType"))
				{
					xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(0).Value.ToString().ToUpper().Equals(TenderName.ToUpper()))
					{
						xmlTextReader.Close();
						return true;
					}
				}
			}
		}
		catch
		{
			Trace.WriteLine("Problem AddTender_Load");
		}
		xmlTextReader.Close();
		return false;
	}

	private void OKButton_Click(object sender, EventArgs e)
	{
		if (TenderTypeName != null)
		{
			if (TenderName.Text.Length == 0)
			{
				MessageBox.Show("Invalid Tender Name!", "Add Tender Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (TenderValue.Text.Length == 0)
			{
				MessageBox.Show("Invalid Tender Value!", "Add Tender Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			_ = string.Empty;
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
				XmlNode xmlNode = xmlDocument.SelectSingleNode("Config/TenderType[@Name='" + TenderTypeName + "']");
				if (xmlNode != null)
				{
					xmlNode.Attributes[0].Value = TenderName.Text;
					xmlNode.Attributes[1].Value = TenderValue.Text;
					xmlDocument.Save(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
				}
			}
			catch
			{
				Trace.WriteLine("Impossible to load cfg file");
			}
			Dispose();
		}
		else if (TenderName.Text.Length == 0)
		{
			MessageBox.Show("Invalid Tender Name!", "Add Tender Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else if (TenderValue.Text.Length == 0)
		{
			MessageBox.Show("Invalid Tender Value!", "Add Tender Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else if (TenderTypeExist(TenderName.Text))
		{
			MessageBox.Show("Tender Type already used!", "Add Tender Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			XmlDocument xmlDocument2 = new XmlDocument();
			try
			{
				xmlDocument2.Load(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
				XmlNode xmlNode2 = xmlDocument2.SelectSingleNode("Config");
				XmlNode xmlNode3 = xmlDocument2.CreateNode(XmlNodeType.Element, "TenderType", null);
				XmlAttribute xmlAttribute = xmlDocument2.CreateAttribute("Name");
				xmlAttribute.Value = TenderName.Text;
				XmlAttribute xmlAttribute2 = xmlDocument2.CreateAttribute("Value");
				xmlAttribute2.Value = TenderValue.Text;
				xmlNode3.Attributes.Append(xmlAttribute);
				xmlNode3.Attributes.Append(xmlAttribute2);
				xmlNode2.AppendChild(xmlNode3);
				xmlDocument2.Save(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			}
			catch
			{
				Trace.WriteLine("Impossible to load cfg file");
			}
			Dispose();
		}
	}

	private void CancelButton_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void Cancel_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void TextBoxNum_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar != '\b')
		{
			e.Handled = !char.IsDigit(e.KeyChar);
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
		this.label1 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.OKButton = new System.Windows.Forms.Button();
		this.TenderName = new System.Windows.Forms.TextBox();
		this.TenderValue = new System.Windows.Forms.TextBox();
		this.Cancel = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 20);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(75, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Tender Name:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(51, 54);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(37, 13);
		this.label3.TabIndex = 1;
		this.label3.Text = "Value:";
		this.OKButton.Location = new System.Drawing.Point(113, 90);
		this.OKButton.Name = "OKButton";
		this.OKButton.Size = new System.Drawing.Size(64, 22);
		this.OKButton.TabIndex = 4;
		this.OKButton.Text = "OK";
		this.OKButton.UseVisualStyleBackColor = true;
		this.OKButton.Click += new System.EventHandler(OKButton_Click);
		this.TenderName.Location = new System.Drawing.Point(93, 17);
		this.TenderName.Name = "TenderName";
		this.TenderName.Size = new System.Drawing.Size(148, 20);
		this.TenderName.TabIndex = 1;
		this.TenderValue.Location = new System.Drawing.Point(92, 51);
		this.TenderValue.MaxLength = 1;
		this.TenderValue.Name = "TenderValue";
		this.TenderValue.Size = new System.Drawing.Size(148, 20);
		this.TenderValue.TabIndex = 3;
		this.TenderValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNum_KeyPress);
		this.Cancel.Location = new System.Drawing.Point(180, 90);
		this.Cancel.Name = "Cancel";
		this.Cancel.Size = new System.Drawing.Size(61, 22);
		this.Cancel.TabIndex = 5;
		this.Cancel.Text = "Cancel";
		this.Cancel.UseVisualStyleBackColor = true;
		this.Cancel.Click += new System.EventHandler(Cancel_Click);
		base.AcceptButton = this.OKButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.ControlLight;
		base.ClientSize = new System.Drawing.Size(258, 126);
		base.Controls.Add(this.Cancel);
		base.Controls.Add(this.TenderValue);
		base.Controls.Add(this.TenderName);
		base.Controls.Add(this.OKButton);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddTender";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Tender Type";
		base.Load += new System.EventHandler(AddTender_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}

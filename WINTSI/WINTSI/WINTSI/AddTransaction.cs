#define TRACE
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;


public class AddTransaction : Form
{
	private class allTags
	{
		public string name;

		public CheckBox element;

		public allTags(string name, CheckBox element)
		{
			this.name = name;
			this.element = element;
		}
	}

	private string TransName;

	private int x;

	private int y = -10;

	private allTags[] allTheTags;

	private IContainer components;

	private Label label1;

	private Label label3;

	private Button OKButton;

	private TextBox TransactionName;

	private TextBox TransactionValue;

	private Button Cancel;

	private CheckBox checkBoxAmount;

	private CheckBox checkBoxTender;

	private CheckBox checkBoxClkId;

	private CheckBox checkBoxInvoiceNum;

	private CheckBox checkBoxAuth;

	private GroupBox groupBox1;

	private GroupBox groupBox2;

	private CheckBox checkBoxOrigRef;

	private CheckBox checkBoxOrigSeq;

	private CheckBox checkBoxTraceNum;

	private CheckBox checkBoxParameterType;

	private CheckBox checkBoxReprintType;

	private CheckBox checkBoxCustRef;

	private CheckBox checkBoxForcedUp;

	private CheckBox checkBoxDCCTrans;

	private CheckBox checkBoxPAN;

	private CheckBox checkBoxRefNum;

	private CheckBox checkBoxTranType;

	private CheckBox chkSpecificData;

	private CheckBox checkBoxFormattedRcpt;

	private CheckBox checkBoxRcptName;

	private CheckBox chkMerchURL;

	private CheckBox chkMerchID;

	private CheckBox chkFilterCateg;

	private CheckBox chkEncryptReq;

	private CheckBox chkVasMode;

	private CheckBox chkMerchIndex;

	private CheckBox chkEccKey;

	private CheckBox ChkFinalAmount;

	private void allTagsInit()
	{
		allTheTags = new allTags[27];
		allTheTags[0] = new allTags("Amount", checkBoxAmount);
		allTheTags[1] = new allTags("Authorization", checkBoxAuth);
		allTheTags[2] = new allTags("ClerkId", checkBoxClkId);
		allTheTags[3] = new allTags("CustRefNum", checkBoxCustRef);
		allTheTags[4] = new allTags("DCC", checkBoxDCCTrans);
		allTheTags[5] = new allTags("ForcedUP", checkBoxForcedUp);
		allTheTags[6] = new allTags("InvoiceNum", checkBoxInvoiceNum);
		allTheTags[7] = new allTags("OrigRef", checkBoxOrigRef);
		allTheTags[8] = new allTags("OrigSeq", checkBoxOrigSeq);
		allTheTags[9] = new allTags("PAN", checkBoxPAN);
		allTheTags[10] = new allTags("ParameterType", checkBoxParameterType);
		allTheTags[11] = new allTags("RefNumber", checkBoxRefNum);
		allTheTags[12] = new allTags("ReprintType", checkBoxReprintType);
		allTheTags[13] = new allTags("TenderType", checkBoxTender);
		allTheTags[14] = new allTags("TraceNum", checkBoxTraceNum);
		allTheTags[15] = new allTags("TranType", checkBoxTranType);
		allTheTags[16] = new allTags("SpecificData", chkSpecificData);
		allTheTags[17] = new allTags("FormattedRcpt", checkBoxFormattedRcpt);
		allTheTags[18] = new allTags("RcptName", checkBoxRcptName);
		allTheTags[19] = new allTags("MerchURL", chkMerchURL);
		allTheTags[20] = new allTags("MerchID", chkMerchID);
		allTheTags[21] = new allTags("FilterCateg", chkFilterCateg);
		allTheTags[22] = new allTags("EncryptReq", chkEncryptReq);
		allTheTags[23] = new allTags("VasMode", chkVasMode);
		allTheTags[24] = new allTags("MerchIndex", chkMerchIndex);
		allTheTags[25] = new allTags("EccKey", chkEccKey);
		allTheTags[26] = new allTags("FinalAmount", ChkFinalAmount);
	}

	public AddTransaction(string CName)
	{
		TransName = CName;
		InitializeComponent();
		allTagsInit();
	}

	private void SetComboBoxPosition(CheckBox checkBox)
	{
		if (x == 30)
		{
			x = 158;
		}
		else
		{
			x = 30;
			y += 34;
		}
		checkBox.Location = new Point(x, y);
	}

	private void AddTransaction_LoadForm(string transactionName)
	{
		bool flag = false;
		if (transactionName == null)
		{
			transactionName = "Sale";
			flag = true;
		}
		XmlDocument xmlDocument = new XmlDocument();
		XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		XmlNode xmlNode = null;
		try
		{
			while (xmlTextReader.Read())
			{
				if (!xmlTextReader.Name.Equals("TransactionType"))
				{
					continue;
				}
				xmlNode = xmlDocument.ReadNode(xmlTextReader);
				if (xmlNode.Attributes["Name"] == null || !(xmlNode.Attributes["Name"].Value == transactionName))
				{
					continue;
				}
				if (!flag)
				{
					TransactionName.Text = transactionName;
					TransactionValue.Text = xmlNode.Attributes["Value"].Value;
				}
				for (int i = 0; allTheTags[i] != null; i++)
				{
					if (xmlNode.Attributes[allTheTags[i].name] != null)
					{
						if (!allTheTags[i].element.Visible)
						{
							SetComboBoxPosition(allTheTags[i].element);
							allTheTags[i].element.Visible = true;
						}
						if (xmlNode.Attributes[allTheTags[i].name].Value == "ON" && !flag)
						{
							allTheTags[i].element.Checked = true;
						}
					}
				}
			}
		}
		catch
		{
			Trace.WriteLine("Problem AddTransaction_Load");
		}
		xmlTextReader.Close();
		OKButton.Top = groupBox1.Location.Y + groupBox1.Size.Height + 15;
		Cancel.Top = OKButton.Top;
	}

	private void AddTransaction_Load(object sender, EventArgs e)
	{
		AddTransaction_LoadForm(TransName);
		if (TransName != null)
		{
			Text = "Edit Transaction Type";
			TransactionName.ReadOnly = true;
		}
		else
		{
			Text = "Add Transaction Type";
		}
	}

	private bool TransactionExist(string TenderName)
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		XmlNode xmlNode = null;
		try
		{
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.Name.Equals("TransactionType"))
				{
					new ListViewItem();
					xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(0).Value.ToString().Equals(TenderName, StringComparison.OrdinalIgnoreCase))
					{
						xmlTextReader.Close();
						return true;
					}
				}
			}
		}
		catch
		{
			Trace.WriteLine("Problem TransactionExist");
		}
		xmlTextReader.Close();
		return false;
	}

	private void OKButton_Click(object sender, EventArgs e)
	{
		if (TransName != null)
		{
			if (TransactionName.Text.Length == 0)
			{
				MessageBox.Show("Invalid Transaction Type!", "Add Transaction Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			if (TransactionValue.Text.Length == 0)
			{
				MessageBox.Show("Invalid Transaction value Format!", "Add Transaction Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			_ = string.Empty;
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.Load(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
				XmlNode xmlNode = xmlDocument.SelectSingleNode("Config/TransactionType[@Name='" + TransName + "']");
				if (xmlNode != null)
				{
					xmlNode.Attributes["Name"].Value = TransactionName.Text;
					xmlNode.Attributes["Value"].Value = TransactionValue.Text;
					int num = 0;
					while (allTheTags[num] != null)
					{
						if (xmlNode.Attributes[allTheTags[num].name] == null && allTheTags[num].element.Visible)
						{
							XmlAttribute node = xmlDocument.CreateAttribute(allTheTags[num].name);
							xmlNode.Attributes.Append(node);
						}
						if (xmlNode.Attributes[allTheTags[num].name] != null)
						{
							if (allTheTags[num].element.Checked)
							{
								xmlNode.Attributes[allTheTags[num].name].Value = "ON";
							}
							else
							{
								xmlNode.Attributes[allTheTags[num].name].Value = "OFF";
							}
						}
						num++;
						xmlDocument.Save(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
					}
				}
			}
			catch
			{
				Trace.WriteLine("Impossible to load cfg file");
			}
			Dispose();
			return;
		}
		if (TransactionName.Text.Length == 0)
		{
			MessageBox.Show("Invalid Transaction Type!", "Add Transaction Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (TransactionValue.Text.Length == 0)
		{
			MessageBox.Show("Invalid Transaction value Format!", "Add Transaction Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (TransactionExist(TransactionName.Text))
		{
			MessageBox.Show("Transaction Name already used!", "Add Transaction Type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		XmlDocument xmlDocument2 = new XmlDocument();
		try
		{
			xmlDocument2.Load(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			XmlNode xmlNode2 = xmlDocument2.SelectSingleNode("Config");
			XmlNode xmlNode3 = xmlDocument2.CreateNode(XmlNodeType.Element, "TransactionType", null);
			XmlAttribute xmlAttribute = xmlDocument2.CreateAttribute("Name");
			xmlAttribute.Value = TransactionName.Text;
			xmlNode3.Attributes.Append(xmlAttribute);
			XmlAttribute xmlAttribute2 = xmlDocument2.CreateAttribute("Value");
			xmlAttribute2.Value = TransactionValue.Text;
			xmlNode3.Attributes.Append(xmlAttribute2);
			for (int i = 0; allTheTags[i] != null; i++)
			{
				XmlAttribute xmlAttribute3 = xmlDocument2.CreateAttribute(allTheTags[i].name);
				if (allTheTags[i].element.Visible)
				{
					if (allTheTags[i].element.Checked)
					{
						xmlAttribute3.Value = "ON";
					}
					else
					{
						xmlAttribute3.Value = "OFF";
					}
					xmlNode3.Attributes.Append(xmlAttribute3);
				}
			}
			xmlNode2.AppendChild(xmlNode3);
			xmlDocument2.Save(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		}
		catch
		{
			Trace.WriteLine("Problem in Add Transaction");
		}
		Dispose();
	}

	private void Cancel_Click(object sender, EventArgs e)
	{
		Dispose();
	}

	private void TransactionValue_KeyPress(object sender, KeyPressEventArgs e)
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
		this.TransactionName = new System.Windows.Forms.TextBox();
		this.TransactionValue = new System.Windows.Forms.TextBox();
		this.Cancel = new System.Windows.Forms.Button();
		this.checkBoxAmount = new System.Windows.Forms.CheckBox();
		this.checkBoxTender = new System.Windows.Forms.CheckBox();
		this.checkBoxClkId = new System.Windows.Forms.CheckBox();
		this.checkBoxInvoiceNum = new System.Windows.Forms.CheckBox();
		this.checkBoxAuth = new System.Windows.Forms.CheckBox();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.checkBoxRcptName = new System.Windows.Forms.CheckBox();
		this.checkBoxFormattedRcpt = new System.Windows.Forms.CheckBox();
		this.chkSpecificData = new System.Windows.Forms.CheckBox();
		this.checkBoxPAN = new System.Windows.Forms.CheckBox();
		this.checkBoxRefNum = new System.Windows.Forms.CheckBox();
		this.checkBoxTranType = new System.Windows.Forms.CheckBox();
		this.checkBoxDCCTrans = new System.Windows.Forms.CheckBox();
		this.checkBoxForcedUp = new System.Windows.Forms.CheckBox();
		this.checkBoxCustRef = new System.Windows.Forms.CheckBox();
		this.checkBoxParameterType = new System.Windows.Forms.CheckBox();
		this.checkBoxReprintType = new System.Windows.Forms.CheckBox();
		this.checkBoxTraceNum = new System.Windows.Forms.CheckBox();
		this.checkBoxOrigRef = new System.Windows.Forms.CheckBox();
		this.checkBoxOrigSeq = new System.Windows.Forms.CheckBox();
		this.chkMerchURL = new System.Windows.Forms.CheckBox();
		this.chkMerchID = new System.Windows.Forms.CheckBox();
		this.chkFilterCateg = new System.Windows.Forms.CheckBox();
		this.chkEncryptReq = new System.Windows.Forms.CheckBox();
		this.chkVasMode = new System.Windows.Forms.CheckBox();
		this.chkEccKey = new System.Windows.Forms.CheckBox();
		this.chkMerchIndex = new System.Windows.Forms.CheckBox();
		this.ChkFinalAmount = new System.Windows.Forms.CheckBox();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.groupBox1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(16, 23);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(68, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Trans Name:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(16, 53);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(37, 13);
		this.label3.TabIndex = 1;
		this.label3.Text = "Value:";
		this.OKButton.Location = new System.Drawing.Point(139, 459);
		this.OKButton.Name = "OKButton";
		this.OKButton.Size = new System.Drawing.Size(64, 22);
		this.OKButton.TabIndex = 4;
		this.OKButton.Text = "OK";
		this.OKButton.UseVisualStyleBackColor = true;
		this.OKButton.Click += new System.EventHandler(OKButton_Click);
		this.TransactionName.Location = new System.Drawing.Point(86, 20);
		this.TransactionName.Name = "TransactionName";
		this.TransactionName.Size = new System.Drawing.Size(148, 20);
		this.TransactionName.TabIndex = 1;
		this.TransactionValue.Location = new System.Drawing.Point(86, 50);
		this.TransactionValue.MaxLength = 2;
		this.TransactionValue.Name = "TransactionValue";
		this.TransactionValue.Size = new System.Drawing.Size(148, 20);
		this.TransactionValue.TabIndex = 3;
		this.TransactionValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TransactionValue_KeyPress);
		this.Cancel.Location = new System.Drawing.Point(208, 459);
		this.Cancel.Name = "Cancel";
		this.Cancel.Size = new System.Drawing.Size(61, 22);
		this.Cancel.TabIndex = 5;
		this.Cancel.Text = "Cancel";
		this.Cancel.UseVisualStyleBackColor = true;
		this.Cancel.Click += new System.EventHandler(Cancel_Click);
		this.checkBoxAmount.AutoSize = true;
		this.checkBoxAmount.Location = new System.Drawing.Point(24, 20);
		this.checkBoxAmount.Name = "checkBoxAmount";
		this.checkBoxAmount.Size = new System.Drawing.Size(62, 17);
		this.checkBoxAmount.TabIndex = 13;
		this.checkBoxAmount.Text = "Amount";
		this.checkBoxAmount.UseVisualStyleBackColor = true;
		this.checkBoxAmount.Visible = false;
		this.checkBoxTender.AutoSize = true;
		this.checkBoxTender.Location = new System.Drawing.Point(157, 20);
		this.checkBoxTender.Name = "checkBoxTender";
		this.checkBoxTender.Size = new System.Drawing.Size(87, 17);
		this.checkBoxTender.TabIndex = 14;
		this.checkBoxTender.Text = "Tender Type";
		this.checkBoxTender.UseVisualStyleBackColor = true;
		this.checkBoxTender.Visible = false;
		this.checkBoxClkId.AutoSize = true;
		this.checkBoxClkId.Location = new System.Drawing.Point(24, 53);
		this.checkBoxClkId.Name = "checkBoxClkId";
		this.checkBoxClkId.Size = new System.Drawing.Size(68, 17);
		this.checkBoxClkId.TabIndex = 15;
		this.checkBoxClkId.Text = "Clerck Id";
		this.checkBoxClkId.UseVisualStyleBackColor = true;
		this.checkBoxClkId.Visible = false;
		this.checkBoxInvoiceNum.AutoSize = true;
		this.checkBoxInvoiceNum.Location = new System.Drawing.Point(157, 53);
		this.checkBoxInvoiceNum.Name = "checkBoxInvoiceNum";
		this.checkBoxInvoiceNum.Size = new System.Drawing.Size(71, 17);
		this.checkBoxInvoiceNum.TabIndex = 16;
		this.checkBoxInvoiceNum.Text = "Invoice #";
		this.checkBoxInvoiceNum.UseVisualStyleBackColor = true;
		this.checkBoxInvoiceNum.Visible = false;
		this.checkBoxAuth.AutoSize = true;
		this.checkBoxAuth.Location = new System.Drawing.Point(24, 86);
		this.checkBoxAuth.Name = "checkBoxAuth";
		this.checkBoxAuth.Size = new System.Drawing.Size(87, 17);
		this.checkBoxAuth.TabIndex = 18;
		this.checkBoxAuth.Text = "Authorization";
		this.checkBoxAuth.UseVisualStyleBackColor = true;
		this.checkBoxAuth.Visible = false;
		this.groupBox1.AutoSize = true;
		this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.groupBox1.Controls.Add(this.ChkFinalAmount);
		this.groupBox1.Controls.Add(this.checkBoxRcptName);
		this.groupBox1.Controls.Add(this.checkBoxFormattedRcpt);
		this.groupBox1.Controls.Add(this.chkSpecificData);
		this.groupBox1.Controls.Add(this.checkBoxPAN);
		this.groupBox1.Controls.Add(this.checkBoxRefNum);
		this.groupBox1.Controls.Add(this.checkBoxTranType);
		this.groupBox1.Controls.Add(this.checkBoxDCCTrans);
		this.groupBox1.Controls.Add(this.checkBoxForcedUp);
		this.groupBox1.Controls.Add(this.checkBoxCustRef);
		this.groupBox1.Controls.Add(this.checkBoxParameterType);
		this.groupBox1.Controls.Add(this.checkBoxReprintType);
		this.groupBox1.Controls.Add(this.checkBoxTraceNum);
		this.groupBox1.Controls.Add(this.checkBoxOrigRef);
		this.groupBox1.Controls.Add(this.checkBoxOrigSeq);
		this.groupBox1.Controls.Add(this.checkBoxClkId);
		this.groupBox1.Controls.Add(this.checkBoxAuth);
		this.groupBox1.Controls.Add(this.checkBoxAmount);
		this.groupBox1.Controls.Add(this.checkBoxInvoiceNum);
		this.groupBox1.Controls.Add(this.checkBoxTender);
		this.groupBox1.Controls.Add(this.chkMerchURL);
		this.groupBox1.Controls.Add(this.chkMerchID);
		this.groupBox1.Controls.Add(this.chkFilterCateg);
		this.groupBox1.Controls.Add(this.chkEncryptReq);
		this.groupBox1.Controls.Add(this.chkVasMode);
		this.groupBox1.Controls.Add(this.chkEccKey);
		this.groupBox1.Controls.Add(this.chkMerchIndex);
		this.groupBox1.Location = new System.Drawing.Point(15, 98);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(253, 446);
		this.groupBox1.TabIndex = 19;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "ECR Fields";
		this.checkBoxRcptName.AutoSize = true;
		this.checkBoxRcptName.Location = new System.Drawing.Point(24, 311);
		this.checkBoxRcptName.Name = "checkBoxRcptName";
		this.checkBoxRcptName.Size = new System.Drawing.Size(77, 17);
		this.checkBoxRcptName.TabIndex = 34;
		this.checkBoxRcptName.Text = "RcptName";
		this.checkBoxRcptName.UseVisualStyleBackColor = true;
		this.checkBoxRcptName.Visible = false;
		this.checkBoxFormattedRcpt.AutoSize = true;
		this.checkBoxFormattedRcpt.Location = new System.Drawing.Point(24, 366);
		this.checkBoxFormattedRcpt.Name = "checkBoxFormattedRcpt";
		this.checkBoxFormattedRcpt.Size = new System.Drawing.Size(96, 17);
		this.checkBoxFormattedRcpt.TabIndex = 33;
		this.checkBoxFormattedRcpt.Text = "FormattedRcpt";
		this.checkBoxFormattedRcpt.UseVisualStyleBackColor = true;
		this.checkBoxFormattedRcpt.Visible = false;
		this.chkSpecificData.AutoSize = true;
		this.chkSpecificData.Location = new System.Drawing.Point(24, 284);
		this.chkSpecificData.Name = "chkSpecificData";
		this.chkSpecificData.Size = new System.Drawing.Size(90, 17);
		this.chkSpecificData.TabIndex = 32;
		this.chkSpecificData.Text = "Specific Data";
		this.chkSpecificData.UseVisualStyleBackColor = true;
		this.chkSpecificData.Visible = false;
		this.checkBoxPAN.AutoSize = true;
		this.checkBoxPAN.Location = new System.Drawing.Point(157, 251);
		this.checkBoxPAN.Name = "checkBoxPAN";
		this.checkBoxPAN.Size = new System.Drawing.Size(48, 17);
		this.checkBoxPAN.TabIndex = 31;
		this.checkBoxPAN.Text = "PAN";
		this.checkBoxPAN.UseVisualStyleBackColor = true;
		this.checkBoxPAN.Visible = false;
		this.checkBoxRefNum.AutoSize = true;
		this.checkBoxRefNum.Location = new System.Drawing.Point(24, 251);
		this.checkBoxRefNum.Name = "checkBoxRefNum";
		this.checkBoxRefNum.Size = new System.Drawing.Size(53, 17);
		this.checkBoxRefNum.TabIndex = 30;
		this.checkBoxRefNum.Text = "Ref #";
		this.checkBoxRefNum.UseVisualStyleBackColor = true;
		this.checkBoxRefNum.Visible = false;
		this.checkBoxTranType.AutoSize = true;
		this.checkBoxTranType.Location = new System.Drawing.Point(157, 218);
		this.checkBoxTranType.Name = "checkBoxTranType";
		this.checkBoxTranType.Size = new System.Drawing.Size(90, 17);
		this.checkBoxTranType.TabIndex = 29;
		this.checkBoxTranType.Text = "Orig TnxType";
		this.checkBoxTranType.UseVisualStyleBackColor = true;
		this.checkBoxTranType.Visible = false;
		this.checkBoxDCCTrans.AutoSize = true;
		this.checkBoxDCCTrans.Location = new System.Drawing.Point(24, 218);
		this.checkBoxDCCTrans.Name = "checkBoxDCCTrans";
		this.checkBoxDCCTrans.Size = new System.Drawing.Size(78, 17);
		this.checkBoxDCCTrans.TabIndex = 28;
		this.checkBoxDCCTrans.Text = "DCC Trans";
		this.checkBoxDCCTrans.UseVisualStyleBackColor = true;
		this.checkBoxDCCTrans.Visible = false;
		this.checkBoxForcedUp.AutoSize = true;
		this.checkBoxForcedUp.Location = new System.Drawing.Point(157, 185);
		this.checkBoxForcedUp.Name = "checkBoxForcedUp";
		this.checkBoxForcedUp.Size = new System.Drawing.Size(76, 17);
		this.checkBoxForcedUp.TabIndex = 27;
		this.checkBoxForcedUp.Text = "Forced Up";
		this.checkBoxForcedUp.UseVisualStyleBackColor = true;
		this.checkBoxForcedUp.Visible = false;
		this.checkBoxCustRef.AutoSize = true;
		this.checkBoxCustRef.Location = new System.Drawing.Point(157, 86);
		this.checkBoxCustRef.Name = "checkBoxCustRef";
		this.checkBoxCustRef.Size = new System.Drawing.Size(77, 17);
		this.checkBoxCustRef.TabIndex = 26;
		this.checkBoxCustRef.Text = "Cust Ref #";
		this.checkBoxCustRef.UseVisualStyleBackColor = true;
		this.checkBoxCustRef.Visible = false;
		this.checkBoxParameterType.AutoSize = true;
		this.checkBoxParameterType.Location = new System.Drawing.Point(157, 152);
		this.checkBoxParameterType.Name = "checkBoxParameterType";
		this.checkBoxParameterType.Size = new System.Drawing.Size(83, 17);
		this.checkBoxParameterType.TabIndex = 25;
		this.checkBoxParameterType.Text = "Param Type";
		this.checkBoxParameterType.UseVisualStyleBackColor = true;
		this.checkBoxParameterType.Visible = false;
		this.checkBoxReprintType.AutoSize = true;
		this.checkBoxReprintType.Location = new System.Drawing.Point(24, 152);
		this.checkBoxReprintType.Name = "checkBoxReprintType";
		this.checkBoxReprintType.Size = new System.Drawing.Size(87, 17);
		this.checkBoxReprintType.TabIndex = 24;
		this.checkBoxReprintType.Text = "Reprint Type";
		this.checkBoxReprintType.UseVisualStyleBackColor = true;
		this.checkBoxReprintType.Visible = false;
		this.checkBoxTraceNum.AutoSize = true;
		this.checkBoxTraceNum.Location = new System.Drawing.Point(24, 185);
		this.checkBoxTraceNum.Name = "checkBoxTraceNum";
		this.checkBoxTraceNum.Size = new System.Drawing.Size(64, 17);
		this.checkBoxTraceNum.TabIndex = 21;
		this.checkBoxTraceNum.Text = "Trace #";
		this.checkBoxTraceNum.UseVisualStyleBackColor = true;
		this.checkBoxTraceNum.Visible = false;
		this.checkBoxOrigRef.AutoSize = true;
		this.checkBoxOrigRef.Location = new System.Drawing.Point(157, 119);
		this.checkBoxOrigRef.Name = "checkBoxOrigRef";
		this.checkBoxOrigRef.Size = new System.Drawing.Size(65, 17);
		this.checkBoxOrigRef.TabIndex = 20;
		this.checkBoxOrigRef.Text = "Orig Ref";
		this.checkBoxOrigRef.UseVisualStyleBackColor = true;
		this.checkBoxOrigRef.Visible = false;
		this.checkBoxOrigSeq.AutoSize = true;
		this.checkBoxOrigSeq.Location = new System.Drawing.Point(24, 119);
		this.checkBoxOrigSeq.Name = "checkBoxOrigSeq";
		this.checkBoxOrigSeq.Size = new System.Drawing.Size(67, 17);
		this.checkBoxOrigSeq.TabIndex = 19;
		this.checkBoxOrigSeq.Text = "Orig Seq";
		this.checkBoxOrigSeq.UseVisualStyleBackColor = true;
		this.checkBoxOrigSeq.Visible = false;
		this.chkMerchURL.AutoSize = true;
		this.chkMerchURL.Location = new System.Drawing.Point(157, 278);
		this.chkMerchURL.Name = "chkMerchURL";
		this.chkMerchURL.Size = new System.Drawing.Size(81, 17);
		this.chkMerchURL.TabIndex = 33;
		this.chkMerchURL.Text = "Merch URL";
		this.chkMerchURL.UseVisualStyleBackColor = true;
		this.chkMerchURL.Visible = false;
		this.chkMerchID.AutoSize = true;
		this.chkMerchID.Location = new System.Drawing.Point(24, 310);
		this.chkMerchID.Name = "chkMerchID";
		this.chkMerchID.Size = new System.Drawing.Size(70, 17);
		this.chkMerchID.TabIndex = 34;
		this.chkMerchID.Text = "Merch ID";
		this.chkMerchID.Visible = false;
		this.chkFilterCateg.AutoSize = true;
		this.chkFilterCateg.Location = new System.Drawing.Point(157, 310);
		this.chkFilterCateg.Name = "chkFilterCateg";
		this.chkFilterCateg.Size = new System.Drawing.Size(79, 17);
		this.chkFilterCateg.TabIndex = 35;
		this.chkFilterCateg.Text = "Filter Categ";
		this.chkFilterCateg.UseVisualStyleBackColor = true;
		this.chkFilterCateg.Visible = false;
		this.chkEncryptReq.AutoSize = true;
		this.chkEncryptReq.Location = new System.Drawing.Point(24, 342);
		this.chkEncryptReq.Name = "chkEncryptReq";
		this.chkEncryptReq.Size = new System.Drawing.Size(85, 17);
		this.chkEncryptReq.TabIndex = 36;
		this.chkEncryptReq.Text = "Encrypt Req";
		this.chkEncryptReq.UseVisualStyleBackColor = true;
		this.chkEncryptReq.Visible = false;
		this.chkVasMode.AutoSize = true;
		this.chkVasMode.Location = new System.Drawing.Point(157, 342);
		this.chkVasMode.Name = "chkVasMode";
		this.chkVasMode.Size = new System.Drawing.Size(74, 17);
		this.chkVasMode.TabIndex = 37;
		this.chkVasMode.Text = "Vas Mode";
		this.chkVasMode.UseVisualStyleBackColor = true;
		this.chkVasMode.Visible = false;
		this.chkEccKey.AutoSize = true;
		this.chkEccKey.Location = new System.Drawing.Point(157, 410);
		this.chkEccKey.Name = "chkEccKey";
		this.chkEccKey.Size = new System.Drawing.Size(90, 17);
		this.chkEccKey.TabIndex = 41;
		this.chkEccKey.Text = "VAS Ecc Key";
		this.chkEccKey.UseVisualStyleBackColor = true;
		this.chkEccKey.Visible = false;
		this.chkMerchIndex.AutoSize = true;
		this.chkMerchIndex.Location = new System.Drawing.Point(155, 366);
		this.chkMerchIndex.Name = "chkMerchIndex";
		this.chkMerchIndex.Size = new System.Drawing.Size(85, 17);
		this.chkMerchIndex.TabIndex = 39;
		this.chkMerchIndex.Text = "Merch Index";
		this.chkMerchIndex.UseVisualStyleBackColor = true;
		this.chkMerchIndex.Visible = false;
		this.groupBox2.Controls.Add(this.label1);
		this.groupBox2.Controls.Add(this.label3);
		this.groupBox2.Controls.Add(this.TransactionName);
		this.groupBox2.Controls.Add(this.TransactionValue);
		this.groupBox2.Location = new System.Drawing.Point(15, 9);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(250, 83);
		this.groupBox2.TabIndex = 20;
		this.groupBox2.TabStop = false;
		this.ChkFinalAmount.AutoSize = true;
		this.ChkFinalAmount.Location = new System.Drawing.Point(75, 33);
		this.ChkFinalAmount.Name = "ChkFinalAmount";
		this.ChkFinalAmount.Size = new System.Drawing.Size(87, 17);
		this.ChkFinalAmount.TabIndex = 42;
		this.ChkFinalAmount.Text = "Final Amount";
		this.ChkFinalAmount.UseVisualStyleBackColor = true;
		this.ChkFinalAmount.Visible = false;
		base.AcceptButton = this.OKButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.BackColor = System.Drawing.SystemColors.ControlLight;
		base.ClientSize = new System.Drawing.Size(281, 490);
		base.Controls.Add(this.groupBox2);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.Cancel);
		base.Controls.Add(this.OKButton);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AddTransaction";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Add Transaction Type";
		base.Load += new System.EventHandler(AddTransaction_Load);
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}

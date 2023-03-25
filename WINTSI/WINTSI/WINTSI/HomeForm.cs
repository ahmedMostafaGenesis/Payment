#define TRACE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Ingenico.GUI;
using Ingenico.Properties;
using Ingenico.Reports;
using Ingenico.Tools;
using WINTSI.WebSocket;

namespace Ingenico
{
	public class HomeForm : Form
	{
		private List<byte[]> merchImgBytesBuffer;

		private List<byte[]> custImgBytesBuffer;

		private List<byte[]> formatRcptImgStringBuffer;

		private List<string> merchHtmlStringBuffer;

		private List<string> custHtmlStringBuffer;

		private List<string> formatRcptHtmlStringBuffer;

		private ImgRcptViewer imgReceiptViewerForm;

		private HtmlRcptViewer htmlReceiptViewerForm;

		private readonly SoundPlayer myPlayer;

		private readonly ApplicationProtocol applicationProtocol;

		private readonly CultureInfo culture = CultureInfo.CurrentUICulture;

		private ComSettingForm comSettingForm1;

		public readonly Communication Com;

		private readonly System.Windows.Forms.Timer timerLatePrintResp = new();

		private readonly int iParamType;

		private string currentTrxType = "";

		private int nbreOfRecord = -1;

		private bool bScriptStarted;

		private bool bScriptStopped;

		private int scriptLoopValue = 1;

		private int currentLoopNmb = 1;

		private List<Command> commandList;

		private int indexCmd;

		private List<Dictionary<int, string>> listDicoOfReport = new List<Dictionary<int, string>>();

		private IContainer components;

		private GroupBox transactionGroupBox;

		private GroupBox traceGroupBox;

		private ComboBox tnxType;

		private Label tnxTypeLabel;

		public RichTextBox Trace;

		private Button sendRequest;

		private readonly List<byte[]> rxBufferData = new();

		private readonly List<byte[]> rxBufferPrint = new();

		private System.Windows.Forms.Timer timer1;

		private Button resetAll;

		private MenuStrip menuStripHome;

		private ToolStripMenuItem mainToolStripMenu;

		public ToolStripMenuItem SaveTraceMenu;

		private ToolStripMenuItem exit;

		private ToolStripMenuItem settingToolStripMenuItem;

		private ToolStripMenuItem help;

		private ToolStripMenuItem userManualToolStripMenuItem;

		private ToolStripSeparator toolStripSeparator5;

		private ToolStripMenuItem aboutToolStripMenuItem;

		private ToolStripMenuItem settingTraceToolStripMenuItem;

		private ToolStripMenuItem enableToolStripMenuItem;

		private ToolStripMenuItem disableToolStripMenuItem;

		private SaveFileDialog saveTrace;

		private TextBox amount;

		private ComboBox tenderType;

		private Label tenderTypeLabel;

		private ToolStripMenuItem exportReceiptToolStripMenuItem;

		private SaveFileDialog saveReceipt;

		private ToolStripMenuItem communicationToolStripMenuItem;

		private DataGridViewTextBoxColumn valueColumn;

		public RichTextBox Ticket;

		private Button clearReceiptButton;

		private GroupBox receiptResponseGroupBox;

		private GroupBox excepTestgroupBox;

		private Panel panel2;

		private Label printingLabel;

		public RadioButton LatePrintRadioButton;

		public RadioButton TimeOutPrintRadioButton;

		public RadioButton NokPrintRadioButton;

		public RadioButton NonePrintRadioButton;

		private Panel panel1;

		public RadioButton RequestGabageRadioButton;

		private Label protocolLabel;

		public RadioButton LateAckRadioButton;

		public RadioButton TimeOutAckRadioButton;

		public RadioButton BadLrcRadioButton;

		public RadioButton NakRadioButton;

		public RadioButton NoneRadioButton;

		private Panel panel3;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel toolStripStatusLabel2;

		private ToolStripStatusLabel comSettingStatusLabel;

		private ToolStripStatusLabel toolStripStatusLabel1;

		private ToolStripMenuItem transactionConfigToolStripMenuItem;

		private ToolStripMenuItem tenderTypeToolStripMenuItem;

		private ToolStripMenuItem importConfigMenu;

		private ToolStripMenuItem importTransTypeMenu;

		private ToolStripMenuItem exportConfigMenu;

		private OpenFileDialog openConfig;

		private OpenFileDialog openSciptFileDialog;

		private ToolStripMenuItem importTagListMenu;

		private SaveFileDialog saveConfig;

		private ToolStripMenuItem autoSaveOnExitToolStripMenuItem;

		private ToolStripMenuItem offToolStripMenuItem;

		private ToolStripMenuItem onToolStripMenuItem;

		private Button recallButton;

		private ComboBox closeBatch;

		private Label closeBatchLabel;

		private TextBox origRefNum;

		private Label origRefLabel;

		private TextBox origSeqNum;

		private Label origSequenceLabel;

		private TextBox invoiceNum;

		private TextBox clerkId;

		private CheckBox forcedUp;

		private TextBox authorization;

		private Label authorizationLabel;

		private Label invoiceNumLabel;

		private Label clerkIdLabel;

		private Label traceNumLabel;

		private TextBox traceNumTBox;

		private Label parameterTypeLabel;

		private ComboBox parameterType;

		private Label reprintTypeLabel;

		private ComboBox reprintTypeCBox;

		private ProgressBar progressBar1;

		private Label gettingRptLabel;

		private Label percentLabel;

		private ToolStripMenuItem reportViewerToolStripMenuItem;

		private ToolStripMenuItem enableToolStripMenuItem1;

		private ToolStripMenuItem disableToolStripMenuItem1;

		private TextBox custRefNumTBox;

		private Label custRefNumLabel;

		private RadioButton rbNo;

		private RadioButton rbYes;

		private GroupBox groupBox1;

		private Button getCapBt;

		private Button openCdBt;

		private Button getStatusBt;

		private Button clearTrace;

		private Label ecdStatusLabel;

		private Label ecdCapLabel;

		private TabControl mainTabCtrl;

		private TabPage tnxTabPage;

		private TabPage cdTabPage;

		private ToolStripMenuItem advancedToolStripMenuItem;

		private CheckBox dccCheckBox;

		private TextBox pan;

		private Label panLabel;

		private Label refNumberLabel;

		private Label tranTypeLabel;

		private TextBox refNumber;

		private ComboBox tranType;

		private TextBox txtSpecificData;

		private Label lblSpecificData;

		private GroupBox isReportGroupBox;

		private CheckBox chkFormattedRcpt;

		private Label rcptNameLabel;

		private TextBox rcptNameEdit;

		private TextBox txtMerchUrl;

		private Label lblMerchUrl;

		private TextBox txtMerchId;

		private Label lblMerchId;

		private Label lblFilterCateg;

		private TextBox txtFilterCateg;

		private Label lblEncryptReq;

		private TextBox txtEncryptReq;

		private Label lblMerchIndex;

		private TextBox txtMerchIndex;

		private ComboBox vasMode;

		private Label vasModeLabel;

		private TextBox textEccKey;

		private Label lblEccKey;

		private TabPage scriptTabPage;

		private Button continueButton;

		private Button selectScriptFileButton;

		private Label selectScriptFileNameLabel;

		private ListView cmdListView;

		private Button stopButton;

		private Button startButton;

		private Label loopLabel;

		private Label scriptPathLabel;

		private Label finalAmountLabel;

		private TextBox finalAmount;

		private TextBox searchAmount;

		private Label searchAmountLabel;

		[DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

		[DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		public static HomeForm Instance;
		public static Client Client;

		public HomeForm()
		{
			Instance = this;
			iParamType = 0;
			Com = new Communication();
			applicationProtocol = new ApplicationProtocol(this);
			InitializeComponent();
			InitializeContext();
			myPlayer = new SoundPlayer();
			timerLatePrintResp.Interval = 30000;
			timerLatePrintResp.Tick += timerLatePrintRespOnTick;
			UpdateListTenderType();
			UpdateListTransactionType();
			UpdateCBox();
			Client = new Client();
			Client.Initialize();
		}

		private void UpdateListTenderType()
		{
			var xmlDocument = new XmlDocument();
			var xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			try
			{
				tenderType.Items.Clear();
				while (xmlTextReader.Read())
				{
					if (!xmlTextReader.Name.Equals("TenderType")) continue;
					var xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(1).Name.Equals("Value"))
					{
						tenderType.Items.Add(xmlNode.Attributes.Item(0).Value);
					}
				}
				tenderType.SelectedIndex = 0;
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Problem updateListTenderType");
				tenderType.Text = "";
			}
			xmlTextReader.Close();
		}

		private void UpdateListTransactionType()
		{
			var xmlDocument = new XmlDocument();
			var xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			tnxType.Items.Clear();
			try
			{
				while (xmlTextReader.Read())
				{
					if (!xmlTextReader.Name.Equals("TransactionType")) continue;
					var xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(1).Name.Equals("Value"))
					{
						tnxType.Items.Add(xmlNode.Attributes.Item(0).Value);
					}
				}
				tnxType.SelectedIndex = 0;
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Problem updateListTransactionType");
				tnxType.Text = "";
			}
			xmlTextReader.Close();
		}

		private void UpdateCBox()
		{
			parameterType.SelectedIndex = 0;
			reprintTypeCBox.SelectedIndex = 0;
			tranType.SelectedIndex = 0;
			vasMode.SelectedIndex = 0;
		}

		private void Home_Load(object sender, EventArgs e)
		{
			DisplayCommSetting();
		}

		private void InitializeContext()
		{
			applicationProtocol.EndConnection();
			if (commandList == null || !bScriptStarted || bScriptStopped || indexCmd + 1 > commandList.Count)
			{
				sendRequest.Enabled = true;
				recallButton.Enabled = true;
			}
			rbYes.Enabled = true;
			rbNo.Enabled = true;
			resetAll.Enabled = true;
			timer1.Stop();
			progressBar1.Value = 0;
			progressBar1.Visible = false;
			gettingRptLabel.Visible = false;
			percentLabel.Visible = false;
			percentLabel.Text = "0%";
			ReleaseMemory();
		}

		private static void ReleaseMemory()
		{
			SetProcessWorkingSetSize(GetCurrentProcess(), -1, -1);
		}

		private void TnxType_SelectedIndexChanged(object sender, EventArgs e)
		{
			var num = 10;
			var num2 = 31;
			InitComponentForm();
			var text = tnxType.SelectedItem.ToString();
			var list = GetComponentFromCfgFile(text);
			for (var i = 0; i < list.Count; i++)
			{
				if (list[i].Equals("Amount"))
				{
					if (tnxType.Text is "Void" or "Pre-Auth Comp" or "Reprint Receipt")
					{
						num2 += 30;
						if (num2 > 250)
						{
							num = 240;
							num2 = 31;
						}
						searchAmount.Visible = true;
						searchAmount.Location = new Point(num + 70, num2 - 3);
						searchAmount.TabIndex = i + 1;
						searchAmountLabel.Visible = true;
						searchAmountLabel.Location = new Point(num, num2);
					}
					else
					{
						amount.Visible = true;
					}
					continue;
				}
				foreach (Control control in transactionGroupBox.Controls)
				{
					if (!control.Name.Contains(list[i]))
					{
						continue;
					}
					if (control.GetType() == typeof(TextBox) || control.GetType() == typeof(ComboBox) || control.GetType() == typeof(CheckBox))
					{
						num2 += 30;
						if (num2 > 250)
						{
							num = 240;
							num2 = 31;
						}
						control.Location = list[i].Equals("SpecificData") ? new Point(num + 70 + 50, num2 - 3) : new Point(num + 70, num2 - 3);
					}
					else if (control.GetType() == typeof(Label))
					{
						control.Location = new Point(num, num2);
					}
					control.TabIndex = i + 1;
					control.Visible = true;
				}
			}
			switch (text)
			{
				case "Set VAS Mode" when !vasMode.Items.Contains("VAS Only"):
					vasMode.Items.Insert(0, "VAS Only");
					break;
				case "Sale" when vasMode.Items.Contains("VAS Only"):
					vasMode.Items.Remove("VAS Only");
					break;
			}
			if (text == "Close Batch")
			{
				num2 += 30;
				if (num2 > 190)
				{
					num = 240;
					num2 = 31;
				}
				closeBatch.Visible = true;
				closeBatch.Location = new Point(num + 70, num2 - 3);
				closeBatchLabel.Visible = true;
				closeBatchLabel.Location = new Point(num, num2);
				closeBatch.SelectedIndex = 0;
			}
			else
			{
				closeBatch.Visible = false;
				closeBatchLabel.Visible = false;
			}
		}

		private void InitComponentForm()
		{
			tenderTypeLabel.Visible = false;
			tenderType.Visible = false;
			tenderType.Text = "None";
			clerkIdLabel.Visible = false;
			clerkId.Visible = false;
			clerkId.Text = "";
			invoiceNumLabel.Visible = false;
			invoiceNum.Visible = false;
			invoiceNum.Text = "";
			authorizationLabel.Visible = false;
			authorization.Visible = false;
			authorization.Text = "";
			origRefLabel.Visible = false;
			origRefNum.Visible = false;
			origRefNum.Text = "";
			origSequenceLabel.Visible = false;
			origSeqNum.Visible = false;
			origSeqNum.Text = "";
			traceNumLabel.Visible = false;
			traceNumTBox.Visible = false;
			traceNumTBox.Text = "";
			parameterTypeLabel.Visible = false;
			parameterType.Visible = false;
			reprintTypeLabel.Visible = false;
			reprintTypeCBox.Visible = false;
			custRefNumLabel.Visible = false;
			custRefNumTBox.Visible = false;
			custRefNumTBox.Text = "";
			forcedUp.Visible = false;
			forcedUp.Checked = false;
			dccCheckBox.Checked = false;
			dccCheckBox.Visible = false;
			tranTypeLabel.Visible = false;
			tranType.Visible = false;
			refNumberLabel.Visible = false;
			refNumber.Visible = false;
			refNumber.Text = "";
			panLabel.Visible = false;
			pan.Visible = false;
			pan.Text = "";
			lblSpecificData.Visible = false;
			txtSpecificData.Visible = false;
			txtSpecificData.Text = "";
			chkFormattedRcpt.Visible = false;
			chkFormattedRcpt.Checked = false;
			rcptNameEdit.Visible = false;
			rcptNameLabel.Visible = false;
			rcptNameEdit.Text = "";
			lblMerchUrl.Visible = false;
			txtMerchUrl.Visible = false;
			txtMerchUrl.Text = "";
			lblMerchId.Visible = false;
			txtMerchId.Visible = false;
			txtMerchId.Text = "";
			lblFilterCateg.Visible = false;
			txtFilterCateg.Visible = false;
			txtFilterCateg.Text = "";
			lblEncryptReq.Visible = false;
			txtEncryptReq.Visible = false;
			txtEncryptReq.Text = "";
			lblMerchIndex.Visible = false;
			txtMerchIndex.Visible = false;
			txtMerchIndex.Text = "";
			vasModeLabel.Visible = false;
			vasMode.Visible = false;
			vasMode.Text = tnxType.Text == "Sale" ? "VAS and Payment" : "VAS Only";
			lblEccKey.Visible = false;
			textEccKey.Visible = false;
			textEccKey.Text = "";
			finalAmount.Visible = false;
			finalAmountLabel.Visible = false;
			searchAmount.Visible = false;
			searchAmountLabel.Visible = false;
			amount.Visible = false;
			searchAmount.Text = "$_.__";
			searchAmount.Tag = "0";
			finalAmount.Text = "$_.__";
			finalAmount.Tag = "0";
			amount.Text = "$0.00";
			amount.Tag = "0";
		}

		private static List<string> GetComponentFromCfgFile(string trxType)
		{
			var list = new List<string>();
			var xmlDocument = new XmlDocument();
			var xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
			try
			{
				while (xmlTextReader.Read())
				{
					if (!xmlTextReader.Name.Equals("TransactionType"))
					{
						continue;
					}
					var xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes["Name"] == null || trxType != xmlNode.Attributes["Name"].Value) continue;
					if (!list.Contains("Amount") && xmlNode.Attributes["Amount"] != null && xmlNode.Attributes["Amount"].Value == "ON")
					{
						list.Add("Amount");
					}
					if (!list.Contains("TenderType") && xmlNode.Attributes["TenderType"] != null && xmlNode.Attributes["TenderType"].Value == "ON")
					{
						list.Add("TenderType");
					}
					if (!list.Contains("ClerkId") && xmlNode.Attributes["ClerkId"] != null && xmlNode.Attributes["ClerkId"].Value == "ON")
					{
						list.Add("ClerkId");
					}
					if (!list.Contains("InvoiceNum") && xmlNode.Attributes["InvoiceNum"] != null && xmlNode.Attributes["InvoiceNum"].Value == "ON")
					{
						list.Add("InvoiceNum");
					}
					if (!list.Contains("Authorization") && xmlNode.Attributes["Authorization"] != null && xmlNode.Attributes["Authorization"].Value == "ON")
					{
						list.Add("Authorization");
					}
					if (!list.Contains("OrigSeq") && xmlNode.Attributes["OrigSeq"] != null && xmlNode.Attributes["OrigSeq"].Value == "ON")
					{
						list.Add("OrigSeq");
					}
					if (!list.Contains("OrigRef") && xmlNode.Attributes["OrigRef"] != null && xmlNode.Attributes["OrigRef"].Value == "ON")
					{
						list.Add("OrigRef");
					}
					if (!list.Contains("TraceNum") && xmlNode.Attributes["TraceNum"] != null && xmlNode.Attributes["TraceNum"].Value == "ON")
					{
						list.Add("TraceNum");
					}
					if (!list.Contains("ParameterType") && xmlNode.Attributes["ParameterType"] != null && xmlNode.Attributes["ParameterType"].Value == "ON")
					{
						list.Add("ParameterType");
					}
					if (!list.Contains("ReprintType") && xmlNode.Attributes["ReprintType"] != null && xmlNode.Attributes["ReprintType"].Value == "ON")
					{
						list.Add("ReprintType");
					}
					if (!list.Contains("CustRefNum") && xmlNode.Attributes["CustRefNum"] != null && xmlNode.Attributes["CustRefNum"].Value == "ON")
					{
						list.Add("CustRefNum");
					}
					if (!list.Contains("RefNumber") && xmlNode.Attributes["RefNumber"] != null && xmlNode.Attributes["RefNumber"].Value == "ON")
					{
						list.Add("RefNumber");
					}
					if (!list.Contains("PAN") && xmlNode.Attributes["PAN"] != null && xmlNode.Attributes["PAN"].Value == "ON")
					{
						list.Add("PAN");
					}
					if (!list.Contains("TranType") && xmlNode.Attributes["TranType"] != null && xmlNode.Attributes["TranType"].Value == "ON")
					{
						list.Add("TranType");
					}
					if (!list.Contains("ForcedUP") && xmlNode.Attributes["ForcedUP"] != null && xmlNode.Attributes["ForcedUP"].Value == "ON")
					{
						list.Add("ForcedUP");
					}
					if (!list.Contains("DCC") && xmlNode.Attributes["DCC"] != null && xmlNode.Attributes["DCC"].Value == "ON")
					{
						list.Add("DCC");
					}
					if (!list.Contains("SpecificData") && xmlNode.Attributes["SpecificData"] != null && xmlNode.Attributes["SpecificData"].Value == "ON")
					{
						list.Add("SpecificData");
					}
					if (!list.Contains("RcptName") && xmlNode.Attributes["RcptName"] != null && xmlNode.Attributes["RcptName"].Value == "ON")
					{
						list.Add("RcptName");
					}
					if (!list.Contains("MerchID") && xmlNode.Attributes["MerchID"] != null && xmlNode.Attributes["MerchID"].Value == "ON")
					{
						list.Add("MerchID");
					}
					if (!list.Contains("MerchURL") && xmlNode.Attributes["MerchURL"] != null && xmlNode.Attributes["MerchURL"].Value == "ON")
					{
						list.Add("MerchURL");
					}
					if (!list.Contains("FilterCateg") && xmlNode.Attributes["FilterCateg"] != null && xmlNode.Attributes["FilterCateg"].Value == "ON")
					{
						list.Add("FilterCateg");
					}
					if (!list.Contains("EncryptReq") && xmlNode.Attributes["EncryptReq"] != null && xmlNode.Attributes["EncryptReq"].Value == "ON")
					{
						list.Add("EncryptReq");
					}
					if (!list.Contains("EccKey") && xmlNode.Attributes["EccKey"] != null && xmlNode.Attributes["EccKey"].Value == "ON")
					{
						list.Add("EccKey");
					}
					if (!list.Contains("VasMode") && xmlNode.Attributes["VasMode"] != null && xmlNode.Attributes["VasMode"].Value == "ON")
					{
						list.Add("VasMode");
					}
					if (!list.Contains("MerchIndex") && xmlNode.Attributes["MerchIndex"] != null && xmlNode.Attributes["MerchIndex"].Value == "ON")
					{
						list.Add("MerchIndex");
					}
					if (!list.Contains("FinalAmount") && xmlNode.Attributes["FinalAmount"] != null && xmlNode.Attributes["FinalAmount"].Value == "ON")
					{
						list.Add("FinalAmount");
					}
					if (!list.Contains("FormattedRcpt") && xmlNode.Attributes["FormattedRcpt"] != null && xmlNode.Attributes["FormattedRcpt"].Value == "ON")
					{
						list.Add("FormattedRcpt");
					}
				}
				return list;
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Problem parsing config file");
				return list;
			}
		}

		private void TextBoxNum_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b')
			{
				e.Handled = !char.IsDigit(e.KeyChar);
			}
		}

		private void TextBoxAlphaNum_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b')
			{
				e.Handled = !char.IsLetterOrDigit(e.KeyChar);
			}
		}

		private void AlphaNum_Sapace_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\b')
			{
				e.Handled = !char.IsLetterOrDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar);
			}
		}

		private void ResetAll_Click(object sender, EventArgs e)
		{
			clerkId.Text = "";
			invoiceNum.Text = "";
			authorization.Text = "";
			origRefNum.Text = "";
			origSeqNum.Text = "";
			traceNumTBox.Text = "";
			custRefNumTBox.Text = "";
			tranType.Text = "";
			refNumber.Text = "";
			pan.Text = "";
			rcptNameEdit.Text = "";
			txtMerchUrl.Text = "";
			txtMerchId.Text = "";
			txtFilterCateg.Text = "";
			txtEncryptReq.Text = "";
			txtMerchIndex.Text = "";
			textEccKey.Text = "";
			chkFormattedRcpt.Checked = false;
			if (tnxType.Text is "Void" or "Pre-Auth Comp" or "Reprint Receipt")
			{
				searchAmount.Text = "$_.__";
				searchAmount.Tag = "0";
				finalAmount.Text = "$_.__";
				finalAmount.Tag = "0";
			}
			else
			{
				amount.Text = "$0.00";
				amount.Tag = "0";
			}
			if (tnxType.Text != "Close Batch")
			{
				closeBatchLabel.Visible = false;
				closeBatch.Visible = false;
			}
			else
			{
				closeBatch.SelectedIndex = 0;
			}
			InitializeContext();
		}

		private static string GetAmountFormated(string szValue)
		{
			var text = szValue.Replace("$", "").Replace(",", "").Replace(".", "");
			text = text.Replace(' ', '0');
			text = text.TrimStart('0');
			if (text == "")
			{
				text = "0";
			}
			return text;
		}

		private void Amount_KeyPress(object sender, KeyPressEventArgs e)
		{
			var textBox = (TextBox)sender;
			var text = (string)textBox.Tag;
			if (text != null)
			{
				switch (e.KeyChar)
				{
					case '0' when text == "0":
						e.Handled = true;
						return;
					case '\b' when text.Length > 0:
					{
						text = text.Substring(0, text.Length - 1);
						if (tnxType.Text != "Void" && tnxType.Text != "Pre-Auth Comp" && tnxType.Text != "Reprint Receipt" && text == "")
						{
							text = "0";
						}

						break;
					}
					default:
					{
						if (char.IsNumber(e.KeyChar) && text.Length < 10)
						{
							text = ((text != "0") ? (text + e.KeyChar.ToString(culture)) : e.KeyChar.ToString(culture));
						}

						break;
					}
				}

				textBox.Text = text.Length switch
				{
					0 => "$_.__",
					1 => "$0.0" + text,
					2 => "$0." + text,
					> 2 and < 6 => "$" + text.Substring(0, text.Length - 2) + "." + text.Substring(text.Length - 2),
					>= 6 => "$" + text.Substring(0, text.Length - 5) + "," + text.Substring(text.Length - 5, 3) + "." +
					        text.Substring(text.Length - 2, 2),
					_ => textBox.Text
				};
				textBox.SelectionStart = textBox.Text.Length + 1;
				textBox.Tag = text;
			}
			e.Handled = true;
		}

		private static void Amount_KeyDown(object sender, KeyEventArgs e)
		{
			var obj = (TextBox)sender;
			obj.SelectionStart = obj.Text.Length + 1;
			e.Handled = true;
		}

		private static void Amount_MouseClick(object sender, MouseEventArgs e)
		{
			var obj = (TextBox)sender;
			obj.SelectionStart = obj.Text.Length + 1;
		}

		private static void Amount_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			var obj = (TextBox)sender;
			obj.SelectionStart = obj.Text.Length + 1;
		}

		private static void Amount_MouseDown(object sender, MouseEventArgs e)
		{
			var obj = (TextBox)sender;
			obj.SelectionStart = obj.Text.Length + 1;
		}

		private static void Amount_MouseEnter(object sender, EventArgs e)
		{
			var obj = (TextBox)sender;
			obj.SelectionStart = obj.Text.Length + 1;
		}

		private static void Amount_MouseUp(object sender, MouseEventArgs e)
		{
			var obj = (TextBox)sender;
			obj.SelectionStart = obj.Text.Length + 1;
		}

		private void SendRequest_Click(object sender, EventArgs e)
		{
			var dataElement = new DataElement();
			Cursor = Cursors.WaitCursor;
			rxBufferData.Clear();
			rxBufferPrint.Clear();
			merchImgBytesBuffer = new List<byte[]>();
			custImgBytesBuffer = new List<byte[]>();
			formatRcptImgStringBuffer = new List<byte[]>();
			merchHtmlStringBuffer = new List<string>();
			custHtmlStringBuffer = new List<string>();
			formatRcptHtmlStringBuffer = new List<string>();
			if (imgReceiptViewerForm != null)
			{
				imgReceiptViewerForm.Close();
				imgReceiptViewerForm = null;
			}
			if (htmlReceiptViewerForm != null)
			{
				htmlReceiptViewerForm.Close();
				htmlReceiptViewerForm = null;
			}
			var request = new Request();
			currentTrxType = "";
			bScriptStarted = false;
			if (tnxType.Enabled)
			{
				request.tnxCode = tnxType.Text;
				currentTrxType = dataElement.Get_TransTypeTag(tnxType.Text);
			}
			if (amount.Visible)
			{
				request.amount = GetAmountFormated(amount.Text);

			}
			else if (searchAmount.Visible && searchAmount.Text != "$_.__")
			{
				request.amount = GetAmountFormated(searchAmount.Text);

			}
			if (vasMode.Visible)
			{
				if (currentTrxType == dataElement.Get_TransTypeTag("Sale"))
				{
					request.vasMode = vasMode.SelectedIndex + 1;
				}
				else
				{
					request.vasMode = vasMode.SelectedIndex;
				}

			}
			else
			{
				request.vasMode = -1;

			}
			if (tenderType.Visible)
			{
				request.tenderType = tenderType.Text;

			}
			if (clerkId.Visible)
			{
				request.clerkId = clerkId.Text;

			}
			if (invoiceNum.Visible)
			{
				request.invoiceNumber = invoiceNum.Text;

			}
			if (authorization.Visible)
			{
				request.authorNumber = authorization.Text;

			}
			if (origSeqNum.Visible)
			{
				request.origSeqNumber = origSeqNum.Text;

			}
			if (origRefNum.Visible)
			{
				request.origRefNumber = origRefNum.Text;

			}
			if (traceNumTBox.Visible)
			{
				request.traceNum = traceNumTBox.Text;

			}
			if (closeBatch.Visible)
			{
				request.closeBatch = closeBatch.SelectedIndex + 1;

			}
			if (reprintTypeCBox.Visible)
			{
				request.reprintType = reprintTypeCBox.SelectedIndex.ToString();

			}
			if (parameterType.Visible)
			{
				request.parameterType = parameterType.SelectedIndex + 1;

			}
			if (forcedUp.Visible)
			{
				request.forcedUP = forcedUp.Checked ? "1" : "0";
			}
			if (custRefNumTBox.Visible)
			{
				request.custRef = custRefNumTBox.Text;
			}
			if (dccCheckBox.Visible)
			{
				request.Dcc = dccCheckBox.Checked ? "1" : "0";
			}
			if (txtSpecificData.Visible)
			{
				request.specificData = txtSpecificData.Text;
			}
			if (txtMerchUrl.Visible)
			{
				request.merchURL = txtMerchUrl.Text;
			}
			if (txtMerchId.Visible)
			{
				request.merchID = txtMerchId.Text;
			}
			if (txtFilterCateg.Visible)
			{
				request.filterCateg = txtFilterCateg.Text;
			}
			if (txtEncryptReq.Visible)
			{
				request.encryptReq = txtEncryptReq.Text;
			}
			if (textEccKey.Visible)
			{
				request.vasEccKey = textEccKey.Text;
			}
			if (txtMerchIndex.Visible)
			{
				request.merchIndex = txtMerchIndex.Text;
			}
			if (tranType.Visible)
			{
				request.tranType = tranType.Text switch
				{
					"Open Batch" => "01",
					"Pre-Auth" => "02",
					_ => request.tranType
				};
			}
			if (refNumber.Visible)
			{
				request.refNum = refNumber.Text;
			}
			if (pan.Visible)
			{
				request.PAN = pan.Text;
			}
			if (chkFormattedRcpt.Visible)
			{
				request.formattedRcpt = chkFormattedRcpt.Checked ? "1" : "0";
			}
			if (rcptNameEdit.Visible)
			{
				request.RcptName = rcptNameEdit.Text;
			}
			if (finalAmount.Visible && finalAmount.Text != "$_.__")
			{
				request.finalAmount = GetAmountFormated(finalAmount.Text);
			}
			if (StartCommunicationProcess())
			{
				applicationProtocol.Request = request;
				if (applicationProtocol.RequestApplyTransaction())
				{
					Console.WriteLine("Starting timer.");
					timer1.Start();
					Thread.Sleep(1);
					sendRequest.Enabled = false;
					recallButton.Enabled = false;
					rbYes.Enabled = false;
					rbNo.Enabled = false;
					resetAll.Enabled = false;
				}
				else
				{
					InitializeContext();
				}
			}
			Cursor = Cursors.Default;
		}

		
		public void SendTheRequest(int requestAmount, int requestClerkId)
		{
			var dataElement = new DataElement();
			// Cursor = Cursors.WaitCursor;
			// rxBufferData.Clear();
			// rxBufferPrint.Clear();
			// merchImgBytesBuffer = new List<byte[]>();
			// custImgBytesBuffer = new List<byte[]>();
			// formatRcptImgStringBuffer = new List<byte[]>();
			// merchHtmlStringBuffer = new List<string>();
			// custHtmlStringBuffer = new List<string>();
			// formatRcptHtmlStringBuffer = new List<string>();
			// if (imgReceiptViewerForm != null)
			// {
			// 	imgReceiptViewerForm.Close();
			// 	imgReceiptViewerForm = null;
			// }
			// if (htmlReceiptViewerForm != null)
			// {
			// 	htmlReceiptViewerForm.Close();
			// 	htmlReceiptViewerForm = null;
			// }
			var request = new Request();
			currentTrxType = "";
			bScriptStarted = false;
			if (tnxType.Enabled)
			{
				request.tnxCode = "Sale";
				currentTrxType = dataElement.Get_TransTypeTag("Sale");
			}
			if (this.amount.Visible || requestAmount > 0 )
			{
				request.amount = GetAmountFormated($"{requestAmount}");
			}
			else if (searchAmount.Visible && searchAmount.Text != "$_.__")
			{
				request.amount = GetAmountFormated(searchAmount.Text);
			}
			if (vasMode.Visible)
			{
				if (currentTrxType == dataElement.Get_TransTypeTag("Sale"))
				{
					request.vasMode = vasMode.SelectedIndex + 1;
				}
				else
				{
					request.vasMode = vasMode.SelectedIndex;
				}

			}
			else
			{
				request.vasMode = -1;

			}
			if (tenderType.Visible)
			{
				request.tenderType = tenderType.Text;

			}
			if (requestClerkId > 0)
			{
				request.clerkId = $"{requestClerkId}";

			}
			if (invoiceNum.Visible)
			{
				request.invoiceNumber = invoiceNum.Text;

			}
			if (authorization.Visible)
			{
				request.authorNumber = authorization.Text;

			}
			if (origSeqNum.Visible)
			{
				request.origSeqNumber = origSeqNum.Text;

			}
			if (origRefNum.Visible)
			{
				request.origRefNumber = origRefNum.Text;

			}
			if (traceNumTBox.Visible)
			{
				request.traceNum = traceNumTBox.Text;

			}
			if (closeBatch.Visible)
			{
				request.closeBatch = closeBatch.SelectedIndex + 1;

			}
			if (reprintTypeCBox.Visible)
			{
				request.reprintType = reprintTypeCBox.SelectedIndex.ToString();

			}
			if (parameterType.Visible)
			{
				request.parameterType = parameterType.SelectedIndex + 1;

			}
			if (forcedUp.Visible)
			{
				request.forcedUP = forcedUp.Checked ? "1" : "0";
			}
			if (custRefNumTBox.Visible)
			{
				request.custRef = custRefNumTBox.Text;
			}
			if (dccCheckBox.Visible)
			{
				request.Dcc = dccCheckBox.Checked ? "1" : "0";
			}
			if (txtSpecificData.Visible)
			{
				request.specificData = txtSpecificData.Text;
			}
			if (txtMerchUrl.Visible)
			{
				request.merchURL = txtMerchUrl.Text;
			}
			if (txtMerchId.Visible)
			{
				request.merchID = txtMerchId.Text;
			}
			if (txtFilterCateg.Visible)
			{
				request.filterCateg = txtFilterCateg.Text;
			}
			if (txtEncryptReq.Visible)
			{
				request.encryptReq = txtEncryptReq.Text;
			}
			if (textEccKey.Visible)
			{
				request.vasEccKey = textEccKey.Text;
			}
			if (txtMerchIndex.Visible)
			{
				request.merchIndex = txtMerchIndex.Text;
			}
			if (tranType.Visible)
			{
				request.tranType = tranType.Text switch
				{
					"Open Batch" => "01",
					"Pre-Auth" => "02",
					_ => request.tranType
				};
			}
			if (refNumber.Visible)
			{
				request.refNum = refNumber.Text;
			}
			if (pan.Visible)
			{
				request.PAN = pan.Text;
			}
			if (chkFormattedRcpt.Visible)
			{
				request.formattedRcpt = chkFormattedRcpt.Checked ? "1" : "0";
			}
			if (rcptNameEdit.Visible)
			{
				request.RcptName = rcptNameEdit.Text;
			}
			if (finalAmount.Visible && finalAmount.Text != "$_.__")
			{
				request.finalAmount = GetAmountFormated(finalAmount.Text);
			}
			if (StartCommunicationProcess())
			{
				applicationProtocol.Request = request;
				if (applicationProtocol.RequestApplyTransaction())
				{
					Console.WriteLine("Timer starting 3.");
					timer1.Start();
					Thread.Sleep(1);
					sendRequest.Enabled = false;
					recallButton.Enabled = false;
					rbYes.Enabled = false;
					rbNo.Enabled = false;
					resetAll.Enabled = false;
				}
				else
				{
					InitializeContext();
				}
			}

			try
			{
				Cursor = Cursors.Default;
			}
			catch (Exception ex)
			{
				
			}
		}

		
		
		private void UpdateCmdList()
		{
			foreach (ListViewItem item in cmdListView.Items)
			{
				item.ForeColor = cmdListView.Items[indexCmd] == item ? Color.Blue : Color.Black;
			}
			cmdListView.Update();
			cmdListView.Refresh();
		}

		private int GetNextCmdIndex(int startIndex, IReadOnlyCollection<Command> commandList)
		{
			while (true)
			{
				if (startIndex < commandList.Count)
				{
					foreach (ListViewItem item in cmdListView.Items)
					{
						if (cmdListView.Items[startIndex] != item) continue;
						if (item.Checked)
						{
							break;
						}
						startIndex++;
					}
				}
				if (startIndex < commandList.Count || scriptLoopValue <= 0 || currentLoopNmb >= scriptLoopValue)
				{
					break;
				}
				currentLoopNmb++;
				startIndex = 0;
			}
			return startIndex;
		}

		private void ExecuteCommand(Command command)
		{
			currentTrxType = "";
			Cursor = Cursors.WaitCursor;
			merchImgBytesBuffer = new List<byte[]>();
			custImgBytesBuffer = new List<byte[]>();
			formatRcptImgStringBuffer = new List<byte[]>();
			merchHtmlStringBuffer = new List<string>();
			custHtmlStringBuffer = new List<string>();
			formatRcptHtmlStringBuffer = new List<string>();
			rxBufferData.Clear();
			rxBufferPrint.Clear();
			if (imgReceiptViewerForm != null)
			{
				imgReceiptViewerForm.Close();
				imgReceiptViewerForm = null;
			}
			if (htmlReceiptViewerForm != null)
			{
				htmlReceiptViewerForm.Close();
				htmlReceiptViewerForm = null;
			}
			var request = new Request();
			currentTrxType = "";
			var text = request.tnxCode = command.Label;
			currentTrxType = new DataElement().Get_TransTypeTag(text);
			tnxType.Enabled = true;
			tnxType.Text = text;
			UpdateCmdList();
			if (scriptLoopValue > 1)
			{
				loopLabel.Text = "Loop:" + currentLoopNmb + "/" + scriptLoopValue;
			}

			var dictionary = command.Params;
			foreach (var key in dictionary.Keys)
			{
				switch (key)
				{
				case "Amount":
					request.amount = dictionary[key];
					break;
				case "TenderType":
					request.tenderType = dictionary[key];
					break;
				case "ClerkId":
					request.clerkId = dictionary[key];
					break;
				case "InvoiceNum":
					request.invoiceNumber = dictionary[key];
					break;
				case "Authorization":
					request.authorNumber = dictionary[key];
					break;
				case "OrigSeq":
					request.origSeqNumber = dictionary[key];
					break;
				case "OrigRef":
					request.origRefNumber = dictionary[key];
					break;
				case "TraceNum":
					request.traceNum = dictionary[key];
					break;
				case "ReprintType":
					request.reprintType = dictionary[key];
					break;
				case "CustRefNum":
					request.custRef = dictionary[key];
					break;
				case "ForcedUP":
					request.forcedUP = dictionary[key];
					break;
				case "DCC":
					request.Dcc = dictionary[key];
					break;
				case "RefNumber":
					request.refNum = dictionary[key];
					break;
				case "PAN":
					request.PAN = dictionary[key];
					break;
				case "SpecificData":
					request.specificData = dictionary[key];
					break;
				case "FormattedRcpt":
					request.formattedRcpt = dictionary[key];
					break;
				case "RcptName":
					request.RcptName = dictionary[key];
					break;
				case "MerchURL":
					request.merchURL = dictionary[key];
					break;
				case "MerchID":
					request.merchID = dictionary[key];
					break;
				case "FilterCateg":
					request.filterCateg = dictionary[key];
					break;
				case "EncryptReq":
					request.encryptReq = dictionary[key];
					break;
				case "VasMode":
					request.vasMode = int.Parse(dictionary[key]);
					break;
				case "EccKey":
					request.vasEccKey = dictionary[key];
					break;
				case "MerchIndex":
					request.merchIndex = dictionary[key];
					break;
				case "FinalAmount":
					request.finalAmount = dictionary[key];
					break;
				}
			}
			if (text != "" && StartCommunicationProcess())
			{
				applicationProtocol.Request = request;
				if (applicationProtocol.RequestApplyTransaction())
				{
					Console.WriteLine("Starting timer 2.");
					timer1.Start();
					Thread.Sleep(1);
					startButton.Enabled = false;
					continueButton.Enabled = false;
					selectScriptFileButton.Enabled = false;
					sendRequest.Enabled = false;
					recallButton.Enabled = false;
					rbYes.Enabled = false;
					rbNo.Enabled = false;
					resetAll.Enabled = false;
				}
				else
				{
					InitializeContext();
				}
			}

			Cursor = Cursors.Default;
		}

		private void ClearTrace_Click(object sender, EventArgs e)
		{
			Trace.Clear();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			Console.WriteLine("Timer");
			if (applicationProtocol.ResponseData(out var msgData, out var szRespStatus, out var packageSeq, out var bDisconnectCom))
			{
				if (szRespStatus == "99")
				{
					if (msgData is {Length: >= 3})
					{
						rxBufferPrint.Add(msgData);
					}
					if (packageSeq == 0 && rxBufferPrint.Count != 0)
					{
						string text;
						if (IsReportRequest(currentTrxType))
						{
							text = ">>> ECR: Part of Report";
							applicationProtocol.PrintingResponseMessage("0");
							var list = DisplayResponseMsg("Report", rxBufferPrint);
							listDicoOfReport.AddRange(list);
							rxBufferPrint.Clear();
						}
						else
						{
							DisplayReceiptMsg(rxBufferPrint);
							rxBufferPrint.Clear();
							if (NonePrintRadioButton.Checked)
							{
								string szPrintingStatus;
								if (MessageBox.Show("Print Receipt?", "Print", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
								{
									szPrintingStatus = "0";
									text = ">>> ECR: Printing Accepted";
								}
								else
								{
									szPrintingStatus = "1";
									text = ">>> ECR: Printing Rejected";
								}
								applicationProtocol.PrintingResponseMessage(szPrintingStatus);
							}
							else if (NokPrintRadioButton.Checked)
							{
								const string szPrintingStatus = "1";
								text = ">>> ECR: Printing Failure on ECR";
								applicationProtocol.PrintingResponseMessage(szPrintingStatus);
							}
							else if (LatePrintRadioButton.Checked)
							{
								text = ">>> ECR: Late Printing Response";
								timerLatePrintResp.Start();
							}
							else
							{
								text = ">>> ECR: No Printing Response from ECR";
							}
						}
						DisplayPrintingRspInTrace(text);
					}
				}
				else
				{
					if (msgData is {Length: >= 3})
					{
						byte[] array;
						if (Encoding.ASCII.GetString(msgData, 0, 3) == "91\u001c")
						{
							array = new byte[msgData.Length - 3];
							Array.Copy(msgData, 3, array, 0, msgData.Length - 3);
						}
						else
						{
							array = new byte[msgData.Length];
							Array.Copy(msgData, array, msgData.Length);
						}
						rxBufferData.Add(array);
					}
					if (packageSeq == 0 && rxBufferData.Count != 0)
					{
						var list2 = DisplayResponseMsg("Transaction", rxBufferData);
						if (!bScriptStarted || indexCmd + 1 > commandList.Count)
						{
							myPlayer.SoundLocation = "Ressources\\\\ECR Sound.wav";
							try
							{
								myPlayer.Play();
							}
							catch
							{
								System.Diagnostics.Trace.WriteLine("Couldn't play sound.");
							}
						}
						if (IsReportRequest(currentTrxType))
						{
							if (szRespStatus == "00" && !enableToolStripMenuItem1.Enabled)
							{
								var subRequest = -1;
								if (iParamType > 0)
								{
									subRequest = iParamType;
								}
								new HandleReports(this, listDicoOfReport, currentTrxType, subRequest).GenerateReport();
							}
							listDicoOfReport.Clear();
						}
						else if (IsCashDrawerTnx(szRespStatus))
						{
							DisplayCashDrawerRsp(list2);
						}
						else if (80.ToString() == currentTrxType)
						{
							DisplayReceipt();
						}
						else if (applicationProtocol.Request is {formattedRcpt: "1"})
						{
							DisplayReceipt();
						}
						rxBufferData.Clear();
					}
				}
			}
			if (!bDisconnectCom)
			{
				return;
			}
			InitializeContext();
			timerLatePrintResp.Stop();
			listDicoOfReport.Clear();
			if (commandList == null || !bScriptStarted) return;
			Thread.Sleep(1000);
			if (indexCmd + 1 <= commandList.Count && bScriptStopped)
			{
				continueButton.Enabled = true;
			}
			if (indexCmd + 1 <= commandList.Count && !bScriptStopped)
			{
				ExecuteCommand(commandList[indexCmd]);
				indexCmd++;
				indexCmd = GetNextCmdIndex(indexCmd, commandList);
			}
			else
			{
				startButton.Enabled = true;
				selectScriptFileButton.Enabled = true;
				bScriptStarted = false;
			}
		}

		private void timerLatePrintRespOnTick(object sender, EventArgs ea)
		{
			const string szPrintingStatus = "0";
			applicationProtocol.PrintingResponseMessage(szPrintingStatus);
		}

		private void GetReceiptBuffer(StringBuilder _rcptStringBuffer, ref List<string> rcptStringBuffer, ref List<byte[]> rcptByteBuffer)
		{
			var converter = new Converter();
			const int encodeType = 0;
			if (_rcptStringBuffer.Length == 0)
			{
				return;
			}
			var text = _rcptStringBuffer.ToString();
			if (IsHtmlRcpt(text) && rcptStringBuffer != null)
			{
				try
				{
					rcptStringBuffer.Add(Encoding.UTF8.GetString(Convert.FromBase64String(text)));
					return;
				}
				catch
				{
					rcptStringBuffer.Add(text);
					return;
				}
			}

			if (!IsPngImage(text) || rcptByteBuffer == null) return;
			try
			{
				rcptByteBuffer.Add(Convert.FromBase64String(text));
			}
			catch
			{
				rcptByteBuffer.Add(converter.convertStringToByteArray(text, encodeType));
			}
		}

		private void DisplayFormattedDataInTrace(string buffer)
		{
			if (buffer.Length > 80)
			{
				Ticket.AppendText(buffer.Substring(0, 80) + "...");
			}
			else
			{
				Ticket.AppendText(buffer);
			}
		}

		private void DisplayPrintingRspInTrace(string printingResp)
		{
			_ = Ticket.SelectionFont.Style;
			var text = printingResp + "\n";
			Ticket.SelectionStart = Ticket.TextLength;
			Ticket.SelectionLength = text.Length;
			Ticket.AppendText(text);
			Ticket.ScrollToCaret();
		}

		private void DisplayVasDataInTrace(string buffer)
		{
			try
			{
				var bytes = Convert.FromBase64String(buffer);
				Ticket.AppendText("\n");
				Ticket.AppendText("Base64 decode: ");
				Ticket.AppendText("\n");
				Ticket.AppendText(Encoding.UTF7.GetString(bytes).Replace("\0", ""));
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.WriteLine("Unable to decode base 64 data!");
			}
		}

		private void DisplayReceiptMsg(List<byte[]> szListRespPrint)
		{
			var num = 0;
			var dataElement = new DataElement();
			var responseMsg = new ResponseMsg();
			var encodeType = Encoding.UTF7;
			var stringBuilder = new StringBuilder();
			var stringBuilder2 = new StringBuilder();
			var stringBuilderForReport = new StringBuilder();
			var style = Ticket.SelectionFont.Style;
			var num2 = szListRespPrint.Sum(respPrint => respPrint.Length - 3);
			var array = new byte[num2];
			foreach (var respPrint in szListRespPrint)
			{
				if (respPrint.Length >= 3)
				{
					Array.Copy(respPrint, 3, array, num, respPrint.Length - 3);
					num += respPrint.Length - 3;
				}
				else
				{
					System.Diagnostics.Trace.WriteLine("Error : Receipt message doesn't contain any field");
				}
			}
			const string text = "\n============ Receipt Data =============\n\n";
			Ticket.SelectionStart = Ticket.TextLength;
			Ticket.SelectionLength = text.Length;
			style |= FontStyle.Bold;
			Ticket.SelectionFont = new Font(Ticket.SelectionFont, style);
			Ticket.AppendText(text);
			if (applicationProtocol.Request.formattedRcpt == "1")
			{
				encodeType = Encoding.UTF8;
			}
			var list = responseMsg.ParseReceivedResponse(array, bPrintResp: true, encodeType);
			if (list.Count > 0)
			{
				var dictionary = list[list.Count - 1];
				foreach (var key in dictionary.Keys)
				{
					Ticket.AppendText(dataElement.FormatTagThreeDigit(key) + "  " + dataElement.Get_DataListLabel(key) + " : ");
					if (key == dataElement.Get_TransRecordTag()) continue;
					if (dictionary.ContainsKey(Tags.TAG_MERCH_FORMATTED_RECEIPT_BUFFER))
					{
						stringBuilder.Append(dictionary[Tags.TAG_MERCH_FORMATTED_RECEIPT_BUFFER]);
						DisplayFormattedDataInTrace(dictionary[Tags.TAG_MERCH_FORMATTED_RECEIPT_BUFFER]);
					}
					else if (dictionary.ContainsKey(Tags.TAG_CUST_FORMATTED_RECEIPT_BUFFER))
					{
						stringBuilder2.Append(dictionary[Tags.TAG_CUST_FORMATTED_RECEIPT_BUFFER]);
						DisplayFormattedDataInTrace(dictionary[Tags.TAG_CUST_FORMATTED_RECEIPT_BUFFER]);

					}
					else
					{
						stringBuilderForReport.Append($"{dictionary[key]}/");
						//Console.WriteLine(dictionary[key]);
						Ticket.AppendText(dictionary[key]);
					}
					Ticket.AppendText("\n");
				}
				for (var j = 0; j < list.Count - 1; j++)
				{
					dictionary = list[j];
					Ticket.AppendText("\n");
					Ticket.AppendText("RECORD" + (j + 1) + ":");
					Ticket.AppendText("\n");
					foreach (var key2 in dictionary.Keys.Where(key2 => key2 <= 999))
					{
						Ticket.AppendText(dataElement.FormatTagThreeDigit(key2) + "  " + dataElement.Get_DataListLabel(key2) + " : ");
						Ticket.AppendText(dictionary[key2]);
						Ticket.AppendText("\n");
					}
				}
				Ticket.ScrollToCaret();
			}
			else
			{
				System.Diagnostics.Trace.WriteLine("Error : Receipt message doesn't contain any data field or record field");
			}
			if (stringBuilder.Length != 0)
			{
				GetReceiptBuffer(stringBuilder, ref merchHtmlStringBuffer, ref merchImgBytesBuffer);
			}
			if (stringBuilder2.Length != 0)
			{
				GetReceiptBuffer(stringBuilder2, ref custHtmlStringBuffer, ref custImgBytesBuffer);
			}
			Console.WriteLine(stringBuilderForReport.ToString());
			Client.SendResponse(stringBuilderForReport.ToString());
		}

		private void DisplayCashDrawerRsp(IReadOnlyList<Dictionary<int, string>> listDico)
		{
			if (listDico.Count <= 0)
			{
				return;
			}
			var dictionary = listDico[listDico.Count - 1];
			foreach (var key in dictionary.Keys)
			{
				if (key == Tags.TAG_CD_STATUS)
				{
					switch (dictionary[Tags.TAG_CD_STATUS])
					{
					case "0":
						ecdStatusLabel.Text = "Is Open";
						ecdStatusLabel.ForeColor = Color.Green;
						break;
					case "1":
						ecdStatusLabel.Text = "Is Closed";
						ecdStatusLabel.ForeColor = Color.Red;
						break;
					case "2":
						ecdStatusLabel.Text = "Connected Unknown";
						ecdStatusLabel.ForeColor = Color.Black;
						break;
					case "3":
						ecdStatusLabel.Text = "Not Connected";
						ecdStatusLabel.ForeColor = Color.Red;
						break;
					default:
						ecdStatusLabel.Text = "Unkown status";
						ecdStatusLabel.ForeColor = Color.Gray;
						break;
					}
				}
				else
				{
					if (key != Tags.TAG_CD_CAP)
					{
						continue;
					}
					try
					{
						var num = int.Parse(dictionary[Tags.TAG_CD_CAP]);
						if (((uint)num & (true ? 1u : 0u)) != 0 && ((uint)num & 2u) != 0)
						{
							ecdCapLabel.Text = "Manual & Auto";
							ecdCapLabel.ForeColor = Color.Black;
						}
						else if (((uint)num & (true ? 1u : 0u)) != 0)
						{
							ecdCapLabel.Text = "Manual";
							ecdCapLabel.ForeColor = Color.Black;
						}
						else if (((uint)num & 2u) != 0)
						{
							ecdCapLabel.Text = "Auto";
							ecdCapLabel.ForeColor = Color.Black;
						}
						else
						{
							ecdCapLabel.Text = "Unknown Capability";
							ecdCapLabel.ForeColor = Color.Gray;
						}
					}
					catch
					{
						// ignored
					}
				}
			}
		}

		private void DisplayImgReceipt()
		{
			if (imgReceiptViewerForm == null)
			{
				imgReceiptViewerForm = new ImgRcptViewer();
				if (rcptNameEdit.Visible)
				{
					imgReceiptViewerForm.formattedRcptName = rcptNameEdit.Text;
				}
				imgReceiptViewerForm.FormClosing += ImgReceiptViewerForm_FormClosing;
			}
			if (merchImgBytesBuffer.Count != 0)
			{
				imgReceiptViewerForm.MerchantCopyImg = merchImgBytesBuffer;
			}
			if (custImgBytesBuffer.Count != 0)
			{
				imgReceiptViewerForm.CustomerCopyImg = custImgBytesBuffer;
			}
			if (formatRcptImgStringBuffer.Count != 0)
			{
				imgReceiptViewerForm.FormattedRcptCopyImg = formatRcptImgStringBuffer;
			}

			if (imgReceiptViewerForm == null) return;
			if (!imgReceiptViewerForm.Created)
			{
				imgReceiptViewerForm.Show(this);
			}
			imgReceiptViewerForm.Refresh();
		}

		private void DisplayHtmlReceipt()
		{
			if (htmlReceiptViewerForm == null)
			{
				htmlReceiptViewerForm = new HtmlRcptViewer();
				if (rcptNameEdit.Visible)
				{
					htmlReceiptViewerForm.FormattedRcptName = rcptNameEdit.Text;
				}
				htmlReceiptViewerForm.FormClosing += ReceiptViewerForm_FormClosing;
			}
			if (merchHtmlStringBuffer.Count != 0)
			{
				htmlReceiptViewerForm.MerchantCopyHtml = merchHtmlStringBuffer;
			}
			if (custHtmlStringBuffer.Count != 0)
			{
				htmlReceiptViewerForm.CustomerCopyHtml = custHtmlStringBuffer;
			}
			if (formatRcptHtmlStringBuffer.Count != 0)
			{
				htmlReceiptViewerForm.FormattedRcptCopyHtml = formatRcptHtmlStringBuffer;
			}

			if (htmlReceiptViewerForm == null) return;
			if (!htmlReceiptViewerForm.Created)
			{
				htmlReceiptViewerForm.Show(this);
			}
			htmlReceiptViewerForm.Refresh();
		}

		private void DisplayReceipt()
		{
			if (merchImgBytesBuffer.Count != 0 || custImgBytesBuffer.Count != 0 || formatRcptImgStringBuffer.Count != 0)
			{
				DisplayImgReceipt();
			}
			if (merchHtmlStringBuffer.Count != 0 || custHtmlStringBuffer.Count != 0 || formatRcptHtmlStringBuffer.Count != 0)
			{
				DisplayHtmlReceipt();
			}
		}

		private static bool IsPngImage(string sToExtract)
		{
			const string text = "iVBORw0KGgo";
			var result = false;
			const int num = 0;
			if (sToExtract.Length >= text.Length && string.Compare(sToExtract, num, text, num, text.Length) == 0)
			{
				result = true;
			}
			return result;
		}

		private static bool IsHtmlRcpt(string sToExtract)
		{
			const string text = "PCFET0NUWVBFIGh0bWw+";
			var result = false;
			const int num = 0;
			if (sToExtract.Length >= text.Length && sToExtract.Length >= text.Length && string.Compare(sToExtract, num, text, num, text.Length) == 0)
			{
				result = true;
			}
			return result;
		}

		private void ImgReceiptViewerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			imgReceiptViewerForm = null;
		}

		private void ReceiptViewerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			htmlReceiptViewerForm = null;
		}

		private List<Dictionary<int, string>> DisplayResponseMsg(string title, List<byte[]> szResponse)
		{
			Console.WriteLine("DisplayResponseMsg");
			var dataElement = new DataElement();
			var responseMsg = new ResponseMsg();
			var stringBuilder = new StringBuilder();
			var stringBuilderForReport = new StringBuilder();
			var fontStyle = Ticket.SelectionFont.Style;
			var list2 = new List<Dictionary<int, string>>();
			var encodeType = Encoding.UTF7;
			if (80.ToString() == currentTrxType)
			{
				encodeType = Encoding.UTF8;
			}
			foreach (var list in szResponse.Select(t => responseMsg.ParseReceivedResponse(t, bPrintResp: false, encodeType)))
			{
				list2.AddRange(list);
				var text = "\n========= " + title + " Result ==========\n\n";
				Ticket.SelectionStart = Ticket.TextLength;
				Ticket.SelectionLength = text.Length;
				fontStyle |= FontStyle.Bold;
				Ticket.SelectionFont = new Font(Ticket.SelectionFont, fontStyle);
				Ticket.AppendText(text);
				if (list.Count <= 0)
				{
					continue;
				}
				var dictionary = list[list.Count - 1];
				foreach (var key in dictionary.Keys.Where(key => key != dataElement.Get_TransRecordTag()))
				{
					if (key == -1)
					{
						Ticket.AppendText("***Multi Trans Flag : ");
						Ticket.AppendText(dictionary[key]);
						stringBuilderForReport.Append($"{dictionary[key]}/");
					}
					else
					{
						Ticket.AppendText(dataElement.FormatTagThreeDigit(key) + "  " + dataElement.Get_DataListLabel(key) + " : ");
						if (key == Tags.TAG_NBR_RECORDS)
						{
							try
							{
								nbreOfRecord = int.Parse(dictionary[Tags.TAG_NBR_RECORDS]);
								if (nbreOfRecord > 0)
								{
									progressBar1.Maximum = nbreOfRecord;
									progressBar1.Minimum = 0;
									progressBar1.Value = 0;
									progressBar1.Visible = true;
									gettingRptLabel.Visible = true;
									percentLabel.Visible = true;
								}
							}
							catch
							{
								nbreOfRecord = -1;
							}
							Ticket.AppendText(dictionary[key]);
							stringBuilderForReport.Append($"{dictionary[key]}/");
						}
						else if (key == Tags.TAG_ECR_REQUESTED_FORMATTED_RECEIPT)
						{
							stringBuilder.Append(dictionary[Tags.TAG_ECR_REQUESTED_FORMATTED_RECEIPT]);
							DisplayFormattedDataInTrace(dictionary[Tags.TAG_ECR_REQUESTED_FORMATTED_RECEIPT]);
							stringBuilderForReport.Append($"{dictionary[key]}/");
						}
						else
						{
							Ticket.AppendText(dictionary[key]);
							if (key == Tags.TAG_VAS_RESPONSE_DATA)
							{
								DisplayVasDataInTrace(dictionary[key]);
							}
							stringBuilderForReport.Append($"{dictionary[key]}/");
						}
					}
					Ticket.AppendText("\n");
				}
				if (nbreOfRecord > 0)
				{
					var num = progressBar1.Value + list.Count - 1;
					if (progressBar1.Maximum >= num)
					{
						progressBar1.Value += list.Count - 1;
						var value = (float)num / (float)progressBar1.Maximum * 100f;
						percentLabel.Text = Convert.ToInt32(value) + "%";
					}
					else
					{
						progressBar1.Value = progressBar1.Maximum;
						percentLabel.Text = "100%";
					}
				}
				for (var j = 0; j < list.Count - 1; j++)
				{
					dictionary = list[j];
					Ticket.AppendText("\n");
					Ticket.AppendText("RECORD" + (j + 1) + ": (" + dictionary[DataElement.TAG_TRANS_RECORD_TYPE] + ")");
					Ticket.AppendText("\n");
					foreach (var key2 in dictionary.Keys.Where(key2 => key2 < 1000))
					{
						Ticket.AppendText(dataElement.FormatTagThreeDigit(key2) + "  " + dataElement.Get_DataListLabel(key2) + " : ");
						Ticket.AppendText(dictionary[key2]);
						if (key2 == Tags.TAG_VAS_RESPONSE_DATA)
						{
							DisplayVasDataInTrace(dictionary[key2]);
						}
						Ticket.AppendText("\n");
					}
				}
			}
			if (stringBuilder.Length != 0)
			{
				GetReceiptBuffer(stringBuilder, ref formatRcptHtmlStringBuffer, ref formatRcptImgStringBuffer);
			}
			Ticket.ScrollToCaret();
			Console.WriteLine(stringBuilderForReport.ToString());
			Client.SendResponse(stringBuilderForReport.ToString());
			return list2;
		}

		private void EnableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Trace.Enabled = true;
			disableToolStripMenuItem.Enabled = true;
			autoSaveOnExitToolStripMenuItem.Enabled = true;
			reportViewerToolStripMenuItem.Enabled = true;
		}

		private void DisableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Trace.Clear();
			Trace.Enabled = false;
			autoSaveOnExitToolStripMenuItem.Enabled = false;
			enableToolStripMenuItem.Enabled = true;
		}

		private void DisableRptViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			enableToolStripMenuItem1.Enabled = true;
			disableToolStripMenuItem1.Enabled = false;
		}

		private void EnableRptViewerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			enableToolStripMenuItem1.Enabled = false;
			disableToolStripMenuItem1.Enabled = true;
		}

		private void menuStrip_Home_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (Trace.Enabled)
			{
				enableToolStripMenuItem.Enabled = false;
			}
			else
			{
				disableToolStripMenuItem.Enabled = false;
			}
		}

		private static void OpenUserManual()
		{
			try
			{
				Process.Start(new ProcessStartInfo("Help\\0120-08180-0302 WINTSI User's Guide.pdf", ""));
			}
			catch (Exception)
			{
				MessageBox.Show("An error was occured, check your PDF reader!", "Open User Manual Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
		}

		private static void userManualToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenUserManual();
		}

		private static void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutApplication().ShowDialog();
		}

		private void exit_Click(object sender, EventArgs e)
		{
			AutoSaveTrace();
			applicationProtocol.EndApplication();
			Dispose();
		}

		private void AutoSaveTrace()
		{
			if (ReadAutoSaveConfig() != "ON" || !autoSaveOnExitToolStripMenuItem.Enabled) return;
			var now = DateTime.Now;
			const string text = "MM_dd_yyyy_HH_mm";
			var text2 = "Trace_" + now.ToString(text) + ".txt";
			var text3 = Directory.GetCurrentDirectory() + "\\AutoSaveTrace";
			if (!Directory.Exists(text3))
			{
				Directory.CreateDirectory(text3);
			}
			SavingTrace(text3 + "\\" + text2);
		}

		private void HomeForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			AutoSaveTrace();
			applicationProtocol.EndApplication();
		}

		private void communicationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Com.ListPort = Com.GetOpenedPortDescription();
			comSettingForm1 = new ComSettingForm(Com);
			comSettingForm1.ShowDialog();
			DisplayCommSetting();
		}

		private void ClearReceiptButton_Click(object sender, EventArgs e)
		{
			Ticket.Clear();
		}

		private void DisplayCommSetting()
		{
			if (Com.LinkType.Equals("ETHERNET"))
			{
				comSettingStatusLabel.Text = "Ethernet, " + Com.IpAddress + ":" + Com.IpPort;
				protocolLabel.Enabled = false;
				NoneRadioButton.Checked = false;
				NoneRadioButton.Enabled = false;
				NakRadioButton.Enabled = false;
				LateAckRadioButton.Enabled = false;
				BadLrcRadioButton.Enabled = false;
				TimeOutAckRadioButton.Enabled = false;
				RequestGabageRadioButton.Enabled = false;
				NonePrintRadioButton.Checked = true;
				return;
			}
			comSettingStatusLabel.Text = "Serial, " + Com.PortName + ", " + Com.BaudRate + ", " + Communication.DataBit + ", " + Com.Parity + ", " + Communication.StopBit;
			protocolLabel.Enabled = true;
			NoneRadioButton.Checked = true;
			NoneRadioButton.Enabled = true;
			NakRadioButton.Enabled = true;
			LateAckRadioButton.Enabled = true;
			TimeOutAckRadioButton.Enabled = true;
			BadLrcRadioButton.Enabled = true;
			RequestGabageRadioButton.Enabled = true;
			NonePrintRadioButton.Checked = true;
		}

		private bool StartCommunicationProcess()
		{
			var num = applicationProtocol.StartConnection(Com);
			if (num) return true;
			Console.WriteLine("Connection failed: Terminal not found!");
			//MessageBox.Show("Connection failed: Terminal not found!", "WINTSI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}

		private void saveTraceMenu_Click(object sender, EventArgs e)
		{
			saveTrace.RestoreDirectory = true;
			var now = DateTime.Now;
			const string text = "MM_dd_yyyy_HH_mm";
			saveTrace.FileName = "Trace_" + now.ToString(text) + ".txt";
			if (saveTrace.ShowDialog() == DialogResult.OK)
			{
				SavingTrace(saveTrace.FileName);
			}
		}

		private void SavingTrace(string fileName)
		{
			if (fileName.Length <= 0) return;
			try
			{
				Trace.SaveFile(fileName, RichTextBoxStreamType.PlainText);
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Error : Saving Trace has failed");
			}
		}

		private void exportReceiptToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveReceipt.RestoreDirectory = true;
			if (saveReceipt.ShowDialog() != DialogResult.OK || saveReceipt.FileName.Length <= 0) return;
			try
			{
				Ticket.SaveFile(saveReceipt.FileName, RichTextBoxStreamType.PlainText);
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Error : Exporting Trans response has failed");
			}
		}

		private void transactionTypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ConfigTransaction().ShowDialog(this);
			UpdateListTransactionType();
		}

		private void tenderTypeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ConfigTender().ShowDialog(this);
			UpdateListTenderType();
		}

		private void ImportTransTypeMenu_Click(object sender, EventArgs e)
		{
			openConfig.FileName = "Cfg.xml";
			if (openConfig.ShowDialog() != DialogResult.OK || openConfig.FileName.Length <= 0)
			{
				return;
			}
			try
			{
				if (VerifyXmlConfigFile(openConfig.FileName))
				{
					File.Copy(openConfig.FileName, Path.GetDirectoryName(Application.ExecutablePath) + "\\Cfg.xml", overwrite: true);
					UpdateListTenderType();
					UpdateListTransactionType();
					UpdateCBox();
				}
				else
				{
					MessageBox.Show("Invalid file format", "Import Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Error : Importing cfg file failed");
			}
		}

		private void ImportTagListMenu_Click(object sender, EventArgs e)
		{
			openConfig.FileName = "Tag.xml";
			if (openConfig.ShowDialog() != DialogResult.OK || openConfig.FileName.Length <= 0)
			{
				return;
			}
			try
			{
				if (VerifyXmlTagFile(openConfig.FileName))
				{
					File.Copy(openConfig.FileName, Path.GetDirectoryName(Application.ExecutablePath) + "\\Tag.xml", overwrite: true);
				}
				else
				{
					MessageBox.Show("Invalid file format", "Import Configuration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.WriteLine("Error : Importing Tag List file failed");
			}
		}

		private void ExportConfigMenu_Click(object sender, EventArgs e)
		{
			saveConfig.FileName = "Cfg.xml";
			if (saveConfig.ShowDialog() != DialogResult.OK || saveConfig.FileName.Length <= 0) return;
			try
			{
				File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + "\\Cfg.xml", saveConfig.FileName, overwrite: true);
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Error : Exporting cfg file has failed");
			}
		}

		private static bool VerifyXmlConfigFile(string fileName)
		{
			var xmlDocument = new XmlDocument();
			var xmlTextReader = new XmlTextReader(fileName);
			var num = 0;
			try
			{
				while (xmlTextReader.Read())
				{
					XmlNode xmlNode = null;
					switch (xmlTextReader.Name)
					{
						case "TransactionType":
						{
							xmlNode = xmlDocument.ReadNode(xmlTextReader);
							if (!xmlNode.Attributes.Item(0).Name.Equals("Name") || !xmlNode.Attributes.Item(1).Name.Equals("Value"))
							{
								xmlTextReader.Close();
								return false;
							}
							num++;
							break;
						}
						case "TenderType":
						{
							xmlNode = xmlDocument.ReadNode(xmlTextReader);
							if (!xmlNode.Attributes.Item(0).Name.Equals("Name") || !xmlNode.Attributes.Item(1).Name.Equals("Value"))
							{
								xmlTextReader.Close();
								return false;
							}

							break;
						}
					}
				}
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Problem in XML configuration file format");
				xmlTextReader.Close();
				return false;
			}
			xmlTextReader.Close();
			return num > 0;
		}

		private bool VerifyXmlTagFile(string fileName)
		{
			var xmlDocument = new XmlDocument();
			var xmlTextReader = new XmlTextReader(fileName);
			var num = 0;
			try
			{
				while (xmlTextReader.Read())
				{
					if (!xmlTextReader.Name.Equals("DataElement")) continue;
					var xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (!xmlNode.Attributes.Item(0).Name.Equals("Tag") || !xmlNode.Attributes.Item(1).Name.Equals("Description"))
					{
						xmlTextReader.Close();
						return false;
					}
					num++;
				}
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Problem in XML Tag file format");
				xmlTextReader.Close();
				return false;
			}
			xmlTextReader.Close();
			return num > 0;
		}

		private void onToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (onToolStripMenuItem.Enabled)
			{
				SaveAutoSaveConfig("ON");
			}
		}

		private void offToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (offToolStripMenuItem.Enabled)
			{
				SaveAutoSaveConfig("OFF");
			}
		}

		private void recallButton_Click(object sender, EventArgs e)
		{
			formatRcptImgStringBuffer = new List<byte[]>();
			formatRcptHtmlStringBuffer = new List<string>();
			if (!StartCommunicationProcess()) return;
			applicationProtocol.SendRequestToTerminal("91");
			timer1.Start();
		}

		private static string ReadAutoSaveConfig()
		{
			try
			{
				return ConfigurationManager.AppSettings["AutoSave"];
			}
			catch
			{
				return "OFF";
			}
		}

		private static void SaveAutoSaveConfig(string autoSave)
		{
			try
			{
				var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				configuration.AppSettings.Settings.Remove("AutoSave");
				configuration.AppSettings.Settings.Add("AutoSave", autoSave);
				configuration.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("appSettings");
			}
			catch
			{
				System.Diagnostics.Trace.WriteLine("Error : Save AutoSave Config");
			}
		}

		private void settingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var text = ReadAutoSaveConfig();
			switch (text)
			{
				case "ON":
					onToolStripMenuItem.Enabled = false;
					offToolStripMenuItem.Enabled = true;
					break;
				case "OFF":
					onToolStripMenuItem.Enabled = true;
					offToolStripMenuItem.Enabled = false;
					break;
			}
		}

		private bool IsReportRequest(string request)
		{
			switch (request)
			{
			case "30":
			case "31":
			case "32":
			case "33":
			case "34":
			case "35":
			case "36":
			case "37":
			case "38":
			case "39":
			case "40":
			case "41":
			case "42":
			case "43":
				return rbYes.Checked;
			default:
				return false;
			}
		}

		private static bool IsCashDrawerTnx(string response)
		{
			return response == 41.ToString() || response == 40.ToString();
		}

		private void GetStatusBt_Click(object sender, EventArgs e)
		{
			if (!StartCommunicationProcess()) return;
			applicationProtocol.SendRequestToTerminal(70.ToString());
			timer1.Start();
		}

		private void OpenCdBt_Click(object sender, EventArgs e)
		{
			if (!StartCommunicationProcess()) return;
			applicationProtocol.SendRequestToTerminal(71.ToString());
			timer1.Start();
		}

		private void GetCapBt_Click(object sender, EventArgs e)
		{
			if (!StartCommunicationProcess()) return;
			applicationProtocol.SendRequestToTerminal(72.ToString());
			timer1.Start();
		}

		private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var passWordAuth = new PassWordAuth();
			passWordAuth.ShowDialog();
			if (!passWordAuth.isCorrectPassWord) return;
			passWordAuth.Dispose();
			var advancedSetting = new Advanced_Setting();
			advancedSetting.ShowDialog();
			if (advancedSetting.isCDEnabled)
			{
				mainTabCtrl.Controls.Add(cdTabPage);
			}
			else if (mainTabCtrl.TabPages.Count > 1)
			{
				mainTabCtrl.Controls.Remove(cdTabPage);
			}
		}

		private void Amount_EnabledChanged(object sender, EventArgs e)
		{
			amount.BackColor = amount.Enabled ? Color.GreenYellow : Color.Silver;
		}

		private void continueButton_Click(object sender, EventArgs e)
		{
			var indexCmd = this.indexCmd;
			bScriptStopped = false;
			indexCmd = GetNextCmdIndex(indexCmd, commandList);
			if (indexCmd + 1 > commandList.Count) return;
			bScriptStarted = true;
			this.indexCmd = indexCmd;
			ExecuteCommand(commandList[this.indexCmd]);
			this.indexCmd++;
			this.indexCmd = GetNextCmdIndex(this.indexCmd, commandList);
		}

		private void SelectScriptFileButton_Click(object sender, EventArgs e)
		{
			var script = new Script();
			var text = "";
			if (openSciptFileDialog.ShowDialog() != DialogResult.OK || openSciptFileDialog.FileName.Length <= 0)
			{
				return;
			}
			text = openSciptFileDialog.FileName;
			if (text.Length >= 55)
			{
				var num = 55 - ".\\...\\.".Length - 10;
				scriptPathLabel.Text = text.Substring(0, 10) + ".\\...\\." + text.Substring(text.Length - num, num);
			}
			else
			{
				scriptPathLabel.Text = text;
			}
			scriptPathLabel.Visible = true;
			commandList = script.Initialize(text);
			try
			{
				cmdListView.Clear();
				for (var i = 0; i < commandList.Count; i++)
				{
					var listViewItem = new ListViewItem(commandList[i].Label)
					{
						Checked = true
					};
					cmdListView.Items.Add(listViewItem);
				}
				scriptLoopValue = script.GetLoopValue();
				loopLabel.Text = "";
				currentLoopNmb = 1;
				if (scriptLoopValue > 1)
				{
					loopLabel.Text = "Loop:" + currentLoopNmb + "/" + scriptLoopValue;
				}
				cmdListView.Update();
				cmdListView.Refresh();
			}
			catch (Exception)
			{
				System.Diagnostics.Trace.WriteLine("Problem in Reading script file");
			}
		}

		private void StopButton_Click(object sender, EventArgs e)
		{
			bScriptStopped = true;
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			if (scriptPathLabel.Text != "")
			{
				if (cmdListView.Items.Count > 0)
				{
					bScriptStopped = false;
					currentLoopNmb = 1;
					indexCmd = 0;
					foreach (ListViewItem item in cmdListView.Items)
					{
						if (cmdListView.Items[indexCmd] == item && item.Checked)
						{
							item.ForeColor = Color.Black;
						}
					}
					cmdListView.Update();
					cmdListView.Refresh();
					indexCmd = GetNextCmdIndex(indexCmd, commandList);
					if (indexCmd + 1 > commandList.Count) return;
					bScriptStarted = true;
					ExecuteCommand(commandList[indexCmd]);
					indexCmd++;
					indexCmd = GetNextCmdIndex(indexCmd, commandList);
				}
				else
				{
					const MessageBoxButtons buttons = MessageBoxButtons.OK;
					MessageBox.Show("Your list is empty. Check your script file", "Script file", buttons, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				const MessageBoxButtons buttons2 = MessageBoxButtons.OK;
				MessageBox.Show("Select Script File First", "Script file", buttons2, MessageBoxIcon.Exclamation);
			}
		}

		private void CmdListView_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (bScriptStarted && !bScriptStopped && indexCmd + 1 <= commandList.Count)
			{
				e.NewValue = e.CurrentValue;
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
			components = new Container();
			var resources = new ComponentResourceManager(typeof(HomeForm));
			transactionGroupBox = new GroupBox();
			finalAmount = new TextBox();
			finalAmountLabel = new Label();
			rcptNameEdit = new TextBox();
			rcptNameLabel = new Label();
			vasMode = new ComboBox();
			vasModeLabel = new Label();
			chkFormattedRcpt = new CheckBox();
			textEccKey = new TextBox();
			lblEccKey = new Label();
			txtSpecificData = new TextBox();
			lblSpecificData = new Label();
			txtMerchUrl = new TextBox();
			lblMerchUrl = new Label();
			txtMerchId = new TextBox();
			lblMerchId = new Label();
			txtFilterCateg = new TextBox();
			lblFilterCateg = new Label();
			txtEncryptReq = new TextBox();
			lblEncryptReq = new Label();
			txtMerchIndex = new TextBox();
			lblMerchIndex = new Label();
			tranType = new ComboBox();
			refNumber = new TextBox();
			pan = new TextBox();
			panLabel = new Label();
			refNumberLabel = new Label();
			tranTypeLabel = new Label();
			dccCheckBox = new CheckBox();
			recallButton = new Button();
			forcedUp = new CheckBox();
			sendRequest = new Button();
			resetAll = new Button();
			custRefNumTBox = new TextBox();
			custRefNumLabel = new Label();
			reprintTypeCBox = new ComboBox();
			reprintTypeLabel = new Label();
			parameterType = new ComboBox();
			parameterTypeLabel = new Label();
			traceNumTBox = new TextBox();
			traceNumLabel = new Label();
			closeBatch = new ComboBox();
			closeBatchLabel = new Label();
			origRefNum = new TextBox();
			origRefLabel = new Label();
			origSeqNum = new TextBox();
			origSequenceLabel = new Label();
			amount = new TextBox();
			tenderType = new ComboBox();
			tenderTypeLabel = new Label();
			invoiceNum = new TextBox();
			clerkId = new TextBox();
			authorization = new TextBox();
			authorizationLabel = new Label();
			invoiceNumLabel = new Label();
			clerkIdLabel = new Label();
			tnxType = new ComboBox();
			tnxTypeLabel = new Label();
			rbNo = new RadioButton();
			rbYes = new RadioButton();
			traceGroupBox = new GroupBox();
			clearTrace = new Button();
			Trace = new RichTextBox();
			timer1 = new System.Windows.Forms.Timer(components);
			menuStripHome = new MenuStrip();
			mainToolStripMenu = new ToolStripMenuItem();
			importConfigMenu = new ToolStripMenuItem();
			importTagListMenu = new ToolStripMenuItem();
			importTransTypeMenu = new ToolStripMenuItem();
			exportConfigMenu = new ToolStripMenuItem();
			exportReceiptToolStripMenuItem = new ToolStripMenuItem();
			SaveTraceMenu = new ToolStripMenuItem();
			exit = new ToolStripMenuItem();
			settingToolStripMenuItem = new ToolStripMenuItem();
			communicationToolStripMenuItem = new ToolStripMenuItem();
			tenderTypeToolStripMenuItem = new ToolStripMenuItem();
			transactionConfigToolStripMenuItem = new ToolStripMenuItem();
			settingTraceToolStripMenuItem = new ToolStripMenuItem();
			enableToolStripMenuItem = new ToolStripMenuItem();
			disableToolStripMenuItem = new ToolStripMenuItem();
			autoSaveOnExitToolStripMenuItem = new ToolStripMenuItem();
			onToolStripMenuItem = new ToolStripMenuItem();
			offToolStripMenuItem = new ToolStripMenuItem();
			reportViewerToolStripMenuItem = new ToolStripMenuItem();
			enableToolStripMenuItem1 = new ToolStripMenuItem();
			disableToolStripMenuItem1 = new ToolStripMenuItem();
			advancedToolStripMenuItem = new ToolStripMenuItem();
			help = new ToolStripMenuItem();
			userManualToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator5 = new ToolStripSeparator();
			aboutToolStripMenuItem = new ToolStripMenuItem();
			saveTrace = new SaveFileDialog();
			saveReceipt = new SaveFileDialog();
			Ticket = new RichTextBox();
			clearReceiptButton = new Button();
			receiptResponseGroupBox = new GroupBox();
			excepTestgroupBox = new GroupBox();
			panel2 = new Panel();
			printingLabel = new Label();
			LatePrintRadioButton = new RadioButton();
			TimeOutPrintRadioButton = new RadioButton();
			NokPrintRadioButton = new RadioButton();
			NonePrintRadioButton = new RadioButton();
			panel1 = new Panel();
			RequestGabageRadioButton = new RadioButton();
			protocolLabel = new Label();
			LateAckRadioButton = new RadioButton();
			TimeOutAckRadioButton = new RadioButton();
			BadLrcRadioButton = new RadioButton();
			NakRadioButton = new RadioButton();
			NoneRadioButton = new RadioButton();
			valueColumn = new DataGridViewTextBoxColumn();
			panel3 = new Panel();
			mainTabCtrl = new TabControl();
			tnxTabPage = new TabPage();
			scriptTabPage = new TabPage();
			scriptPathLabel = new Label();
			loopLabel = new Label();
			startButton = new Button();
			stopButton = new Button();
			cmdListView = new ListView();
			selectScriptFileNameLabel = new Label();
			selectScriptFileButton = new Button();
			continueButton = new Button();
			isReportGroupBox = new GroupBox();
			percentLabel = new Label();
			gettingRptLabel = new Label();
			progressBar1 = new ProgressBar();
			statusStrip1 = new StatusStrip();
			toolStripStatusLabel1 = new ToolStripStatusLabel();
			toolStripStatusLabel2 = new ToolStripStatusLabel();
			comSettingStatusLabel = new ToolStripStatusLabel();
			groupBox1 = new GroupBox();
			ecdCapLabel = new Label();
			ecdStatusLabel = new Label();
			getStatusBt = new Button();
			getCapBt = new Button();
			openCdBt = new Button();
			openConfig = new OpenFileDialog();
			openSciptFileDialog = new OpenFileDialog();
			saveConfig = new SaveFileDialog();
			cdTabPage = new TabPage();
			searchAmount = new TextBox();
			searchAmountLabel = new Label();
			transactionGroupBox.SuspendLayout();
			traceGroupBox.SuspendLayout();
			menuStripHome.SuspendLayout();
			receiptResponseGroupBox.SuspendLayout();
			excepTestgroupBox.SuspendLayout();
			panel2.SuspendLayout();
			panel1.SuspendLayout();
			panel3.SuspendLayout();
			mainTabCtrl.SuspendLayout();
			tnxTabPage.SuspendLayout();
			scriptTabPage.SuspendLayout();
			isReportGroupBox.SuspendLayout();
			statusStrip1.SuspendLayout();
			groupBox1.SuspendLayout();
			cdTabPage.SuspendLayout();
			SuspendLayout();
			transactionGroupBox.Controls.Add(searchAmountLabel);
			transactionGroupBox.Controls.Add(searchAmount);
			transactionGroupBox.Controls.Add(finalAmount);
			transactionGroupBox.Controls.Add(finalAmountLabel);
			transactionGroupBox.Controls.Add(rcptNameEdit);
			transactionGroupBox.Controls.Add(rcptNameLabel);
			transactionGroupBox.Controls.Add(vasMode);
			transactionGroupBox.Controls.Add(vasModeLabel);
			transactionGroupBox.Controls.Add(chkFormattedRcpt);
			transactionGroupBox.Controls.Add(textEccKey);
			transactionGroupBox.Controls.Add(lblEccKey);
			transactionGroupBox.Controls.Add(txtSpecificData);
			transactionGroupBox.Controls.Add(lblSpecificData);
			transactionGroupBox.Controls.Add(txtMerchUrl);
			transactionGroupBox.Controls.Add(lblMerchUrl);
			transactionGroupBox.Controls.Add(txtMerchId);
			transactionGroupBox.Controls.Add(lblMerchId);
			transactionGroupBox.Controls.Add(txtFilterCateg);
			transactionGroupBox.Controls.Add(lblFilterCateg);
			transactionGroupBox.Controls.Add(txtEncryptReq);
			transactionGroupBox.Controls.Add(lblEncryptReq);
			transactionGroupBox.Controls.Add(txtMerchIndex);
			transactionGroupBox.Controls.Add(lblMerchIndex);
			transactionGroupBox.Controls.Add(tranType);
			transactionGroupBox.Controls.Add(refNumber);
			transactionGroupBox.Controls.Add(pan);
			transactionGroupBox.Controls.Add(panLabel);
			transactionGroupBox.Controls.Add(refNumberLabel);
			transactionGroupBox.Controls.Add(tranTypeLabel);
			transactionGroupBox.Controls.Add(dccCheckBox);
			transactionGroupBox.Controls.Add(recallButton);
			transactionGroupBox.Controls.Add(forcedUp);
			transactionGroupBox.Controls.Add(sendRequest);
			transactionGroupBox.Controls.Add(resetAll);
			transactionGroupBox.Controls.Add(custRefNumTBox);
			transactionGroupBox.Controls.Add(custRefNumLabel);
			transactionGroupBox.Controls.Add(reprintTypeCBox);
			transactionGroupBox.Controls.Add(reprintTypeLabel);
			transactionGroupBox.Controls.Add(parameterType);
			transactionGroupBox.Controls.Add(parameterTypeLabel);
			transactionGroupBox.Controls.Add(traceNumTBox);
			transactionGroupBox.Controls.Add(traceNumLabel);
			transactionGroupBox.Controls.Add(closeBatch);
			transactionGroupBox.Controls.Add(closeBatchLabel);
			transactionGroupBox.Controls.Add(origRefNum);
			transactionGroupBox.Controls.Add(origRefLabel);
			transactionGroupBox.Controls.Add(origSeqNum);
			transactionGroupBox.Controls.Add(origSequenceLabel);
			transactionGroupBox.Controls.Add(amount);
			transactionGroupBox.Controls.Add(tenderType);
			transactionGroupBox.Controls.Add(tenderTypeLabel);
			transactionGroupBox.Controls.Add(invoiceNum);
			transactionGroupBox.Controls.Add(clerkId);
			transactionGroupBox.Controls.Add(authorization);
			transactionGroupBox.Controls.Add(authorizationLabel);
			transactionGroupBox.Controls.Add(invoiceNumLabel);
			transactionGroupBox.Controls.Add(clerkIdLabel);
			transactionGroupBox.Controls.Add(tnxType);
			transactionGroupBox.Controls.Add(tnxTypeLabel);
			transactionGroupBox.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			transactionGroupBox.Location = new Point(2, -6);
			transactionGroupBox.Name = "transactionGroupBox";
			transactionGroupBox.Size = new Size(467, 338);
			transactionGroupBox.TabIndex = 1;
			transactionGroupBox.TabStop = false;
			finalAmount.Location = new Point(78, 278);
			finalAmount.MaxLength = 30;
			finalAmount.Name = "finalAmount";
			finalAmount.Size = new Size(138, 20);
			finalAmount.TabIndex = 77;
			finalAmount.Tag = "0";
			finalAmount.Visible = false;
			finalAmount.MouseClick += new MouseEventHandler(Amount_MouseClick);
			finalAmount.KeyDown += new KeyEventHandler(Amount_KeyDown);
			finalAmount.KeyPress += new KeyPressEventHandler(Amount_KeyPress);
			finalAmount.MouseDoubleClick += new MouseEventHandler(Amount_MouseDoubleClick);
			finalAmount.MouseDown += new MouseEventHandler(Amount_MouseDown);
			finalAmount.MouseEnter += new EventHandler(Amount_MouseEnter);
			finalAmountLabel.Location = new Point(4, 280);
			finalAmountLabel.Name = "finalAmountLabel";
			finalAmountLabel.Size = new Size(68, 13);
			finalAmountLabel.TabIndex = 76;
			finalAmountLabel.Text = "Final Amount";
			finalAmountLabel.Visible = false;
			rcptNameEdit.Location = new Point(80, 243);
			rcptNameEdit.MaxLength = 30;
			rcptNameEdit.Name = "rcptNameEdit";
			rcptNameEdit.Size = new Size(138, 20);
			rcptNameEdit.TabIndex = 62;
			rcptNameEdit.Visible = false;
			rcptNameLabel.AutoSize = true;
			rcptNameLabel.Location = new Point(20, 255);
			rcptNameLabel.Name = "rcptNameLabel";
			rcptNameLabel.Size = new Size(61, 13);
			rcptNameLabel.TabIndex = 60;
			rcptNameLabel.Text = "Rcpt Name";
			rcptNameLabel.Visible = false;
			vasMode.DropDownStyle = ComboBoxStyle.DropDownList;
			vasMode.FormattingEnabled = true;
			vasMode.Items.AddRange(new object[4] { "VAS Only", "VAS and payment", "No VAS", "VAS or payment" });
			vasMode.Location = new Point(380, 205);
			vasMode.Name = "vasMode";
			vasMode.Size = new Size(138, 21);
			vasMode.TabIndex = 66;
			vasMode.Visible = false;
			vasModeLabel.AutoSize = true;
			vasModeLabel.Location = new Point(240, 205);
			vasModeLabel.Name = "vasModeLabel";
			vasModeLabel.Size = new Size(55, 13);
			vasModeLabel.TabIndex = 67;
			vasModeLabel.Text = "Vas Mode";
			vasModeLabel.Visible = false;
			chkFormattedRcpt.AutoSize = true;
			chkFormattedRcpt.Location = new Point(310, 243);
			chkFormattedRcpt.Name = "chkFormattedRcpt";
			chkFormattedRcpt.Size = new Size(108, 17);
			chkFormattedRcpt.TabIndex = 58;
			chkFormattedRcpt.Text = "Formatted receipt";
			chkFormattedRcpt.UseVisualStyleBackColor = true;
			chkFormattedRcpt.Visible = false;
			textEccKey.Location = new Point(78, 239);
			textEccKey.Name = "textEccKey";
			textEccKey.Size = new Size(140, 20);
			textEccKey.TabIndex = 74;
			textEccKey.Visible = false;
			lblEccKey.AutoSize = true;
			lblEccKey.Location = new Point(25, 235);
			lblEccKey.Name = "lblEccKey";
			lblEccKey.Size = new Size(49, 13);
			lblEccKey.TabIndex = 75;
			lblEccKey.Text = "ECC Key";
			lblEccKey.Visible = false;
			txtSpecificData.Location = new Point(360, 205);
			txtSpecificData.MaxLength = 1;
			txtSpecificData.Name = "txtSpecificData";
			txtSpecificData.Size = new Size(88, 20);
			txtSpecificData.TabIndex = 56;
			txtSpecificData.Visible = false;
			lblSpecificData.AutoSize = true;
			lblSpecificData.Location = new Point(240, 205);
			lblSpecificData.Name = "lblSpecificData";
			lblSpecificData.Size = new Size(109, 13);
			lblSpecificData.TabIndex = 57;
			lblSpecificData.Text = "Acquirer specific data";
			lblSpecificData.Visible = false;
			txtMerchUrl.Location = new Point(380, 205);
			txtMerchUrl.Name = "txtMerchUrl";
			txtMerchUrl.Size = new Size(140, 20);
			txtMerchUrl.TabIndex = 58;
			txtMerchUrl.Visible = false;
			lblMerchUrl.AutoSize = true;
			lblMerchUrl.Location = new Point(240, 205);
			lblMerchUrl.Name = "lblMerchUrl";
			lblMerchUrl.Size = new Size(62, 13);
			lblMerchUrl.TabIndex = 59;
			lblMerchUrl.Text = "Merch URL";
			lblMerchUrl.Visible = false;
			txtMerchId.Location = new Point(380, 220);
			txtMerchId.Name = "txtMerchId";
			txtMerchId.Size = new Size(140, 20);
			txtMerchId.TabIndex = 60;
			txtMerchId.Visible = false;
			lblMerchId.AutoSize = true;
			lblMerchId.Location = new Point(240, 220);
			lblMerchId.Name = "lblMerchId";
			lblMerchId.Size = new Size(51, 13);
			lblMerchId.TabIndex = 61;
			lblMerchId.Text = "Merch ID";
			lblMerchId.Visible = false;
			txtFilterCateg.Location = new Point(380, 235);
			txtFilterCateg.Name = "txtFilterCateg";
			txtFilterCateg.Size = new Size(140, 20);
			txtFilterCateg.TabIndex = 62;
			txtFilterCateg.Visible = false;
			lblFilterCateg.AutoSize = true;
			lblFilterCateg.Location = new Point(240, 235);
			lblFilterCateg.Name = "lblFilterCateg";
			lblFilterCateg.Size = new Size(60, 13);
			lblFilterCateg.TabIndex = 63;
			lblFilterCateg.Text = "Filter Categ";
			lblFilterCateg.Visible = false;
			txtEncryptReq.Location = new Point(380, 250);
			txtEncryptReq.Name = "txtEncryptReq";
			txtEncryptReq.Size = new Size(140, 20);
			txtEncryptReq.TabIndex = 64;
			txtEncryptReq.Visible = false;
			lblEncryptReq.AutoSize = true;
			lblEncryptReq.Location = new Point(240, 250);
			lblEncryptReq.Name = "lblEncryptReq";
			lblEncryptReq.Size = new Size(66, 13);
			lblEncryptReq.TabIndex = 65;
			lblEncryptReq.Text = "Encrypt Req";
			lblEncryptReq.Visible = false;
			txtMerchIndex.Location = new Point(380, 205);
			txtMerchIndex.Name = "txtMerchIndex";
			txtMerchIndex.Size = new Size(140, 20);
			txtMerchIndex.TabIndex = 70;
			txtMerchIndex.Visible = false;
			lblMerchIndex.AutoSize = true;
			lblMerchIndex.Location = new Point(240, 205);
			lblMerchIndex.Name = "lblMerchIndex";
			lblMerchIndex.Size = new Size(65, 13);
			lblMerchIndex.TabIndex = 71;
			lblMerchIndex.Text = "Merch index";
			lblMerchIndex.Visible = false;
			tranType.DropDownStyle = ComboBoxStyle.DropDownList;
			tranType.FormattingEnabled = true;
			tranType.Items.AddRange(new object[3] { "None", "Open Batch", "Pre-Auth" });
			tranType.Location = new Point(80, 176);
			tranType.Name = "tranType";
			tranType.Size = new Size(138, 21);
			tranType.TabIndex = 9;
			tranType.Visible = false;
			refNumber.Location = new Point(310, 179);
			refNumber.MaxLength = 30;
			refNumber.Name = "refNumber";
			refNumber.Size = new Size(138, 20);
			refNumber.TabIndex = 7;
			refNumber.Visible = false;
			pan.Location = new Point(80, 201);
			pan.MaxLength = 30;
			pan.Name = "pan";
			pan.Size = new Size(138, 20);
			pan.TabIndex = 8;
			pan.Visible = false;
			panLabel.AutoSize = true;
			panLabel.Location = new Point(10, 203);
			panLabel.Name = "panLabel";
			panLabel.Size = new Size(29, 13);
			panLabel.TabIndex = 54;
			panLabel.Text = "PAN";
			panLabel.Visible = false;
			refNumberLabel.AutoSize = true;
			refNumberLabel.Location = new Point(240, 179);
			refNumberLabel.Name = "refNumberLabel";
			refNumberLabel.Size = new Size(67, 13);
			refNumberLabel.TabIndex = 52;
			refNumberLabel.Text = "Reference #";
			refNumberLabel.Visible = false;
			tranTypeLabel.AutoSize = true;
			tranTypeLabel.Location = new Point(10, 179);
			tranTypeLabel.Name = "tranTypeLabel";
			tranTypeLabel.Size = new Size(71, 13);
			tranTypeLabel.TabIndex = 50;
			tranTypeLabel.Text = "Orig TnxType";
			tranTypeLabel.Visible = false;
			dccCheckBox.AutoSize = true;
			dccCheckBox.Location = new Point(80, 227);
			dccCheckBox.Name = "dccCheckBox";
			dccCheckBox.Size = new Size(78, 17);
			dccCheckBox.TabIndex = 46;
			dccCheckBox.Text = "DCC Trans";
			dccCheckBox.UseVisualStyleBackColor = true;
			dccCheckBox.Visible = false;
			recallButton.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			recallButton.Location = new Point(360, 310);
			recallButton.Name = "recallButton";
			recallButton.Size = new Size(88, 25);
			recallButton.TabIndex = 21;
			recallButton.Text = "Recall Request";
			recallButton.UseVisualStyleBackColor = true;
			recallButton.Click += new EventHandler(recallButton_Click);
			forcedUp.AutoSize = true;
			forcedUp.Location = new Point(310, 226);
			forcedUp.Name = "forcedUp";
			forcedUp.Size = new Size(77, 17);
			forcedUp.TabIndex = 43;
			forcedUp.Text = "Forced UP";
			forcedUp.UseVisualStyleBackColor = true;
			forcedUp.Visible = false;
			sendRequest.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			sendRequest.Location = new Point(80, 310);
			sendRequest.Name = "sendRequest";
			sendRequest.Size = new Size(88, 25);
			sendRequest.TabIndex = 19;
			sendRequest.Text = "Send Request";
			sendRequest.UseVisualStyleBackColor = true;
			sendRequest.Click += new EventHandler(SendRequest_Click);
			resetAll.Location = new Point(266, 310);
			resetAll.Name = "resetAll";
			resetAll.Size = new Size(88, 25);
			resetAll.TabIndex = 20;
			resetAll.Text = "Reset All";
			resetAll.UseVisualStyleBackColor = true;
			resetAll.Click += new EventHandler(ResetAll_Click);
			custRefNumTBox.Location = new Point(310, 42);
			custRefNumTBox.MaxLength = 30;
			custRefNumTBox.Name = "custRefNumTBox";
			custRefNumTBox.Size = new Size(138, 20);
			custRefNumTBox.TabIndex = 6;
			custRefNumTBox.Visible = false;
			custRefNumLabel.AutoSize = true;
			custRefNumLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			custRefNumLabel.Location = new Point(240, 42);
			custRefNumLabel.Name = "custRefNumLabel";
			custRefNumLabel.Size = new Size(58, 13);
			custRefNumLabel.TabIndex = 44;
			custRefNumLabel.Text = "Cust Ref #";
			custRefNumLabel.Visible = false;
			reprintTypeCBox.DropDownStyle = ComboBoxStyle.DropDownList;
			reprintTypeCBox.FormattingEnabled = true;
			reprintTypeCBox.Items.AddRange(new object[2] { "merchant copy ", "customer copy" });
			reprintTypeCBox.Location = new Point(310, 71);
			reprintTypeCBox.Name = "reprintTypeCBox";
			reprintTypeCBox.Size = new Size(138, 21);
			reprintTypeCBox.TabIndex = 42;
			reprintTypeCBox.Visible = false;
			reprintTypeLabel.AutoSize = true;
			reprintTypeLabel.Location = new Point(240, 73);
			reprintTypeLabel.Name = "reprintTypeLabel";
			reprintTypeLabel.Size = new Size(65, 13);
			reprintTypeLabel.TabIndex = 41;
			reprintTypeLabel.Text = "ReprintType";
			reprintTypeLabel.Visible = false;
			parameterType.DropDownStyle = ComboBoxStyle.DropDownList;
			parameterType.FormattingEnabled = true;
			parameterType.Items.AddRange(new object[10] { "Clerk Parameters", "Comms Parameters", "Receipts Parameters", "Trans Option Parameters", "Terminal Setting Parameters", "Security Parameters", "Setup Parameters", "Download Only Parameters", "All Parameters", "Semi-Integrated Parameters" });
			parameterType.Location = new Point(310, 96);
			parameterType.Name = "parameterType";
			parameterType.Size = new Size(138, 21);
			parameterType.TabIndex = 10;
			parameterType.Visible = false;
			parameterTypeLabel.AutoSize = true;
			parameterTypeLabel.Location = new Point(240, 100);
			parameterTypeLabel.Name = "parameterTypeLabel";
			parameterTypeLabel.Size = new Size(64, 13);
			parameterTypeLabel.TabIndex = 40;
			parameterTypeLabel.Text = "Param Type";
			parameterTypeLabel.Visible = false;
			traceNumTBox.Location = new Point(310, 16);
			traceNumTBox.MaxLength = 30;
			traceNumTBox.Name = "traceNumTBox";
			traceNumTBox.Size = new Size(138, 20);
			traceNumTBox.TabIndex = 38;
			traceNumTBox.Visible = false;
			traceNumLabel.AutoSize = true;
			traceNumLabel.Location = new Point(240, 19);
			traceNumLabel.Name = "traceNumLabel";
			traceNumLabel.Size = new Size(45, 13);
			traceNumLabel.TabIndex = 37;
			traceNumLabel.Text = "Trace #";
			traceNumLabel.Visible = false;
			closeBatch.DropDownStyle = ComboBoxStyle.DropDownList;
			closeBatch.FormattingEnabled = true;
			closeBatch.Items.AddRange(new object[3] { "Payment", "Ernex", "Payment and Ernex" });
			closeBatch.Location = new Point(310, 123);
			closeBatch.Name = "closeBatch";
			closeBatch.Size = new Size(138, 21);
			closeBatch.TabIndex = 34;
			closeBatch.Visible = false;
			closeBatchLabel.AutoSize = true;
			closeBatchLabel.Location = new Point(240, 126);
			closeBatchLabel.Name = "closeBatchLabel";
			closeBatchLabel.Size = new Size(64, 13);
			closeBatchLabel.TabIndex = 35;
			closeBatchLabel.Text = "Close Batch";
			closeBatchLabel.Visible = false;
			origRefNum.Location = new Point(80, 51);
			origRefNum.MaxLength = 30;
			origRefNum.Name = "origRefNum";
			origRefNum.Size = new Size(138, 20);
			origRefNum.TabIndex = 32;
			origRefNum.Visible = false;
			origRefLabel.AutoSize = true;
			origRefLabel.Location = new Point(10, 54);
			origRefLabel.Name = "origRefLabel";
			origRefLabel.Size = new Size(59, 13);
			origRefLabel.TabIndex = 31;
			origRefLabel.Text = "Orig. Ref #";
			origRefLabel.Visible = false;
			origSeqNum.Location = new Point(80, 74);
			origSeqNum.MaxLength = 30;
			origSeqNum.Name = "origSeqNum";
			origSeqNum.Size = new Size(138, 20);
			origSeqNum.TabIndex = 30;
			origSeqNum.Visible = false;
			origSeqNum.WordWrap = false;
			origSequenceLabel.AutoSize = true;
			origSequenceLabel.Location = new Point(10, 76);
			origSequenceLabel.Name = "origSequenceLabel";
			origSequenceLabel.Size = new Size(61, 13);
			origSequenceLabel.TabIndex = 29;
			origSequenceLabel.Text = "Orig. Seq #";
			origSequenceLabel.Visible = false;
			amount.BackColor = Color.GreenYellow;
			amount.Font = new Font("Microsoft Sans Serif", 26.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			amount.ForeColor = SystemColors.ControlDarkDark;
			amount.Location = new Point(80, 260);
			amount.Multiline = true;
			amount.Name = "amount";
			amount.Size = new Size(368, 44);
			amount.TabIndex = 18;
			amount.Tag = "0";
			amount.Text = "$0.00";
			amount.TextAlign = HorizontalAlignment.Center;
			amount.MouseClick += new MouseEventHandler(Amount_MouseClick);
			amount.EnabledChanged += new EventHandler(Amount_EnabledChanged);
			amount.KeyDown += new KeyEventHandler(Amount_KeyDown);
			amount.KeyPress += new KeyPressEventHandler(Amount_KeyPress);
			amount.MouseDoubleClick += new MouseEventHandler(Amount_MouseDoubleClick);
			amount.MouseDown += new MouseEventHandler(Amount_MouseDown);
			amount.MouseEnter += new EventHandler(Amount_MouseEnter);
			amount.MouseUp += new MouseEventHandler(Amount_MouseUp);
			tenderType.DropDownStyle = ComboBoxStyle.DropDownList;
			tenderType.FormattingEnabled = true;
			tenderType.Items.AddRange(new object[1] { "" });
			tenderType.Location = new Point(80, 100);
			tenderType.Name = "tenderType";
			tenderType.Size = new Size(138, 21);
			tenderType.TabIndex = 2;
			tenderType.Visible = false;
			tenderTypeLabel.AutoSize = true;
			tenderTypeLabel.Location = new Point(6, 104);
			tenderTypeLabel.Name = "tenderTypeLabel";
			tenderTypeLabel.Size = new Size(68, 13);
			tenderTypeLabel.TabIndex = 28;
			tenderTypeLabel.Text = "Tender Type";
			tenderTypeLabel.Visible = false;
			invoiceNum.Location = new Point(80, 125);
			invoiceNum.MaxLength = 40;
			invoiceNum.Name = "invoiceNum";
			invoiceNum.Size = new Size(138, 20);
			invoiceNum.TabIndex = 4;
			invoiceNum.Visible = false;
			clerkId.Location = new Point(80, 152);
			clerkId.MaxLength = 30;
			clerkId.Name = "clerkId";
			clerkId.Size = new Size(138, 20);
			clerkId.TabIndex = 3;
			clerkId.Visible = false;
			authorization.Location = new Point(310, 150);
			authorization.MaxLength = 30;
			authorization.Name = "authorization";
			authorization.Size = new Size(138, 20);
			authorization.TabIndex = 5;
			authorization.Visible = false;
			authorizationLabel.AutoSize = true;
			authorizationLabel.Location = new Point(240, 153);
			authorizationLabel.Name = "authorizationLabel";
			authorizationLabel.Size = new Size(39, 13);
			authorizationLabel.TabIndex = 7;
			authorizationLabel.Text = "Auth #";
			authorizationLabel.Visible = false;
			invoiceNumLabel.AutoSize = true;
			invoiceNumLabel.Location = new Point(10, 128);
			invoiceNumLabel.Name = "invoiceNumLabel";
			invoiceNumLabel.Size = new Size(52, 13);
			invoiceNumLabel.TabIndex = 5;
			invoiceNumLabel.Text = "Invoice #";
			invoiceNumLabel.Visible = false;
			clerkIdLabel.AutoSize = true;
			clerkIdLabel.Location = new Point(10, 153);
			clerkIdLabel.Name = "clerkIdLabel";
			clerkIdLabel.Size = new Size(43, 13);
			clerkIdLabel.TabIndex = 4;
			clerkIdLabel.Text = "Clerk Id";
			clerkIdLabel.Visible = false;
			tnxType.DropDownStyle = ComboBoxStyle.DropDownList;
			tnxType.FormattingEnabled = true;
			tnxType.Items.AddRange(new object[1] { "" });
			tnxType.Location = new Point(80, 26);
			tnxType.Name = "tnxType";
			tnxType.Size = new Size(138, 21);
			tnxType.TabIndex = 1;
			tnxType.SelectedIndexChanged += new EventHandler(TnxType_SelectedIndexChanged);
			tnxTypeLabel.AutoSize = true;
			tnxTypeLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			tnxTypeLabel.Location = new Point(10, 29);
			tnxTypeLabel.Name = "tnxTypeLabel";
			tnxTypeLabel.Size = new Size(61, 13);
			tnxTypeLabel.TabIndex = 0;
			tnxTypeLabel.Text = "Trans Type";
			rbNo.AutoSize = true;
			rbNo.Checked = true;
			rbNo.Location = new Point(89, 17);
			rbNo.Name = "rbNo";
			rbNo.Size = new Size(39, 17);
			rbNo.TabIndex = 23;
			rbNo.TabStop = true;
			rbNo.Text = "No";
			rbNo.UseVisualStyleBackColor = true;
			rbYes.AutoSize = true;
			rbYes.Location = new Point(17, 17);
			rbYes.Name = "rbYes";
			rbYes.Size = new Size(43, 17);
			rbYes.TabIndex = 22;
			rbYes.Text = "Yes";
			rbYes.UseVisualStyleBackColor = true;
			traceGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			traceGroupBox.Controls.Add(clearTrace);
			traceGroupBox.Controls.Add(Trace);
			traceGroupBox.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			traceGroupBox.Location = new Point(10, 371);
			traceGroupBox.Name = "traceGroupBox";
			traceGroupBox.Size = new Size(660, 365);
			traceGroupBox.TabIndex = 2;
			traceGroupBox.TabStop = false;
			traceGroupBox.Text = "Trace";
			clearTrace.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			clearTrace.Location = new Point(564, 334);
			clearTrace.Name = "clearTrace";
			clearTrace.Size = new Size(82, 25);
			clearTrace.TabIndex = 35;
			clearTrace.Text = "Clear Trace";
			clearTrace.UseVisualStyleBackColor = true;
			clearTrace.Click += new EventHandler(ClearTrace_Click);
			Trace.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			Trace.BackColor = Color.White;
			Trace.BorderStyle = BorderStyle.FixedSingle;
			Trace.Font = new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			Trace.Location = new Point(8, 18);
			Trace.Name = "Trace";
			Trace.ReadOnly = true;
			Trace.Size = new Size(638, 310);
			Trace.TabIndex = 34;
			Trace.Text = "";
			timer1.Interval = 50;
			timer1.Tick += new EventHandler(timer1_Tick);
			menuStripHome.BackColor = SystemColors.Control;
			menuStripHome.Items.AddRange(new ToolStripItem[3] { mainToolStripMenu, settingToolStripMenuItem, help });
			menuStripHome.Location = new Point(0, 0);
			menuStripHome.Name = "menuStripHome";
			menuStripHome.Size = new Size(1000, 24);
			menuStripHome.TabIndex = 4;
			menuStripHome.Text = "menuStrip1";
			menuStripHome.ItemClicked += new ToolStripItemClickedEventHandler(menuStrip_Home_ItemClicked);
			mainToolStripMenu.DropDownItems.AddRange(new ToolStripItem[5] { importConfigMenu, exportConfigMenu, exportReceiptToolStripMenuItem, SaveTraceMenu, exit });
			mainToolStripMenu.Name = "mainToolStripMenu";
			mainToolStripMenu.Size = new Size(37, 20);
			mainToolStripMenu.Text = "File";
			importConfigMenu.DropDownItems.AddRange(new ToolStripItem[2] { importTagListMenu, importTransTypeMenu });
			importConfigMenu.Name = "importConfigMenu";
			importConfigMenu.Size = new Size(248, 22);
			importConfigMenu.Text = "Import Configuration";
			importTagListMenu.Name = "importTagListMenu";
			importTagListMenu.Size = new Size(212, 22);
			importTagListMenu.Text = "Tag Configuration";
			importTagListMenu.Click += new EventHandler(ImportTagListMenu_Click);
			importTransTypeMenu.Name = "importTransTypeMenu";
			importTransTypeMenu.Size = new Size(212, 22);
			importTransTypeMenu.Text = "Transaction Configuration";
			importTransTypeMenu.Click += new EventHandler(ImportTransTypeMenu_Click);
			exportConfigMenu.Name = "exportConfigMenu";
			exportConfigMenu.Size = new Size(248, 22);
			exportConfigMenu.Text = "Export Transaction Configuration";
			exportConfigMenu.Click += new EventHandler(ExportConfigMenu_Click);
			exportReceiptToolStripMenuItem.Name = "exportReceiptToolStripMenuItem";
			exportReceiptToolStripMenuItem.Size = new Size(248, 22);
			exportReceiptToolStripMenuItem.Text = "Export Response";
			exportReceiptToolStripMenuItem.Click += new EventHandler(exportReceiptToolStripMenuItem_Click);
			SaveTraceMenu.Name = "SaveTraceMenu";
			SaveTraceMenu.Size = new Size(248, 22);
			SaveTraceMenu.Text = "Export Trace";
			SaveTraceMenu.Click += new EventHandler(saveTraceMenu_Click);
			exit.Name = "exit";
			exit.Size = new Size(248, 22);
			exit.Text = "Exit";
			exit.Click += new EventHandler(exit_Click);
			settingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[7] { communicationToolStripMenuItem, tenderTypeToolStripMenuItem, transactionConfigToolStripMenuItem, settingTraceToolStripMenuItem, autoSaveOnExitToolStripMenuItem, reportViewerToolStripMenuItem, advancedToolStripMenuItem });
			settingToolStripMenuItem.Name = "settingToolStripMenuItem";
			settingToolStripMenuItem.Size = new Size(56, 20);
			settingToolStripMenuItem.Text = "Setting";
			settingToolStripMenuItem.Click += new EventHandler(settingToolStripMenuItem_Click);
			communicationToolStripMenuItem.Name = "communicationToolStripMenuItem";
			communicationToolStripMenuItem.Size = new Size(165, 22);
			communicationToolStripMenuItem.Text = "Communication ";
			communicationToolStripMenuItem.Click += new EventHandler(communicationToolStripMenuItem_Click);
			tenderTypeToolStripMenuItem.Name = "tenderTypeToolStripMenuItem";
			tenderTypeToolStripMenuItem.Size = new Size(165, 22);
			tenderTypeToolStripMenuItem.Text = "Tender";
			tenderTypeToolStripMenuItem.Click += new EventHandler(tenderTypeToolStripMenuItem_Click);
			transactionConfigToolStripMenuItem.Name = "transactionConfigToolStripMenuItem";
			transactionConfigToolStripMenuItem.Size = new Size(165, 22);
			transactionConfigToolStripMenuItem.Text = "Transaction";
			transactionConfigToolStripMenuItem.Click += new EventHandler(transactionTypeToolStripMenuItem_Click);
			settingTraceToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2] { enableToolStripMenuItem, disableToolStripMenuItem });
			settingTraceToolStripMenuItem.Name = "settingTraceToolStripMenuItem";
			settingTraceToolStripMenuItem.Size = new Size(165, 22);
			settingTraceToolStripMenuItem.Text = "Trace";
			enableToolStripMenuItem.Name = "enableToolStripMenuItem";
			enableToolStripMenuItem.Size = new Size(112, 22);
			enableToolStripMenuItem.Text = "Enable";
			enableToolStripMenuItem.Click += new EventHandler(EnableToolStripMenuItem_Click);
			disableToolStripMenuItem.Name = "disableToolStripMenuItem";
			disableToolStripMenuItem.Size = new Size(112, 22);
			disableToolStripMenuItem.Text = "Disable";
			disableToolStripMenuItem.Click += new EventHandler(DisableToolStripMenuItem_Click);
			autoSaveOnExitToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2] { onToolStripMenuItem, offToolStripMenuItem });
			autoSaveOnExitToolStripMenuItem.Name = "autoSaveOnExitToolStripMenuItem";
			autoSaveOnExitToolStripMenuItem.Size = new Size(165, 22);
			autoSaveOnExitToolStripMenuItem.Text = "Auto Save on Exit";
			onToolStripMenuItem.Name = "onToolStripMenuItem";
			onToolStripMenuItem.Size = new Size(112, 22);
			onToolStripMenuItem.Text = "Enable";
			onToolStripMenuItem.Click += new EventHandler(onToolStripMenuItem_Click);
			offToolStripMenuItem.Name = "offToolStripMenuItem";
			offToolStripMenuItem.Size = new Size(112, 22);
			offToolStripMenuItem.Text = "Disable";
			offToolStripMenuItem.Click += new EventHandler(offToolStripMenuItem_Click);
			reportViewerToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2] { enableToolStripMenuItem1, disableToolStripMenuItem1 });
			reportViewerToolStripMenuItem.Name = "reportViewerToolStripMenuItem";
			reportViewerToolStripMenuItem.Size = new Size(165, 22);
			reportViewerToolStripMenuItem.Text = "Report Viewer";
			enableToolStripMenuItem1.Enabled = false;
			enableToolStripMenuItem1.Name = "enableToolStripMenuItem1";
			enableToolStripMenuItem1.Size = new Size(112, 22);
			enableToolStripMenuItem1.Text = "Enable";
			enableToolStripMenuItem1.Click += new EventHandler(EnableRptViewerToolStripMenuItem_Click);
			disableToolStripMenuItem1.Name = "disableToolStripMenuItem1";
			disableToolStripMenuItem1.Size = new Size(112, 22);
			disableToolStripMenuItem1.Text = "Disable";
			disableToolStripMenuItem1.Click += new EventHandler(DisableRptViewerToolStripMenuItem_Click);
			advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
			advancedToolStripMenuItem.Size = new Size(165, 22);
			advancedToolStripMenuItem.Text = "Advanced";
			advancedToolStripMenuItem.Click += new EventHandler(advancedToolStripMenuItem_Click);
			help.DropDownItems.AddRange(new ToolStripItem[3] { userManualToolStripMenuItem, toolStripSeparator5, aboutToolStripMenuItem });
			help.Name = "help";
			help.Size = new Size(44, 20);
			help.Text = "Help";
			userManualToolStripMenuItem.Image = Resources.help;
			userManualToolStripMenuItem.Name = "userManualToolStripMenuItem";
			userManualToolStripMenuItem.Size = new Size(149, 22);
			userManualToolStripMenuItem.Text = "User Manual";
			userManualToolStripMenuItem.Click += new EventHandler(userManualToolStripMenuItem_Click);
			toolStripSeparator5.Name = "toolStripSeparator5";
			toolStripSeparator5.Size = new Size(146, 6);
			aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			aboutToolStripMenuItem.Size = new Size(149, 22);
			aboutToolStripMenuItem.Text = "About WINTSI";
			aboutToolStripMenuItem.Click += new EventHandler(aboutToolStripMenuItem_Click);
			saveTrace.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			saveReceipt.FileName = "Terminal Response.txt";
			saveReceipt.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
			Ticket.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			Ticket.BackColor = SystemColors.ButtonFace;
			Ticket.BorderStyle = BorderStyle.FixedSingle;
			Ticket.Font = new Font("Courier New", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			Ticket.Location = new Point(7, 22);
			Ticket.Name = "Ticket";
			Ticket.ReadOnly = true;
			Ticket.Size = new Size(302, 674);
			Ticket.TabIndex = 36;
			Ticket.Text = "";
			clearReceiptButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			clearReceiptButton.Location = new Point(243, 704);
			clearReceiptButton.Name = "clearReceiptButton";
			clearReceiptButton.Size = new Size(66, 25);
			clearReceiptButton.TabIndex = 37;
			clearReceiptButton.Text = "Clear";
			clearReceiptButton.UseVisualStyleBackColor = true;
			clearReceiptButton.Click += new EventHandler(ClearReceiptButton_Click);
			receiptResponseGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			receiptResponseGroupBox.Controls.Add(clearReceiptButton);
			receiptResponseGroupBox.Controls.Add(Ticket);
			receiptResponseGroupBox.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			receiptResponseGroupBox.Location = new Point(677, 3);
			receiptResponseGroupBox.Name = "receiptResponseGroupBox";
			receiptResponseGroupBox.Size = new Size(317, 733);
			receiptResponseGroupBox.TabIndex = 28;
			receiptResponseGroupBox.TabStop = false;
			receiptResponseGroupBox.Text = "Terminal Response";
			excepTestgroupBox.Controls.Add(panel2);
			excepTestgroupBox.Controls.Add(panel1);
			excepTestgroupBox.Location = new Point(495, 62);
			excepTestgroupBox.Name = "excepTestgroupBox";
			excepTestgroupBox.Size = new Size(175, 292);
			excepTestgroupBox.TabIndex = 33;
			excepTestgroupBox.TabStop = false;
			excepTestgroupBox.Text = "Exceptional Test Cases";
			panel2.Controls.Add(printingLabel);
			panel2.Controls.Add(LatePrintRadioButton);
			panel2.Controls.Add(TimeOutPrintRadioButton);
			panel2.Controls.Add(NokPrintRadioButton);
			panel2.Controls.Add(NonePrintRadioButton);
			panel2.Location = new Point(12, 178);
			panel2.Name = "panel2";
			panel2.Size = new Size(156, 98);
			panel2.TabIndex = 24;
			printingLabel.AutoSize = true;
			printingLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			printingLabel.Location = new Point(3, 1);
			printingLabel.Name = "printingLabel";
			printingLabel.Size = new Size(88, 13);
			printingLabel.TabIndex = 27;
			printingLabel.Text = "Printing Issue:";
			LatePrintRadioButton.AutoSize = true;
			LatePrintRadioButton.Location = new Point(5, 77);
			LatePrintRadioButton.Name = "LatePrintRadioButton";
			LatePrintRadioButton.Size = new Size(136, 17);
			LatePrintRadioButton.TabIndex = 33;
			LatePrintRadioButton.Text = "Late Print Resp (30sec)";
			LatePrintRadioButton.UseVisualStyleBackColor = true;
			TimeOutPrintRadioButton.AutoSize = true;
			TimeOutPrintRadioButton.Location = new Point(5, 57);
			TimeOutPrintRadioButton.Name = "TimeOutPrintRadioButton";
			TimeOutPrintRadioButton.Size = new Size(128, 17);
			TimeOutPrintRadioButton.TabIndex = 32;
			TimeOutPrintRadioButton.Text = "No Printing Response";
			TimeOutPrintRadioButton.UseVisualStyleBackColor = true;
			NokPrintRadioButton.AutoSize = true;
			NokPrintRadioButton.Location = new Point(5, 37);
			NokPrintRadioButton.Name = "NokPrintRadioButton";
			NokPrintRadioButton.Size = new Size(134, 17);
			NokPrintRadioButton.TabIndex = 31;
			NokPrintRadioButton.Text = "Printing Failure on ECR";
			NokPrintRadioButton.UseVisualStyleBackColor = true;
			NonePrintRadioButton.AutoSize = true;
			NonePrintRadioButton.Checked = true;
			NonePrintRadioButton.Location = new Point(5, 17);
			NonePrintRadioButton.Name = "NonePrintRadioButton";
			NonePrintRadioButton.Size = new Size(51, 17);
			NonePrintRadioButton.TabIndex = 30;
			NonePrintRadioButton.TabStop = true;
			NonePrintRadioButton.Text = "None";
			NonePrintRadioButton.UseVisualStyleBackColor = true;
			panel1.Controls.Add(RequestGabageRadioButton);
			panel1.Controls.Add(protocolLabel);
			panel1.Controls.Add(LateAckRadioButton);
			panel1.Controls.Add(TimeOutAckRadioButton);
			panel1.Controls.Add(BadLrcRadioButton);
			panel1.Controls.Add(NakRadioButton);
			panel1.Controls.Add(NoneRadioButton);
			panel1.Location = new Point(12, 23);
			panel1.Name = "panel1";
			panel1.Size = new Size(156, 138);
			panel1.TabIndex = 23;
			RequestGabageRadioButton.AutoSize = true;
			RequestGabageRadioButton.Location = new Point(5, 97);
			RequestGabageRadioButton.Name = "RequestGabageRadioButton";
			RequestGabageRadioButton.Size = new Size(134, 17);
			RequestGabageRadioButton.TabIndex = 28;
			RequestGabageRadioButton.TabStop = true;
			RequestGabageRadioButton.Text = "Request with Garbage ";
			RequestGabageRadioButton.UseVisualStyleBackColor = true;
			protocolLabel.AutoSize = true;
			protocolLabel.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
			protocolLabel.Location = new Point(3, 4);
			protocolLabel.Name = "protocolLabel";
			protocolLabel.Size = new Size(128, 13);
			protocolLabel.TabIndex = 29;
			protocolLabel.Text = "Serial Protocol Issue:";
			LateAckRadioButton.AutoSize = true;
			LateAckRadioButton.Location = new Point(5, 77);
			LateAckRadioButton.Name = "LateAckRadioButton";
			LateAckRadioButton.Size = new Size(85, 17);
			LateAckRadioButton.TabIndex = 27;
			LateAckRadioButton.TabStop = true;
			LateAckRadioButton.Text = "<ACK> Late ";
			LateAckRadioButton.UseVisualStyleBackColor = true;
			TimeOutAckRadioButton.AutoSize = true;
			TimeOutAckRadioButton.Location = new Point(5, 57);
			TimeOutAckRadioButton.Name = "TimeOutAckRadioButton";
			TimeOutAckRadioButton.Size = new Size(83, 17);
			TimeOutAckRadioButton.TabIndex = 26;
			TimeOutAckRadioButton.TabStop = true;
			TimeOutAckRadioButton.Text = "<ACK> Stop";
			TimeOutAckRadioButton.UseVisualStyleBackColor = true;
			BadLrcRadioButton.AutoSize = true;
			BadLrcRadioButton.Location = new Point(5, 117);
			BadLrcRadioButton.Name = "BadLrcRadioButton";
			BadLrcRadioButton.Size = new Size(145, 17);
			BadLrcRadioButton.TabIndex = 29;
			BadLrcRadioButton.TabStop = true;
			BadLrcRadioButton.Text = "Request with Bad <LRC>";
			BadLrcRadioButton.UseVisualStyleBackColor = true;
			NakRadioButton.AutoSize = true;
			NakRadioButton.Location = new Point(5, 37);
			NakRadioButton.Name = "NakRadioButton";
			NakRadioButton.Size = new Size(154, 17);
			NakRadioButton.TabIndex = 25;
			NakRadioButton.TabStop = true;
			NakRadioButton.Text = "<NAK> All Term Messages ";
			NakRadioButton.UseVisualStyleBackColor = true;
			NoneRadioButton.AutoSize = true;
			NoneRadioButton.Checked = true;
			NoneRadioButton.Location = new Point(5, 17);
			NoneRadioButton.Name = "NoneRadioButton";
			NoneRadioButton.Size = new Size(51, 17);
			NoneRadioButton.TabIndex = 24;
			NoneRadioButton.TabStop = true;
			NoneRadioButton.Text = "None";
			NoneRadioButton.UseVisualStyleBackColor = true;
			valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
			valueColumn.FillWeight = 1.558134f;
			valueColumn.HeaderText = "Value";
			valueColumn.MinimumWidth = 90;
			valueColumn.Name = "valueColumn";
			valueColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
			panel3.Controls.Add(mainTabCtrl);
			panel3.Controls.Add(isReportGroupBox);
			panel3.Controls.Add(percentLabel);
			panel3.Controls.Add(gettingRptLabel);
			panel3.Controls.Add(progressBar1);
			panel3.Controls.Add(statusStrip1);
			panel3.Controls.Add(excepTestgroupBox);
			panel3.Controls.Add(receiptResponseGroupBox);
			panel3.Controls.Add(traceGroupBox);
			panel3.Dock = DockStyle.Fill;
			panel3.Location = new Point(0, 24);
			panel3.Name = "panel3";
			panel3.Size = new Size(1000, 776);
			panel3.TabIndex = 34;
			mainTabCtrl.Controls.Add(tnxTabPage);
			mainTabCtrl.Controls.Add(scriptTabPage);
			mainTabCtrl.Location = new Point(6, 3);
			mainTabCtrl.Name = "mainTabCtrl";
			mainTabCtrl.SelectedIndex = 0;
			mainTabCtrl.Size = new Size(483, 362);
			mainTabCtrl.TabIndex = 40;
			tnxTabPage.Controls.Add(transactionGroupBox);
			tnxTabPage.Location = new Point(4, 22);
			tnxTabPage.Name = "tnxTabPage";
			tnxTabPage.Padding = new Padding(3);
			tnxTabPage.Size = new Size(475, 336);
			tnxTabPage.TabIndex = 0;
			tnxTabPage.Text = "Transaction";
			tnxTabPage.UseVisualStyleBackColor = true;
			scriptTabPage.Controls.Add(scriptPathLabel);
			scriptTabPage.Controls.Add(loopLabel);
			scriptTabPage.Controls.Add(startButton);
			scriptTabPage.Controls.Add(stopButton);
			scriptTabPage.Controls.Add(cmdListView);
			scriptTabPage.Controls.Add(selectScriptFileNameLabel);
			scriptTabPage.Controls.Add(selectScriptFileButton);
			scriptTabPage.Controls.Add(continueButton);
			scriptTabPage.Location = new Point(4, 22);
			scriptTabPage.Name = "scriptTabPage";
			scriptTabPage.Padding = new Padding(3);
			scriptTabPage.Size = new Size(475, 336);
			scriptTabPage.TabIndex = 1;
			scriptTabPage.Text = "Sequence";
			scriptTabPage.UseVisualStyleBackColor = true;
			scriptPathLabel.AutoSize = true;
			scriptPathLabel.BackColor = Color.Gainsboro;
			scriptPathLabel.Location = new Point(118, 23);
			scriptPathLabel.Name = "scriptPathLabel";
			scriptPathLabel.Padding = new Padding(5);
			scriptPathLabel.Size = new Size(10, 23);
			scriptPathLabel.TabIndex = 31;
			scriptPathLabel.Visible = false;
			loopLabel.AutoSize = true;
			loopLabel.Location = new Point(24, 119);
			loopLabel.Name = "loopLabel";
			loopLabel.Size = new Size(0, 13);
			loopLabel.TabIndex = 27;
			startButton.Location = new Point(17, 79);
			startButton.Name = "startButton";
			startButton.Size = new Size(75, 23);
			startButton.TabIndex = 26;
			startButton.Text = "Start";
			startButton.UseVisualStyleBackColor = true;
			startButton.Click += new EventHandler(startButton_Click);
			stopButton.Location = new Point(116, 78);
			stopButton.Name = "stopButton";
			stopButton.Size = new Size(75, 25);
			stopButton.TabIndex = 25;
			stopButton.Text = "Stop";
			stopButton.UseVisualStyleBackColor = true;
			stopButton.Click += new EventHandler(StopButton_Click);
			cmdListView.BackColor = SystemColors.Menu;
			cmdListView.BorderStyle = BorderStyle.None;
			cmdListView.CheckBoxes = true;
			cmdListView.Font = new Font("Microsoft Sans Serif", 8.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			cmdListView.FullRowSelect = true;
			cmdListView.HideSelection = false;
			cmdListView.Location = new Point(21, 140);
			cmdListView.Name = "cmdListView";
			cmdListView.Size = new Size(169, 170);
			cmdListView.TabIndex = 24;
			cmdListView.TileSize = new Size(168, 25);
			cmdListView.UseCompatibleStateImageBehavior = false;
			cmdListView.View = View.List;
			cmdListView.ItemCheck += new ItemCheckEventHandler(CmdListView_ItemCheck);
			selectScriptFileNameLabel.AutoSize = true;
			selectScriptFileNameLabel.BackColor = Color.Beige;
			selectScriptFileNameLabel.Location = new Point(106, 46);
			selectScriptFileNameLabel.Name = "selectScriptFileNameLabel";
			selectScriptFileNameLabel.Size = new Size(0, 13);
			selectScriptFileNameLabel.TabIndex = 23;
			selectScriptFileButton.Location = new Point(17, 23);
			selectScriptFileButton.Name = "selectScriptFileButton";
			selectScriptFileButton.Size = new Size(75, 23);
			selectScriptFileButton.TabIndex = 22;
			selectScriptFileButton.Text = "Select File";
			selectScriptFileButton.UseVisualStyleBackColor = true;
			selectScriptFileButton.Click += new EventHandler(SelectScriptFileButton_Click);
			continueButton.Enabled = false;
			continueButton.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			continueButton.Location = new Point(211, 78);
			continueButton.Name = "continueButton";
			continueButton.Size = new Size(75, 25);
			continueButton.TabIndex = 20;
			continueButton.Text = "Continue";
			continueButton.UseVisualStyleBackColor = true;
			continueButton.Click += new EventHandler(continueButton_Click);
			isReportGroupBox.Controls.Add(rbNo);
			isReportGroupBox.Controls.Add(rbYes);
			isReportGroupBox.Location = new Point(495, 5);
			isReportGroupBox.Name = "isReportGroupBox";
			isReportGroupBox.Size = new Size(175, 41);
			isReportGroupBox.TabIndex = 39;
			isReportGroupBox.TabStop = false;
			isReportGroupBox.Text = "Is Report Request";
			percentLabel.AutoSize = true;
			percentLabel.Location = new Point(659, 756);
			percentLabel.Name = "percentLabel";
			percentLabel.Size = new Size(21, 13);
			percentLabel.TabIndex = 38;
			percentLabel.Text = "0%";
			percentLabel.Visible = false;
			gettingRptLabel.AutoSize = true;
			gettingRptLabel.Location = new Point(576, 756);
			gettingRptLabel.Name = "gettingRptLabel";
			gettingRptLabel.Size = new Size(79, 13);
			gettingRptLabel.TabIndex = 37;
			gettingRptLabel.Text = "Getting Report:";
			gettingRptLabel.Visible = false;
			progressBar1.Location = new Point(696, 756);
			progressBar1.Name = "progressBar1";
			progressBar1.Size = new Size(224, 13);
			progressBar1.TabIndex = 36;
			statusStrip1.Items.AddRange(new ToolStripItem[3] { toolStripStatusLabel1, toolStripStatusLabel2, comSettingStatusLabel });
			statusStrip1.Location = new Point(0, 749);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new Size(1000, 27);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 34;
			toolStripStatusLabel1.Image = Resources.ingenico;
			toolStripStatusLabel1.ImageScaling = ToolStripItemImageScaling.None;
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new Size(64, 22);
			toolStripStatusLabel1.TextImageRelation = TextImageRelation.Overlay;
			toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			toolStripStatusLabel2.Size = new Size(0, 22);
			comSettingStatusLabel.Name = "comSettingStatusLabel";
			comSettingStatusLabel.Size = new Size(0, 22);
			comSettingStatusLabel.TextImageRelation = TextImageRelation.ImageAboveText;
			groupBox1.Controls.Add(ecdCapLabel);
			groupBox1.Controls.Add(ecdStatusLabel);
			groupBox1.Controls.Add(getStatusBt);
			groupBox1.Controls.Add(getCapBt);
			groupBox1.Controls.Add(openCdBt);
			groupBox1.Location = new Point(2, 2);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new Size(467, 300);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			ecdCapLabel.BackColor = SystemColors.Control;
			ecdCapLabel.BorderStyle = BorderStyle.Fixed3D;
			ecdCapLabel.ForeColor = Color.Gray;
			ecdCapLabel.Location = new Point(141, 129);
			ecdCapLabel.Name = "ecdCapLabel";
			ecdCapLabel.Size = new Size(138, 20);
			ecdCapLabel.TabIndex = 11;
			ecdCapLabel.Text = "ECD Capability";
			ecdStatusLabel.BackColor = SystemColors.Control;
			ecdStatusLabel.BorderStyle = BorderStyle.Fixed3D;
			ecdStatusLabel.ForeColor = Color.Gray;
			ecdStatusLabel.Location = new Point(141, 97);
			ecdStatusLabel.Name = "ecdStatusLabel";
			ecdStatusLabel.Size = new Size(138, 20);
			ecdStatusLabel.TabIndex = 10;
			ecdStatusLabel.Text = "ECD Status";
			getStatusBt.Location = new Point(25, 63);
			getStatusBt.Name = "getStatusBt";
			getStatusBt.Size = new Size(88, 25);
			getStatusBt.TabIndex = 0;
			getStatusBt.Text = "Get Status";
			getStatusBt.UseVisualStyleBackColor = true;
			getStatusBt.Click += new EventHandler(GetStatusBt_Click);
			getCapBt.Location = new Point(25, 127);
			getCapBt.Name = "getCapBt";
			getCapBt.Size = new Size(88, 25);
			getCapBt.TabIndex = 2;
			getCapBt.Text = "Get Capability";
			getCapBt.UseVisualStyleBackColor = true;
			getCapBt.Click += new EventHandler(GetCapBt_Click);
			openCdBt.Location = new Point(25, 95);
			openCdBt.Name = "openCdBt";
			openCdBt.Size = new Size(88, 25);
			openCdBt.TabIndex = 1;
			openCdBt.Text = "Open CD";
			openCdBt.UseVisualStyleBackColor = true;
			openCdBt.Click += new EventHandler(OpenCdBt_Click);
			openConfig.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
			saveConfig.FileName = "Cfg.xml";
			saveConfig.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
			cdTabPage.Controls.Add(groupBox1);
			cdTabPage.Location = new Point(4, 22);
			cdTabPage.Name = "cdTabPage";
			cdTabPage.Padding = new Padding(3);
			cdTabPage.Size = new Size(475, 309);
			cdTabPage.TabIndex = 1;
			cdTabPage.Text = "Cash Drawer";
			cdTabPage.UseVisualStyleBackColor = true;
			searchAmount.Location = new Point(78, 295);
			searchAmount.MaxLength = 30;
			searchAmount.Name = "searchAmount";
			searchAmount.Size = new Size(138, 20);
			searchAmount.TabIndex = 78;
			searchAmount.Tag = "0";
			searchAmount.Visible = false;
			searchAmount.MouseClick += new MouseEventHandler(Amount_MouseClick);
			searchAmount.KeyDown += new KeyEventHandler(Amount_KeyDown);
			searchAmount.KeyPress += new KeyPressEventHandler(Amount_KeyPress);
			searchAmount.MouseDoubleClick += new MouseEventHandler(Amount_MouseDoubleClick);
			searchAmount.MouseDown += new MouseEventHandler(Amount_MouseDown);
			searchAmount.MouseEnter += new EventHandler(Amount_MouseEnter);
			searchAmountLabel.Location = new Point(6, 300);
			searchAmountLabel.Name = "searchAmountLabel";
			searchAmountLabel.Size = new Size(68, 13);
			searchAmountLabel.TabIndex = 79;
			searchAmountLabel.Text = "Amount";
			searchAmountLabel.Visible = false;
			AutoScaleDimensions = new SizeF(6f, 13f);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.Control;
			ClientSize = new Size(1000, 800);
			Controls.Add(panel3);
			Controls.Add(menuStripHome);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			//base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MaximizeBox = false;
			Name = "HomeForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Mr Payment";
			FormClosing += new FormClosingEventHandler(HomeForm_FormClosing);
			Load += new EventHandler(Home_Load);
			transactionGroupBox.ResumeLayout(false);
			transactionGroupBox.PerformLayout();
			traceGroupBox.ResumeLayout(false);
			menuStripHome.ResumeLayout(false);
			menuStripHome.PerformLayout();
			receiptResponseGroupBox.ResumeLayout(false);
			excepTestgroupBox.ResumeLayout(false);
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			mainTabCtrl.ResumeLayout(false);
			tnxTabPage.ResumeLayout(false);
			scriptTabPage.ResumeLayout(false);
			scriptTabPage.PerformLayout();
			isReportGroupBox.ResumeLayout(false);
			isReportGroupBox.PerformLayout();
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			groupBox1.ResumeLayout(false);
			cdTabPage.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
			Size = new Size(0, 0);
			ControlBox = false;
			ShowInTaskbar = false;
		}
	}
}
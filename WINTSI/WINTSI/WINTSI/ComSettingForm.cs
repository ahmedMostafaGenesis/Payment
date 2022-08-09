#define TRACE
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using IPAddressControlLib;

namespace Ingenico{

public class ComSettingForm : Form
{
	private Communication Communication;

	private IContainer components;

	private GroupBox CommsSetupGroup;

	public TextBox IpPort;

	public IPAddressControl IpAddress;

	private Label IpPortLabel;

	private Label IpAddressLabel;

	public ComboBox ComPort;

	private Label ComPortLabel;

	private Label BaudRatelabel;

	private Button OkButton;

	private Button Cancelbutton;

	private RadioButton EthernetRadioButton;

	private RadioButton SerialRadioButton;

	private GroupBox groupBox1;

	public ComboBox BaudRate;

	public ComSettingForm()
	{
		InitializeComponent();
	}

	public ComSettingForm(Communication Communication)
	{
		InitializeComponent();
		this.Communication = Communication;
		Fill_OldComSetting();
		ComPort_SetItems();
	}

	public void Fill_OldComSetting()
	{
		if (Communication.LinkType == "ETHERNET")
		{
			EthernetRadioButton.Checked = true;
			SerialRadioButton.Checked = false;
		}
		else
		{
			EthernetRadioButton.Checked = false;
			SerialRadioButton.Checked = true;
		}
		ComPort.Text = Communication.ComGetPortName();
		BaudRate.Text = Communication.BaudRate.ToString();
		IpPort.Text = Communication.IpPort.ToString();
		IpAddress.Text = Communication.IpAddress;
	}

	private void SerialRadioButton_CheckedChanged(object sender, EventArgs e)
	{
		if (SerialRadioButton.Checked)
		{
			ComPort_SetItems();
			ComPort.Enabled = true;
			ComPortLabel.Enabled = true;
			BaudRate.Enabled = true;
			BaudRatelabel.Enabled = true;
			BaudRate.Text = Communication.BaudRate.ToString();
			IpAddress.Enabled = false;
			IpAddressLabel.Enabled = false;
			IpPort.Enabled = false;
			IpPortLabel.Enabled = false;
		}
	}

	private void EthernetRadioButton_CheckedChanged(object sender, EventArgs e)
	{
		if (EthernetRadioButton.Checked)
		{
			ComPort.Enabled = false;
			ComPortLabel.Enabled = false;
			BaudRate.Enabled = false;
			BaudRatelabel.Enabled = false;
			IpAddress.Enabled = true;
			IpAddressLabel.Enabled = true;
			IpPort.Enabled = true;
			IpPortLabel.Enabled = true;
		}
	}

	private void OkButton_Click(object sender, EventArgs e)
	{
		bool flag = false;
		if (EthernetRadioButton.Checked)
		{
			Communication.LinkType = "ETHERNET";
			if (IpAddress.AnyBlank)
			{
				MessageBox.Show("Invalid IP Adress!", "WINTSI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				flag = true;
			}
			else
			{
				Communication.IpAddress = IpAddress.Text;
				if (IpPort.Text == "")
				{
					MessageBox.Show("Invalid IP Port!", "WINTSI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					flag = true;
				}
				else
				{
					Communication.IpPort = Convert.ToInt32(IpPort.Text);
				}
			}
		}
		else
		{
			Communication.LinkType = "SERIAL";
			Communication.PortName = "";
			try
			{
				Communication.PortName = ComPort.SelectedItem.ToString();
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Invalid Port Com value: " + ex.Message);
			}
			try
			{
				Communication.BaudRate = int.Parse(BaudRate.Text);
			}
			catch (Exception ex2)
			{
				Trace.WriteLine("Invalid BaudRate value: " + ex2.Message);
			}
		}
		if (!flag)
		{
			Communication.SaveConfigInFile();
			Dispose();
		}
	}

	public void ComPort_SetItems()
	{
		string text = "";
		try
		{
			text = Communication.PortName;
			ComPort.Items.Clear();
			if (Communication.ListPort.Count > 0)
			{
				Communication.ListPort.Sort();
				ComboBox.ObjectCollection items = ComPort.Items;
				object[] items2 = Communication.ListPort.ToArray();
				items.AddRange(items2);
			}
			if (Communication.ListPort.Contains(text))
			{
				ComPort.Text = text;
				return;
			}
			ComPort.Text = Communication.ListPort[0];
			Communication.PortName = ComPort.Text;
		}
		catch
		{
			Trace.WriteLine("No serial port");
			ComPort.Text = "";
		}
	}

	private void TextBoxNum_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar != '\b')
		{
			e.Handled = !char.IsDigit(e.KeyChar);
		}
	}

	private void Cancelbutton_Click(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComSettingForm));
		this.CommsSetupGroup = new System.Windows.Forms.GroupBox();
		this.BaudRate = new System.Windows.Forms.ComboBox();
		this.BaudRatelabel = new System.Windows.Forms.Label();
		this.IpPort = new System.Windows.Forms.TextBox();
		this.IpAddress = new IPAddressControlLib.IPAddressControl();
		this.IpPortLabel = new System.Windows.Forms.Label();
		this.IpAddressLabel = new System.Windows.Forms.Label();
		this.ComPort = new System.Windows.Forms.ComboBox();
		this.ComPortLabel = new System.Windows.Forms.Label();
		this.EthernetRadioButton = new System.Windows.Forms.RadioButton();
		this.SerialRadioButton = new System.Windows.Forms.RadioButton();
		this.OkButton = new System.Windows.Forms.Button();
		this.Cancelbutton = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.CommsSetupGroup.SuspendLayout();
		this.groupBox1.SuspendLayout();
		base.SuspendLayout();
		this.CommsSetupGroup.Controls.Add(this.BaudRate);
		this.CommsSetupGroup.Controls.Add(this.BaudRatelabel);
		this.CommsSetupGroup.Controls.Add(this.IpPort);
		this.CommsSetupGroup.Controls.Add(this.IpAddress);
		this.CommsSetupGroup.Controls.Add(this.IpPortLabel);
		this.CommsSetupGroup.Controls.Add(this.IpAddressLabel);
		this.CommsSetupGroup.Controls.Add(this.ComPort);
		this.CommsSetupGroup.Controls.Add(this.ComPortLabel);
		this.CommsSetupGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.CommsSetupGroup.Location = new System.Drawing.Point(17, 104);
		this.CommsSetupGroup.Name = "CommsSetupGroup";
		this.CommsSetupGroup.Size = new System.Drawing.Size(263, 214);
		this.CommsSetupGroup.TabIndex = 1;
		this.CommsSetupGroup.TabStop = false;
		this.CommsSetupGroup.Text = "Communication Setup";
		this.BaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.BaudRate.FormattingEnabled = true;
		this.BaudRate.Items.AddRange(new object[5] { "4800", "9600", "19200", "38400", "115200" });
		this.BaudRate.Location = new System.Drawing.Point(70, 72);
		this.BaudRate.Name = "BaudRate";
		this.BaudRate.Size = new System.Drawing.Size(144, 21);
		this.BaudRate.TabIndex = 4;
		this.BaudRatelabel.AutoSize = true;
		this.BaudRatelabel.Location = new System.Drawing.Point(14, 76);
		this.BaudRatelabel.Name = "BaudRatelabel";
		this.BaudRatelabel.Size = new System.Drawing.Size(53, 13);
		this.BaudRatelabel.TabIndex = 17;
		this.BaudRatelabel.Text = "Baud rate";
		this.IpPort.Enabled = false;
		this.IpPort.Location = new System.Drawing.Point(70, 159);
		this.IpPort.MaxLength = 8;
		this.IpPort.Name = "IpPort";
		this.IpPort.Size = new System.Drawing.Size(144, 20);
		this.IpPort.TabIndex = 9;
		this.IpPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(TextBoxNum_KeyPress);
		this.IpAddress.AllowInternalTab = true;
		this.IpAddress.AutoHeight = true;
		this.IpAddress.BackColor = System.Drawing.SystemColors.Window;
		this.IpAddress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
		this.IpAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
		this.IpAddress.Enabled = false;
		this.IpAddress.Location = new System.Drawing.Point(70, 116);
		this.IpAddress.MinimumSize = new System.Drawing.Size(87, 20);
		this.IpAddress.Name = "IpAddress";
		this.IpAddress.ReadOnly = false;
		this.IpAddress.Size = new System.Drawing.Size(144, 20);
		this.IpAddress.TabIndex = 5;
		this.IpAddress.Text = "...";
		this.IpPortLabel.AutoSize = true;
		this.IpPortLabel.Enabled = false;
		this.IpPortLabel.Location = new System.Drawing.Point(28, 163);
		this.IpPortLabel.Name = "IpPortLabel";
		this.IpPortLabel.Size = new System.Drawing.Size(39, 13);
		this.IpPortLabel.TabIndex = 15;
		this.IpPortLabel.Text = "IP Port";
		this.IpAddressLabel.AutoSize = true;
		this.IpAddressLabel.Enabled = false;
		this.IpAddressLabel.Location = new System.Drawing.Point(9, 120);
		this.IpAddressLabel.Name = "IpAddressLabel";
		this.IpAddressLabel.Size = new System.Drawing.Size(58, 13);
		this.IpAddressLabel.TabIndex = 14;
		this.IpAddressLabel.Text = "IP Address";
		this.ComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.ComPort.FormattingEnabled = true;
		this.ComPort.Items.AddRange(new object[15]
		{
			"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10",
			"COM11", "COM12", "COM13", "COM14", "COM15"
		});
		this.ComPort.Location = new System.Drawing.Point(70, 28);
		this.ComPort.Name = "ComPort";
		this.ComPort.Size = new System.Drawing.Size(167, 21);
		this.ComPort.TabIndex = 3;
		this.ComPortLabel.AutoSize = true;
		this.ComPortLabel.Location = new System.Drawing.Point(14, 32);
		this.ComPortLabel.Name = "ComPortLabel";
		this.ComPortLabel.Size = new System.Drawing.Size(53, 13);
		this.ComPortLabel.TabIndex = 9;
		this.ComPortLabel.Text = "COM Port";
		this.EthernetRadioButton.AutoSize = true;
		this.EthernetRadioButton.Location = new System.Drawing.Point(26, 59);
		this.EthernetRadioButton.Name = "EthernetRadioButton";
		this.EthernetRadioButton.Size = new System.Drawing.Size(65, 17);
		this.EthernetRadioButton.TabIndex = 2;
		this.EthernetRadioButton.Text = "Ethernet";
		this.EthernetRadioButton.UseVisualStyleBackColor = true;
		this.EthernetRadioButton.CheckedChanged += new System.EventHandler(EthernetRadioButton_CheckedChanged);
		this.SerialRadioButton.AutoSize = true;
		this.SerialRadioButton.Checked = true;
		this.SerialRadioButton.Location = new System.Drawing.Point(26, 28);
		this.SerialRadioButton.Name = "SerialRadioButton";
		this.SerialRadioButton.Size = new System.Drawing.Size(120, 17);
		this.SerialRadioButton.TabIndex = 1;
		this.SerialRadioButton.TabStop = true;
		this.SerialRadioButton.Text = "Serial (RS232/USB)";
		this.SerialRadioButton.UseVisualStyleBackColor = true;
		this.SerialRadioButton.CheckedChanged += new System.EventHandler(SerialRadioButton_CheckedChanged);
		this.OkButton.Location = new System.Drawing.Point(123, 324);
		this.OkButton.Name = "OkButton";
		this.OkButton.Size = new System.Drawing.Size(67, 23);
		this.OkButton.TabIndex = 10;
		this.OkButton.Text = "OK";
		this.OkButton.UseVisualStyleBackColor = true;
		this.OkButton.Click += new System.EventHandler(OkButton_Click);
		this.Cancelbutton.Location = new System.Drawing.Point(196, 324);
		this.Cancelbutton.Name = "Cancelbutton";
		this.Cancelbutton.Size = new System.Drawing.Size(67, 23);
		this.Cancelbutton.TabIndex = 11;
		this.Cancelbutton.Text = "Cancel";
		this.Cancelbutton.UseVisualStyleBackColor = true;
		this.Cancelbutton.Click += new System.EventHandler(Cancelbutton_Click);
		this.groupBox1.Controls.Add(this.EthernetRadioButton);
		this.groupBox1.Controls.Add(this.SerialRadioButton);
		this.groupBox1.Location = new System.Drawing.Point(17, 10);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(263, 87);
		this.groupBox1.TabIndex = 4;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Communication Type";
		base.AcceptButton = this.OkButton;
		base.AutoScaleDimensions = new System.Drawing.SizeF(96f, 96f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
		base.ClientSize = new System.Drawing.Size(292, 359);
		base.Controls.Add(this.groupBox1);
		base.Controls.Add(this.Cancelbutton);
		base.Controls.Add(this.OkButton);
		base.Controls.Add(this.CommsSetupGroup);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		//base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ComSettingForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Communication Settings";
		this.CommsSetupGroup.ResumeLayout(false);
		this.CommsSetupGroup.PerformLayout();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		base.ResumeLayout(false);
	}
}
}
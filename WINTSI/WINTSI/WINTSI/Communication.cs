#define TRACE
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using WINTSI.WebSocket;

namespace Ingenico
{
public class Communication
{
	public string PortName;

	public string LinkType;

	public string IpAddress;

	public int IpPort;

	public int BaudRate = 19200;

	public readonly Parity Parity = Parity.Even;

	public const int DataBit = 7;

	public const StopBits StopBit = StopBits.One;

	private static bool _bSerial = true;

	public List<string> ListPort;

	private const int SerialPortTimeOut = 100;

	private static SerialPort _port;

	private static TcpClient _tcpClient;

	private static Stream _streamTcp;

	private static int _receivedDataSize;

	private Converter converter = new();

	private bool bStartReceiving;

	private const int MaxDataSize = 24576;

	private byte[] receivedData;

	public Communication()
	{
		if (File.Exists("Mr Payment.exe.config"))
		{
			ReadConfigFile();
			return;
		}
		ComDefaultSerialSetting();
		SaveConfigInFile();
	}

	private void ComDefaultSerialSetting()
	{
		LinkType = "Serial";
		PortName = "COM3";
		BaudRate = 19200;
		IpAddress = "127.0.0.1";
		IpPort = 8091;
		
	}

	public void SaveConfigInFile()
	{
		/*try
		{
			var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			configuration.AppSettings.Settings.Remove("link_Type");
			configuration.AppSettings.Settings.Remove("port_Name");
			configuration.AppSettings.Settings.Remove("Baud_Rate");
			configuration.AppSettings.Settings.Remove("ip_Address");
			configuration.AppSettings.Settings.Remove("ip_port");
			configuration.AppSettings.Settings.Add("link_Type", LinkType);
			configuration.AppSettings.Settings.Add("port_Name", PortName);
			string value;
			try
			{
				value = BaudRate.ToString();
			}
			catch
			{
				value = "19200";
				Trace.WriteLine("Error : Convert baudRate");
			}
			configuration.AppSettings.Settings.Add("Baud_Rate", value);
			configuration.AppSettings.Settings.Add("ip_Address", IpAddress);
			string value2;
			try
			{
				value2 = IpPort.ToString();
			}
			catch
			{
				value2 = "9999";
				Trace.WriteLine("Error : Convert IP Port");
			}
			configuration.AppSettings.Settings.Add("ip_port", value2);
			configuration.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("appSettings");
		}
		catch
		{
			Trace.WriteLine("Error : Save Config Params");
		}*/
	}

	private void ReadConfigFile()
	{
		/*try
		{
			LinkType = ConfigurationManager.AppSettings["link_Type"];
			PortName = ConfigurationManager.AppSettings["port_Name"];
			var s = ConfigurationManager.AppSettings["Baud_Rate"];
			try
			{
				BaudRate = int.Parse(s);
			}
			catch
			{
				BaudRate = 19200;
				Trace.WriteLine("Error : Convert baudRate");
			}
			IpAddress = ConfigurationManager.AppSettings["ip_Address"];
			var s2 = ConfigurationManager.AppSettings["ip_port"];
			try
			{
				IpPort = int.Parse(s2);
			}
			catch
			{
				IpPort = 9999;
				Trace.WriteLine("Error : Read IP port From Config file");
			}
		}
		catch
		{
			Trace.WriteLine("Error : Read Config Params");
		}*/
		LinkType = "Serial";
		PortName = "COM3";
		BaudRate = 19200;
		IpAddress = "127.0.0.1";
		IpPort = 8091;
	}

	public string ComGetPortName()
	{
		return PortName.IndexOf("SAGEM", StringComparison.Ordinal) > -1 ? PortName.Substring(0, PortName.IndexOf(" – ", StringComparison.Ordinal)) : PortName;
	}

	public List<string> GetOpenedPortDescription()
	{
		var list = new List<string>();
		var openedPortNames = GetOpenedPortNames();
		try
		{
			list.AddRange(from openPort in openedPortNames
				let cOmPortInfo = new COMPortInfo()
				select cOmPortInfo.isEmulSagemCOMPorts(openPort)
					? cOmPortInfo.Description
					: cOmPortInfo.Name);

			return list;
		}
		catch
		{
			Trace.WriteLine("No serial port");
			return list;
		}
	}

	private IEnumerable<string> GetOpenedPortNames()
	{
		var list = new List<string>();
		var portNames = SerialPort.GetPortNames();
		var array = new string[portNames.Length];
		var num = 0;
		foreach (var portName in portNames)
		{
			try
			{
				var serialPort = new SerialPort(portName, BaudRate, Parity, DataBit, StopBit);
				serialPort.Open();
				array[num] = portName;
				num++;
				serialPort.Close();
			}
			catch (Exception)
			{
				// ignored
			}
		}
		for (var j = 0; j < num; j++)
		{
			list.Add(array[j]);
		}
		return list;
	}

	public bool ComConfiguration()
	{
		var result = false;
		if (LinkType != "ETHERNET")
		{
			_bSerial = true;
			try
			{
				_port = new SerialPort(ComGetPortName(), BaudRate, Parity, DataBit, StopBit);
				if (_port == null) return false;
				result = true;
				return true;
			}
			catch
			{
				Trace.WriteLine("Error config serial port");
				return result;
			}
		}
		_bSerial = false;
		_tcpClient = new TcpClient();
		if (_tcpClient != null)
		{
			result = true;
		}
		return result;
	}

	public bool StartReceiving()
	{
		return bStartReceiving;
	}

	private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		if (!GetConnectStatus() || _port is not {IsOpen: true} || _port.BytesToRead == 0)
		{
			return;
		}
		try
		{
			var array = new byte[MaxDataSize];
			var num = 0;
			while (true)
			{
				if (_port.BytesToRead != 0 && num < MaxDataSize)
				{
					array[num] = (byte)ReceiveByte();
					bStartReceiving = true;
					num++;
					continue;
				}
				Thread.Sleep(90);
				if (_port.BytesToRead == 0 || num >= MaxDataSize)
				{
					break;
				}
			}
			_receivedDataSize = num;
			receivedData = array;
			_ = _receivedDataSize;
		}
		catch
		{
			Trace.WriteLine("Error receive data from serial port");
		}
	}

	public void Ethernet_DataReceived()
	{
		var array = new byte[MaxDataSize];
		if (_tcpClient is not {Connected: true})
		{
			return;
		}
		try
		{
			var num = _streamTcp.Read(array, 0, MaxDataSize);
			if (num == 0) return;
			_receivedDataSize = num;
			receivedData = array;
		}
		catch
		{
			Trace.WriteLine("Error receive data from Ethernet TCP");
		}
	}

	public bool Connect()
	{
		bool flag;
		if (_bSerial)
		{
			if (_port is not {IsOpen: false}) return false;
			try
			{
				_port.ReadTimeout = SerialPortTimeOut;
				_port.Open();
			}
			catch
			{
				Client.SendResponse("Result:Technical Error/", PaymentStatus.FAIL);
				Trace.WriteLine("Error opening serial port");
			}
			Thread.Sleep(100);
			_port.DiscardInBuffer();
			_port.DiscardOutBuffer();
			_port.DataReceived += serialPort1_DataReceived;
			flag = _port.IsOpen;
		}
		else
		{
			try
			{
				_tcpClient.Connect(IpAddress, IpPort);
			}
			catch
			{
				Trace.WriteLine("Error Connecting TCP");
			}
			Thread.Sleep(5);
			flag = _tcpClient.Connected;
			if (flag)
			{
				_streamTcp = _tcpClient.GetStream();
			}
		}
		return flag;
	}

	public bool Disconnect()
	{
		var result = false;
		if (_bSerial)
		{
			if (_port is {IsOpen: true})
			{
				try
				{
					_port.DataReceived -= serialPort1_DataReceived;
					_port.Close();
					_port = null;
				}
				catch
				{
					Trace.WriteLine("Error close serial port");
				}
			}
			if (_port is {IsOpen: false})
			{
				result = true;
			}
		}
		else
		{
			if (_tcpClient is {Connected: true})
			{
				try
				{
					_tcpClient.Close();
				}
				catch
				{
					Trace.WriteLine("Error close Ethernet Tcp");
				}
			}
			if (_tcpClient is {Connected: false})
			{
				result = true;
			}
		}
		return result;
	}

	public static bool Send(byte[] buffer, int offset, int count)
	{
		var result = false;
		if (!GetConnectStatus()) return false;
		if (_bSerial)
		{
			try
			{
				_port.WriteTimeout = 100;
				var text = "";
				buffer.ToList().ForEach(c=> text+=c);
				// Console.WriteLine(count);
				// Console.WriteLine(text);
				_port.Write(buffer, offset, count);
				result = true;
				return true;
			}
			catch
			{
				Trace.WriteLine("Error Send from serial port");
				return result;
			}
		}
		try
		{
			_streamTcp.WriteTimeout = 1000;
			_streamTcp.Write(buffer, offset, count);
			result = true;
			return true;
		}
		catch
		{
			Trace.WriteLine("Error Send from Ethernet Tcp");
			return result;
		}
	}

	private static int ReceiveByte()
	{
		var result = 0;
		if (_bSerial)
		{
			try
			{
				result = _port.ReadByte();
				return result;
			}
			catch
			{
				Trace.WriteLine("Error receive Byte from serial port");
				return result;
			}
		}
		try
		{
			result = _streamTcp.ReadByte();
			return result;
		}
		catch
		{
			Trace.WriteLine("Error receive Byte from Ethernet TCP");
			return result;
		}
	}

	public byte[] Receive(out int recvdSize)
	{
		recvdSize = _receivedDataSize;
		if (_receivedDataSize == 0) return receivedData;
		bStartReceiving = false;
		_receivedDataSize = 0;
		return receivedData;
	}

	public static bool GetConnectStatus()
	{
		if (_bSerial)
		{
			return _port is {IsOpen: true};
		}
		return _tcpClient is {Connected: true};
	}

	public int BytesInReceiveBuffer()
	{
		return _receivedDataSize;
	}
}
}
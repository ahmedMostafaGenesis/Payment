using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace ingenico
{
    public class Communication
    {
      public string port_Name;
      public string link_Type;
      public string ip_Address;
      public int ip_port;
      public int Baud_Rate = 19200;
      public Parity parity = Parity.Even;
      public int Data_Bit = 7;
      public StopBits Stop_Bit = StopBits.One;
      private static bool bSerial = true;
      public List<string> ListPort;
      private static int serialPortTimeOut = 100;
      private static SerialPort port = (SerialPort)null;
      private static TcpClient tcpClient = (TcpClient) null;
      private static Stream streamTcp = (Stream) null;
      private static int ReceivedDataSize = 0;
      private Converter converter = new Converter();
      private bool bStartReceiving;
      private int maxDataSize = 24576;
      private byte[] ReceivedData;

      public Communication()
      {
        if (File.Exists("ingenico.exe.config"))
        {
          this.ReadConfigFile();
        }
        else
        {
          this.COMDefaultSerialSetting();
          this.saveConfigInFile();
        }
      }

      public void COMDefaultSerialSetting()
      {
        this.link_Type = "Serial";
        this.port_Name = "COM3";
        this.Baud_Rate = 115200;
        this.ip_Address = "127.0.0.1";
        this.ip_port = 9999;
      }

      public void saveConfigInFile()
      {
        try
        {
          System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
          configuration.AppSettings.Settings.Remove("link_Type");
          configuration.AppSettings.Settings.Remove("port_Name");
          configuration.AppSettings.Settings.Remove("Baud_Rate");
          configuration.AppSettings.Settings.Remove("ip_Address");
          configuration.AppSettings.Settings.Remove("ip_port");
          configuration.AppSettings.Settings.Add("link_Type", this.link_Type);
          configuration.AppSettings.Settings.Add("port_Name", this.port_Name);
          string str1;
          try
          {
            str1 = this.Baud_Rate.ToString();
          }
          catch
          {
            str1 = "19200";
            Console.WriteLine("Error : Convert baudrate");
          }
          configuration.AppSettings.Settings.Add("Baud_Rate", str1);
          configuration.AppSettings.Settings.Add("ip_Address", this.ip_Address);
          string str2;
          try
          {
            str2 = this.ip_port.ToString();
          }
          catch
          {
            str2 = "9999";
            Console.WriteLine("Error : Convert IP Port");
          }
          configuration.AppSettings.Settings.Add("ip_port", str2);
          configuration.Save(ConfigurationSaveMode.Modified);
          ConfigurationManager.RefreshSection("appSettings");
        }
        catch
        {
          Console.WriteLine("Error : Save Config Params");
        }
      }

      public void ReadConfigFile()
      {
        try
        {
          this.link_Type = ConfigurationManager.AppSettings["link_Type"];
          this.port_Name = ConfigurationManager.AppSettings["port_Name"];
          string appSetting1 = ConfigurationManager.AppSettings["Baud_Rate"];
          try
          {
            this.Baud_Rate = int.Parse(appSetting1);
          }
          catch
          {
            this.Baud_Rate = 19200;
            Console.WriteLine("Error : Convert baudrate");
          }
          this.ip_Address = ConfigurationManager.AppSettings["ip_Address"];
          string appSetting2 = ConfigurationManager.AppSettings["ip_port"];
          try
          {
            this.ip_port = int.Parse(appSetting2);
          }
          catch
          {
            this.ip_port = 9999;
            Console.WriteLine("Error : Read IP port From Config file");
          }
        }
        catch
        {
          Console.WriteLine("Error : Read Config Params");
        }
      }

      public string COMGetPortName() => this.port_Name.IndexOf("SAGEM") <= -1 ? this.port_Name : this.port_Name.Substring(0, this.port_Name.IndexOf(" – "));
      

      public List<string> getOpenedPortNames()
      {
        List<string> openedPortNames = new List<string>();
        string[] portNames = SerialPort.GetPortNames();
        string[] strArray = new string[portNames.Length];
        int index1 = 0;
        for (int index2 = 0; index2 < portNames.Length; ++index2)
        {
          try
          {
            SerialPort serialPort = new SerialPort(portNames[index2], this.Baud_Rate, this.parity, this.Data_Bit, this.Stop_Bit);
            serialPort.Open();
            strArray[index1] = portNames[index2];
            ++index1;
            serialPort.Close();
          }
          catch (Exception ex)
          {
          }
        }
        for (int index3 = 0; index3 < index1; ++index3)
          openedPortNames.Add(strArray[index3]);
        return openedPortNames;
      }

      public bool COMConfiguration()
      {
        bool flag = false;
        if (this.link_Type != "ETHERNET")
        {
          Communication.bSerial = true;
          try
          {
            Communication.port = new SerialPort(this.COMGetPortName(), this.Baud_Rate, this.parity, this.Data_Bit, this.Stop_Bit);
            if (Communication.port != null)
              flag = true;
          }
          catch
          {
            Console.WriteLine("Error config serial port");
          }
        }
        else
        {
          Communication.bSerial = false;
          Communication.tcpClient = new TcpClient();
          if (Communication.tcpClient != null)
            flag = true;
        }
        return flag;
      }

      public bool StartReceiving() => this.bStartReceiving;

      private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
      {
        if (!this.GetConnectStatus() || Communication.port == null || !Communication.port.IsOpen)
          return;
        if (Communication.port.BytesToRead == 0)
          return;
        try
        {
          byte[] numArray = new byte[this.maxDataSize];
          int index = 0;
          while (true)
          {
            while (Communication.port.BytesToRead == 0 || index >= this.maxDataSize)
            {
              Thread.Sleep(90);
              if (Communication.port.BytesToRead == 0 || index >= this.maxDataSize)
              {
                Communication.ReceivedDataSize = index;
                this.ReceivedData = numArray;
                int receivedDataSize = Communication.ReceivedDataSize;
                return;
              }
            }
            numArray[index] = (byte) this.ReceiveByte();
            this.bStartReceiving = true;
            ++index;
          }
        }
        catch
        {
          Console.WriteLine("Error receive data from serial port");
        }
      }

      public void Ethernet_DataReceived()
      {
        byte[] buffer = new byte[this.maxDataSize];
        if (Communication.tcpClient == null)
          return;
        if (!Communication.tcpClient.Connected)
          return;
        try
        {
          int num = Communication.streamTcp.Read(buffer, 0, this.maxDataSize);
          if (num == 0)
            return;
          Communication.ReceivedDataSize = num;
          this.ReceivedData = buffer;
        }
        catch
        {
          Console.WriteLine("Error receive data from Ethernet TCP");
        }
      }

      public bool Connect()
      {
        bool flag = false;
        if (Communication.bSerial)
        {
          if (Communication.port != null)
          {
            if (!Communication.port.IsOpen)
            {
              try
              {
                Communication.port.ReadTimeout = Communication.serialPortTimeOut;
                Communication.port.Open();
              }
              catch
              {
                Console.WriteLine("Error opening serial port");
              }
              Thread.Sleep(100);
              Communication.port.DiscardInBuffer();
              Communication.port.DiscardOutBuffer();
              Communication.port.DataReceived += new SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
              flag = Communication.port.IsOpen;
            }
          }
          else
          {
            Console.WriteLine("Port Is opened");
          }
        }
        else
        {
          try
          {
            Communication.tcpClient.Connect(this.ip_Address, this.ip_port);
          }
          catch
          {
            Console.WriteLine("Error Connecting TCP");
          }
          Thread.Sleep(5);
          flag = Communication.tcpClient.Connected;
          if (flag)
            Communication.streamTcp = (Stream) Communication.tcpClient.GetStream();
        }
        return flag;
      }

      public bool Disconnect()
      {
        bool flag = false;
        if (Communication.bSerial)
        {
          if (Communication.port != null)
          {
            if (Communication.port.IsOpen)
            {
              try
              {
                Communication.port.DataReceived -= new SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
                Communication.port.Close();
                Communication.port = (SerialPort) null;
              }
              catch
              {
                Console.WriteLine("Error close serial port");
              }
            }
          }
          if (Communication.port != null && !Communication.port.IsOpen)
            flag = true;
        }
        else
        {
          if (Communication.tcpClient != null)
          {
            if (Communication.tcpClient.Connected)
            {
              try
              {
                Communication.tcpClient.Close();
              }
              catch
              {
                Console.WriteLine("Error close Ethernet Tcp");
              }
            }
          }
          if (Communication.tcpClient != null && !Communication.tcpClient.Connected)
            flag = true;
        }
        return flag;
      }

      public bool Send(byte[] buffer, int offset, int count)
      {
        bool flag = false;
        if (this.GetConnectStatus())
        {
          if (Communication.bSerial)
          {
            try
            {
              Communication.port.WriteTimeout = 100;
              Communication.port.Write(buffer, offset, count);
              buffer.ToList().ForEach(c=>Console.Write(c));
              Console.WriteLine();
              flag = true;
            }
            catch
            {
              Console.WriteLine("Error Send from serial port");
            }
          }
          else
          {
            try
            {
              Communication.streamTcp.WriteTimeout = 1000;
              Communication.streamTcp.Write(buffer, offset, count);
              flag = true;
            }
            catch
            {
              Console.WriteLine("Error Send from Ethernet Tcp");
            }
          }
        }
        return flag;
      }

      public int ReceiveByte()
      {
        int num = 0;
        if (Communication.bSerial)
        {
          try
          {
            num = Communication.port.ReadByte();
          }
          catch
          {
            Console.WriteLine("Error receive Byte from serial port");
          }
        }
        else
        {
          try
          {
            num = Communication.streamTcp.ReadByte();
          }
          catch
          {
            Console.WriteLine("Error receive Byte from Ethernet TCP");
          }
        }
        return num;
      }

      public byte[] Receive(out int recvdSize)
      {
        recvdSize = Communication.ReceivedDataSize;
        if (Communication.ReceivedDataSize != 0)
        {
          this.bStartReceiving = false;
          Communication.ReceivedDataSize = 0;
        }
        return this.ReceivedData;
      }

      public bool GetConnectStatus() => Communication.bSerial ? Communication.port != null && Communication.port.IsOpen : Communication.tcpClient != null && Communication.tcpClient.Connected;

      public int BytesInReceiveBuffer() => Communication.ReceivedDataSize;
  }
}
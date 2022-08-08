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
          ReadConfigFile();
        }
        else
        {
          COMDefaultSerialSetting();
          saveConfigInFile();
        }
      }

      public void COMDefaultSerialSetting()
      {
        link_Type = "Serial";
        port_Name = "COM3";
        Baud_Rate = 115200;
      }

      public void saveConfigInFile()
      {
        try
        {
          Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
          configuration.AppSettings.Settings.Remove("link_Type");
          configuration.AppSettings.Settings.Remove("port_Name");
          configuration.AppSettings.Settings.Remove("Baud_Rate");
          configuration.AppSettings.Settings.Remove("ip_Address");
          configuration.AppSettings.Settings.Remove("ip_port");
          configuration.AppSettings.Settings.Add("link_Type", link_Type);
          configuration.AppSettings.Settings.Add("port_Name", port_Name);
          string str1;
          try
          {
            str1 = Baud_Rate.ToString();
          }
          catch
          {
            str1 = "19200";
            Console.WriteLine("Error : Convert baudrate");
          }
          configuration.AppSettings.Settings.Add("Baud_Rate", str1);
          configuration.AppSettings.Settings.Add("ip_Address", ip_Address);
          string str2;
          try
          {
            str2 = ip_port.ToString();
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
          link_Type = ConfigurationManager.AppSettings["link_Type"];
          port_Name = ConfigurationManager.AppSettings["port_Name"];
          string appSetting1 = ConfigurationManager.AppSettings["Baud_Rate"];
          try
          {
            Baud_Rate = int.Parse(appSetting1);
          }
          catch
          {
            Baud_Rate = 19200;
            Console.WriteLine("Error : Convert baudrate");
          }
          ip_Address = ConfigurationManager.AppSettings["ip_Address"];
          string appSetting2 = ConfigurationManager.AppSettings["ip_port"];
          try
          {
            ip_port = int.Parse(appSetting2);
          }
          catch
          {
            ip_port = 9999;
            Console.WriteLine("Error : Read IP port From Config file");
          }
        }
        catch
        {
          Console.WriteLine("Error : Read Config Params");
        }
      }

      public string COMGetPortName() => port_Name.IndexOf("SAGEM") <= -1 ? port_Name : port_Name.Substring(0, port_Name.IndexOf(" – "));
      

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
            SerialPort serialPort = new SerialPort(portNames[index2], Baud_Rate, parity, Data_Bit, Stop_Bit);
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
        if (link_Type != "ETHERNET")
        {
          bSerial = true;
          try
          {
            port = new SerialPort(COMGetPortName(), Baud_Rate, parity, Data_Bit, Stop_Bit);
            if (port != null)
              flag = true;
          }
          catch
          {
            Console.WriteLine("Error config serial port");
          }
        }
        else
        {
          bSerial = false;
          tcpClient = new TcpClient();
          if (tcpClient != null)
            flag = true;
        }
        return flag;
      }

      public bool StartReceiving() => bStartReceiving;

      private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
      {
        if (!GetConnectStatus() || port == null || !port.IsOpen)
          return;
        if (port.BytesToRead == 0)
          return;
        try
        {
          byte[] numArray = new byte[maxDataSize];
          int index = 0;
          while (true)
          {
            while (port.BytesToRead == 0 || index >= maxDataSize)
            {
              Thread.Sleep(90);
              if (port.BytesToRead == 0 || index >= maxDataSize)
              {
                ReceivedDataSize = index;
                ReceivedData = numArray;
                int receivedDataSize = ReceivedDataSize;
                return;
              }
            }
            numArray[index] = (byte) ReceiveByte();
            bStartReceiving = true;
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
        byte[] buffer = new byte[maxDataSize];
        if (tcpClient == null)
          return;
        if (!tcpClient.Connected)
          return;
        try
        {
          int num = streamTcp.Read(buffer, 0, maxDataSize);
          if (num == 0)
            return;
          ReceivedDataSize = num;
          ReceivedData = buffer;
        }
        catch
        {
          Console.WriteLine("Error receive data from Ethernet TCP");
        }
      }

      public bool Connect()
      {
        bool flag = false;
        if (bSerial)
        {
          if (port != null)
          {
            if (!port.IsOpen)
            {
              try
              {
                port.ReadTimeout = serialPortTimeOut;
                port.Open();
              }
              catch
              {
                Console.WriteLine("Error opening serial port");
              }
              Thread.Sleep(100);
              port.DiscardInBuffer();
              port.DiscardOutBuffer();
              port.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
              flag = port.IsOpen;
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
            tcpClient.Connect(ip_Address, ip_port);
          }
          catch
          {
            Console.WriteLine("Error Connecting TCP");
          }
          Thread.Sleep(5);
          flag = tcpClient.Connected;
          if (flag)
            streamTcp = (Stream) tcpClient.GetStream();
        }
        return flag;
      }

      public bool Disconnect()
      {
        bool flag = false;
        if (bSerial)
        {
          if (port != null)
          {
            if (port.IsOpen)
            {
              try
              {
                port.DataReceived -= new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                port.Close();
                port = (SerialPort) null;
              }
              catch
              {
                Console.WriteLine("Error close serial port");
              }
            }
          }
          if (port != null && !port.IsOpen)
            flag = true;
        }
        else
        {
          if (tcpClient != null)
          {
            if (tcpClient.Connected)
            {
              try
              {
                tcpClient.Close();
              }
              catch
              {
                Console.WriteLine("Error close Ethernet Tcp");
              }
            }
          }
          if (tcpClient != null && !tcpClient.Connected)
            flag = true;
        }
        return flag;
      }

      public bool Send(byte[] buffer, int offset, int count)
      {
        bool flag = false;
        if (GetConnectStatus())
        {
          if (bSerial)
          {
            try
            {
              port.WriteTimeout = 100;
              port.Write(buffer, offset, count);
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
              streamTcp.WriteTimeout = 1000;
              streamTcp.Write(buffer, offset, count);
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
        if (bSerial)
        {
          try
          {
            num = port.ReadByte();
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
            num = streamTcp.ReadByte();
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
        recvdSize = ReceivedDataSize;
        if (ReceivedDataSize != 0)
        {
          bStartReceiving = false;
          ReceivedDataSize = 0;
        }
        return ReceivedData;
      }

      public bool GetConnectStatus() => bSerial ? port != null && port.IsOpen : tcpClient != null && tcpClient.Connected;

      public int BytesInReceiveBuffer() => ReceivedDataSize;
  }
}
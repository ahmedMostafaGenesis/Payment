using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace ingenico
{
  internal class LowLevelProtocol
  {
    public const byte HEARTBEAT = 17;
    private byte[] ACK = new byte[1]{ (byte) 6 };
    private byte[] NACK = new byte[1]{ (byte) 21 };
    private Communication COM;
    public Thread readThread;
    private static byte[] receivedData;
    private static int receivedSize;
    private static byte[] msgResponse;
    private int ackCount;
    private bool bAckDelayed;
    private static eReceivedMsg eRcvMsg;
    private static eStatus eState;
    private byte[] RequestToSend;
    private int nRetryCount;
    private static bool _shouldStop;
    private static bool bIsPrintingResponse;
    private bool WaitUserAction;
    private Converter converter = new Converter();
    private System.Windows.Forms.Timer timerAck = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerRetryResponse = new System.Windows.Forms.Timer();
    private System.Windows.Forms.Timer timerHeartBeat = new System.Windows.Forms.Timer();
    public System.Windows.Forms.Timer timerAckLate = new System.Windows.Forms.Timer();
    private bool isEcrDisconnected = true;

    public LowLevelProtocol(Communication communication)
    {
      this.timerAck.Tick += new EventHandler(this.TimerACKOnTick);
      this.timerAck.Interval = 1000;
      this.timerRetryResponse.Tick += new EventHandler(this.TimerRetryResponseOnTick);
      this.timerRetryResponse.Interval = 2000;
      this.timerHeartBeat.Tick += new EventHandler(this.TimerHeartBeatOnTick);
      this.timerHeartBeat.Interval = 15000;
      this.timerAckLate.Tick += new EventHandler(this.TimerAckLateOnTick);
      this.timerAckLate.Interval = 1200;
      this.COM = communication;
    }

    private void TimerHeartBeatOnTick(object sender, EventArgs ea)
    {
      if (this.WaitUserAction || this.isEcrDisconnected)
        return;
      this.WaitUserAction = true;
      // if (MessageBox.Show("Heart Beat missing!", "Connection Problem", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
      // {
      //   LowLevelProtocol.eRcvMsg = eReceivedMsg.E_HB_ABSCENCE;
      //   if (this.COM.link_Type != "ETHERNET" && this.COM.port_Name.IndexOf("SAGEM") > -1 && !this.currentHome.NAKRadioButton.Checked && !this.currentHome.TimeOutACKRadioButton.Checked)
      //   {
      //     int num = (int) MessageBox.Show("Please unplug and replug your USB device!", "WINTSI", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      //   }
      // }
      this.WaitUserAction = false;
    }

    private void TimerRetryResponseOnTick(object sender, EventArgs ea) => LowLevelProtocol.eRcvMsg = eReceivedMsg.E_RETRY_RESP_TO;

    private void TimerACKOnTick(object sender, EventArgs ea) => LowLevelProtocol.eRcvMsg = eReceivedMsg.E_TO_ACK;

    private void TimerAckLateOnTick(object sender, EventArgs ea)
    {
      if (this.COM.Send(this.ACK, 0, this.ACK.Length))
        this.TraceData(this.ACK, this.ACK.Length, "==> ");
      if (this.ackCount > 0)
        --this.ackCount;
      if (this.ackCount == 0)
        this.timerAckLate.Stop();
      else
        this.timerAckLate.Start();
    }

    public byte[] FormatTnxCommand(string Data)
    {
      byte[] byteArray;
      if (this.COM.link_Type == "ETHERNET")
      {
        byteArray = this.converter.HexStringToByteArray(Data, 0, 0);
      }
      else
      {
        byteArray = this.converter.HexStringToByteArray(Data, 1, 3);
        if (Data.Length >= 2 && byteArray.Length <= 1021)
        {
          byteArray[byteArray.Length - 2] = (byte) 3;
          int lrc = this.converter.calculateLrc(byteArray, "command");
          byteArray[byteArray.Length - 1] = true ? (byte) (lrc + 1) : (byte) lrc;
          byteArray[0] = (byte) 2;
          if (true)
          {
            byte[] numArray = new byte[byteArray.Length + 6];
            new Random().NextBytes(numArray);
            Array.Copy((Array) byteArray, 0, (Array) numArray, 3, byteArray.Length);
            return numArray;
          }
        }
      }
      return byteArray;
    }

    public void TraceData(byte[] commandByte, int size, string arrow)
    {
      int length = 16;
      byte[] numArray1 = new byte[length];
      if (commandByte == null)
        return;
      DateTime now = DateTime.Now;
      string text = arrow + now.ToString() + "." + now.Millisecond.ToString() + " : \r\n";
      Console.WriteLine(text);
      int num;
      for (num = 0; num < size / length; ++num)
      {
        Array.Copy((Array) commandByte, num * length, (Array) numArray1, 0, length);
        Console.WriteLine(string.Format("{0,-55}{1}", (object) BitConverter.ToString(numArray1).Replace("-", " "), (object) this.converter.convertByteArreyToString(numArray1, length)) + "\r\n");
      }
      if (size > length * num)
      {
        byte[] numArray2 = new byte[size - length * num];
        Array.Copy((Array) commandByte, num * length, (Array) numArray2, 0, size - length * num);
        Console.WriteLine(string.Format("{0,-55}{1}", (object) BitConverter.ToString(numArray2).Replace("-", " "), (object) this.converter.convertByteArreyToString(numArray2, size - length * num)) + "\r\n");
      }
    }

    public bool SendRequestData(string DataToSend, bool bPrintingResponse)
    {
      bool flag = false;
      this.nRetryCount = 0;
      LowLevelProtocol.bIsPrintingResponse = bPrintingResponse;
      this.RequestToSend = this.FormatTnxCommand(DataToSend);
      eStatus eStatus = LowLevelProtocol.bIsPrintingResponse ? eStatus.E_WAIT_RESPONSE : eStatus.E_CONNECTED;
      if (LowLevelProtocol.eState == eStatus && this.RequestToSend != null)
      {
        flag = this.COM.Send(this.RequestToSend, 0, this.RequestToSend.Length);
        if (this.COM.link_Type != "ETHERNET")
        {
          this.timerAck.Stop();
          this.timerAck.Start();
        }
        if (flag)
          this.TraceData(this.RequestToSend, this.RequestToSend.Length, "==> ");
        if (this.COM.link_Type != "ETHERNET")
        {
          LowLevelProtocol.eState = eStatus.E_WAIT_ACK;
        }
        else
        {
          LowLevelProtocol.eState = eStatus.E_WAIT_RESPONSE;
          this.timerHeartBeat.Stop();
          this.timerHeartBeat.Start();
        }
      }
      return flag;
    }

    public bool ComOpen(Communication communication)
    {
      this.COM = communication;
      if (this.COM.GetConnectStatus())
      {
        LowLevelProtocol.eState = eStatus.E_CONNECTED;
        return true;
      }
      try
      {
        this.COM.COMConfiguration();
        this.COM.Connect();
        if (!this.COM.GetConnectStatus())
          return false;
        LowLevelProtocol.eState = eStatus.E_CONNECTED;
        if (true)
        {
          string text = "Connected\n\n";
          Console.WriteLine(text);
        }
        if (this.readThread != null && this.readThread.IsAlive)
          this.RequestStop();
        if (this.COM.link_Type == "ETHERNET" && this.readThread == null)
        {
          LowLevelProtocol._shouldStop = false;
          this.readThread = new Thread(new ThreadStart(this.ReceiveData));
          this.readThread.Start();
          do
            ;
          while (!this.readThread.IsAlive);
          Thread.Sleep(1);
        }
        return true;
      }
      catch (Exception ex)
      {
        //int num = (int) MessageBox.Show("COM port Error", "WINTSI", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        return false;
      }
    }

    public bool ComClose()
    {
      bool flag = true;
      if (this.readThread != null && this.readThread.IsAlive)
        this.RequestStop();
      if (this.COM.GetConnectStatus())
      {
        this.COM.Disconnect();
        if (this.COM.GetConnectStatus())
          flag = false;
        else if (true)
        {
          string text = "Disconnected\r\n=================================================================================\r\n\r\n";
          Console.WriteLine(text);
        }
      }
      return flag;
    }

    public bool Processing(
      out byte[] msgResp,
      out string szRespStatus,
      out int packageSeq,
      out bool bDisconnect)
    {
      msgResp = (byte[]) null;
      bool flag = false;
      bDisconnect = false;
      szRespStatus = "";
      packageSeq = 0;
      LowLevelProtocol.receivedData = this.COM.Receive(out LowLevelProtocol.receivedSize);
      if (LowLevelProtocol.receivedData != null && LowLevelProtocol.receivedSize != 0)
      {
        LowLevelProtocol.eRcvMsg = this.VerifyTypeMsgReceived();
        this.TraceData(LowLevelProtocol.receivedData, LowLevelProtocol.receivedSize, "<== ");
        this.timerHeartBeat.Stop();
      }
      else if (this.COM.StartReceiving())
        this.timerHeartBeat.Stop();
      switch (LowLevelProtocol.eState)
      {
        case eStatus.E_IDLE:
          this.nRetryCount = 0;
          break;
        case eStatus.E_WAIT_ACK:
          switch (LowLevelProtocol.eRcvMsg)
          {
            case eReceivedMsg.E_NACK:
            case eReceivedMsg.E_TO_ACK:
              ++this.nRetryCount;
              if (this.nRetryCount < 3)
              {
                if (this.COM.Send(this.RequestToSend, 0, this.RequestToSend.Length))
                  this.TraceData(this.RequestToSend, this.RequestToSend.Length, "==> ");
                if (this.COM.link_Type != "ETHERNET")
                {
                  this.timerAck.Stop();
                  this.timerAck.Start();
                  break;
                }
                break;
              }
              this.nRetryCount = 0;
              this.timerHeartBeat.Stop();
              this.timerAck.Stop();
              this.timerRetryResponse.Stop();
              if (!LowLevelProtocol.bIsPrintingResponse)
              {
                LowLevelProtocol.eState = eStatus.E_IDLE;
                bDisconnect = true;
                break;
              }
              LowLevelProtocol.eState = eStatus.E_WAIT_RESPONSE;
              this.timerHeartBeat.Start();
              break;
            case eReceivedMsg.E_ACK:
              LowLevelProtocol.eState = eStatus.E_WAIT_RESPONSE;
              this.nRetryCount = 0;
              this.timerHeartBeat.Start();
              break;
          }
          break;
        case eStatus.E_WAIT_RESPONSE:
          switch (LowLevelProtocol.eRcvMsg)
          {
            case eReceivedMsg.E_ERRORFRAME:
              if (this.COM.link_Type != "ETHERNET")
              {
                this.timerHeartBeat.Stop();
                this.timerHeartBeat.Start();
                if (this.COM.Send(this.NACK, 0, this.NACK.Length))
                {
                  this.TraceData(this.NACK, this.NACK.Length, "==> ");
                  break;
                }
                break;
              }
              break;
            case eReceivedMsg.E_CORRECTFRAME:
              if (this.COM.link_Type != "ETHERNET")
              {
                if (false)
                {
                  if (!this.timerAckLate.Enabled)
                  {
                    this.timerAckLate.Start();
                    this.ackCount = 1;
                  }
                  else
                    ++this.ackCount;
                }
                else
                {
                  if (this.COM.Send(this.ACK, 0, this.ACK.Length))
                    this.TraceData(this.ACK, this.ACK.Length, "==> ");
                  this.timerRetryResponse.Stop();
                  this.bAckDelayed = false;
                }
              }
              szRespStatus = this.converter.convertByteArreyToString(LowLevelProtocol.msgResponse, 2);
              if (szRespStatus != "91")
                packageSeq = (int) Convert.ToChar(LowLevelProtocol.msgResponse[2]) - 48;
              else if (szRespStatus == "91" && LowLevelProtocol.msgResponse.Length >= 6)
                packageSeq = (int) Convert.ToChar(LowLevelProtocol.msgResponse[5]) - 48;
              this.timerHeartBeat.Stop();
              if (szRespStatus == "99")
              {
                flag = true;
                msgResp = LowLevelProtocol.msgResponse;
                break;
              }
              flag = true;
              msgResp = LowLevelProtocol.msgResponse;
              if (packageSeq == 0)
              {
                if (this.ackCount == 0)
                {
                  bDisconnect = true;
                  this.timerAck.Stop();
                  this.timerRetryResponse.Stop();
                  this.bAckDelayed = false;
                  break;
                }
                this.bAckDelayed = true;
                break;
              }
              break;
            case eReceivedMsg.E_HEARTBEAT:
              this.timerHeartBeat.Start();
              break;
            case eReceivedMsg.E_RETRY_RESP_TO:
              bDisconnect = true;
              flag = false;
              this.timerHeartBeat.Stop();
              this.timerAck.Stop();
              this.timerRetryResponse.Stop();
              break;
            case eReceivedMsg.E_HB_ABSCENCE:
              bDisconnect = true;
              this.timerHeartBeat.Stop();
              this.timerAck.Stop();
              this.timerRetryResponse.Stop();
              break;
            case eReceivedMsg.E_NO_ACK:
              this.timerHeartBeat.Start();
              break;
            default:
              if (this.bAckDelayed && this.ackCount == 0)
              {
                bDisconnect = true;
                this.timerHeartBeat.Stop();
                this.timerAck.Stop();
                this.timerRetryResponse.Stop();
                this.bAckDelayed = false;
                break;
              }
              break;
          }
          break;
      }
      this.isEcrDisconnected = bDisconnect;
      LowLevelProtocol.eRcvMsg = eReceivedMsg.E_NONE;
      LowLevelProtocol.receivedData = (byte[]) null;
      LowLevelProtocol.receivedSize = 0;
      return flag;
    }

    public void RequestStop()
    {
      if (LowLevelProtocol._shouldStop || this.readThread == null)
        return;
      LowLevelProtocol._shouldStop = true;
      this.readThread.Abort();
      this.readThread = (Thread) null;
    }

    public void ReceiveData()
    {
      while (!LowLevelProtocol._shouldStop)
      {
        this.COM.Ethernet_DataReceived();
        Thread.Sleep(100);
      }
    }

    private eReceivedMsg VerifyTypeMsgReceived()
    {
      if (LowLevelProtocol.receivedData[0] == (byte) 17)
        return eReceivedMsg.E_HEARTBEAT;
      eReceivedMsg received;
      if (this.COM.link_Type != "ETHERNET")
      {
        if ((int) LowLevelProtocol.receivedData[0] == (int) this.ACK[0])
        {
          if (this.COM.link_Type != "ETHERNET")
            this.timerAck.Stop();
          return eReceivedMsg.E_ACK;
        }
        if ((int) LowLevelProtocol.receivedData[0] == (int) this.NACK[0])
        {
          if (this.COM.link_Type != "ETHERNET")
            this.timerAck.Stop();
          return eReceivedMsg.E_NACK;
        }
        if (false)
          return eReceivedMsg.E_NO_ACK;
        if (false)
          return eReceivedMsg.E_ERRORFRAME;
        Dictionary<string, object> response = new ResponseMsg().extractResponse(LowLevelProtocol.receivedData, LowLevelProtocol.receivedSize);
        if (response != null)
        {
          byte[] data = (byte[]) response["RESPONSEWSTXLRC"];
          if ((int) (byte) response["LRC"] == this.converter.calculateLrc(data, "command") && LowLevelProtocol.receivedSize >= 3)
          {
            LowLevelProtocol.msgResponse = (byte[]) response["MSG"];
            received = eReceivedMsg.E_CORRECTFRAME;
          }
          else
            received = eReceivedMsg.E_ERRORFRAME;
        }
        else
          received = eReceivedMsg.E_ERRORFRAME;
      }
      else if (LowLevelProtocol.receivedSize >= 2)
      {
        LowLevelProtocol.msgResponse = new byte[LowLevelProtocol.receivedSize];
        Array.Copy((Array) LowLevelProtocol.receivedData, (Array) LowLevelProtocol.msgResponse, LowLevelProtocol.receivedSize);
        received = eReceivedMsg.E_CORRECTFRAME;
      }
      else
        received = eReceivedMsg.E_ERRORFRAME;
      return received;
    }
  }
}

    internal enum eReceivedMsg
    {
      E_NONE,
      E_ERRORFRAME,
      E_FRAGMENT,
      E_CORRECTFRAME,
      E_HEARTBEAT,
      E_NACK,
      E_ACK,
      E_TO_ACK,
      E_RETRY_RESP_TO,
      E_HB_ABSCENCE,
      E_NO_ACK,
    }
    internal enum eStatus
    {
      E_IDLE,
      E_CONNECTED,
      E_WAIT_ACK,
      E_WAIT_RESPONSE,
      E_PARSE_RESULT,
      E_END_RESPONSE,
    }
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
      timerAck.Tick += new EventHandler(TimerACKOnTick);
      timerAck.Interval = 1000;
      timerRetryResponse.Tick += new EventHandler(TimerRetryResponseOnTick);
      timerRetryResponse.Interval = 2000;
      timerHeartBeat.Tick += new EventHandler(TimerHeartBeatOnTick);
      timerHeartBeat.Interval = 15000;
      timerAckLate.Tick += new EventHandler(TimerAckLateOnTick);
      timerAckLate.Interval = 1200;
      COM = communication;
    }

    private void TimerHeartBeatOnTick(object sender, EventArgs ea)
    {
      if (WaitUserAction || isEcrDisconnected)
        return;
      WaitUserAction = true;
      // if (MessageBox.Show("Heart Beat missing!", "Connection Problem", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
      // {
      //   LowLevelProtocol.eRcvMsg = eReceivedMsg.E_HB_ABSCENCE;
      //   if (this.COM.link_Type != "ETHERNET" && this.COM.port_Name.IndexOf("SAGEM") > -1 && !this.currentHome.NAKRadioButton.Checked && !this.currentHome.TimeOutACKRadioButton.Checked)
      //   {
      //     int num = (int) MessageBox.Show("Please unplug and replug your USB device!", "WINTSI", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
      //   }
      // }
      WaitUserAction = false;
    }

    private void TimerRetryResponseOnTick(object sender, EventArgs ea) => eRcvMsg = eReceivedMsg.E_RETRY_RESP_TO;

    private void TimerACKOnTick(object sender, EventArgs ea) => eRcvMsg = eReceivedMsg.E_TO_ACK;

    private void TimerAckLateOnTick(object sender, EventArgs ea)
    {
      if (COM.Send(ACK, 0, ACK.Length))
        TraceData(ACK, ACK.Length, "==> ");
      if (ackCount > 0)
        --ackCount;
      if (ackCount == 0)
        timerAckLate.Stop();
      else
        timerAckLate.Start();
    }

    private byte[] FormatTnxCommand(string data)
    {
      var byteArray = converter.HexStringToByteArray(data, 1, 3);
      if (data.Length < 2 || byteArray.Length > 1021) return byteArray;
      byteArray[byteArray.Length - 2] = 3;
      int lrc = converter.calculateLrc(byteArray, "command");
      byteArray[byteArray.Length - 1] = (byte) (lrc + 1);
      byteArray[0] = 2;
      // byte[] numArray = new byte[byteArray.Length + 6];
      // new Random().NextBytes(numArray);
      // Array.Copy(byteArray, 0, numArray, 3, byteArray.Length);
      return byteArray;
    }

    private void TraceData(byte[] commandByte, int size, string arrow)
    {
      const int length = 16;
      var numArray1 = new byte[length];
      if (commandByte == null)
        return;
      var now = DateTime.Now;
      var text = arrow + now.ToString(CultureInfo.InvariantCulture) + "." + now.Millisecond + " : \r\n";
      Console.WriteLine(text);
      int num;
      for (num = 0; num < size / length; ++num)
      {
        Array.Copy(commandByte, num * length, numArray1, 0, length);
        Console.WriteLine(
          $"{BitConverter.ToString(numArray1).Replace("-", " "),-55}{converter.ConvertByteArrayToString(numArray1, length)}" + "\r\n");
      }
      if (size > length * num)
      {
        var numArray2 = new byte[size - length * num];
        Array.Copy(commandByte, num * length, numArray2, 0, size - length * num);
        Console.WriteLine(
          $"{BitConverter.ToString(numArray2).Replace("-", " "),-55}{converter.ConvertByteArrayToString(numArray2, size - length * num)}" + "\r\n");
      }
    }

    public bool SendRequestData(string DataToSend, bool bPrintingResponse)
    {
      bool flag = false;
      nRetryCount = 0;
      bIsPrintingResponse = bPrintingResponse;
      RequestToSend = FormatTnxCommand(DataToSend);
      eStatus eStatus = bIsPrintingResponse ? eStatus.E_WAIT_RESPONSE : eStatus.E_CONNECTED;
      if (eState == eStatus && RequestToSend != null)
      {
        Console.WriteLine(DataToSend);
        flag = COM.Send(RequestToSend, 0, RequestToSend.Length);
        if (flag)
          TraceData(RequestToSend, RequestToSend.Length, "==> ");
        eState = eStatus.E_WAIT_RESPONSE;
        timerHeartBeat.Stop();
        timerHeartBeat.Start();
      }
      return flag;
    }

    public bool ComOpen(Communication communication)
    {
      COM = communication;
      if (COM.GetConnectStatus())
      {
        eState = eStatus.E_CONNECTED;
        return true;
      }
      try
      {
        COM.COMConfiguration();
        COM.Connect();
        if (!COM.GetConnectStatus())
          return false;
        eState = eStatus.E_CONNECTED;
        if (true)
        {
          string text = "Connected\n\n";
          Console.WriteLine(text);
        }
        if (readThread != null && readThread.IsAlive)
          RequestStop();
        if (COM.link_Type == "ETHERNET" && readThread == null)
        {
          _shouldStop = false;
          readThread = new Thread(new ThreadStart(ReceiveData));
          readThread.Start();
          do
          {
          } while (!readThread.IsAlive);
          Thread.Sleep(1);
        }
        return true;
      }
      catch (Exception ex)
      {
         Console.WriteLine("COM port Error");
        return false;
      }
    }

    public bool ComClose()
    {
      bool flag = true;
      if (readThread != null && readThread.IsAlive)
        RequestStop();
      if (COM.GetConnectStatus())
      {
        COM.Disconnect();
        if (COM.GetConnectStatus())
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
      receivedData = COM.Receive(out receivedSize);
      if (receivedData != null && receivedSize != 0)
      {
        eRcvMsg = VerifyTypeMsgReceived();
        TraceData(receivedData, receivedSize, "<== ");
        timerHeartBeat.Stop();
      }
      else if (COM.StartReceiving())
        timerHeartBeat.Stop();
      switch (eState)
      {
        case eStatus.E_IDLE:
          nRetryCount = 0;
          break;
        case eStatus.E_WAIT_ACK:
          switch (eRcvMsg)
          {
            case eReceivedMsg.E_NACK:
            case eReceivedMsg.E_TO_ACK:
              ++nRetryCount;
              if (nRetryCount < 3)
              {
                if (COM.Send(RequestToSend, 0, RequestToSend.Length))
                  TraceData(RequestToSend, RequestToSend.Length, "==> ");
                if (COM.link_Type != "ETHERNET")
                {
                  timerAck.Stop();
                  timerAck.Start();
                  break;
                }
                break;
              }
              nRetryCount = 0;
              timerHeartBeat.Stop();
              timerAck.Stop();
              timerRetryResponse.Stop();
              if (!bIsPrintingResponse)
              {
                eState = eStatus.E_IDLE;
                bDisconnect = true;
                break;
              }
              eState = eStatus.E_WAIT_RESPONSE;
              timerHeartBeat.Start();
              break;
            case eReceivedMsg.E_ACK:
              eState = eStatus.E_WAIT_RESPONSE;
              nRetryCount = 0;
              timerHeartBeat.Start();
              break;
          }
          break;
        case eStatus.E_WAIT_RESPONSE:
          switch (eRcvMsg)
          {
            case eReceivedMsg.E_ERRORFRAME:
              if (COM.link_Type != "ETHERNET")
              {
                timerHeartBeat.Stop();
                timerHeartBeat.Start();
                if (COM.Send(NACK, 0, NACK.Length))
                {
                  TraceData(NACK, NACK.Length, "==> ");
                  break;
                }
                break;
              }
              break;
            case eReceivedMsg.E_CORRECTFRAME:
              if (COM.link_Type != "ETHERNET")
              {
                if (false)
                {
                  if (!timerAckLate.Enabled)
                  {
                    timerAckLate.Start();
                    ackCount = 1;
                  }
                  else
                    ++ackCount;
                }
                else
                {
                  if (COM.Send(ACK, 0, ACK.Length))
                    TraceData(ACK, ACK.Length, "==> ");
                  timerRetryResponse.Stop();
                  bAckDelayed = false;
                }
              }
              szRespStatus = converter.ConvertByteArrayToString(msgResponse, 2);
              if (szRespStatus != "91")
                packageSeq = (int) Convert.ToChar(msgResponse[2]) - 48;
              else if (szRespStatus == "91" && msgResponse.Length >= 6)
                packageSeq = (int) Convert.ToChar(msgResponse[5]) - 48;
              timerHeartBeat.Stop();
              if (szRespStatus == "99")
              {
                flag = true;
                msgResp = msgResponse;
                break;
              }
              flag = true;
              msgResp = msgResponse;
              if (packageSeq == 0)
              {
                if (ackCount == 0)
                {
                  bDisconnect = true;
                  timerAck.Stop();
                  timerRetryResponse.Stop();
                  bAckDelayed = false;
                  break;
                }
                bAckDelayed = true;
                break;
              }
              break;
            case eReceivedMsg.E_HEARTBEAT:
              timerHeartBeat.Start();
              break;
            case eReceivedMsg.E_RETRY_RESP_TO:
              bDisconnect = true;
              flag = false;
              timerHeartBeat.Stop();
              timerAck.Stop();
              timerRetryResponse.Stop();
              break;
            case eReceivedMsg.E_HB_ABSCENCE:
              bDisconnect = true;
              timerHeartBeat.Stop();
              timerAck.Stop();
              timerRetryResponse.Stop();
              break;
            case eReceivedMsg.E_NO_ACK:
              timerHeartBeat.Start();
              break;
            default:
              if (bAckDelayed && ackCount == 0)
              {
                bDisconnect = true;
                timerHeartBeat.Stop();
                timerAck.Stop();
                timerRetryResponse.Stop();
                bAckDelayed = false;
                break;
              }
              break;
          }
          break;
      }
      isEcrDisconnected = bDisconnect;
      eRcvMsg = eReceivedMsg.E_NONE;
      receivedData = (byte[]) null;
      receivedSize = 0;
      return flag;
    }

    public void RequestStop()
    {
      if (_shouldStop || readThread == null)
        return;
      _shouldStop = true;
      readThread.Abort();
      readThread = (Thread) null;
    }

    public void ReceiveData()
    {
      while (!_shouldStop)
      {
        COM.Ethernet_DataReceived();
        Thread.Sleep(100);
      }
    }

    private eReceivedMsg VerifyTypeMsgReceived()
    {
      if (receivedData[0] == (byte) 17)
        return eReceivedMsg.E_HEARTBEAT;
      eReceivedMsg received;
      if (COM.link_Type != "ETHERNET")
      {
        if ((int) receivedData[0] == (int) ACK[0])
        {
          if (COM.link_Type != "ETHERNET")
            timerAck.Stop();
          return eReceivedMsg.E_ACK;
        }
        if ((int) receivedData[0] == (int) NACK[0])
        {
          if (COM.link_Type != "ETHERNET")
            timerAck.Stop();
          return eReceivedMsg.E_NACK;
        }
        if (false)
          return eReceivedMsg.E_NO_ACK;
        if (false)
          return eReceivedMsg.E_ERRORFRAME;
        Dictionary<string, object> response = new ResponseMsg().extractResponse(receivedData, receivedSize);
        if (response != null)
        {
          byte[] data = (byte[]) response["RESPONSEWSTXLRC"];
          if ((int) (byte) response["LRC"] == converter.calculateLrc(data, "command") && receivedSize >= 3)
          {
            msgResponse = (byte[]) response["MSG"];
            received = eReceivedMsg.E_CORRECTFRAME;
          }
          else
            received = eReceivedMsg.E_ERRORFRAME;
        }
        else
          received = eReceivedMsg.E_ERRORFRAME;
      }
      else if (receivedSize >= 2)
      {
        msgResponse = new byte[receivedSize];
        Array.Copy((Array) receivedData, (Array) msgResponse, receivedSize);
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
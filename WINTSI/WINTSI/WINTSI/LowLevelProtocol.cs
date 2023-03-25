using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Ingenico
{
	internal class LowLevelProtocol
	{
		public const byte Heartbeat = 17;

		private readonly byte[] ack = new byte[1] {6};

		private readonly byte[] nack = new byte[1] {21};

		private Communication com;

		private Thread readThread;

		private static byte[] _receivedData;

		private static int _receivedSize;

		private static byte[] _msgResponse;

		private int ackCount;

		private bool bAckDelayed;

		private static eReceivedMsg _eRcvMsg;

		private static eStatus _eState;

		private byte[] requestToSend;

		private int nRetryCount;

		private readonly HomeForm currentHome;

		private static bool _shouldStop;

		private static bool _bIsPrintingResponse;

		private bool waitUserAction;

		private readonly Converter converter = new();

		private readonly System.Windows.Forms.Timer timerAck = new();

		private readonly System.Windows.Forms.Timer timerRetryResponse = new();

		private readonly System.Windows.Forms.Timer timerHeartBeat = new();

		public readonly System.Windows.Forms.Timer TimerAckLate = new();

		private bool isEcrDisconnected = true;

		public LowLevelProtocol(HomeForm currentHome)
		{
			this.currentHome = currentHome;
			timerAck.Tick += TimerACKOnTick;
			timerAck.Interval = 1000;
			timerRetryResponse.Tick += TimerRetryResponseOnTick;
			timerRetryResponse.Interval = 2000;
			timerHeartBeat.Tick += TimerHeartBeatOnTick;
			timerHeartBeat.Interval = 15000;
			TimerAckLate.Tick += TimerAckLateOnTick;
			TimerAckLate.Interval = 1200;
			com = currentHome.Com;
		}

		private void TimerHeartBeatOnTick(object sender, EventArgs ea)
		{
			if (waitUserAction || isEcrDisconnected)
			{
				return;
			}

			waitUserAction = true;
			if (MessageBox.Show("Heart Beat missing!", "Connection Problem", MessageBoxButtons.RetryCancel,
				    MessageBoxIcon.Exclamation) == DialogResult.Cancel)
			{
				_eRcvMsg = eReceivedMsg.E_HB_ABSCENCE;
				if (com.LinkType != "ETHERNET" && com.PortName.IndexOf("SAGEM", StringComparison.Ordinal) > -1 &&
				    !currentHome.NakRadioButton.Checked && !currentHome.TimeOutAckRadioButton.Checked)
				{
					MessageBox.Show("Please unplug and replug your USB device!", "WINTSI", MessageBoxButtons.OK,
						MessageBoxIcon.Asterisk);
				}
			}

			waitUserAction = false;
		}

		private void TimerRetryResponseOnTick(object sender, EventArgs ea)
		{
			_eRcvMsg = eReceivedMsg.E_RETRY_RESP_TO;
		}

		private void TimerACKOnTick(object sender, EventArgs ea)
		{
			_eRcvMsg = eReceivedMsg.E_TO_ACK;
		}

		private void TimerAckLateOnTick(object sender, EventArgs ea)
		{
			if (Communication.Send(ack, 0, ack.Length))
			{
				TraceData(ack, ack.Length, "==> ", Color.Blue);
			}

			if (ackCount > 0)
			{
				ackCount--;
			}

			if (ackCount == 0)
			{
				TimerAckLate.Stop();
			}
			else
			{
				TimerAckLate.Start();
			}
		}

		private byte[] FormatTnxCommand(string data)
		{
			byte[] array;
			if (com.LinkType == "ETHERNET")
			{
				array = converter.HexStringToByteArray(data, 0, 0);
			}
			else
			{
				array = converter.HexStringToByteArray(data, 1, 3);
				if (data.Length < 2 || array.Length > 1021) return array;
				array[array.Length - 2] = 3;
				var num = converter.calculateLrc(array, "command");
				if (!currentHome.BadLrcRadioButton.Checked)
				{
					array[array.Length - 1] = (byte) num;
				}
				else
				{
					array[array.Length - 1] = (byte) (num + 1);
				}

				array[0] = 2;
				if (!currentHome.RequestGabageRadioButton.Checked) return array;
				var array2 = new byte[array.Length + 6];
				new Random().NextBytes(array2);
				Array.Copy(array, 0, array2, 3, array.Length);
				return array2;
			}

			return array;
		}

		private void TraceData(byte[] commandByte, int size, string arrow, Color color)
		{
			const int num = 16;
			var array = new byte[num];
			if (commandByte == null || !currentHome.Trace.Enabled) return;
			var now = DateTime.Now;
			var text = arrow + now.ToString(CultureInfo.InvariantCulture) + "." + now.Millisecond + " : \r\n";
			currentHome.Trace.SelectionStart = currentHome.Trace.TextLength;
			currentHome.Trace.SelectionLength = text.Length;
			currentHome.Trace.SelectionColor = color;
			currentHome.Trace.AppendText(text);
			int i;
			for (i = 0; i < size / num; i++)
			{
				Array.Copy(commandByte, i * num, array, 0, num);
				var arg = BitConverter.ToString(array).Replace("-", " ");
				var arg2 = converter.convertByteArreyToString(array, num);
				var text2 = $"{arg,-55}{arg2}";
				currentHome.Trace.AppendText(text2 + "\r\n");
			}

			if (size > num * i)
			{
				var array2 = new byte[size - num * i];
				Array.Copy(commandByte, i * num, array2, 0, size - num * i);
				var arg = BitConverter.ToString(array2).Replace("-", " ");
				var arg2 = converter.convertByteArreyToString(array2, size - num * i);
				var text2 = $"{arg,-55}{arg2}";
				currentHome.Trace.AppendText(text2 + "\r\n");
			}

			currentHome.Trace.AppendText("\r\n");
			currentHome.Update();
			currentHome.Trace.ScrollToCaret();
		}

		public bool SendRequestData(string dataToSend, bool bPrintingResponse)
		{
			nRetryCount = 0;
			_bIsPrintingResponse = bPrintingResponse;
			requestToSend = FormatTnxCommand(dataToSend);
			var eStatus2 = ((!_bIsPrintingResponse) ? eStatus.E_CONNECTED : eStatus.E_WAIT_RESPONSE);
			if (_eState != eStatus2 || requestToSend == null) return false;
			var flag = Communication.Send(requestToSend, 0, requestToSend.Length);
			if (com.LinkType != "ETHERNET")
			{
				timerAck.Stop();
				timerAck.Start();
			}

			if (flag)
			{
				TraceData(requestToSend, requestToSend.Length, "==> ", Color.Blue);
			}

			if (com.LinkType != "ETHERNET")
			{
				_eState = eStatus.E_WAIT_ACK;
			}
			else
			{
				_eState = eStatus.E_WAIT_RESPONSE;
				timerHeartBeat.Stop();
				timerHeartBeat.Start();
			}

			return flag;
		}

		public bool ComOpen(Communication communication)
		{
			com = communication;
			if (Communication.GetConnectStatus())
			{
				_eState = eStatus.E_CONNECTED;
				return true;
			}

			try
			{
				com.ComConfiguration();
				com.Connect();
				if (!Communication.GetConnectStatus()) return false;
				_eState = eStatus.E_CONNECTED;

				if (readThread is {IsAlive: true})
				{
					RequestStop();
				}

				if (com.LinkType != "ETHERNET" || readThread != null) return true;
				_shouldStop = false;
				readThread = new Thread(ReceiveData);
				readThread.Start();
				while (!readThread.IsAlive)
				{
				}

				Thread.Sleep(1);

				return true;

			}
			catch (Exception)
			{
				Console.Write("COM port Error.");
				return false;
			}
		}

		public bool ComClose()
		{
			var result = true;
			if (readThread is {IsAlive: true})
			{
				RequestStop();
			}

			if (!Communication.GetConnectStatus()) return true;
			com.Disconnect();
			if (Communication.GetConnectStatus())
			{
				result = false;
			}
			else if (currentHome.Trace.Enabled)
			{
				const string text = "Disconnected\r\n=================================================================================\r\n\r\n";
				currentHome.Trace.SelectionStart = currentHome.Trace.TextLength;
				currentHome.Trace.SelectionLength = text.Length;
				currentHome.Trace.SelectionColor = Color.Green;
				currentHome.Trace.AppendText(text);
				currentHome.Trace.ScrollToCaret();
			}

			return result;
		}

		public bool Processing(out byte[] msgResp, out string szRespStatus, out int packageSeq, out bool bDisconnect)
		{
			msgResp = null;
			var result = false;
			bDisconnect = false;
			szRespStatus = "";
			packageSeq = 0;
			_receivedData = com.Receive(out _receivedSize);
			if (_receivedData != null && _receivedSize != 0)
			{
				_eRcvMsg = VerifyTypeMsgReceived();
				TraceData(_receivedData, _receivedSize, "<== ", Color.Red);
				timerHeartBeat.Stop();
			}
			else if (com.StartReceiving())
			{
				timerHeartBeat.Stop();
			}

			switch (_eState)
			{
				case eStatus.E_IDLE:
					nRetryCount = 0;
					break;
				case eStatus.E_WAIT_ACK:
					if (_eRcvMsg == eReceivedMsg.E_ACK)
					{
						_eState = eStatus.E_WAIT_RESPONSE;
						nRetryCount = 0;
						timerHeartBeat.Start();
					}
					else
					{
						if (_eRcvMsg != eReceivedMsg.E_NACK && _eRcvMsg != eReceivedMsg.E_TO_ACK)
						{
							break;
						}

						nRetryCount++;
						if (nRetryCount < 3)
						{
							if (Communication.Send(requestToSend, 0, requestToSend.Length))
							{
								TraceData(requestToSend, requestToSend.Length, "==> ", Color.Blue);
							}

							if (com.LinkType != "ETHERNET")
							{
								timerAck.Stop();
								timerAck.Start();
							}

							break;
						}

						nRetryCount = 0;
						timerHeartBeat.Stop();
						timerAck.Stop();
						timerRetryResponse.Stop();
						if (!_bIsPrintingResponse)
						{
							_eState = eStatus.E_IDLE;
							bDisconnect = true;
						}
						else
						{
							_eState = eStatus.E_WAIT_RESPONSE;
							timerHeartBeat.Start();
						}
					}

					break;
				case eStatus.E_WAIT_RESPONSE:
					if (_eRcvMsg == eReceivedMsg.E_CORRECTFRAME)
					{
						if (com.LinkType != "ETHERNET")
						{
							if (currentHome.LateAckRadioButton.Checked)
							{
								if (!TimerAckLate.Enabled)
								{
									TimerAckLate.Start();
									ackCount = 1;
								}
								else
								{
									ackCount++;
								}
							}
							else
							{
								if (Communication.Send(ack, 0, ack.Length))
								{
									TraceData(ack, ack.Length, "==> ", Color.Blue);
								}

								timerRetryResponse.Stop();
								bAckDelayed = false;
							}
						}

						szRespStatus = converter.convertByteArreyToString(_msgResponse, 2);
						if (szRespStatus != "91")
						{
							packageSeq = Convert.ToChar(_msgResponse[2]) - 48;
						}
						else if (szRespStatus == "91" && _msgResponse.Length >= 6)
						{
							packageSeq = Convert.ToChar(_msgResponse[5]) - 48;
						}

						timerHeartBeat.Stop();
						if (szRespStatus == "99")
						{
							result = true;
							msgResp = _msgResponse;
							break;
						}

						result = true;
						msgResp = _msgResponse;
						if (packageSeq == 0)
						{
							if (ackCount == 0)
							{
								bDisconnect = true;
								timerAck.Stop();
								timerRetryResponse.Stop();
								bAckDelayed = false;
							}
							else
							{
								bAckDelayed = true;
							}
						}
					}
					else switch (_eRcvMsg)
					{
						case eReceivedMsg.E_ERRORFRAME:
						{
							if (com.LinkType != "ETHERNET")
							{
								timerHeartBeat.Stop();
								timerHeartBeat.Start();
								if (Communication.Send(nack, 0, nack.Length))
								{
									TraceData(nack, nack.Length, "==> ", Color.Blue);
								}
							}

							break;
						}
						case eReceivedMsg.E_HEARTBEAT:
							timerHeartBeat.Start();
							break;
						case eReceivedMsg.E_RETRY_RESP_TO:
							bDisconnect = true;
							result = false;
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
						case eReceivedMsg.E_NONE:
						case eReceivedMsg.E_FRAGMENT:
						case eReceivedMsg.E_CORRECTFRAME:
						case eReceivedMsg.E_NACK:
						case eReceivedMsg.E_ACK:
						case eReceivedMsg.E_TO_ACK:
						default:
						{
							if (bAckDelayed && ackCount == 0)
							{
								bDisconnect = true;
								timerHeartBeat.Stop();
								timerAck.Stop();
								timerRetryResponse.Stop();
								bAckDelayed = false;
							}

							break;
						}
					}

					break;
				case eStatus.E_CONNECTED:
					break;
				case eStatus.E_PARSE_RESULT:
					break;
				case eStatus.E_END_RESPONSE:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			isEcrDisconnected = bDisconnect;
			_eRcvMsg = eReceivedMsg.E_NONE;
			_receivedData = null;
			_receivedSize = 0;
			return result;
		}

		public void RequestStop()
		{
			if (_shouldStop || readThread == null) return;
			_shouldStop = true;
			readThread.Abort();
			readThread = null;
		}

		private void ReceiveData()
		{
			while (!_shouldStop)
			{
				com.Ethernet_DataReceived();
				Thread.Sleep(100);
			}
		}

		private eReceivedMsg VerifyTypeMsgReceived()
		{
			if (_receivedData[0] == 17)
			{
				return eReceivedMsg.E_HEARTBEAT;
			}

			if (com.LinkType != "ETHERNET")
			{
				if (_receivedData[0] == ack[0])
				{
					if (com.LinkType != "ETHERNET")
					{
						timerAck.Stop();
					}

					return eReceivedMsg.E_ACK;
				}

				if (_receivedData[0] == nack[0])
				{
					if (com.LinkType != "ETHERNET")
					{
						timerAck.Stop();
					}

					return eReceivedMsg.E_NACK;
				}

				if (currentHome.TimeOutAckRadioButton.Checked)
				{
					return eReceivedMsg.E_NO_ACK;
				}

				if (currentHome.NakRadioButton.Checked)
				{
					return eReceivedMsg.E_ERRORFRAME;
				}

				var dictionary = new ResponseMsg().extractResponse(_receivedData, _receivedSize);
				if (dictionary == null) return eReceivedMsg.E_ERRORFRAME;
				var data = (byte[]) dictionary["RESPONSEWSTXLRC"];
				if ((byte) dictionary["LRC"] != converter.calculateLrc(data, "command") || _receivedSize < 3)
					return eReceivedMsg.E_ERRORFRAME;
				_msgResponse = (byte[]) dictionary["MSG"];
				return eReceivedMsg.E_CORRECTFRAME;

			}

			if (_receivedSize < 2) return eReceivedMsg.E_ERRORFRAME;
			_msgResponse = new byte[_receivedSize];
			Array.Copy(_receivedData, _msgResponse, _receivedSize);
			return eReceivedMsg.E_CORRECTFRAME;

		}
	}
}
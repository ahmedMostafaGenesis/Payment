using System;

namespace Ingenico
{
	public class ApplicationProtocol
	{
		public Request Request;

		private readonly LowLevelProtocol lowLevelProtocol;

		public ApplicationProtocol(HomeForm currentHome)
		{
			lowLevelProtocol = new LowLevelProtocol(currentHome);
		}

		public bool IsLateAckStarted()
		{
			return lowLevelProtocol.TimerAckLate.Enabled;
		}

		public bool EndConnection()
		{
			return lowLevelProtocol.ComClose();
		}

		public bool StartConnection(Communication com)
		{
			return lowLevelProtocol.ComOpen(com);
		}

		public bool RequestApplyTransaction()
		{
			var result = false;
			var text = Request.BuildRequest();
			if (text.Length != 0)
			{
				result = lowLevelProtocol.SendRequestData(text, bPrintingResponse: false);
			}
			return result;
		}

		public bool PrintingResponseMessage(string szPrintingStatus)
		{
			var text = Request.BuildPrintResponse(szPrintingStatus);
			if (text.Length != 0)
			{
				lowLevelProtocol.SendRequestData(text, bPrintingResponse: true);
			}
			return false;
		}

		public bool ResponseData(out byte[] msgData, out string szRespStatus, out int packageSeq, out bool bDisconnectCom)
		{
			return lowLevelProtocol.Processing(out msgData, out szRespStatus, out packageSeq, out bDisconnectCom);
		}

		public void EndApplication()
		{
			lowLevelProtocol.RequestStop();
		}

		public void SendRequestToTerminal(string request)
		{
			var dataToSend = new Request().ConvertToHex(request);
			lowLevelProtocol.SendRequestData(dataToSend, bPrintingResponse: false);
		}
	}
}
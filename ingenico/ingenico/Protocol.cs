namespace ingenico
{
    public class Protocol
    {
        public Request request;
        private LowLevelProtocol Lowlevelprotocol;

        public Protocol(Communication communication) => Lowlevelprotocol = new LowLevelProtocol(communication);
        
        public bool EndConnection() => Lowlevelprotocol.ComClose();

        public bool StartConnection(Communication COM) => Lowlevelprotocol.ComOpen(COM);

        public bool RequestApplyTransaction()
        {
            bool flag = false;
            string DataToSend = request.BuildRequest();
            if (DataToSend.Length != 0)
                flag = Lowlevelprotocol.SendRequestData(DataToSend, false);
            return flag;
        }

        public bool PrintingResponseMessage(string szPrintingStatus)
        {
            int num = 0;
            string DataToSend = request.BuildPrintResponse(szPrintingStatus);
            if (DataToSend.Length == 0)
                return num != 0;
            Lowlevelprotocol.SendRequestData(DataToSend, true);
            return num != 0;
        }

        public bool ResponseData(
            out byte[] msgData,
            out string szRespStatus,
            out int packageSeq,
            out bool bDisconnectCom)
        {
            return Lowlevelprotocol.Processing(out msgData, out szRespStatus, out packageSeq, out bDisconnectCom);
        }

        public void EndApplication() => Lowlevelprotocol.RequestStop();

        public void SendRequestToTerminal(string request) => Lowlevelprotocol.SendRequestData(new Request().ConvertToHex(request), false);
    }
}
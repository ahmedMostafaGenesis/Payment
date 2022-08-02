namespace ingenico
{
    public class Protocol
    {
        public Request request;
        private LowLevelProtocol Lowlevelprotocol;

        public Protocol(Communication communication) => this.Lowlevelprotocol = new LowLevelProtocol(communication);
        
        public bool EndConnection() => this.Lowlevelprotocol.ComClose();

        public bool StartConnection(Communication COM) => this.Lowlevelprotocol.ComOpen(COM);

        public bool RequestApplyTransaction()
        {
            bool flag = false;
            string DataToSend = this.request.BuildRequest();
            if (DataToSend.Length != 0)
                flag = this.Lowlevelprotocol.SendRequestData(DataToSend, false);
            return flag;
        }

        public bool PrintingResponseMessage(string szPrintingStatus)
        {
            int num = 0;
            string DataToSend = this.request.BuildPrintResponse(szPrintingStatus);
            if (DataToSend.Length == 0)
                return num != 0;
            this.Lowlevelprotocol.SendRequestData(DataToSend, true);
            return num != 0;
        }

        public bool ResponseData(
            out byte[] msgData,
            out string szRespStatus,
            out int packageSeq,
            out bool bDisconnectCom)
        {
            return this.Lowlevelprotocol.Processing(out msgData, out szRespStatus, out packageSeq, out bDisconnectCom);
        }

        public void EndApplication() => this.Lowlevelprotocol.RequestStop();

        public void SendRequestToTerminal(string request) => this.Lowlevelprotocol.SendRequestData(new Request().ConvertToHex(request), false);
    }
}
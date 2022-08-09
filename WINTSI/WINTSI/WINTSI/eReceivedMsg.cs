namespace Ingenico
{

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
		E_NO_ACK
	}
}
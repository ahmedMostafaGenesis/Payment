namespace Ingenico
{



	internal enum eStatus
	{
		E_IDLE,
		E_CONNECTED,
		E_WAIT_ACK,
		E_WAIT_RESPONSE,
		E_PARSE_RESULT,
		E_END_RESPONSE
	}
}
using System.Collections.Generic;

namespace Ingenico.Reports
{

	internal class RecentErrorReport
	{
		private FormatReport formatRER;

		private Dictionary<int, string> dicoPAR;

		public const string SYSTEM_ERROR_SEC = "01";

		public const string APP_ERROR_SEC = "02";

		public const string HOST_RESP_SEC = "03";

		public RecentErrorReport(FormatReport formatRER, Dictionary<int, string> dicoPAR)
		{
			this.formatRER = formatRER;
			this.dicoPAR = dicoPAR;
		}

		public void setReport()
		{
			formatRER.reportAddText(
				ReportTools.FormatDateTime(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_DATE), "-") + " " +
				ReportTools.FormatDateTime(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_TIME), "-") + " " +
				ReportTools.SimpleText(dicoPAR, Tags.TAG_ERR_RESULT_CODE), "");
			formatRER.reportAddText(ReportTools.SimpleText(dicoPAR, Tags.TAG_ERR_MESSAGE), "");
			formatRER.reportAddLine(1);
		}
	}
}
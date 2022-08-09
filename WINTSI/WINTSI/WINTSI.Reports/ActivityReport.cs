using System.Collections.Generic;

namespace Ingenico.Reports
{



	internal class ActivityReport
	{
		private FormatReport formatAR;

		private Dictionary<int, string> dicoPAR;

		public const string ACTIVITY_RECORD = "00";

		public ActivityReport(FormatReport formatAR, Dictionary<int, string> dicoPAR)
		{
			this.formatAR = formatAR;
			this.dicoPAR = dicoPAR;
		}

		public void setReport()
		{
			DataElement dataElement = new DataElement();
			formatAR.reportAddTexts(ReportTools.FormatDateTime(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_DATE), "-"),
				"", ReportTools.FormatDateTime(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_TIME), "-"), "", 50, 50);
			formatAR.reportAddText(
				dataElement.Get_DataListLabel(Tags.TAG_ACTIVITY_EVENT) + ": " +
				ReportTools.SimpleText(dicoPAR, Tags.TAG_ACTIVITY_EVENT), "");
			formatAR.reportAddText(
				dataElement.Get_DataListLabel(Tags.TAG_CLERK_ID) + ": " +
				ReportTools.SimpleText(dicoPAR, Tags.TAG_CLERK_ID), "");
			formatAR.reportAddText(
				dataElement.Get_DataListLabel(Tags.TAG_ACTIVITY_DATA) + ": " +
				ReportTools.SimpleText(dicoPAR, Tags.TAG_ACTIVITY_DATA), "");
			formatAR.reportAddCenterText("-------------------------");
			formatAR.reportAddLine(1);
		}
	}
}
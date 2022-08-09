using System.Collections.Generic;

namespace Ingenico.Reports
{



	internal class GeneralReport
	{
		private FormatReport formatGR;

		private Dictionary<int, string> dicoGR;

		public GeneralReport(FormatReport formatGR, Dictionary<int, string> dicoGR)
		{
			this.formatGR = formatGR;
			this.dicoGR = dicoGR;
		}

		public void setReport()
		{
			DataElement dataElement = new DataElement();
			formatGR.reportAddCenterText(ReportTools.SimpleText(dicoGR, Tags.TAG_HEADER_1));
			formatGR.reportAddCenterText(ReportTools.SimpleText(dicoGR, Tags.TAG_HEADER_2));
			formatGR.reportAddCenterText(ReportTools.SimpleText(dicoGR, Tags.TAG_HEADER_3));
			string text = ReportTools.FormatDateTime(ReportTools.SimpleText(dicoGR, Tags.TAG_TRX_DATE), "-");
			string text2 = ReportTools.FormatDateTime(ReportTools.SimpleText(dicoGR, Tags.TAG_TRX_TIME), ":");
			formatGR.reportAddTexts(text, "", text2, "", 50, 50);
			formatGR.reportAddTexts(ReportTools.SimpleText(dicoGR, Tags.TAG_TERMINAL_ID),
				dataElement.Get_DataListLabel(Tags.TAG_TERMINAL_ID), ReportTools.SimpleText(dicoGR, Tags.TAG_BATCH_NUM),
				dataElement.Get_DataListLabel(Tags.TAG_BATCH_NUM), 50, 50);
			string text3 = ReportTools.SimpleText(dicoGR, Tags.TAG_MERCHANT_ID);
			if (text3.Length != 0)
			{
				formatGR.reportAddCenterText("MID: " + text3);
			}
		}
	}
}
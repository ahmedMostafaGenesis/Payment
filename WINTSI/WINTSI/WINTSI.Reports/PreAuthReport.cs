using System.Collections.Generic;

namespace Ingenico.Reports
{



	internal class PreAuthReport
	{
		private FormatReport formatPAR;

		private Dictionary<int, string> dicoPAR;

		public const string OPEN_PRE_AUTH_REC = "10";

		public PreAuthReport(FormatReport formatPAR, Dictionary<int, string> dicoPAR)
		{
			this.formatPAR = formatPAR;
			this.dicoPAR = dicoPAR;
		}

		public void setReport()
		{
			string text = "";
			int num = ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoPAR, Tags.TAG_CARD_ENTRY_MODE));
			text = ((num <= -1)
				? ReportTools.SimpleText(dicoPAR, Tags.TAG_CARD_ENTRY_MODE)
				: ReportTools.GetTrxCardEntryMode(num));
			formatPAR.reportAddTexts(ReportTools.SimpleText(dicoPAR, Tags.TAG_ACCOUNT_NUM), "", text, "",
				ReportTools.SimpleText(dicoPAR, Tags.TAG_AUTH), "", 50, 25, 25);
			string text2 = "";
			int num2 = ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoPAR, Tags.TAG_CARD_TYPE));
			text2 = ((num2 <= -1)
				? ReportTools.SimpleText(dicoPAR, Tags.TAG_CARD_TYPE)
				: ReportTools.GetTrxCardType(num2));
			formatPAR.reportAddTexts(
				ReportTools.FormatDateTime(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_DATE), "-"), "",
				ReportTools.FormatDateTime(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_TIME), ":"), "", text2, "",
				ReportTools.SimpleText(dicoPAR, Tags.TAG_INVOICE), "");
			formatPAR.reportAddTexts(ReportTools.SimpleText(dicoPAR, Tags.TAG_TRX_REF), "",
				ReportTools.SimpleText(dicoPAR, Tags.TAG_CLERK_ID), "", getAmountData(Tags.TAG_TOTAL_AMNT), "", 30, 35,
				35);
			formatPAR.reportAddCenterText("-------------------------");
		}

		private string getAmountData(int Tag)
		{
			int num = ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoPAR, Tag));
			if (num == 16777215)
			{
				return ReportTools.SimpleText(dicoPAR, Tag);
			}

			return ReportTools.FormatAmount(num, "$");
		}
	}
}
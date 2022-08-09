using System.Collections.Generic;

namespace Ingenico.Reports
{
	
 

internal class DetailReport
{
	private FormatReport formatDR;

	private Dictionary<int, string> dicoDR;

	public static string CREDIT_DETAIL_REC = "10";

	public DetailReport(FormatReport formatDR, Dictionary<int, string> dicoDR)
	{
		this.formatDR = formatDR;
		this.dicoDR = dicoDR;
	}

	private string getAmountData(int Tag)
	{
		int num = ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoDR, Tag));
		if (num == 16777215)
		{
			return ReportTools.SimpleText(dicoDR, Tag);
		}
		return ReportTools.FormatAmount(num, "$");
	}

	public void setReport()
	{
		formatDR.reportAddTexts(ReportTools.SimpleText(dicoDR, Tags.TAG_ACCOUNT_NUM), "", getCardType(), "", getEntryMode(), "", 50, 25, 25);
		formatDR.reportAddTexts(ReportTools.SimpleText(dicoDR, Tags.TAG_TRX_TYPE), "", ReportTools.SimpleText(dicoDR, Tags.TAG_CLERK_ID), "", getAmountData(Tags.TAG_TRX_AMNT), "", 33, 33, 33);
		formatDR.reportAddTexts(ReportTools.SimpleText(dicoDR, Tags.TAG_TRX_REF), "", getAmountData(Tags.TAG_TIP_AMNT), "", 50, 50);
		formatDR.reportAddTexts(ReportTools.SimpleText(dicoDR, Tags.TAG_AUTH), "", getAmountData(Tags.TAG_SC_AMNT), "", getAmountData(Tags.TAG_CB_AMNT), "", 33, 33, 33);
		formatDR.reportAddTexts(ReportTools.SimpleText(dicoDR, Tags.TAG_INVOICE), "", getAmountData(Tags.TAG_TOTAL_AMNT), "", 50, 50);
		string text = ReportTools.FormatDateTime(ReportTools.SimpleText(dicoDR, Tags.TAG_TRX_DATE), "-");
		string text2 = ReportTools.FormatDateTime(ReportTools.SimpleText(dicoDR, Tags.TAG_TRX_TIME), ":");
		formatDR.reportAddTexts(text, "", text2, "", 50, 50);
		formatDR.reportAddCenterText("-------------------------");
	}

	private string getEntryMode()
	{
		int num = ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoDR, Tags.TAG_CARD_ENTRY_MODE));
		if (num > -1)
		{
			return ReportTools.GetTrxCardEntryMode(num);
		}
		return ReportTools.SimpleText(dicoDR, Tags.TAG_CARD_ENTRY_MODE);
	}

	private string getCardType()
	{
		int num = ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoDR, Tags.TAG_CARD_TYPE));
		if (num > -1)
		{
			return ReportTools.GetTrxCardType(num);
		}
		return ReportTools.SimpleText(dicoDR, Tags.TAG_CARD_TYPE);
	}
}
}
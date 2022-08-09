using System.Collections.Generic;

namespace Ingenico.Reports
{
	


internal class EmvLastTrxReport
{
	private FormatReport formatELTR;

	private Dictionary<int, string> dicoELTR;

	public EmvLastTrxReport(FormatReport formatELTR, Dictionary<int, string> dicoELTR)
	{
		this.formatELTR = formatELTR;
		this.dicoELTR = dicoELTR;
	}

	public void setReport()
	{
		if (ReportTools.SimpleText(dicoELTR, Tags.TAG_EMV_DATA).Length > 0)
		{
			formatELTR.reportAddTexts(ReportTools.SimpleText(dicoELTR, Tags.TAG_EMV_DATA), "", ReportTools.SimpleText(dicoELTR, Tags.TAG_EMV_VALUE), "", 50, 50);
		}
		else
		{
			formatELTR.reportAddTitle(ReportTools.SimpleText(dicoELTR, Tags.TAG_EMV_VALUE));
		}
	}
}
}
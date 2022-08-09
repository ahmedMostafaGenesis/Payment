using System.Collections.Generic;

namespace Ingenico.Reports
{



	internal class ParameterReport
	{
		private FormatReport formatPR;

		private Dictionary<int, string> dicoPR;

		public ParameterReport(FormatReport formatPR, Dictionary<int, string> dicoPR)
		{
			this.formatPR = formatPR;
			this.dicoPR = dicoPR;
		}

		public void setReport()
		{
			if (ReportTools.SimpleText(dicoPR, Tags.TAG_TER_SET_LABEL).Length > 0)
			{
				formatPR.reportAddTexts(ReportTools.SimpleText(dicoPR, Tags.TAG_TER_SET_LABEL), "",
					ReportTools.SimpleText(dicoPR, Tags.TAG_TER_SET_VALUE), "", 50, 50);
			}
			else
			{
				formatPR.reportAddTitle(ReportTools.SimpleText(dicoPR, Tags.TAG_TER_SET_VALUE));
			}
		}
	}
}
namespace Ingenico.Reports
{


	internal class FormatReport
	{
		private string ReportBody = "";

		public void startReport()
		{
			ReportBody = "<html><body>";
		}

		public void endReport()
		{
			ReportBody += "</body></html>";
		}

		public string getReport()
		{
			return ReportBody;
		}

		public void setReport(string report)
		{
			ReportBody = report;
		}

		public void reportAddTitle(string text)
		{
			if (text.Length > 0)
			{
				ReportBody = ReportBody + "<font face=\"Arial\" size=\"5\"> <center><b>" + text +
				             "</b></center></font>";
				ReportBody += "<BR>";
			}
		}

		public void reportAddLine(int numLine)
		{
			int num = 0;
			for (num = 0; num < numLine; num++)
			{
				ReportBody += "<BR>";
			}
		}

		public void reportAddText(string text, string description)
		{
			if (text.Length > 0)
			{
				description = checkdescription(text, description);
				ReportBody = ReportBody + "<font face=\"Arial\" size=\"3\">" + description + text + "</font>";
				ReportBody += "<BR>";
			}
		}

		public void reportAddCenterText(string text)
		{
			if (text.Length > 0)
			{
				ReportBody = ReportBody + "<font face=\"Arial\" size=\"3\"><center>" + text + "</center></font>";
				ReportBody += "<BR>";
			}
		}

		public void reportAddBoldText(string text)
		{
			if (text.Length > 0)
			{
				ReportBody = ReportBody + "<font face=\"Arial\" size=\"3\"><B>" + text + "</B></font>";
				ReportBody += "<BR>";
			}
		}

		public void reportAddTexts(string text1, string description1, string text2, string description2, int width1,
			int width2)
		{
			if (text1.Length > 0 || text2.Length > 0)
			{
				description1 = checkdescription(text1, description1);
				description2 = checkdescription(text2, description2);
				ReportBody += "<font face=\"Arial\" size=\"3\">";
				ReportBody = ReportBody + "<table width=100%><tr><td width=" + width1 + "%>" + description1 + text1 +
				             "</center></td><td width=" + width2 + "%><p align=\"right\">" + description2 + text2 +
				             "</p></td></tr></table>";
				ReportBody += "</font>";
			}
		}

		public void reportAddTexts(string text1, string description1, string text2, string description2, string text3,
			string description3, int width1, int width2, int width3)
		{
			if (text1.Length > 0 || text2.Length > 0 || text3.Length > 0)
			{
				description1 = checkdescription(text1, description1);
				description2 = checkdescription(text2, description2);
				description3 = checkdescription(text3, description3);
				ReportBody += "<font face=\"Arial\" size=\"3\">";
				ReportBody = ReportBody + "<table width=100%><tr><td width=" + width1 + "%>" + description1 + text1 +
				             "</td><td width=" + width2 + "%><center>" + description2 + text2 +
				             "</center></td><td width=" + width3 + "%><p align=\"right\">" + description3 + text3 +
				             "</p></td></tr></table>";
				ReportBody += "</font>";
			}
		}

		public void reportAddTexts(string text1, string description1, string text2, string description2, string text3,
			string description3, string text4, string description4)
		{
			if (text1.Length > 0 || text2.Length > 0 || text3.Length > 0 || text4.Length > 0)
			{
				description1 = checkdescription(text1, description1);
				description2 = checkdescription(text2, description2);
				description3 = checkdescription(text3, description3);
				description4 = checkdescription(text4, description4);
				ReportBody += "<font face=\"Arial\" size=\"3\">";
				ReportBody = ReportBody + "<table width=100%><tr><td>" + description1 + text1 + "</td><td><center>" +
				             description2 + text2 + "</center></td><td><center>" + description3 + text3 +
				             "</center></td><td><p align=\"right\">" + description4 + text4 + "</p></td></tr></table>";
				ReportBody += "</font>";
			}
		}

		private string checkdescription(string text, string description)
		{
			if (text.Length > 0)
			{
				if (description.Length > 0)
				{
					description += ": ";
				}
			}
			else
			{
				description = "";
			}

			return description;
		}
	}
}

using System.Collections.Generic;
using System.Windows.Forms;
using Ingenico.GUI;

namespace Ingenico.Reports
{
	

internal class HandleReports
{
	private int subRequest;

	private List<Dictionary<int, string>> listDictHR;

	private string requestHR;

	private HomeForm hForm;

	private string DEMO_MODE = "DEMO MODE";

	private string DETAIL_REPORT = "Detail Report";

	private string SUMMARY_REPORT = "Summary Report";

	private string EMV_LAST_TRX_REPORT = "EMV Last Transaction Report";

	private string CLERK_SUMMARY_REPORT = "Clerk Summary Report";

	private string PARAMETER_REPORT = "Parameter Report";

	private string PRE_AUTH_REPORT = "Pre-Auth Report";

	private string REC_ERR_REPORT = "Recent Error Report";

	private string ACTIVITY_REPORT = "Activity Report";

	private string CLERK_ID_LIST_REPORT = "Clerk Id List Report";

	private string EMV_PARAM_REPORT = "EMV Parameter Report";

	private string EMV_TERM_PARAM = "EMV Terminal Parameters";

	private string AID_EMV_PARAM = "AID EMV Parameters";

	private string CLESS_EMV_PARAM = "Cless EMV Parameters";

	private string MSD_ONLY_PARAM = "MSD Only Parameters";

	private string EMV_STAT_REPORT = "EMV Statistic Report";

	private string EMV_PUB_KEY_REPORT = "EMV Public Key Report";

	private string TERMINAL_INFO_REPORT = "Terminal Info Report";

	private string EMV_KEY_DATE_REPORT = "EMV KEY DATE REPORT";

	private string END_OF_REPORT = "END OF REPORT";

	private const string EMV_TERM_PARAM_ID = "15";

	private const string AID_EMV_PARAM_ID = "16";

	private const string CLESS_EMV_PARAM_ID = "17";

	private const string MSD_ONLY_PARAM_ID = "18";

	private const string RID = "RID";

	private const string KEY_INDEX = "KEY INDEX";

	private const string KEY_MODULUS = "KEY MODULUS";

	private const string KEY_EXPONENT = "KEY EXPONENT";

	public HandleReports(HomeForm hForm, List<Dictionary<int, string>> listDictHR, string requestHR, int subRequest)
	{
		this.hForm = hForm;
		this.listDictHR = listDictHR;
		this.requestHR = requestHR;
		this.subRequest = subRequest;
	}

	public void GenerateReport()
	{
		bool flag = true;
		try
		{
			FormatReport formatReport = new FormatReport();
			switch (requestHR)
			{
			case "30":
				detailedReport(formatReport);
				break;
			case "31":
				summaryReport(formatReport, listDictHR, independent: true);
				break;
			case "32":
				EmvLastTrxReport(formatReport);
				break;
			case "33":
				ClerkSummReport(formatReport);
				break;
			case "34":
				ParameterReport(formatReport);
				break;
			case "35":
				OpenPreAuthReport(formatReport);
				break;
			case "36":
				RecentErrReport(formatReport);
				break;
			case "37":
				ActivReport(formatReport);
				break;
			case "38":
				ClerkIdListReport(formatReport);
				break;
			case "39":
				EmvParamReport(formatReport);
				break;
			case "40":
				EMVStatisticReport(formatReport);
				break;
			case "41":
				EMVPublicKeyReport(formatReport);
				break;
			case "42":
				TerminalInfoReport(formatReport);
				break;
			case "43":
				EMVKeyDateReport(formatReport);
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				formatReport.reportAddLine(1);
				printTitle(formatReport, END_OF_REPORT);
				formatReport.endReport();
				new ReportViewer(formatReport.getReport()).ShowDialog(hForm);
			}
		}
		catch
		{
			MessageBox.Show("Impossible to display Report", "Report", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void detailedReport(FormatReport fr)
	{
		int num = 0;
		fr.startReport();
		bool num2 = generalReport(fr);
		printTitle(fr, DETAIL_REPORT);
		if (num2)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 2)
		{
			new DetailReport(fr, DetailReportSample()).setReport();
			for (num = 0; num < listDictHR.Count; num++)
			{
				if (listDictHR[num].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE) && listDictHR[num][DataElement.TAG_TRANS_RECORD_TYPE] == DetailReport.CREDIT_DETAIL_REC)
				{
					new DetailReport(fr, listDictHR[num]).setReport();
				}
			}
			summaryReport(fr, listDictHR, independent: false);
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private Dictionary<int, string> DetailReportSample()
	{
		return new Dictionary<int, string>
		{
			[Tags.TAG_ACCOUNT_NUM] = "ACCT#",
			[Tags.TAG_CARD_TYPE] = "CTYP",
			[Tags.TAG_CARD_ENTRY_MODE] = "EM",
			[Tags.TAG_TRX_TYPE] = "TT",
			[Tags.TAG_CLERK_ID] = "CLRK#",
			[Tags.TAG_TRX_AMNT] = "AMN",
			[Tags.TAG_TRX_REF] = "REF#",
			[Tags.TAG_TIP_AMNT] = "TIP",
			[Tags.TAG_AUTH] = "AUTH#",
			[Tags.TAG_SC_AMNT] = "FEE",
			[Tags.TAG_CB_AMNT] = "CSHBACK",
			[Tags.TAG_INVOICE] = "INV#/PO#",
			[Tags.TAG_TOTAL_AMNT] = "TOTAL",
			[Tags.TAG_TRX_DATE] = "YYMMDD",
			[Tags.TAG_TRX_TIME] = "HHMMSS"
		};
	}

	private void summaryReport(FormatReport fr, List<Dictionary<int, string>> listDictSum, bool independent)
	{
		List<Dictionary<int, string>> list = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list2 = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list3 = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list4 = new List<Dictionary<int, string>>();
		if (independent)
		{
			fr.startReport();
			bool num = generalReport(fr);
			printTitle(fr, SUMMARY_REPORT);
			if (num)
			{
				printTitle(fr, DEMO_MODE);
			}
		}
		if ((listDictHR.Count > 1 && independent) || (listDictHR.Count > 0 && !independent))
		{
			for (int i = 0; i < listDictSum.Count; i++)
			{
				if (listDictSum[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE))
				{
					if (listDictSum[i][DataElement.TAG_TRANS_RECORD_TYPE] == "11")
					{
						list.Add(listDictSum[i]);
					}
					else if (listDictSum[i][DataElement.TAG_TRANS_RECORD_TYPE] == "12")
					{
						list2.Add(listDictSum[i]);
					}
					else if (listDictSum[i][DataElement.TAG_TRANS_RECORD_TYPE] == "13")
					{
						list3.Add(listDictSum[i]);
					}
					else if (listDictSum[i][DataElement.TAG_TRANS_RECORD_TYPE] == "14")
					{
						list4.Add(listDictSum[i]);
					}
				}
			}
			if (list.Count > 0)
			{
				new SummaryReport(fr, null).printTTByRecType("11");
				for (int i = 0; i < list.Count; i++)
				{
					new SummaryReport(fr, list[i]).printRBByRecType("11");
				}
			}
			if (list2.Count > 0)
			{
				new SummaryReport(fr, null).printTTByRecType("12");
				for (int i = 0; i < list2.Count; i++)
				{
					new SummaryReport(fr, list2[i]).printRBByRecType("12");
				}
			}
			if (list3.Count > 0)
			{
				new SummaryReport(fr, null).printTTByRecType("13");
				for (int i = 0; i < list3.Count; i++)
				{
					new SummaryReport(fr, list3[i]).printRBByRecType("13");
				}
			}
			if (list4.Count > 0)
			{
				new SummaryReport(fr, null).printTTByRecType("14");
				for (int i = 0; i < list4.Count; i++)
				{
					new SummaryReport(fr, list4[i]).printRBByRecType("14");
				}
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void printTitle(FormatReport fr, string title)
	{
		fr.reportAddTitle(title);
		fr.reportAddLine(1);
	}

	private bool generalReport(FormatReport fr)
	{
		bool result = false;
		int i;
		for (i = 0; i < listDictHR.Count; i++)
		{
			if (listDictHR[i].ContainsKey(-1))
			{
				new GeneralReport(fr, listDictHR[i]).setReport();
				break;
			}
		}
		if (listDictHR.Count > 0 && listDictHR[i].ContainsKey(Tags.TAG_DEMO_MODE) && listDictHR[i][Tags.TAG_DEMO_MODE] == "1")
		{
			result = true;
		}
		return result;
	}

	private void EmvLastTrxReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, EMV_LAST_TRX_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 1)
		{
			for (int i = 0; i < listDictHR.Count; i++)
			{
				new EmvLastTrxReport(fr, listDictHR[i]).setReport();
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void ClerkSummReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, CLERK_SUMMARY_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 1)
		{
			List<List<Dictionary<int, string>>> list = new List<List<Dictionary<int, string>>>();
			List<Dictionary<int, string>> list2 = new List<Dictionary<int, string>>();
			string text = ReportTools.SimpleText(listDictHR[0], Tags.TAG_CLERK_ID);
			for (int i = 0; i < listDictHR.Count; i++)
			{
				if (text.Length > 0)
				{
					if (listDictHR[i].ContainsKey(Tags.TAG_CLERK_ID))
					{
						if (ReportTools.SimpleText(listDictHR[i], Tags.TAG_CLERK_ID) == text)
						{
							list2.Add(listDictHR[i]);
							listDictHR.Remove(listDictHR[i]);
							i--;
						}
						else
						{
							text = ReportTools.SimpleText(listDictHR[i], Tags.TAG_CLERK_ID);
							list.Add(list2);
							list2 = new List<Dictionary<int, string>>();
							i--;
						}
					}
				}
				else if (i < listDictHR.Count - 1)
				{
					text = ReportTools.SimpleText(listDictHR[i + 1], Tags.TAG_CLERK_ID);
				}
			}
			if (list2.Count > 0)
			{
				list.Add(list2);
			}
			DataElement dataElement = new DataElement();
			for (int i = 0; i < list.Count; i++)
			{
				fr.reportAddBoldText(dataElement.Get_DataListLabel(Tags.TAG_CLERK_ID) + ": " + ReportTools.SimpleText(list[i][0], Tags.TAG_CLERK_ID));
				fr.reportAddBoldText(ReportTools.SimpleText(list[i][0], Tags.TAG_CLERK_NAME));
				summaryReport(fr, list[i], independent: false);
				fr.reportAddLine(1);
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void ParameterReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, PARAMETER_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		for (int i = 0; i < listDictHR.Count; i++)
		{
			new ParameterReport(fr, listDictHR[i]).setReport();
		}
	}

	private void OpenPreAuthReport(FormatReport fr)
	{
		List<Dictionary<int, string>> list = new List<Dictionary<int, string>>();
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, PRE_AUTH_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 2)
		{
			new PreAuthReport(fr, PreAuthReportSample()).setReport();
			for (int i = 0; i < listDictHR.Count; i++)
			{
				if (listDictHR[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE))
				{
					if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == DetailReport.CREDIT_DETAIL_REC)
					{
						new PreAuthReport(fr, listDictHR[i]).setReport();
					}
					else if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "14")
					{
						list.Add(listDictHR[i]);
					}
				}
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					new SummaryReport(fr, list[i]).printRBByRecType("11");
				}
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private Dictionary<int, string> PreAuthReportSample()
	{
		return new Dictionary<int, string>
		{
			[Tags.TAG_ACCOUNT_NUM] = "ACCT#",
			[Tags.TAG_CARD_ENTRY_MODE] = "EM",
			[Tags.TAG_AUTH] = "AUTH#",
			[Tags.TAG_TRX_DATE] = "YYMMDD",
			[Tags.TAG_TRX_TIME] = "HHMMSS",
			[Tags.TAG_CARD_TYPE] = "CTYP",
			[Tags.TAG_INVOICE] = "INV#",
			[Tags.TAG_TRX_REF] = "REF#",
			[Tags.TAG_CLERK_ID] = "CLRK#",
			[Tags.TAG_TRX_AMNT] = "AUTH AMT"
		};
	}

	private void RecentErrReport(FormatReport fr)
	{
		List<Dictionary<int, string>> list = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list2 = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list3 = new List<Dictionary<int, string>>();
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, REC_ERR_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 1)
		{
			for (int i = 0; i < listDictHR.Count; i++)
			{
				if (listDictHR[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE))
				{
					if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "01")
					{
						list.Add(listDictHR[i]);
					}
					else if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "02")
					{
						list2.Add(listDictHR[i]);
					}
					else if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "03")
					{
						list3.Add(listDictHR[i]);
					}
				}
			}
			if (list.Count > 0)
			{
				fr.reportAddBoldText("System Error Messages");
				for (int i = 0; i < list.Count; i++)
				{
					new RecentErrorReport(fr, list[i]).setReport();
				}
			}
			if (list2.Count > 0)
			{
				fr.reportAddBoldText("Application Error Messages");
				for (int i = 0; i < list2.Count; i++)
				{
					new RecentErrorReport(fr, list2[i]).setReport();
				}
			}
			if (list3.Count > 0)
			{
				fr.reportAddBoldText("Host Response Messages");
				for (int i = 0; i < list3.Count; i++)
				{
					new RecentErrorReport(fr, list3[i]).setReport();
				}
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void ActivReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, ACTIVITY_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 1)
		{
			for (int i = 0; i < listDictHR.Count; i++)
			{
				if (listDictHR[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE) && listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "00")
				{
					new ActivityReport(fr, listDictHR[i]).setReport();
				}
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void ClerkIdListReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, CLERK_ID_LIST_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 1)
		{
			for (int i = 0; i < listDictHR.Count; i++)
			{
				if (listDictHR[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE))
				{
					string text = ReportTools.SimpleText(listDictHR[i], Tags.TAG_CLERK_ID);
					string text2 = ReportTools.SimpleText(listDictHR[i], Tags.TAG_CLERK_NAME);
					fr.reportAddTexts(text, "", text2, "", 50, 50);
				}
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void EmvParamReport(FormatReport fr)
	{
		List<Dictionary<int, string>> list = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list2 = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list3 = new List<Dictionary<int, string>>();
		List<Dictionary<int, string>> list4 = new List<Dictionary<int, string>>();
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, EMV_PARAM_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		for (int i = 0; i < listDictHR.Count; i++)
		{
			if (listDictHR[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE))
			{
				if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "15")
				{
					list.Add(listDictHR[i]);
				}
				else if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "16")
				{
					list2.Add(listDictHR[i]);
				}
				else if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "17")
				{
					list3.Add(listDictHR[i]);
				}
				else if (listDictHR[i][DataElement.TAG_TRANS_RECORD_TYPE] == "18")
				{
					list4.Add(listDictHR[i]);
				}
			}
		}
		DataElement dataElement = new DataElement();
		if (list.Count > 0)
		{
			fr.reportAddTitle(EMV_TERM_PARAM);
			for (int i = 0; i < list.Count; i++)
			{
				foreach (int item in new List<int>(list[i].Keys))
				{
					if (item != DataElement.TAG_TRANS_RECORD_TYPE)
					{
						fr.reportAddTexts(dataElement.Get_DataListLabel(item) ?? "", "", ReportTools.SimpleText(list[i], item), "", 50, 50);
					}
				}
				if (i < list.Count - 1)
				{
					fr.reportAddCenterText("---------------------------");
				}
			}
		}
		if (list2.Count > 0)
		{
			fr.reportAddTitle(AID_EMV_PARAM);
			for (int i = 0; i < list2.Count; i++)
			{
				foreach (int item2 in new List<int>(list2[i].Keys))
				{
					if (item2 != DataElement.TAG_TRANS_RECORD_TYPE)
					{
						fr.reportAddTexts(dataElement.Get_DataListLabel(item2) ?? "", "", ReportTools.SimpleText(list2[i], item2), "", 50, 50);
					}
				}
				if (i < list2.Count - 1)
				{
					fr.reportAddCenterText("---------------------------");
				}
			}
		}
		if (list3.Count > 0)
		{
			fr.reportAddTitle(CLESS_EMV_PARAM);
			for (int i = 0; i < list3.Count; i++)
			{
				foreach (int item3 in new List<int>(list3[i].Keys))
				{
					if (item3 != DataElement.TAG_TRANS_RECORD_TYPE)
					{
						fr.reportAddTexts(dataElement.Get_DataListLabel(item3) ?? "", "", ReportTools.SimpleText(list3[i], item3), "", 50, 50);
					}
				}
				if (i < list3.Count - 1)
				{
					fr.reportAddCenterText("---------------------------");
				}
			}
		}
		if (list4.Count <= 0)
		{
			return;
		}
		fr.reportAddTitle(MSD_ONLY_PARAM);
		for (int i = 0; i < list4.Count; i++)
		{
			foreach (int item4 in new List<int>(list4[i].Keys))
			{
				if (item4 != DataElement.TAG_TRANS_RECORD_TYPE)
				{
					fr.reportAddTexts(dataElement.Get_DataListLabel(item4) ?? "", "", ReportTools.SimpleText(list4[i], item4), "", 50, 50);
				}
			}
			if (i < list4.Count - 1)
			{
				fr.reportAddCenterText("---------------------------");
			}
		}
	}

	private void EMVStatisticReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, EMV_STAT_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		for (int i = 0; i < listDictHR.Count; i++)
		{
			new ParameterReport(fr, listDictHR[i]).setReport();
		}
	}

	private void EMVKeyDateReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, EMV_KEY_DATE_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR[0].Count > 1)
		{
			string text = ReportTools.SimpleText(listDictHR[0], Tags.TAG_KEY_EMV_DATE);
			if (text.Length == 8)
			{
				fr.reportAddCenterText(text.Substring(4, 2) + "/" + text.Substring(6, 2) + "/" + text.Substring(0, 4));
			}
		}
	}

	private void EMVPublicKeyReport(FormatReport fr)
	{
		string text = "";
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, EMV_PUB_KEY_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		if (listDictHR.Count > 1)
		{
			for (int i = 0; i < listDictHR.Count; i++)
			{
				if (listDictHR[i].ContainsKey(DataElement.TAG_TRANS_RECORD_TYPE))
				{
					int j = 1;
					fr.reportAddBoldText("RID");
					fr.reportAddText(ReportTools.SimpleText(listDictHR[i], Tags.TAG_RID), "");
					fr.reportAddBoldText("KEY INDEX");
					fr.reportAddText(ReportTools.SimpleText(listDictHR[i], Tags.TAG_KEY_INDEX), "");
					for (text = ReportTools.SimpleText(listDictHR[i], Tags.TAG_KEY_MODULUS); j * 36 + j < text.Length; j++)
					{
						text = text.Insert(j * 36 + j, " ");
					}
					fr.reportAddBoldText("KEY MODULUS");
					fr.reportAddText(text, "");
					fr.reportAddBoldText("KEY EXPONENT");
					fr.reportAddText(ReportTools.SimpleText(listDictHR[i], Tags.TAG_KEY_EXPONENT), "");
					fr.reportAddLine(2);
				}
			}
		}
		else
		{
			printNoRecordsFound(fr);
		}
	}

	private void TerminalInfoReport(FormatReport fr)
	{
		fr.startReport();
		bool num = generalReport(fr);
		printTitle(fr, TERMINAL_INFO_REPORT);
		if (num)
		{
			printTitle(fr, DEMO_MODE);
		}
		for (int i = 0; i < listDictHR.Count; i++)
		{
			new ParameterReport(fr, listDictHR[i]).setReport();
		}
	}

	private void printNoRecordsFound(FormatReport fr)
	{
		fr.reportAddCenterText("No Records");
		fr.reportAddCenterText("Found");
	}
}
}
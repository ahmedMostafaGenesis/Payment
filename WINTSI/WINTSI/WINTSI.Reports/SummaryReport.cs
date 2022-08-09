using System.Collections.Generic;

namespace Ingenico.Reports
{
	


internal class SummaryReport
{
	private FormatReport formatSR;

	private Dictionary<int, string> dicoSR;

	private static int BLOCK = 1;

	private static int CURRENT_TOTAL = 2;

	private static int GRAND_TOTAL = 3;

	private static string SPACES = "&nbsp;&nbsp;&nbsp;";

	public const string CREDIT_SUMMARY_REC = "11";

	public const string CREDIT_TOTAL_REC = "12";

	public const string DEBIT_SUMMARY_REC = "13";

	public const string GRAND_TOTAL_SUM_REC = "14";

	private static string str_Total = "Total";

	private static string str_Credit = "Credit";

	private static string str_Debit = "Debit";

	private static string str_Sales = "Sales";

	private static string str_Tips = SPACES + "Tips";

	private static string str_CashBack = SPACES + "CashBack";

	private static string str_TerminalFee = SPACES + "Terminal Fee";

	private static string str_Tax = SPACES + "Tax";

	private static string str_Returns = "Returns";

	private static string str_Voids = "Voids";

	private static string str_Grand_Total = "Grand Total";

	private static string str_Grand_Totals = "Grand Totals";

	private static string DEBIT = "1";

	private static string CREDIT = "2";

	public SummaryReport(FormatReport formatSR, Dictionary<int, string> dicoSR)
	{
		this.formatSR = formatSR;
		this.dicoSR = dicoSR;
	}

	public void printRBByRecType(string currentSummaryRecType)
	{
		switch (currentSummaryRecType)
		{
		case "11":
			printRecordBlock(BLOCK);
			break;
		case "13":
			printRecordBlock(BLOCK);
			break;
		case "12":
			printRecordBlock(CURRENT_TOTAL);
			break;
		case "14":
			printRecordBlock(GRAND_TOTAL);
			break;
		}
	}

	public void printTTByRecType(string currentSummaryRecType)
	{
		switch (currentSummaryRecType)
		{
		default:
			_ = currentSummaryRecType == "12";
			break;
		case "11":
			printTenderType(str_Credit);
			break;
		case "13":
			printTenderType(str_Debit);
			break;
		case "14":
			printTenderType(str_Grand_Totals);
			break;
		}
	}

	private void printTenderType(string TenderType)
	{
		formatSR.reportAddLine(1);
		formatSR.reportAddBoldText(TenderType.ToUpper());
	}

	private void printRecordBlock(int printType)
	{
		formatSR.reportAddLine(1);
		if (printType == BLOCK)
		{
			formatSR.reportAddText(ReportTools.SimpleText(dicoSR, Tags.TAG_CARD_DESCRIPTION).ToUpper(), "");
		}
		else if (printType == CURRENT_TOTAL)
		{
			formatSR.reportAddText(str_Total.ToUpper() + " " + str_Credit.ToUpper(), "");
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_SALE_AMNT).Length > 0)
		{
			string text = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_SALE_AMNT)), "$");
			formatSR.reportAddTexts(str_Sales, "", ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_SALE_COUNT), "", text, "", 40, 20, 40);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TIP_AMNT).Length > 0)
		{
			string text2 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TIP_AMNT)), "$");
			formatSR.reportAddTexts(str_Tips, "", text2, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_CB_AMNT).Length > 0 && ReportTools.SimpleText(dicoSR, Tags.TAG_ECR_TENDER_TYPE) != CREDIT)
		{
			string text3 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_CB_AMNT)), "$");
			formatSR.reportAddTexts(str_CashBack, "", text3, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_SC_AMNT).Length > 0 && ReportTools.SimpleText(dicoSR, Tags.TAG_ECR_TENDER_TYPE) != CREDIT)
		{
			string text4 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_SC_AMNT)), "$");
			formatSR.reportAddTexts(str_TerminalFee, "", text4, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TAX_AMNT).Length > 0 && ReportTools.SimpleText(dicoSR, Tags.TAG_ECR_TENDER_TYPE) != DEBIT)
		{
			string text5 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TAX_AMNT)), "$");
			formatSR.reportAddTexts(str_Tax, "", text5, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_RET_AMNT).Length > 0)
		{
			string text6 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_RET_AMNT)), "$");
			string text7 = ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_RET_COUNT);
			formatSR.reportAddTexts(str_Returns, "", text7, "", text6, "", 40, 20, 40);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_AMNT).Length > 0)
		{
			string text8 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_AMNT)), "$");
			formatSR.reportAddTexts(str_Voids, "", ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_COUNT), "", text8, "", 40, 20, 40);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_TIP_AMNT).Length > 0)
		{
			string text9 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_TIP_AMNT)), "$");
			formatSR.reportAddTexts(str_Tips, "", text9, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_CB_AMNT).Length > 0 && ReportTools.SimpleText(dicoSR, Tags.TAG_ECR_TENDER_TYPE) != CREDIT)
		{
			string text10 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_CB_AMNT)), "$");
			formatSR.reportAddTexts(str_CashBack, "", text10, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_SC_AMNT).Length > 0 && ReportTools.SimpleText(dicoSR, Tags.TAG_ECR_TENDER_TYPE) != CREDIT)
		{
			string text11 = ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_SC_AMNT)), "$");
			formatSR.reportAddTexts(str_TerminalFee, "", text11, "", 50, 50);
		}
		if (ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_TAX_AMNT).Length > 0 && ReportTools.SimpleText(dicoSR, Tags.TAG_ECR_TENDER_TYPE) != DEBIT)
		{
			ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_TAX_AMNT)), "$");
			formatSR.reportAddTexts(str_Tax, "", ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_VOID_TAX_AMNT), "", 50, 50);
		}
		if (printType != GRAND_TOTAL)
		{
			formatSR.reportAddTexts(str_Total, "", ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TOT_COUNT), "", ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TOT_AMNT)), "$"), "", 40, 20, 40);
		}
		else
		{
			formatSR.reportAddTexts(str_Grand_Total, "", ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TOT_COUNT), "", ReportTools.FormatAmount(ReportTools.ParseStringToInt(ReportTools.SimpleText(dicoSR, Tags.TAG_BATCH_TOT_AMNT)), "$"), "", 40, 20, 40);
		}
	}
}
}

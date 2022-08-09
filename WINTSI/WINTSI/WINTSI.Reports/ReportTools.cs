using System;
using System.Collections.Generic;

namespace Ingenico.Reports
{
	internal static class ReportTools
	{
		private const int MagneticStripe = 0;

		private const int Insert = 1;

		private const int Tap = 2;

		private const int ManualEntry = 3;

		private const int ChipFallBackToSwipe = 4;

		private const int ChipFallBackToManual = 5;

		private const int CardNotPresentManual = 6;

		private const int Debit = 0;

		private const int Visa = 1;

		private const int MasterCard = 2;

		private const int Amex = 3;

		private const int DinersClub = 4;

		private const int DiscoverCard = 5;

		private const int Jcb = 6;

		private const int UnionPayCard = 7;

		private const int OtherCreditCard = 8;

		private const int GiftCard = 9;

		private const int Cash = 10;

		private const int EbtFoodStamp = 11;

		private const int EbtCashBenefit = 12;

		private const int Approved = 0;

		private const int PartialApproved = 1;

		private const int DeclinedByAcquirer = 10;

		private const int ComesError = 11;

		private const int CancelledByUser = 12;

		private const int TimedOutOnUserInput = 13;

		private const int TrxNotCompleted = 14;

		private const int BatchEmpty = 15;

		private const int DeclinedByMerchant = 16;

		private const int TtAllCardTypes = 0;

		private const int TtDebit = 1;

		private const int TtCredit = 2;

		private const int TtGift = 3;

		private const int TtEbt = 4;

		private const int TtCash = 5;

		public static string GetTrxCardEntryMode(int id)
		{
			var result = id switch
			{
				0 => "S",
				1 => "CP",
				2 => "CL",
				3 => "MCP",
				4 => "CS",
				5 => "CM",
				6 => "MCNP",
				_ => ""
			};
			return result;
		}

		public static string GetTrxCardType(int id)
		{
			var result = id switch
			{
				0 => "Debit",
				1 => "Visa",
				2 => "MC",
				3 => "Amex",
				4 => "Din",
				5 => "Disc",
				6 => "JCB",
				7 => "UPC",
				8 => "OCC",
				9 => "GC",
				10 => "Cash",
				11 => "EFS",
				12 => "ECB",
				_ => ""
			};
			return result;
		}

		public static string GetTrxStatusLabel(int id)
		{
			var result = id switch
			{
				0 => "Approved",
				1 => "Partial Approved",
				10 => "Declined By Acquirer",
				11 => "Communication Error",
				12 => "Cancelled By User",
				13 => "Timed Out On User Input",
				14 => "Transaction Not Completed",
				15 => "Batch Empty",
				16 => "Declined By Merchant",
				_ => ""
			};
			return result;
		}

		public static string GetTrxTenderType(int id)
		{
			var result = id switch
			{
				0 => "All Card Types",
				1 => "Debit",
				2 => "Credit",
				3 => "Gift",
				4 => "EBT",
				5 => "Cash",
				_ => ""
			};
			return result;
		}

		public static string FormatAmount(int amount, string currency)
		{
			var text2 = " ";
			if (amount < 0)
			{
				amount = Math.Abs(amount);
				text2 = "-";
			}

			if (amount % 100 < 10)
			{
				return text2 + currency + amount / 100 + ",0" + amount % 100;
			}

			return text2 + currency + amount / 100 + "," + amount % 100;
		}

		public static int ParseStringToInt(string text)
		{
			try
			{
				return int.Parse(text);
			}
			catch
			{
				return 16777215;
			}
		}

		public static string SimpleText(Dictionary<int, string> dicoDr, int tags)
		{
			return dicoDr.ContainsKey(tags) ? dicoDr[tags] : "";
		}

		public static string FormatDateTime(string text, string sep)
		{
			var result = "";
			if (text.Length <= 5) return result;
			result = text.Substring(0, 2);
			result += sep;
			result += text.Substring(2, 2);
			result += sep;
			result += text.Substring(4, 2);
			return result;
		}
	}
}
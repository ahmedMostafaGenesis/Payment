using System;

namespace Ingenico
{
	public class Request
{
	public const string FS = "1C";

	public string tnxCode;

	public string amount;

	public string tenderType;

	public string clerkId;

	public string invoiceNumber;

	public string authorNumber;

	public string origSeqNumber;

	public string origRefNumber;

	public int closeBatch;

	public string traceNum;

	public string forcedUP;

	public string reprintType;

	public int parameterType;

	public string custRef;

	public string Dcc;

	public string refNum;

	public string PAN;

	public string tranType;

	public string specificData;

	public string formattedRcpt;

	public string RcptName;

	public string merchURL;

	public string merchID;

	public string filterCateg;

	public string encryptReq;

	public int vasMode;

	public string vasEccKey;

	public string merchIndex;

	public string finalAmount;

	public string ConvertToHex(string asciiString)
	{
		string text = "";
		for (int i = 0; i < asciiString.Length; i++)
		{
			text += $"{Convert.ToUInt32(((int)asciiString[i]).ToString()):x2}";
		}
		return text;
	}

	public string FormatField(string asciiString)
	{
		string text = "1C";
		if (asciiString.Length != 0)
		{
			text += ConvertToHex(asciiString);
		}
		return text;
	}

	public string BuildRequest(string szTnxType, int tag, int value)
	{
		string text = "";
		DataElement dataElement = new DataElement();
		text += ConvertToHex(szTnxType);
		if (tag >= 0)
		{
			string text2 = dataElement.FormatTagThreeDigit(tag);
			text += FormatField(text2 + value);
		}
		return text;
	}

	public string BuildRequest()
	{
		string text = "";
		DataElement dataElement = new DataElement();
		text += ConvertToHex(dataElement.Get_TransTypeTag(tnxCode));
		if (!string.IsNullOrEmpty(amount))
		{
			string text2 = dataElement.FormatTagThreeDigit(1);
			text += FormatField(text2 + amount);
		}
		if (tenderType != null && tenderType != "None" && tenderType != "")
		{
			string text3 = dataElement.FormatTagThreeDigit(2);
			text += FormatField(text3 + dataElement.Get_TenderTypeTag(tenderType));
		}
		if (closeBatch > 0)
		{
			string text4 = dataElement.FormatTagThreeDigit(7);
			text += FormatField(text4 + closeBatch);
		}
		if (!string.IsNullOrEmpty(clerkId))
		{
			string text5 = dataElement.FormatTagThreeDigit(3);
			text += FormatField(text5 + clerkId);
		}
		if (!string.IsNullOrEmpty(invoiceNumber))
		{
			string text6 = dataElement.FormatTagThreeDigit(4);
			text += FormatField(text6 + invoiceNumber);
		}
		if (!string.IsNullOrEmpty(authorNumber))
		{
			string text7 = dataElement.FormatTagThreeDigit(5);
			text += FormatField(text7 + authorNumber);
		}
		if (!string.IsNullOrEmpty(origSeqNumber))
		{
			string text8 = dataElement.FormatTagThreeDigit(112);
			text += FormatField(text8 + origSeqNumber);
		}
		if (!string.IsNullOrEmpty(origRefNumber))
		{
			string text9 = dataElement.FormatTagThreeDigit(6);
			text += FormatField(text9 + origRefNumber);
		}
		if (!string.IsNullOrEmpty(traceNum))
		{
			string text10 = dataElement.FormatTagThreeDigit(20);
			text += FormatField(text10 + traceNum);
		}
		if (!string.IsNullOrEmpty(reprintType))
		{
			string text11 = dataElement.FormatTagThreeDigit(50);
			text += FormatField(text11 + reprintType);
		}
		if (parameterType > 0)
		{
			string text12 = dataElement.FormatTagThreeDigit(51);
			text += FormatField(text12 + parameterType);
		}
		if (!string.IsNullOrEmpty(forcedUP))
		{
			string text13 = dataElement.FormatTagThreeDigit(8);
			text += FormatField(text13 + forcedUP);
		}
		if (!string.IsNullOrEmpty(custRef))
		{
			string text14 = dataElement.FormatTagThreeDigit(10);
			text += FormatField(text14 + custRef);
		}
		if (!string.IsNullOrEmpty(Dcc))
		{
			string text15 = dataElement.FormatTagThreeDigit(9);
			text += FormatField(text15 + Dcc);
		}
		if (!string.IsNullOrEmpty(refNum))
		{
			string text16 = dataElement.FormatTagThreeDigit(11);
			text += FormatField(text16 + refNum);
		}
		if (!string.IsNullOrEmpty(PAN))
		{
			string text17 = dataElement.FormatTagThreeDigit(12);
			text += FormatField(text17 + PAN);
		}
		if (!string.IsNullOrEmpty(tranType))
		{
			string text18 = dataElement.FormatTagThreeDigit(13);
			text += FormatField(text18 + tranType);
		}
		if (!string.IsNullOrEmpty(specificData))
		{
			string text19 = dataElement.FormatTagThreeDigit(14);
			text += FormatField(text19 + specificData);
		}
		if (!string.IsNullOrEmpty(formattedRcpt))
		{
			string text20 = dataElement.FormatTagThreeDigit(15);
			text += FormatField(text20 + formattedRcpt);
		}
		if (!string.IsNullOrEmpty(RcptName))
		{
			string text21 = dataElement.FormatTagThreeDigit(52);
			text += FormatField(text21 + RcptName);
		}
		if (!string.IsNullOrEmpty(merchURL))
		{
			string text22 = dataElement.FormatTagThreeDigit(30);
			text += FormatField(text22 + merchURL);
		}
		if (!string.IsNullOrEmpty(merchID))
		{
			string text23 = dataElement.FormatTagThreeDigit(31);
			text += FormatField(text23 + merchID);
		}
		if (!string.IsNullOrEmpty(filterCateg))
		{
			string text24 = dataElement.FormatTagThreeDigit(32);
			text += FormatField(text24 + filterCateg);
		}
		if (!string.IsNullOrEmpty(encryptReq))
		{
			string text25 = dataElement.FormatTagThreeDigit(33);
			text += FormatField(text25 + encryptReq);
		}
		if (!string.IsNullOrEmpty(vasEccKey))
		{
			string text26 = dataElement.FormatTagThreeDigit(36);
			text += FormatField(text26 + vasEccKey);
		}
		if (vasMode >= 0)
		{
			string text27 = dataElement.FormatTagThreeDigit(34);
			text += FormatField(text27 + vasMode);
		}
		if (!string.IsNullOrEmpty(merchIndex))
		{
			string text28 = dataElement.FormatTagThreeDigit(35);
			text += FormatField(text28 + merchIndex);
		}
		if (!string.IsNullOrEmpty(finalAmount))
		{
			string text28 = dataElement.FormatTagThreeDigit(16);
			text += FormatField(text28 + finalAmount);
		}
		return text;
	}

	public string BuildPrintResponse(string szEcrPrintingStatus)
	{
		new DataElement();
		return string.Concat("" + ConvertToHex("99"), ConvertToHex(szEcrPrintingStatus));
	}
}
}
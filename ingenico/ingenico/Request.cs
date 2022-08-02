using System;

namespace ingenico
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
        string hex = "";
        foreach (int num in asciiString)
          hex += string.Format("{0:x2}", (object) Convert.ToUInt32(num.ToString()));
        return hex;
      }

      public string FormatField(string asciiString)
      {
        string str = "1C";
        if (asciiString.Length != 0)
          str += this.ConvertToHex(asciiString);
        return str;
      }

      public string BuildRequest(string szTnxType, int tag, int value)
      {
        string str1 = "";
        DataElement dataElement = new DataElement();
        string str2 = str1 + this.ConvertToHex(szTnxType);
        if (tag >= 0)
        {
          string str3 = dataElement.FormatTagThreeDigit(tag);
          str2 += this.FormatField(str3 + value.ToString());
        }
        return str2;
      }

      public string BuildRequest()
      {
        string str1 = "";
        DataElement dataElement = new DataElement();
        string str2 = str1 + this.ConvertToHex(dataElement.Get_TransTypeTag(this.tnxCode));
        if (!string.IsNullOrEmpty(this.amount))
        {
          string str3 = dataElement.FormatTagThreeDigit(1);
          str2 += this.FormatField(str3 + this.amount);
        }
        if (this.tenderType != null && this.tenderType != "None" && this.tenderType != "")
        {
          string str4 = dataElement.FormatTagThreeDigit(2);
          str2 += this.FormatField(str4 + dataElement.Get_TenderTypeTag(this.tenderType));
        }
        if (this.closeBatch > 0)
        {
          string str5 = dataElement.FormatTagThreeDigit(7);
          str2 += this.FormatField(str5 + this.closeBatch.ToString());
        }
        if (!string.IsNullOrEmpty(this.clerkId))
        {
          string str6 = dataElement.FormatTagThreeDigit(3);
          str2 += this.FormatField(str6 + this.clerkId);
        }
        if (!string.IsNullOrEmpty(this.invoiceNumber))
        {
          string str7 = dataElement.FormatTagThreeDigit(4);
          str2 += this.FormatField(str7 + this.invoiceNumber);
        }
        if (!string.IsNullOrEmpty(this.authorNumber))
        {
          string str8 = dataElement.FormatTagThreeDigit(5);
          str2 += this.FormatField(str8 + this.authorNumber);
        }
        if (!string.IsNullOrEmpty(this.origSeqNumber))
        {
          string str9 = dataElement.FormatTagThreeDigit(112);
          str2 += this.FormatField(str9 + this.origSeqNumber);
        }
        if (!string.IsNullOrEmpty(this.origRefNumber))
        {
          string str10 = dataElement.FormatTagThreeDigit(6);
          str2 += this.FormatField(str10 + this.origRefNumber);
        }
        if (!string.IsNullOrEmpty(this.traceNum))
        {
          string str11 = dataElement.FormatTagThreeDigit(20);
          str2 += this.FormatField(str11 + this.traceNum);
        }
        if (!string.IsNullOrEmpty(this.reprintType))
        {
          string str12 = dataElement.FormatTagThreeDigit(50);
          str2 += this.FormatField(str12 + this.reprintType);
        }
        if (this.parameterType > 0)
        {
          string str13 = dataElement.FormatTagThreeDigit(51);
          str2 += this.FormatField(str13 + this.parameterType.ToString());
        }
        if (!string.IsNullOrEmpty(this.forcedUP))
        {
          string str14 = dataElement.FormatTagThreeDigit(8);
          str2 += this.FormatField(str14 + this.forcedUP);
        }
        if (!string.IsNullOrEmpty(this.custRef))
        {
          string str15 = dataElement.FormatTagThreeDigit(10);
          str2 += this.FormatField(str15 + this.custRef);
        }
        if (!string.IsNullOrEmpty(this.Dcc))
        {
          string str16 = dataElement.FormatTagThreeDigit(9);
          str2 += this.FormatField(str16 + this.Dcc);
        }
        if (!string.IsNullOrEmpty(this.refNum))
        {
          string str17 = dataElement.FormatTagThreeDigit(11);
          str2 += this.FormatField(str17 + this.refNum);
        }
        if (!string.IsNullOrEmpty(this.PAN))
        {
          string str18 = dataElement.FormatTagThreeDigit(12);
          str2 += this.FormatField(str18 + this.PAN);
        }
        if (!string.IsNullOrEmpty(this.tranType))
        {
          string str19 = dataElement.FormatTagThreeDigit(13);
          str2 += this.FormatField(str19 + this.tranType);
        }
        if (!string.IsNullOrEmpty(this.specificData))
        {
          string str20 = dataElement.FormatTagThreeDigit(14);
          str2 += this.FormatField(str20 + this.specificData);
        }
        if (!string.IsNullOrEmpty(this.formattedRcpt))
        {
          string str21 = dataElement.FormatTagThreeDigit(15);
          str2 += this.FormatField(str21 + this.formattedRcpt);
        }
        if (!string.IsNullOrEmpty(this.RcptName))
        {
          string str22 = dataElement.FormatTagThreeDigit(52);
          str2 += this.FormatField(str22 + this.RcptName);
        }
        if (!string.IsNullOrEmpty(this.merchURL))
        {
          string str23 = dataElement.FormatTagThreeDigit(30);
          str2 += this.FormatField(str23 + this.merchURL);
        }
        if (!string.IsNullOrEmpty(this.merchID))
        {
          string str24 = dataElement.FormatTagThreeDigit(31);
          str2 += this.FormatField(str24 + this.merchID);
        }
        if (!string.IsNullOrEmpty(this.filterCateg))
        {
          string str25 = dataElement.FormatTagThreeDigit(32);
          str2 += this.FormatField(str25 + this.filterCateg);
        }
        if (!string.IsNullOrEmpty(this.encryptReq))
        {
          string str26 = dataElement.FormatTagThreeDigit(33);
          str2 += this.FormatField(str26 + this.encryptReq);
        }
        if (!string.IsNullOrEmpty(this.vasEccKey))
        {
          string str27 = dataElement.FormatTagThreeDigit(36);
          str2 += this.FormatField(str27 + this.vasEccKey);
        }
        if (this.vasMode >= 0)
        {
          string str28 = dataElement.FormatTagThreeDigit(34);
          str2 += this.FormatField(str28 + this.vasMode.ToString());
        }
        if (!string.IsNullOrEmpty(this.merchIndex))
        {
          string str29 = dataElement.FormatTagThreeDigit(35);
          str2 += this.FormatField(str29 + this.merchIndex);
        }
        if (!string.IsNullOrEmpty(this.finalAmount))
        {
          string str30 = dataElement.FormatTagThreeDigit(16);
          str2 += this.FormatField(str30 + this.finalAmount);
        }
        return str2;
      }

      public string BuildPrintResponse(string szEcrPrintingStatus)
      {
        DataElement dataElement = new DataElement();
        return "" + this.ConvertToHex("99") + this.ConvertToHex(szEcrPrintingStatus);
      }
    }
}
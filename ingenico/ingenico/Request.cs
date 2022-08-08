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
          str += ConvertToHex(asciiString);
        return str;
      }

      public string BuildRequest(string szTnxType, int tag, int value)
      {
        string str1 = "";
        DataElement dataElement = new DataElement();
        string str2 = str1 + ConvertToHex(szTnxType);
        if (tag >= 0)
        {
          string str3 = dataElement.FormatTagThreeDigit(tag);
          str2 += FormatField(str3 + value.ToString());
        }
        return str2;
      }

      public string BuildRequest()
      {
        var str1 = "";
        var dataElement = new DataElement();
        var str2 = str1 + ConvertToHex(dataElement.Get_TransTypeTag(tnxCode));
        if (!string.IsNullOrEmpty(amount))
        {
          var str3 = dataElement.FormatTagThreeDigit(1);
          str2 += FormatField(str3 + amount);
        }
        if (tenderType != null && tenderType != "None" && tenderType != "")
        {
          var str4 = dataElement.FormatTagThreeDigit(2);
          str2 += FormatField(str4 + dataElement.Get_TenderTypeTag(tenderType));
        }
        if (closeBatch > 0)
        {
          var str5 = dataElement.FormatTagThreeDigit(7);
          str2 += FormatField(str5 + closeBatch);
        }
        if (!string.IsNullOrEmpty(clerkId))
        {
          var str6 = dataElement.FormatTagThreeDigit(3);
          str2 += FormatField(str6 + clerkId);
        }
        if (!string.IsNullOrEmpty(invoiceNumber))
        {
          var str7 = dataElement.FormatTagThreeDigit(4);
          str2 += FormatField(str7 + invoiceNumber);
        }
        if (!string.IsNullOrEmpty(authorNumber))
        {
          var str8 = dataElement.FormatTagThreeDigit(5);
          str2 += FormatField(str8 + authorNumber);
        }
        if (!string.IsNullOrEmpty(origSeqNumber))
        {
          var str9 = dataElement.FormatTagThreeDigit(112);
          str2 += FormatField(str9 + origSeqNumber);
        }
        if (!string.IsNullOrEmpty(origRefNumber))
        {
          var str10 = dataElement.FormatTagThreeDigit(6);
          str2 += FormatField(str10 + origRefNumber);
        }
        if (!string.IsNullOrEmpty(traceNum))
        {
          var str11 = dataElement.FormatTagThreeDigit(20);
          str2 += FormatField(str11 + traceNum);
        }
        if (!string.IsNullOrEmpty(reprintType))
        {
          var str12 = dataElement.FormatTagThreeDigit(50);
          str2 += FormatField(str12 + reprintType);
        }
        if (parameterType > 0)
        {
          var str13 = dataElement.FormatTagThreeDigit(51);
          str2 += FormatField(str13 + parameterType.ToString());
        }
        if (!string.IsNullOrEmpty(forcedUP))
        {
          var str14 = dataElement.FormatTagThreeDigit(8);
          str2 += FormatField(str14 + forcedUP);
        }
        if (!string.IsNullOrEmpty(custRef))
        {
          var str15 = dataElement.FormatTagThreeDigit(10);
          str2 += FormatField(str15 + custRef);
        }
        if (!string.IsNullOrEmpty(Dcc))
        {
          var str16 = dataElement.FormatTagThreeDigit(9);
          str2 += FormatField(str16 + Dcc);
        }
        if (!string.IsNullOrEmpty(refNum))
        {
          var str17 = dataElement.FormatTagThreeDigit(11);
          str2 += FormatField(str17 + refNum);
        }
        if (!string.IsNullOrEmpty(PAN))
        {
          var str18 = dataElement.FormatTagThreeDigit(12);
          str2 += FormatField(str18 + PAN);
        }
        if (!string.IsNullOrEmpty(tranType))
        {
          var str19 = dataElement.FormatTagThreeDigit(13);
          str2 += FormatField(str19 + tranType);
        }
        if (!string.IsNullOrEmpty(specificData))
        {
          var str20 = dataElement.FormatTagThreeDigit(14);
          str2 += FormatField(str20 + specificData);
        }
        if (!string.IsNullOrEmpty(formattedRcpt))
        {
          var str21 = dataElement.FormatTagThreeDigit(15);
          str2 += FormatField(str21 + formattedRcpt);
        }
        if (!string.IsNullOrEmpty(RcptName))
        {
          var str22 = dataElement.FormatTagThreeDigit(52);
          str2 += FormatField(str22 + RcptName);
        }
        if (!string.IsNullOrEmpty(merchURL))
        {
          var str23 = dataElement.FormatTagThreeDigit(30);
          str2 += FormatField(str23 + merchURL);
        }
        if (!string.IsNullOrEmpty(merchID))
        {
          var str24 = dataElement.FormatTagThreeDigit(31);
          str2 += FormatField(str24 + merchID);
        }
        if (!string.IsNullOrEmpty(filterCateg))
        {
          var str25 = dataElement.FormatTagThreeDigit(32);
          str2 += FormatField(str25 + filterCateg);
        }
        if (!string.IsNullOrEmpty(encryptReq))
        {
          var str26 = dataElement.FormatTagThreeDigit(33);
          str2 += FormatField(str26 + encryptReq);
        }
        if (!string.IsNullOrEmpty(vasEccKey))
        {
          var str27 = dataElement.FormatTagThreeDigit(36);
          str2 += FormatField(str27 + vasEccKey);
        }
        if (vasMode >= 0)
        {
          var str28 = dataElement.FormatTagThreeDigit(34);
          str2 += FormatField(str28 + vasMode);
        }
        if (!string.IsNullOrEmpty(merchIndex))
        {
          var str29 = dataElement.FormatTagThreeDigit(35);
          str2 += FormatField(str29 + merchIndex);
        }
        if (!string.IsNullOrEmpty(finalAmount))
        {
          var str30 = dataElement.FormatTagThreeDigit(16);
          str2 += FormatField(str30 + finalAmount);
        }
        return str2;
      }

      public string BuildPrintResponse(string szEcrPrintingStatus)
      {
        DataElement dataElement = new DataElement();
        return "" + ConvertToHex("99") + ConvertToHex(szEcrPrintingStatus);
      }
    }
}
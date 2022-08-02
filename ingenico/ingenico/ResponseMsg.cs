using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ingenico
{
    public class ResponseMsg
    {
        public List<Dictionary<int, string>> ParseReceivedResponse(
      byte[] ResponseBuffer,
      bool bPrintResp,
      Encoding encodeType)
    {
      int num = 0;
      byte separteur = 28;
      bool flag = false;
      Dictionary<int, string> dictionary1 = new Dictionary<int, string>();
      List<Dictionary<int, string>> receivedResponse = new List<Dictionary<int, string>>();
      List<byte[]> numArrayList1 = this.SplitResponseBuffer(ResponseBuffer, separteur, 0);
      if (!bPrintResp)
      {
        try
        {
          string str1 = encodeType.GetString(numArrayList1[0], 2, 1);
          dictionary1.Add(-1, str1);
          string str2 = encodeType.GetString(numArrayList1[0], 0, 2);
          dictionary1.Add(new DataElement().Get_TransStatusTag(), str2);
        }
        catch (Exception ex)
        {
          Trace.WriteLine("Invalid transaction status: " + ex.Message);
        }
        num = 1;
      }
      for (int index1 = num; index1 < numArrayList1.Count; ++index1)
      {
        if (numArrayList1[index1].Length >= 3)
        {
          string s1 = encodeType.GetString(numArrayList1[index1], 0, 3);
          try
          {
            int key1 = int.Parse(s1);
            if (key1 == Tags.TAG_ECR_REQUESTED_FORMATTED_RECEIPT || key1 == Tags.TAG_VAS_RESPONSE_DATA)
              encodeType = Encoding.UTF8;
            if (key1 != new DataElement().TAG_TRANS_RECORD)
            {
              string str3 = encodeType.GetString(numArrayList1[index1], 3, numArrayList1[index1].Length - 3);
              string str4;
              if (dictionary1.TryGetValue(key1, out str4))
              {
                str4 += str3;
                dictionary1[key1] = str4;
              }
              else
                dictionary1[key1] = str3;
            }
            else
            {
              if (!flag)
              {
                string str = encodeType.GetString(numArrayList1[index1], 7, 2);
                dictionary1[key1] = str;
                flag = true;
              }
              List<byte[]> numArrayList2 = this.SplitResponseBuffer(numArrayList1[index1], (byte) 29, 9);
              Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
              string str5 = encodeType.GetString(numArrayList1[index1], 3, 2);
              dictionary2[DataElement.TAG_TRANS_RECORD_TYPE] = str5;
              for (int index2 = 0; index2 < numArrayList2.Count; ++index2)
              {
                string s2 = encodeType.GetString(numArrayList2[index2], 0, 3);
                try
                {
                  int key2 = int.Parse(s2);
                  if (key2 == Tags.TAG_VAS_RESPONSE_DATA)
                    encodeType = Encoding.UTF8;
                  string str6 = encodeType.GetString(numArrayList2[index2], 3, numArrayList2[index2].Length - 3);
                  dictionary2[key2] = str6;
                }
                catch (Exception ex)
                {
                  Trace.WriteLine("Invalid tag value: " + ex.Message);
                }
              }
              receivedResponse.Add(dictionary2);
            }
          }
          catch (Exception ex)
          {
            Trace.WriteLine("Invalid tag value: " + ex.Message);
          }
        }
      }
      if (dictionary1.Count != 0)
        receivedResponse.Add(dictionary1);
      return receivedResponse;
    }

    public List<byte[]> SplitResponseBuffer(
      byte[] ResponseBuffer,
      byte separteur,
      int indexStartField)
    {
      int length = 0;
      List<byte[]> numArrayList = new List<byte[]>();
      int index = indexStartField;
      while (index < ResponseBuffer.Length)
      {
        if ((int) ResponseBuffer[index] == (int) separteur || index == ResponseBuffer.Length - 1)
        {
          byte[] destinationArray;
          if (index == ResponseBuffer.Length - 1 && (int) ResponseBuffer[index] != (int) separteur)
          {
            destinationArray = new byte[length + 1];
            Array.Copy((Array) ResponseBuffer, index - length, (Array) destinationArray, 0, length + 1);
          }
          else
          {
            destinationArray = new byte[length];
            Array.Copy((Array) ResponseBuffer, index - length, (Array) destinationArray, 0, length);
          }
          if (destinationArray.Length != 0)
            numArrayList.Add(destinationArray);
          length = -1;
        }
        ++index;
        ++length;
      }
      return numArrayList;
    }

    public Dictionary<string, object> extractResponse(byte[] data, int size)
    {
      Dictionary<string, object> response = (Dictionary<string, object>) null;
      int num1 = 0;
      int num2 = 0;
      int index1 = 0;
      int index2 = 0;
      bool flag = true;
      if (data != null && size >= 5)
      {
        response = new Dictionary<string, object>();
        for (int index3 = 0; index3 < size; ++index3)
        {
          if (data[index3] == (byte) 2 & flag)
          {
            num1 = index3;
            flag = false;
          }
          if (data[index3] == (byte) 3 && index3 < size - 1)
          {
            num2 = index3;
            index1 = index3 + 1;
          }
        }
        if (num1 >= num2 || num2 >= size - 1)
          return (Dictionary<string, object>) null;
        byte[] numArray1 = new byte[num2 - num1 - 1];
        for (int index4 = num1 + 1; index4 < num2; ++index4)
        {
          numArray1[index2] = data[index4];
          ++index2;
        }
        int index5 = 0;
        byte[] numArray2 = new byte[num2 - num1 - 2 + 2];
        for (int index6 = num1 + 1; index6 <= num2; ++index6)
        {
          numArray2[index5] = data[index6];
          ++index5;
        }
        if (numArray1 == null || numArray2 == null)
          return (Dictionary<string, object>) null;
        response.Add("MSG", (object) numArray1);
        response.Add("RESPONSEWSTXLRC", (object) numArray2);
        response.Add("LRC", (object) data[index1]);
        response.Add("STX", (object) num1);
        response.Add("ETX", (object) num2);
      }
      return response;
    }
    }
}
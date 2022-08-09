#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Ingenico.Reports;

namespace Ingenico
{
	internal class ResponseMsg
{
	public List<Dictionary<int, string>> ParseReceivedResponse(byte[] ResponseBuffer, bool bPrintResp, Encoding encodeType)
	{
		int num = 0;
		byte separteur = 28;
		bool flag = false;
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		List<Dictionary<int, string>> list = new List<Dictionary<int, string>>();
		List<byte[]> list2 = SplitResponseBuffer(ResponseBuffer, separteur, 0);
		if (!bPrintResp)
		{
			try
			{
				string @string = encodeType.GetString(list2[0], 2, 1);
				dictionary.Add(-1, @string);
				string string2 = encodeType.GetString(list2[0], 0, 2);
				dictionary.Add(new DataElement().Get_TransStatusTag(), string2);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Invalid transaction status: " + ex.Message);
			}
			num = 1;
		}
		for (int i = num; i < list2.Count; i++)
		{
			if (list2[i].Length < 3)
			{
				continue;
			}
			string string3 = encodeType.GetString(list2[i], 0, 3);
			try
			{
				int num2 = int.Parse(string3);
				if (num2 == Tags.TAG_ECR_REQUESTED_FORMATTED_RECEIPT || num2 == Tags.TAG_VAS_RESPONSE_DATA)
				{
					encodeType = Encoding.UTF8;
				}
				string string4;
				if (num2 != new DataElement().TAG_TRANS_RECORD)
				{
					string4 = encodeType.GetString(list2[i], 3, list2[i].Length - 3);
					if (dictionary.TryGetValue(num2, out var value))
					{
						value = (dictionary[num2] = value + string4);
					}
					else
					{
						dictionary[num2] = string4;
					}
					continue;
				}
				if (!flag)
				{
					string4 = (dictionary[num2] = encodeType.GetString(list2[i], 7, 2));
					flag = true;
				}
				List<byte[]> list3 = SplitResponseBuffer(list2[i], 29, 9);
				Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
				string4 = encodeType.GetString(list2[i], 3, 2);
				dictionary2[DataElement.TAG_TRANS_RECORD_TYPE] = string4;
				for (int j = 0; j < list3.Count; j++)
				{
					string3 = encodeType.GetString(list3[j], 0, 3);
					try
					{
						num2 = int.Parse(string3);
						if (num2 == Tags.TAG_VAS_RESPONSE_DATA)
						{
							encodeType = Encoding.UTF8;
						}
						string4 = (dictionary2[num2] = encodeType.GetString(list3[j], 3, list3[j].Length - 3));
					}
					catch (Exception ex2)
					{
						Trace.WriteLine("Invalid tag value: " + ex2.Message);
					}
				}
				list.Add(dictionary2);
			}
			catch (Exception ex3)
			{
				Trace.WriteLine("Invalid tag value: " + ex3.Message);
			}
		}
		if (dictionary.Count != 0)
		{
			list.Add(dictionary);
		}
		return list;
	}

	public List<byte[]> SplitResponseBuffer(byte[] ResponseBuffer, byte separteur, int indexStartField)
	{
		int num = 0;
		List<byte[]> list = new List<byte[]>();
		int num2 = indexStartField;
		while (num2 < ResponseBuffer.Length)
		{
			if (ResponseBuffer[num2] == separteur || num2 == ResponseBuffer.Length - 1)
			{
				byte[] array;
				if (num2 == ResponseBuffer.Length - 1 && ResponseBuffer[num2] != separteur)
				{
					array = new byte[num + 1];
					Array.Copy(ResponseBuffer, num2 - num, array, 0, num + 1);
				}
				else
				{
					array = new byte[num];
					Array.Copy(ResponseBuffer, num2 - num, array, 0, num);
				}
				if (array.Length != 0)
				{
					list.Add(array);
				}
				num = -1;
			}
			num2++;
			num++;
		}
		return list;
	}

	public Dictionary<string, object> extractResponse(byte[] data, int size)
	{
		Dictionary<string, object> dictionary = null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		byte[] array = null;
		byte[] array2 = null;
		bool flag = true;
		if (data != null && size >= 5)
		{
			dictionary = new Dictionary<string, object>();
			for (int i = 0; i < size; i++)
			{
				if (data[i] == 2 && flag)
				{
					num = i;
					flag = false;
				}
				if (data[i] == 3 && i < size - 1)
				{
					num2 = i;
					num3 = i + 1;
				}
			}
			if (num >= num2 || num2 >= size - 1)
			{
				return null;
			}
			array = new byte[num2 - num - 1];
			for (int j = num + 1; j < num2; j++)
			{
				array[num4] = data[j];
				num4++;
			}
			num4 = 0;
			array2 = new byte[num2 - num - 2 + 2];
			for (int k = num + 1; k <= num2; k++)
			{
				array2[num4] = data[k];
				num4++;
			}
			if (array == null || array2 == null)
			{
				return null;
			}
			dictionary.Add("MSG", array);
			dictionary.Add("RESPONSEWSTXLRC", array2);
			dictionary.Add("LRC", data[num3]);
			dictionary.Add("STX", num);
			dictionary.Add("ETX", num2);
		}
		return dictionary;
	}
}
}
#define TRACE
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Ingenico
{
	public class DataElement
{
	public DataList[] DataListInfo;

	public DataList[] TransactionType;

	public DataList[] TenderType;

	public int TAG_TRANS_RECORD = 900;

	public static int TAG_TRANS_RECORD_TYPE = 10000;

	public DataElement()
	{
		InitializeDataElement();
	}

	public void InitializeDataElement()
	{
		updateTagXmlListType();
		updateCfgXmlListType();
	}

	public void updateCfgXmlListType()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Cfg.xml");
		XmlNode xmlNode = null;
		int num = 0;
		int num2 = 0;
		DataList[] array = new DataList[99];
		DataList[] array2 = new DataList[99];
		try
		{
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.Name.Equals("TenderType"))
				{
					xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(1).Name.Equals("Value"))
					{
						array[num] = new DataList(xmlNode.Attributes.Item(1).Value.ToString(), xmlNode.Attributes.Item(0).Value.ToString());
						num++;
					}
				}
				if (xmlTextReader.Name.Equals("TransactionType"))
				{
					xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(1).Name.Equals("Value"))
					{
						array2[num2] = new DataList(xmlNode.Attributes.Item(1).Value.ToString(), xmlNode.Attributes.Item(0).Value.ToString());
						num2++;
					}
				}
			}
			if (num > 0)
			{
				TenderType = new DataList[num];
				Array.Copy(array, TenderType, num);
			}
			if (num2 > 0)
			{
				TransactionType = new DataList[num2];
				Array.Copy(array2, TransactionType, num2);
			}
		}
		catch
		{
			Trace.WriteLine("Problem updateCfgXmlListType");
		}
		xmlTextReader.Close();
	}

	public void updateTagXmlListType()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlTextReader xmlTextReader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + "//Tag.xml");
		XmlNode xmlNode = null;
		int num = 0;
		DataList[] array = new DataList[1000];
		try
		{
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.Name.Equals("DataElement"))
				{
					xmlNode = xmlDocument.ReadNode(xmlTextReader);
					if (xmlNode.Attributes.Item(0).Name.Equals("Tag") && xmlNode.Attributes.Item(1).Name.Equals("Description"))
					{
						array[num] = new DataList(xmlNode.Attributes.Item(0).Value.ToString(), xmlNode.Attributes.Item(1).Value.ToString());
						num++;
					}
				}
			}
			if (num > 0)
			{
				DataListInfo = new DataList[num];
				Array.Copy(array, DataListInfo, num);
			}
		}
		catch
		{
			Trace.WriteLine("Problem updateTagXmlListType");
		}
		xmlTextReader.Close();
	}

	public string FormatTagThreeDigit(int inTag)
	{
		string text = inTag.ToString();
		if (text.Length == 1)
		{
			return $"00{text}";
		}
		if (text.Length == 2)
		{
			return $"0{text}";
		}
		return text;
	}

	public string Get_DataListLabel(int iTag)
	{
		if (DataListInfo != null)
		{
			for (int i = 0; i < DataListInfo.Length; i++)
			{
				if (DataListInfo[i].tag.Equals(FormatTagThreeDigit(iTag)))
				{
					return DataListInfo[i].label;
				}
			}
		}
		return "";
	}

	public string Get_TransTypeTag(string szLabel)
	{
		if (TransactionType != null)
		{
			for (int i = 0; i < TransactionType.Length; i++)
			{
				if (TransactionType[i].label.Equals(szLabel))
				{
					return TransactionType[i].tag;
				}
			}
		}
		return "";
	}

	public string Get_TenderTypeTag(string szLabel)
	{
		if (TenderType != null)
		{
			for (int i = 0; i < TenderType.Length; i++)
			{
				if (TenderType[i].label.Equals(szLabel))
				{
					return TenderType[i].tag;
				}
			}
		}
		return "";
	}

	public int Get_TransStatusTag()
	{
		if (DataListInfo != null)
		{
			for (int i = 0; i < DataListInfo.Length; i++)
			{
				if (DataListInfo[i].label.Equals("Trans Status"))
				{
					try
					{
						return int.Parse(DataListInfo[i].tag);
					}
					catch
					{
						return 101;
					}
				}
			}
		}
		return 101;
	}

	public int Get_TransRecordTag()
	{
		if (DataListInfo != null)
		{
			for (int i = 0; i < DataListInfo.Length; i++)
			{
				if (DataListInfo[i].label.Equals("Trans Record"))
				{
					try
					{
						TAG_TRANS_RECORD = int.Parse(DataListInfo[i].tag);
					}
					catch
					{
						TAG_TRANS_RECORD = 900;
					}
					return TAG_TRANS_RECORD;
				}
			}
		}
		return TAG_TRANS_RECORD;
	}
}
}
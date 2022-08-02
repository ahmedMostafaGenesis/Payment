using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
namespace ingenico
{
    public class DataElement
    {
         public DataList[] DataListInfo;
    public DataList[] TransactionType;
    public DataList[] TenderType;
    public int TAG_TRANS_RECORD = 900;
    public static int TAG_TRANS_RECORD_TYPE = 10000;

    public DataElement() => this.InitializeDataElement();

    public void InitializeDataElement()
    {
      this.updateTagXmlListType();
      this.updateCfgXmlListType();
    }

    public void updateCfgXmlListType()
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlTextReader reader = new XmlTextReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "//Cfg.xml");
      int length1 = 0;
      int length2 = 0;
      DataList[] sourceArray1 = new DataList[99];
      DataList[] sourceArray2 = new DataList[99];
      try
      {
        while (reader.Read())
        {
          if (reader.Name.Equals("TenderType"))
          {
            Console.WriteLine("Found TenderType");
            XmlNode xmlNode = xmlDocument.ReadNode((XmlReader) reader);
            if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(1).Name.Equals("Value"))
            {
              sourceArray1[length1] = new DataList(xmlNode.Attributes.Item(1).Value.ToString(), xmlNode.Attributes.Item(0).Value.ToString());
              ++length1;
            }
          }
          if (reader.Name.Equals("TransactionType"))
          {
            XmlNode xmlNode = xmlDocument.ReadNode((XmlReader) reader);
            if (xmlNode.Attributes.Item(0).Name.Equals("Name") && xmlNode.Attributes.Item(1).Name.Equals("Value"))
            {
              sourceArray2[length2] = new DataList(xmlNode.Attributes.Item(1).Value.ToString(), xmlNode.Attributes.Item(0).Value.ToString());
              ++length2;
            }
          }
        }
        if (length1 > 0)
        {
          this.TenderType = new DataList[length1];
          Array.Copy((Array) sourceArray1, (Array) this.TenderType, length1);
        }
        if (length2 > 0)
        {
          this.TransactionType = new DataList[length2];
          Array.Copy((Array) sourceArray2, (Array) this.TransactionType, length2);
        }
      }
      catch
      {
        Trace.WriteLine("Problem updateCfgXmlListType");
      }
      reader.Close();
    }

    public void updateTagXmlListType()
    {
      XmlDocument xmlDocument = new XmlDocument();
      XmlTextReader reader = new XmlTextReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "//Tag.xml");
      int length = 0;
      DataList[] sourceArray = new DataList[1000];
      try
      {
        while (reader.Read())
        {
          if (reader.Name.Equals(nameof (DataElement)))
          {
            XmlNode xmlNode = xmlDocument.ReadNode((XmlReader) reader);
            if (xmlNode.Attributes.Item(0).Name.Equals("Tag") && xmlNode.Attributes.Item(1).Name.Equals("Description"))
            {
              sourceArray[length] = new DataList(xmlNode.Attributes.Item(0).Value.ToString(), xmlNode.Attributes.Item(1).Value.ToString());
              ++length;
            }
          }
        }
        if (length > 0)
        {
          this.DataListInfo = new DataList[length];
          Array.Copy((Array) sourceArray, (Array) this.DataListInfo, length);
        }
      }
      catch
      {
        Trace.WriteLine("Problem updateTagXmlListType");
      }
      reader.Close();
    }

    public string FormatTagThreeDigit(int inTag)
    {
      string str = inTag.ToString();
      if (str.Length == 1)
        return $"00{(object) str}";
      return str.Length == 2 ? $"0{(object) str}" : str;
    }

    public string Get_DataListLabel(int iTag)
    {
      if (this.DataListInfo != null)
      {
        for (int index = 0; index < this.DataListInfo.Length; ++index)
        {
          if (this.DataListInfo[index].tag.Equals(this.FormatTagThreeDigit(iTag)))
            return this.DataListInfo[index].label;
        }
      }
      return "";
    }

    public string Get_TransTypeTag(string szLabel)
    {
      if (this.TransactionType != null)
      {
        for (int index = 0; index < this.TransactionType.Length; ++index)
        {
          if (this.TransactionType[index].label.Equals(szLabel))
            return this.TransactionType[index].tag;
        }
      }
      return "";
    }

    public string Get_TenderTypeTag(string szLabel)
    {
      if (this.TenderType != null)
      {
        for (int index = 0; index < this.TenderType.Length; ++index)
        {
          if (this.TenderType[index].label.Equals(szLabel))
            return this.TenderType[index].tag;
        }
      }
      return "";
    }

    public int Get_TransStatusTag()
    {
      if (this.DataListInfo != null)
      {
        for (int index = 0; index < this.DataListInfo.Length; ++index)
        {
          if (this.DataListInfo[index].label.Equals("Trans Status"))
          {
            try
            {
              return int.Parse(this.DataListInfo[index].tag);
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
      if (this.DataListInfo != null)
      {
        for (int index = 0; index < this.DataListInfo.Length; ++index)
        {
          if (this.DataListInfo[index].label.Equals("Trans Record"))
          {
            try
            {
              this.TAG_TRANS_RECORD = int.Parse(this.DataListInfo[index].tag);
            }
            catch
            {
              this.TAG_TRANS_RECORD = 900;
            }
            return this.TAG_TRANS_RECORD;
          }
        }
      }
      return this.TAG_TRANS_RECORD;
    }
    }
}
using System.Text;

namespace ingenico
{
    internal class Converter
    {
        
      public const int ASCII = 0;
    public const int UTF8 = 1;
    private bool bIsLRC;

    public byte[] ConvertStringToByteArray(string data, int EncodeType)
    {
      if (EncodeType == 0)
        return new ASCIIEncoding().GetBytes(data);
      return EncodeType == 1 ? new UTF8Encoding().GetBytes(data) : new ASCIIEncoding().GetBytes(data);
    }

    public string ConvertByteArrayToString(byte[] data, int size)
    {
      string str = "";
      //Encoding.ASCII.GetString(data);
      for (int index = 0; index < size; ++index)
      {
        char ch = (char) data[index];
        if (this.bIsLRC)
        {
          str += "<LRC>";
          this.bIsLRC = false;
        }
        else
        {
          switch (ch)
          {
            case '\u0002':
              str += "<STX>";
              continue;
            case '\u0003':
              str += "<ETX>";
              this.bIsLRC = true;
              continue;
            case '\u0006':
              str += "<ACK>";
              continue;
            case '\u0011':
              str += "<HB>";
              continue;
            case '\u0015':
              str += "<NAK>";
              continue;
            case '\u001C':
              str += "<FS>";
              continue;
            case '\u001D':
              str += "<GS>";
              continue;
            default:
              str = ch >= ' ' ? str + ch.ToString() : str + ".";
              continue;
          }
        }
      }
      return str;
    }

    public string ConvertFieldToAscii(byte[] data) => Encoding.ASCII.GetString(data) + "00";

    public int calculateLrc(byte[] data, string origin)
    {
      int lrc = 0;
      if (origin.Equals("res"))
      {
        for (int index = 0; index < data.Length - 2; ++index)
          lrc ^= (int) data[index];
      }
      else if (origin.Equals("command"))
      {
        for (int index = 0; index < data.Length; ++index)
          lrc ^= (int) data[index];
      }
      return lrc;
    }

    public byte[] HexStringToByteArray(string Hex, int starter, int addlength)
    {
      byte[] byteArray = new byte[Hex.Length / 2 + addlength];
      int[] numArray = new int[23]
      {
        0,
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        10,
        11,
        12,
        13,
        14,
        15
      };
      int num = 0;
      int index = 0;
      while (index < Hex.Length)
      {
        byteArray[num + starter] = (byte) (numArray[(int) char.ToUpper(Hex[index]) - 48] << 4 | numArray[(int) char.ToUpper(Hex[index + 1]) - 48]);
        index += 2;
        ++num;
      }
      return byteArray;
    }
    }
}
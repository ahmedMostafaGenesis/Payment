using System.Text;

namespace Ingenico
{
	


internal class Converter
{
	public const int ASCII = 0;

	public const int UTF8 = 1;

	private bool bIsLRC;

	public byte[] convertStringToByteArray(string data, int EncodeType)
	{
		return EncodeType switch
		{
			0 => new ASCIIEncoding().GetBytes(data), 
			1 => new UTF8Encoding().GetBytes(data), 
			_ => new ASCIIEncoding().GetBytes(data), 
		};
	}

	public string convertByteArreyToString(byte[] data, int size)
	{
		string text = "";
		int num = 0;
		Encoding.ASCII.GetString(data);
		for (num = 0; num < size; num++)
		{
			char c = (char)data[num];
			if (bIsLRC)
			{
				text += "<LRC>";
				bIsLRC = false;
			}
			else if (c == '\u0002')
			{
				text += "<STX>";
			}
			else if (c == '\u0003')
			{
				text += "<ETX>";
				bIsLRC = true;
			}
			else
			{
				text = ((c != '\u0006') ? ((c != '\u0015') ? ((c != '\u001c') ? ((c != '\u001d') ? ((c != '\u0011') ? ((c >= ' ') ? (text + c) : (text + ".")) : (text + "<HB>")) : (text + "<GS>")) : (text + "<FS>")) : (text + "<NAK>")) : (text + "<ACK>"));
			}
		}
		return text;
	}

	public string convertFieldToAscii(byte[] data)
	{
		return Encoding.ASCII.GetString(data) + "00";
	}

	public int calculateLrc(byte[] data, string origin)
	{
		int num = 0;
		if (origin.Equals("res"))
		{
			for (int i = 0; i < data.Length - 2; i++)
			{
				num ^= data[i];
			}
		}
		else if (origin.Equals("command"))
		{
			for (int j = 0; j < data.Length; j++)
			{
				num ^= data[j];
			}
		}
		return num;
	}

	public byte[] HexStringToByteArray(string Hex, int starter, int addlength)
	{
		byte[] array = new byte[Hex.Length / 2 + addlength];
		int[] array2 = new int[23]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
			0, 0, 0, 0, 0, 0, 0, 10, 11, 12,
			13, 14, 15
		};
		int num = 0;
		int num2 = 0;
		while (num2 < Hex.Length)
		{
			array[num + starter] = (byte)((array2[char.ToUpper(Hex[num2]) - 48] << 4) | array2[char.ToUpper(Hex[num2 + 1]) - 48]);
			num2 += 2;
			num++;
		}
		return array;
	}
}
}
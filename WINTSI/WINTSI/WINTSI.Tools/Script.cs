#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ingenico.Tools
{



	internal class Script
	{
		private static int loopValue = 1;

		public List<Command> Initialize(string ScriptFilePath)
		{
			string text = "";
			loopValue = 1;
			string text2 = "";
			List<Command> list = new List<Command>();
			Command item = default(Command);
			new List<Dictionary<string, string>>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				string[] array = File.ReadAllLines(ScriptFilePath);
				ExtractLoopValue(array[0]);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text3 = RemoveCommentStrip(array2[i]);
					if (!(text3 != ""))
					{
						continue;
					}

					if (text3.Contains("["))
					{
						if (item.Label != null && item.Label != "")
						{
							item.Params = dictionary;
							list.Add(item);
							item = default(Command);
							dictionary = new Dictionary<string, string>();
						}

						item.Label = ExtractCommand(text3);
					}
					else
					{
						text = ExtractParam(text3);
						text2 = ExtractParamValue(text3);
						if (text != "" && text2 != "")
						{
							dictionary[text] = text2;
						}
					}
				}

				if (dictionary != null)
				{
					if (item.Label != null)
					{
						item.Params = dictionary;
						list.Add(item);
						return list;
					}

					return list;
				}

				return list;
			}
			catch (Exception value)
			{
				Trace.WriteLine(value);
				return list;
			}
		}

		private string ExtractParam(string ParamLine)
		{
			string result = "";
			try
			{
				result = ParamLine.Substring(0, ParamLine.IndexOf('"')).Trim();
				return result;
			}
			catch (Exception)
			{
				Trace.WriteLine("invalid parameter");
				return result;
			}
		}

		private string ExtractParamValue(string ParamLine)
		{
			string result = "";
			try
			{
				int num = ParamLine.IndexOf('"') + 1;
				result = ParamLine.Substring(num, ParamLine.IndexOf('"', num) - num);
				return result;
			}
			catch (Exception)
			{
				Trace.WriteLine("invalid parameter value");
				return result;
			}
		}

		private string ExtractCommand(string Data)
		{
			string result = "";
			try
			{
				result = Data.Substring(Data.IndexOf('[') + 1, Data.IndexOf(']') - 1);
				return result;
			}
			catch (Exception value)
			{
				Trace.WriteLine(value);
				return result;
			}
		}

		private static string RemoveCommentStrip(string Text)
		{
			int num = Text.IndexOf(";");
			if (num > -1)
			{
				Text = Text.Remove(num, Text.Length - num);
			}

			return Text;
		}

		private void ExtractLoopValue(string line)
		{
			string text = "Loop";
			try
			{
				string text2 = RemoveCommentStrip(line);
				if (text2.Contains(text))
				{
					loopValue = int.Parse(text2.Substring(text2.IndexOf(text) + text.Length + 1));
				}
			}
			catch (Exception value)
			{
				Trace.WriteLine(value);
			}
		}

		public int GetLoopValue()
		{
			return loopValue;
		}
	}
}
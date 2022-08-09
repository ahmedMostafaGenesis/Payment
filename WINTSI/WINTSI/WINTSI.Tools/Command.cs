using System.Collections.Generic;

namespace Ingenico.Tools
{


	public struct Command
	{
		public string Label;

		public Dictionary<string, string> Params;

		public Command(string CommandName, Dictionary<string, string> Parameters)
		{
			Label = CommandName;
			Params = Parameters;
		}
	}
}
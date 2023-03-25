using System;
using System.Diagnostics;
using System.Windows.Forms;
using GenesisCreations.PharmaTech;
using WINTSI.WebSocket;

namespace Ingenico

{
	internal static class Program
	{
		private static LogListener _myLogListener;
		public static HomeForm _running;
		[STAThread]
		private static void Main()
		{
			try
			{
				_myLogListener = new LogListener("Log\\Log.txt");
				_myLogListener.MaxLogSize = 10000000L;
				_myLogListener.WriteDateInfo = true;
				Trace.Listeners.Add(_myLogListener);
				Trace.AutoFlush = true;
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(defaultValue: false);
				_running = new HomeForm();
				StartServer();
				Application.Run(_running);

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.ReadLine();
		}


		private static void StartServer()
		{
			//Client.Initialize();
			//var line = Console.ReadLine();
			//CreateRequest(/*int.Parse(line)*/1000);
		}
		public static void CreateRequest(int amount)
		{
			Console.WriteLine(_running.Com?.LinkType);
			_running.SendTheRequest(amount, 1);
		}
	}
}
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ingenico

{
	internal static class Program
	{
		private static LogListener _myLogListener;
		private static HomeForm _running;
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
				//StartClient();
				//Application.Run(Running);

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.ReadLine();

		}


		private static void StartServer()
		{
			SocketListener.StartServer(_running.Com);

		}		
		private static void StartClient()
		{
			SocketClient.StartClient(_running.Com);

		}
		private static void CreateFakeRequest(int amount)
		{
			_running.SendTheRequest(amount, 1);
		}
	}
}
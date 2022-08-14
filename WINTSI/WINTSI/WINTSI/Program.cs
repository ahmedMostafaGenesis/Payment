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
				CreateFakeRequest();
				//Application.Run(Running);

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.ReadLine();

		}

		private static void CreateFakeRequest()
		{
			_running.SendTheRequest(10000, 1);
			//SocketListener.StartServer(_running.Com);
			SocketClient.StartClient(_running.Com);
		}
	}
}
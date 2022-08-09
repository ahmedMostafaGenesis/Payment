#define TRACE
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Ingenico
{
	


public class LogListener : TraceListener
{
	private FileSystemWatcher watcher;

	private string _logPath;

	private object fileLock = new object();

	private bool alreadyMsgBox;

	private static Stack StackDeLogAEcrire;

	private int _maxLogInWait;

	private long _maxLogSize;

	private bool _showFatalErrorInMessageBox;

	private bool _WriteDateInfo;

	private bool _indicateDate;

	private bool _IsErrorDetected;

	private Exception _LastException;

	private string _LastErrMsg;

	public string LogPath
	{
		get
		{
			return _logPath;
		}
		set
		{
			_logPath = value;
			Path.GetFileName(_logPath);
			if (Path.GetExtension(_logPath) == string.Empty)
			{
				throw new Exception("Bad log filename, extension require!");
			}
			string directoryName = Path.GetDirectoryName(_logPath);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}
	}

	public int MaxLogInWait
	{
		get
		{
			return _maxLogInWait;
		}
		set
		{
			_maxLogInWait = value;
		}
	}

	public long MaxLogSize
	{
		get
		{
			return _maxLogSize;
		}
		set
		{
			_maxLogSize = value;
		}
	}

	public bool ShowFatalErrorInMessageBox
	{
		get
		{
			return _showFatalErrorInMessageBox;
		}
		set
		{
			_showFatalErrorInMessageBox = value;
		}
	}

	public bool WriteDateInfo
	{
		get
		{
			return _WriteDateInfo;
		}
		set
		{
			_WriteDateInfo = value;
		}
	}

	public bool IndicateDate
	{
		get
		{
			return _indicateDate;
		}
		set
		{
			_indicateDate = value;
		}
	}

	public bool IsErrorDetected
	{
		get
		{
			return _IsErrorDetected;
		}
		private set
		{
			_IsErrorDetected = value;
		}
	}

	public Exception LastException
	{
		get
		{
			return _LastException;
		}
		private set
		{
			_LastException = value;
		}
	}

	public string LastErrorMsg
	{
		get
		{
			return _LastErrMsg;
		}
		private set
		{
			_LastErrMsg = value;
		}
	}

	public event EventHandler ErrorDetectedEvent;

	public LogListener(string logPath)
	{
		if (logPath == null || logPath == string.Empty)
		{
			throw new Exception("The path of the log file is required.");
		}
		LogPath = logPath;
		MaxLogSize = 1000000L;
		IndicateDate = true;
		WriteDateInfo = true;
		StackDeLogAEcrire = new Stack();
		ShowFatalErrorInMessageBox = true;
		MaxLogInWait = 50;
		watcher = new FileSystemWatcher();
		watcher.Path = Path.GetDirectoryName(LogPath);
		watcher.Filter = Path.GetFileName(LogPath);
		watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.LastAccess;
		watcher.Changed += watcher_Changed;
		watcher.Created += watcher_Changed;
		watcher.Deleted += watcher_Changed;
		watcher.Renamed += watcher_Changed;
	}

	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
	{
		if (eventType != TraceEventType.Information || WriteDateInfo)
		{
			if (message == string.Empty)
			{
				message = "";
			}
			if (eventType == TraceEventType.Error)
			{
				RaiseExceptionDetectedEvent(message, null);
			}
			message = " Type : " + eventType.ToString() + " - message : " + message + "\r\n";
			WriteLine(message);
		}
	}

	public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message, params object[] args)
	{
		if ((eventType != TraceEventType.Information || WriteDateInfo) && args.Length != 0 && args[0] is Exception)
		{
			Exception ex = (Exception)args[0];
			if (message == string.Empty)
			{
				message = "";
			}
			if (eventType == TraceEventType.Error)
			{
				RaiseExceptionDetectedEvent(message, ex);
			}
			string text = " Type : " + eventType.ToString() + " - message : " + message + "\r\n";
			text += $"EXCEPTION type : {ex.GetType().ToString()} \r\n   Message d'erreur: {ex.Message} \r\n   Origine : {ex.StackTrace} \r\n";
			WriteLine(text);
		}
	}

	protected virtual void RaiseExceptionDetectedEvent(string messageCourt, Exception ex)
	{
		LastException = ex;
		LastErrorMsg = messageCourt;
		IsErrorDetected = true;
		this.ErrorDetectedEvent?.Invoke(this, new EventArgs());
	}

	public override void WriteLine(string message, string cate)
	{
		WriteInFic(message + "\r\n");
		Console.WriteLine(message + "\r\n");
	}

	public override void WriteLine(string message)
	{
		Write(message + "\r\n");
		Console.WriteLine(message + "\r\n");
	}

	public override void Write(string message)
	{
		if (IndicateDate)
		{
			message = DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond + " -> " + message;
		}
		WriteInFic(message);
		Console.WriteLine(message + "\r\n");
	}

	public override void WriteLine(object o)
	{
		base.WriteLine(o);
	}

	private void WriteInFic(string message)
	{
		bool flag = false;
		lock (StackDeLogAEcrire)
		{
			if (StackDeLogAEcrire.Count == 0)
			{
				flag = true;
			}
			StackDeLogAEcrire.Push(message);
		}
		if (flag)
		{
			new Thread(WriteInFicThreadStart).Start();
		}
	}

	private void WriteInFicThreadStart()
	{
		try
		{
			lock (fileLock)
			{
				long num = 0L;
				LogPath = Path.GetDirectoryName(Application.ExecutablePath) + "//Log//Log.txt";
				string text = LogPath + ".old";
				bool flag = false;
				if (File.Exists(LogPath))
				{
					flag = true;
				}
				FileStream fileStream = null;
				try
				{
					fileStream = new FileStream(LogPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
				}
				catch
				{
					if (!watcher.EnableRaisingEvents)
					{
						watcher.EnableRaisingEvents = true;
					}
					return;
				}
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				if (flag)
				{
					File.Copy(LogPath, text);
				}
				StreamWriter streamWriter = new StreamWriter(fileStream);
				lock (StackDeLogAEcrire)
				{
					while (StackDeLogAEcrire.Count > 0)
					{
						string text2 = (string)StackDeLogAEcrire.Pop();
						streamWriter.Write(text2);
						num += text2.Length;
					}
				}
				if (flag)
				{
					using (StreamReader streamReader = new StreamReader(text))
					{
						string text3;
						while ((text3 = streamReader.ReadLine()) != null)
						{
							num += text3.Length;
							if (num < MaxLogSize || MaxLogSize == 0L)
							{
								streamWriter.WriteLine(text3);
								continue;
							}
							break;
						}
					}
					File.Delete(text);
				}
				streamWriter.Close();
				fileStream.Close();
				lock (StackDeLogAEcrire)
				{
					if (StackDeLogAEcrire.Count > 0)
					{
						if (StackDeLogAEcrire.Count > MaxLogInWait && MaxLogInWait != 0)
						{
							StackDeLogAEcrire.Clear();
							StackDeLogAEcrire.Push("==== DEPASSEMENT DU NOMBRE DE MESSAGE QUE LE GESTIONNAIRE DE LOG PEUT TRAITER !!! ===");
							StackDeLogAEcrire.Push("==== CERTAINS LOGS N'ONT PAS ETE ECRIT. ===");
						}
						Trace.WriteLine("Le thread d'eriture a ete relance");
						new Thread(WriteInFicThreadStart).Start();
					}
				}
			}
		}
		catch
		{
			if (ShowFatalErrorInMessageBox && !alreadyMsgBox)
			{
				MessageBox.Show("Error occured when writing in log file!", Assembly.GetEntryAssembly().FullName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				alreadyMsgBox = true;
			}
		}
	}

	private void watcher_Changed(object source, FileSystemEventArgs e)
	{
		if (watcher.EnableRaisingEvents)
		{
			watcher.EnableRaisingEvents = false;
		}
		WriteInFicThreadStart();
	}
}
}
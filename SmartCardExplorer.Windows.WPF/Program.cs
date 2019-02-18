using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCardReader.Windows.WPF
{
	class Program
	{
		// http://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application
		static Mutex mutex = new Mutex(true, "{18F599BD-9BE4-4CE0-9C5D-D2767092C5B4}");

		[STAThread]
		public static void Main(string[] args)
		{
			App.SetupLoggingConfig();

			if (mutex.WaitOne(TimeSpan.FromSeconds(5), true))
			{
				try
				{
					StartApp();
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
			else
			{
				// send our Win32 message to make the currently running instance
				// jump on top of all the other windows
				PostMessage(
						(IntPtr)HWND_BROADCAST,
						WM_SHOWME,
						IntPtr.Zero,
						IntPtr.Zero);
			}
		}

		private static void StartApp()
		{
			App.Main();
		}

		public const int HWND_BROADCAST = 0xffff;
		public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME");
		[DllImport("user32")]
		public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
		[DllImport("user32")]
		public static extern int RegisterWindowMessage(string message);
	}
}

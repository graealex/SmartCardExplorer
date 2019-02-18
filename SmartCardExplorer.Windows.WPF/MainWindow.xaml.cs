using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SmartCardReader.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using NLog;

namespace SmartCardExplorer.Windows.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		DispatcherTimer statusTimer = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();

			statusTimer.Tick += OnStatusTimerTick;
			statusTimer.Interval = TimeSpan.FromMilliseconds(100);
		}

		private CardReaderHost cardReaderHost;
		private IntPtr windowHandle;

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			statusTimer.Start();
		}

		private void OnStatusTimerTick(object sender, EventArgs e)
		{
			if (cardReaderHost != null)
			{
				return;
			}

			try
			{
				cardReaderHost = CardReaderHost.GetCardReaderHost();
				cardReaderHost.UpdateCardReaders();

				MainTreeView.ItemsSource = cardReaderHost?.CardReaders;
				StatusText.SetResourceReference(TextBlock.TextProperty, "Ready");
			}
			catch (Exception ex)
			{
				if (cardReaderHost != null)
				{
					cardReaderHost.Dispose();
					cardReaderHost = null;
				}

				Logger.Warn(ex);
				StatusText.Text = ExceptionToString(ex);
			}
		}

		private static string ExceptionToString(Exception ex)
		{
			string message = String.Empty;

			if (ex is Win32Exception)
			{
				string errorCode = null;
				errorCode = Enum.GetName(typeof(SCardInterface.WINDOWS_ERRORS), (ex as Win32Exception).NativeErrorCode);

				if (errorCode == null)
					errorCode = Enum.GetName(typeof(SCardInterface.SCARD_ERRORS), (ex as Win32Exception).NativeErrorCode);

				if (errorCode == null)
					message = ex.Message;
				else
					message = $"{errorCode}: {ex.Message}";
			}
			else
			{
				message = ex.Message;
			}

			if (ex.InnerException != null)
				return message + ": " + ExceptionToString(ex.InnerException);

			return message;
		}

		private void OnWindowStateChanged(object sender, EventArgs e)
		{
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
		}

		private void OnClosed(object sender, EventArgs e)
		{
		}

		private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue is CardReader)
			{
				CardReaderProperties.DataContext = e.NewValue;
				int t = 5;
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			// Adds the windows message processing hook and registers USB device add/removal notification.
			HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
			if (source != null)
			{
				windowHandle = source.Handle;
				source.AddHook(HwndHandler);
				DeviceNotification.RegisterSCDeviceNotification(windowHandle);
			}
		}

		/// <summary>
		/// Method that receives window messages.
		/// </summary>
		private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			try
			{
				if (msg == DeviceNotification.WmDevicechange)
				{
					switch ((int)wparam)
					{
						case DeviceNotification.DbtDeviceRemoveComplete:
							Logger.Info("DeviceNotification.DbtDeviceRemoveComplete received");
							if (cardReaderHost == null)
								cardReaderHost = CardReaderHost.GetCardReaderHost();
							cardReaderHost.UpdateCardReaders();
							break;
						case DeviceNotification.DbtDeviceArrival:
							Logger.Info("DeviceNotification.DbtDeviceArrival received");
							if (cardReaderHost == null)
								cardReaderHost = CardReaderHost.GetCardReaderHost();
							cardReaderHost.UpdateCardReaders();
							break;
					}
				}

				if ((cardReaderHost != null) && 
					(MainTreeView.ItemsSource != cardReaderHost.CardReaders))
					MainTreeView.ItemsSource = cardReaderHost?.CardReaders;
			}
			catch (Exception ex)
			{
				Logger.Warn(ex);
			}
			handled = false;
			return IntPtr.Zero;
		}

	}

}

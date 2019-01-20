using System;
using System.Collections.Generic;
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
using SmartCardReader.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SmartCardExplorer.Windows.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();



		}

		private CardReaderHost cardReaderHost;
		private IntPtr windowHandle;

		private void OnWindowLoaded(object sender, RoutedEventArgs e)
		{

			cardReaderHost = CardReaderHost.GetCardReaderHost();
			MainTreeView.ItemsSource = cardReaderHost.CardReaders;

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
			if (msg == DeviceNotification.WmDevicechange)
			{
				switch ((int)wparam)
				{
					case DeviceNotification.DbtDeviceRemoveComplete:
						cardReaderHost.OnCardReaderRemoved();
						break;
					case DeviceNotification.DbtDeviceArrival:
						cardReaderHost.OnCardReaderAdded();
						break;
				}
			}

			handled = false;
			return IntPtr.Zero;
		}

	}

}

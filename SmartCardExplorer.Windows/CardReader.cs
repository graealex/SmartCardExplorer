using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NLog;

namespace SmartCardExplorer.Windows
{
	public class CardReader : INotifyPropertyChanged
	{
		public static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private SCardInterface sCardInterface;
		private string readerName;
		private string deviceInstanceId;
		private string state;
		private Image icon;
		private string atr;
		private Dictionary<string, string> readerProperties;

		private ObservableCollection<Card> cards = new ObservableCollection<Card>();

		public event PropertyChangedEventHandler PropertyChanged;

		internal CardReader(SCardInterface sCardInterface, string readerName)
		{
			this.sCardInterface = sCardInterface;
			this.readerName = readerName;
		}

		public void UpdateCardReader()
		{
			deviceInstanceId = sCardInterface.GetReaderDeviceInstanceId(readerName);

			icon = sCardInterface.GetReaderIcon(readerName);

			try
			{
				readerProperties = sCardInterface.GetReaderProperties(readerName);
			}
			catch (Exception ex)
			{
				// Non-critical
				Logger.Warn(ex);
			}

			cards.Add(new Card());
		}

		public string Name
		{
			get { return readerName; }
		}

		public string DeviceInstanceId
		{
			get { return deviceInstanceId; }
		}

		public IDictionary<string, string> Properties
		{
			get { return readerProperties; }
		}

		public override string ToString()
		{
			return readerName;
		}

		public Image Icon
		{
			get { return icon; }
		}

		public string State
		{
			get { return state; }
		}

		public IList<Card> Cards
		{
			get { return cards;  }
		}

		public Card Card
		{
			get { return ((cards != null) && (cards.Count > 0)) ? cards[0] : null; }
		}

		public string CardATR
		{
			get { return atr; }
		}
	}
}

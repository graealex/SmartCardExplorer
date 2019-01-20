using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCardExplorer.Windows
{
	public class CardReader : INotifyPropertyChanged
	{
		private SCardInterface sCardInterface;
		private string readerName;
		private string deviceInstanceId;

		private ObservableCollection<Card> cards;

		public event PropertyChangedEventHandler PropertyChanged;

		internal CardReader(SCardInterface sCardInterface, string readerName)
		{
			this.sCardInterface = sCardInterface;
			this.readerName = readerName;

			FillCardReader();
		}

		private void FillCardReader()
		{
			deviceInstanceId = sCardInterface.GetReaderDeviceInstanceId(readerName);

			cards = new ObservableCollection<Card>();
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

		public override string ToString()
		{
			return readerName;
		}

		public IList<Card> Cards
		{
			get { return cards;  }
		}

		public Card Card
		{
			get { return cards[0]; }
		}
	}
}

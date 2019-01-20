using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCardExplorer;

namespace SmartCardExplorer.Windows
{
	public class CardReaderHost : INotifyPropertyChanged
	{
		private SCardInterface sCardInterface;
		private ObservableCollection<CardReader> cardReaders;

		public event PropertyChangedEventHandler PropertyChanged;

		internal CardReaderHost(SCardInterface sCardInterface)
		{
			this.sCardInterface = sCardInterface;
			sCardInterface.EstablishContext(SCardInterface.SCOPE.User);
			cardReaders = new ObservableCollection<CardReader>();

			FillCardReaders();
		}

		public static CardReaderHost GetCardReaderHost()
		{
			SCardInterface sCardInterface = new SCardInterface();

			return new CardReaderHost(sCardInterface);
		}

		private void FillCardReaders()
		{
			string[] readerNames = sCardInterface.ListReaders();

			cardReaders.Clear();

			foreach (string readerName in readerNames)
			{
				CardReader cardReader = new CardReader(sCardInterface, readerName);
				cardReaders.Add(cardReader);
			}
		}

		public void OnCardReaderAdded()
		{
			OnCardReadersChanged();
		}

		public void OnCardReaderRemoved()
		{
			OnCardReadersChanged();
		}

		public void OnCardReadersChanged()
		{
			FillCardReaders();
		}

		public IList<CardReader> CardReaders
		{
			get { return cardReaders; }
		}

	}
}

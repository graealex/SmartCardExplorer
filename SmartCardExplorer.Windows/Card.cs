using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCardExplorer.Windows
{
	public class Card
	{
		public string Name
		{
			get { return "Card"; }
		}

		public string[] Interfaces
		{
			get { return new string[] { "Interface1", "Interface2", "Interface3" }; }
		}
	}
}

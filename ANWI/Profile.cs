using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	/// <summary>
	/// A user's profile.
	/// Incudings things like name, rank, rates, etc
	/// </summary>
	public class Profile {
		public string nickname { get; set; }
		public Rank rank;
		public List<Rate> rates = new List<Rate>();
		public int primaryRate { get; set; }
		public Vessel assignedShip = new Vessel();

		public Rank wpfRank { get { return rank; } }
		public Rate wpfPrimaryRate { get { return rates[primaryRate]; } }
		public List<Rate> wpfRates { get { return rates; } }
	}
}

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
	public struct Profile {
		public string nickname;
		public Rank rank;
		public List<Rate> rates;
		public int primaryRate;
		public Vessel assignedShip;
	}
}

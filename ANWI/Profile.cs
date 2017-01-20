using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	/// <summary>
	/// A user's profile.
	/// Incudings things like name, rank, rates, etc
	/// </summary>
	public class Profile {
		public string nickname { get; set; }
		public Rank rank;
		public List<Rate> rates = null;
		public int primaryRate { get; set; }
		public Vessel assignedShip = null;

		public Rank wpfRank { get { return rank; } }
		public Rate wpfPrimaryRate { get { return rates[primaryRate]; } }
		public List<Rate> wpfRates { get { return rates; } }

		public static Profile FromDatamodel(Datamodel.User u, List<Datamodel.StruckRate> r) {
			Profile p = new Profile();

			p.nickname = u.name;
			p.rank = Rank.FromDatamodel(u.Rank);
			p.rates = Rate.FromDatamodel(r);
			
			// Go through the rate array and find which one matches the primary
			for(int i = 0; i < r.Count; ++i) {
				if (r[i].id == u.rate) {
					p.primaryRate = i;
					break;
				}
			}

			return p;
		}
	}
}

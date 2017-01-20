using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Rate {
		public int id;
		public string name;
		public string abbrev;
		public int rank;
		public string date { get; set; }
		public string expires;

		public string getClass() {
			if (rank == 1)
				return "1st";
			else if (rank == 2)
				return "2nd";
			else
				return "3rd";
		}

		public string FullName { get { return name + " " + getClass() + " Class"; } }

		public string Icon { get { return "images/rates/" + abbrev + rank.ToString() + ".png";  } }

		public static Rate FromDatamodel(Datamodel.StruckRate sr) {
			Rate r = new Rate();

			r.id = sr.id;
			r.name = sr.Rate.name;
			r.abbrev = sr.Rate.abrv;
			r.rank = sr.rank;
			r.date = "TODO";
			r.expires = "TODO";

			return r;
		}

		public static List<Rate> FromDatamodel(List<Datamodel.StruckRate> l) {
			List<Rate> lout = new List<Rate>();

			foreach(Datamodel.StruckRate r in l) {
				lout.Add(FromDatamodel(r));
			}

			return lout;
		}
	}
}

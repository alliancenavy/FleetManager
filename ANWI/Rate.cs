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

		public static Rate UNDESIGNATED = new Rate() {
			id = 0,
			name = "Undesignated Spaceman",
			abbrev = "UN",
			rank = 3
		};

		public string getClass() {
			if (rank == 1)
				return "1st";
			else if (rank == 2)
				return "2nd";
			else
				return "3rd";
		}

		public string FullName { get { return name + " " + getClass() + " Class"; } }

		public string FullAbbrev { get { return abbrev + rank.ToString(); } }

		public string Icon { get { return "images/rates/" + FullAbbrev + ".png";  } }

		public static Rate FromDatamodel(Datamodel.StruckRate sr) {
			if (sr == null) {
				return UNDESIGNATED;
			} else {
				Rate r = new Rate();

				r.id = sr.id;
				r.name = sr.Rate.name;
				r.abbrev = sr.Rate.abrv;
				r.rank = sr.rank;
				r.date = "TODO";
				r.expires = "TODO";

				return r;
			}
		}

		public static Rate FromDatamodel(Datamodel.Rate dr) {
			Rate r = new Rate();

			r.id = dr.id;
			r.name = dr.name;
			r.abbrev = dr.abrv;
			r.rank = 3;
			r.date = "TODO";
			r.expires = "TODO";

			return r;
		}

		public static List<Rate> FromDatamodel(List<Datamodel.StruckRate> l) {
			List<Rate> lout = new List<Rate>();

			if (l != null) {
				foreach (Datamodel.StruckRate r in l) {
					lout.Add(FromDatamodel(r));
				}
			}

			return lout;
		}

		public override int GetHashCode() {
			return id.GetHashCode();
		}

		public bool Equals(Rate other) {
			if (other is Rate)
				return other == this;
			else return false;
		}

		public sealed override bool Equals(object other) {
			if (other is Rate)
				return (other as Rate).id == this.id;
			else
				return base.Equals(other);
		}

		public static bool operator ==(Rate a, Rate b) {
			return a.id == b.id;
		}

		public static bool operator !=(Rate a, Rate b) {
			return a.id != b.id;
		}
	}
}

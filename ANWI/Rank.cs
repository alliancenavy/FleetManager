using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Rank {

		#region Instance Variables
		public int id;
		public string name { get; set; }
		public string abbrev { get; set; }
		public int ordering { get; set; }
		#endregion

		#region WPF helpers
		public string icon { get { return "images/ranks/" + ordering + ".png"; } }
		#endregion

		#region Constructors
		private Rank() {
			id = 0;
			name = "";
			abbrev = "";
			ordering = 0;
		}

		private Rank(Datamodel.Rank r) {
			id = r.id;
			name = r.name;
			abbrev = r.abrv;
			ordering = r.ordering;
		}

		public static Rank FetchById(int id) {
			Datamodel.Rank r = null;
			if(Datamodel.Rank.FetchById(ref r, id)) {
				return new Rank(r);
			} else {
				return null;
			}
		}

		public static List<Rank> FetchAll() {
			List<Datamodel.Rank> dbRanks = null;
			Datamodel.Rank.FetchAll(ref dbRanks);

			return dbRanks.ConvertAll<Rank>((a) => { return new Rank(a); });
		}
		#endregion
	}
}

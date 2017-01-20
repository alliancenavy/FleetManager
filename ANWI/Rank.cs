using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Rank {
		public int id;
		public string name { get; set; }
		public string abbrev { get; set; }
		public int ordering;

		public string Icon { get { return "images/ranks/" + ordering + ".png"; } }

		public static Rank FromDatamodel(Datamodel.Rank dr) {
			Rank r = new Rank();

			r.id = dr.id;
			r.name = dr.name;
			r.abbrev = dr.abrv;
			r.ordering = dr.ordering;

			return r;
		}
	}
}

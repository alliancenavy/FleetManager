using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public struct Rank {
		public int id;
		public string name { get; set; }
		public string abbrev { get; set; }
		public int ordering;

		public string Icon { get { return "images/ranks/" + ordering + ".png"; } }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public struct Rate {
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
	}
}

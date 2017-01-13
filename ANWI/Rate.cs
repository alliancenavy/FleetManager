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
		public string icon;
		public int rank;

		public string getClass() {
			if (rank == 1)
				return "1st";
			else if (rank == 2)
				return "2nd";
			else
				return "3rd";
		}
	}
}

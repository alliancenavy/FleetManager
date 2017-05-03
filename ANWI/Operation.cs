using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public class Operation {
		public enum Type {
			PATROL,
			ASSAULT,
			DEFENSE,
			MINING,
			LOGISTICS
		}

		public enum Status {
			STAGING = 0,
			SORTIED = 1,
			DISMISSING = 2
		}

		public string name { get; set; }
		public Type type { get; set; }
		public Status status { get; set; }
		public int currentMembers { get; set; }
		public int neededMembers { get; set; }
		public int totalSlots { get; set; }

		public int id;

		public string typeString {
			get {
				switch(type) {
					case Type.PATROL:
						return "Patrol";
					case Type.ASSAULT:
						return "Assault";
					case Type.DEFENSE:
						return "Defense";
					case Type.MINING:
						return "Mining";
					case Type.LOGISTICS:
						return "Logistics";
					default:
						return "";
				}
			}
		}

		public string statusString {
			get {
				switch(status) {
					case Status.SORTIED:
						return "Sortied";
					case Status.STAGING:
						return "Staging";
					case Status.DISMISSING:
						return "Dismissing";
					default:
						return "";
				}
			}
		}
	}
}

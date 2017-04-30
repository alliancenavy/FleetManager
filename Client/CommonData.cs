using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;

namespace Client {
	public static class CommonData {

		public static List<Rank> ranks { get; private set; } = null;
		public static List<Rate> rates { get; private set; } = null;
		public static List<AssignmentRole> assignmentRoles { get; private set; } = null;
		public static List<Hull> hulls { get; private set; } = null;

		public static bool loaded { get; private set; } = false;

		public static void LoadAll(ANWI.Messaging.AllCommonData acd) {
			ranks = acd.ranks;
			rates = acd.rates;
			assignmentRoles = acd.assignmentRoles;
			hulls = acd.hulls;

			loaded = true;
		}

	}
}

using System.Collections.Generic;
using ANWI;

namespace Client {

	/// <summary>
	/// Static class which stores data used across the program such as
	/// the list of rates, ranks, etc
	/// </summary>
	public static class CommonData {

#if DEBUG
		public static readonly string serverAddress = "wss://localhost:9000";
#else
		public static readonly string serverAddress 
			= "wss://apps.alliancenavy.org:9000";
#endif

		public static List<Rank> ranks { get; private set; } = null;
		public static List<Rate> rates { get; private set; } = null;
		public static List<AssignmentRole> assignmentRoles { get; private set; }
			= null;
		public static List<Hull> largeHulls { get; private set; } = null;
		public static List<Hull> smallHulls { get; private set; } = null;

		public static bool loaded { get; private set; } = false;

		/// <summary>
		/// Loads all data from the server message.  Sets the loaded flag
		/// when done so the program can continue on.
		/// </summary>
		/// <param name="acd"></param>
		public static void LoadAll(ANWI.Messaging.AllCommonData acd) {
			ranks = acd.ranks;
			rates = acd.rates;
			assignmentRoles = acd.assignmentRoles;
			largeHulls = acd.largeHulls;
			smallHulls = acd.smallHulls;

			loaded = true;
		}

	}
}

using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	
	/// <summary>
	/// The actual job of a user in an assignment
	/// </summary>
	public class AssignmentRole {
		#region Instance Variables
		public int id;

		// Name of the job
		public string name { get; set; }

		// True if the user is considered "Ship's Company"
		// False for embarked personnel such as Marines/Pilots
		public bool isCompany { get; set; }
		#endregion

		#region Constructors
		public AssignmentRole() {
			id = 0;
			name = "";
			isCompany = false;
		}

		private AssignmentRole(Datamodel.AssignmentRole ar) {
			id = ar.id;
			name = ar.name;
			isCompany = ar.isCompany;
		}

		/// <summary>
		/// Gets a role by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static AssignmentRole FetchById(int id) {
			Datamodel.AssignmentRole ar = null;
			if(Datamodel.AssignmentRole.FetchById(ref ar, id)) {
				return new AssignmentRole(ar);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a list of all possible roles
		/// </summary>
		/// <returns></returns>
		public static List<AssignmentRole> FetchAll() {
			List<Datamodel.AssignmentRole> all = null;
			Datamodel.AssignmentRole.FetchAll(ref all);
			return all.ConvertAll<AssignmentRole>((a) => { return new AssignmentRole(a); });
		}
		#endregion

		public override string ToString() {
			if (isCompany)
				return name + " (Company)";
			else
				return name;
		}
	}
}

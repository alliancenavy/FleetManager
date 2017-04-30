using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class AssignmentRole {
		#region Instance Variables
		public int id;
		public string name { get; set; }
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

		public static AssignmentRole FetchById(int id) {
			Datamodel.AssignmentRole ar = null;
			if(Datamodel.AssignmentRole.FetchById(ref ar, id)) {
				return new AssignmentRole(ar);
			} else {
				return null;
			}
		}

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

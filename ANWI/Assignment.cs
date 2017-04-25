using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Assignment {
		public string shipName;
		public string roleName;
		public bool isCompany;

		public string FullText { get { return $"{roleName} on {shipName}"; } }

		public Assignment() {

		}

		public Assignment(string ship, string role, bool company) {
			shipName = ship;
			roleName = role;
			isCompany = company;
		}

		public static Assignment FromDatamodel(Datamodel.Assignment a) {
			Assignment output = new Assignment();

			output.shipName = a.Ship.name;
			output.roleName = a.Role.name;
			output.isCompany = a.Role.isCompany;

			return output;
		}
	}
}

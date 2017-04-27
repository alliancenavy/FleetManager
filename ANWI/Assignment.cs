using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Assignment {
		#region Instance Variables
		public int id;

		private int _userId;

		private int _shipId;
		public string shipName { get; set; }

		private int _roleId;
		public string roleName { get; set; }
		public bool roleIsCompany { get; set; }

		// TODO: dates
		#endregion

		#region WPF Helpers
		public string FullText { get { return $"{roleName} on {shipName}"; } }
		#endregion

		#region Constructors
		private Assignment() {
			id = 0;
			_userId = 0;
			_shipId = 0;
			shipName = "";
			_roleId = 0;
			roleName = "";
			roleIsCompany = false;
		}

		private Assignment(Datamodel.Assignment a) {
			id = a.id;

			_userId = a.user;

			_shipId = a.ship;
			Datamodel.UserShip ship = null;
			if (!Datamodel.UserShip.FetchById(ref ship, _shipId))
				throw new ArgumentException("Assignment does not have valid ship ID");
			shipName = ship.name;

			_roleId = a.role;
			Datamodel.AssignmentRole role = null;
			if (!Datamodel.AssignmentRole.FetchById(ref role, _roleId))
				throw new ArgumentException("Assignment does not have valid role ID");
			roleName = role.name;
			roleIsCompany = role.isCompany;
		}

		public static Assignment FetchById(int id) {
			Datamodel.Assignment a = null;
			if(Datamodel.Assignment.FetchById(ref a, id)) {
				return new Assignment(a);
			} else {
				return null;
			}
		}

		public static Assignment FetchCurrentAssignment(int userId) {
			Datamodel.Assignment a = null;
			if(Datamodel.Assignment.FetchCurrentAssignment(ref a, userId)) {
				return new Assignment(a);
			} else {
				return null;
			}
		}
		#endregion
	}
}

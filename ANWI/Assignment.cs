using System;
using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;

namespace ANWI {

	/// <summary>
	/// An assignment on a ship for a ship
	/// </summary>
	public class Assignment {
		#region Instance Variables
		public int id;

		// The user this is an assignment for
		private int _userId;

		// The ship this assignment is one
		private int _shipId;
		public string shipName { get; set; }
		public string shipHullNumber { get; set; }

		// The user's job on this ship
		private int _roleId;
		public string roleName { get; set; }
		public bool roleIsCompany { get; set; }

		// Dates this assignment was active for
		public DateTime startDate { get; set; }
		public bool hasEndDate { get; set; }
		public DateTime endDate { get; set; }
		#endregion
		
		#region WPF Helpers
		public string fullText { get {
				return $"{roleName} on {shipName} ({startDateFormatted})"; } }
		public string shortText { get { return roleName; } }
		public string startDateFormatted { get {
				return startDate.ToString("dd MMM yyyy"); } }
		public string endDateFormatted {
			get {
				if (hasEndDate)
					return endDate.ToString("dd MMM yyyy");
				else
					return "Present";
			}
		}
		public string dateRange { get {
				return $"{startDateFormatted} - {endDateFormatted}"; } }
		#endregion

		#region Constructors
		public Assignment() {
			id = 0;
			_userId = 0;
			_shipId = 0;
			shipName = "";
			_roleId = 0;
			roleName = "";
			roleIsCompany = false;
			hasEndDate = false;
		}

		private Assignment(Datamodel.Assignment a) {
			id = a.id;

			_userId = a.user;

			_shipId = a.ship;
			Vessel ship = Vessel.FetchById(_shipId);
			if (ship == null)
				throw new ArgumentException(
					"Assignment does not have valid ship ID");
			shipName = ship.name;
			shipHullNumber = ship.fullHullNumber;

			_roleId = a.role;
			Datamodel.AssignmentRole role = null;
			if (!Datamodel.AssignmentRole.FetchById(ref role, _roleId))
				throw new ArgumentException(
					"Assignment does not have valid role ID");
			roleName = role.name;
			roleIsCompany = role.isCompany;

			startDate = DateTimeOffset.FromUnixTimeSeconds(a.from).DateTime; 
			if (a.until != -1) {
				hasEndDate = true;
				endDate = DateTimeOffset.FromUnixTimeSeconds(a.until).DateTime;
			} else {
				hasEndDate = false;
			}
		}

		/// <summary>
		/// Gets an assignment by its ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Assignment FetchById(int id) {
			Datamodel.Assignment a = null;
			if(Datamodel.Assignment.FetchById(ref a, id)) {
				return new Assignment(a);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a user's current assignment
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static Assignment FetchCurrentAssignment(int userId) {
			Datamodel.Assignment a = null;
			if(Datamodel.Assignment.FetchCurrentAssignment(ref a, userId)) {
				return new Assignment(a);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a user's full assignment history.
		/// Ordered from most recent to oldest
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<Assignment> FetchAssignmentHistory(int userId) {
			List<Datamodel.Assignment> history = null;
			Datamodel.Assignment.FetchAssignmentHistory(ref history, userId);
			return history.ConvertAll<Assignment>(
				(a) => { return new Assignment(a); });
		}
		#endregion
	}
}

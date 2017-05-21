using System;
using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class OperationRole {
		#region Instance Members
		public int id { get; set; }
		public string name { get; set; }

		public int rateId { get; set; }
		public string rateAbbrev { get; set; }

		public bool channelCdr { get; set; }
		#endregion

		#region WPF Helpers
		public string associatedRateIcon {
			get { return $"/images/rates/{rateAbbrev}0.png"; }
		}
		#endregion

		#region Constructors
		public OperationRole() {
			id = -1;
			name = "";
			rateId = -1;
			rateAbbrev = "UD";
			channelCdr = false;
		}

		private OperationRole(Datamodel.OperationRole r) {
			id = r.id;
			name = r.name;

			rateId = r.rate;
			Datamodel.Rate rate = null;
			if (!Datamodel.Rate.FetchById(ref rate, rateId))
				throw new ArgumentException("Role does not have valid rate");
			rateAbbrev = rate.abrv;

			channelCdr = r.channelCdr;
		}

		/// <summary>
		/// Fetches a given role
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static OperationRole FetchById(int id) {
			Datamodel.OperationRole r = null;
			if(Datamodel.OperationRole.FetchById(ref r, id)) {
				return new OperationRole(r);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Fetches all roles which can be assigned to a ship in an op
		/// </summary>
		/// <returns></returns>
		public static List<OperationRole> FetchAllShips() {
			List<Datamodel.OperationRole> roles = null;
			if(Datamodel.OperationRole.FetchAllShips(ref roles)) {
				return roles.ConvertAll<OperationRole>(
					(r) => { return new OperationRole(r); }
				);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Fetches all roles which can be assigned to a boat in an op
		/// </summary>
		/// <returns></returns>
		public static List<OperationRole> FetchAllBoats() {
			List<Datamodel.OperationRole> roles = null;
			if (Datamodel.OperationRole.FetchAllBoats(ref roles)) {
				return roles.ConvertAll<OperationRole>(
					(r) => { return new OperationRole(r); }
				);
			} else {
				return null;
			}
		}
		#endregion
	}
}

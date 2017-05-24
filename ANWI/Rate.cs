using System;
using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;

namespace ANWI {

	/// <summary>
	/// A rating.  Used for describing both the rates themselves and a 
	/// struck rate for a user.
	/// </summary>
	public class Rate {
		#region Instance Variables
		// The ID of the rate itself
		public int rateId;
		// The ID of this user's struck instance
		public int struckId;
		public string name { get; set; }
		public string abbrev { get; set; }
		public int rank { get; set; }
		public DateTime earnedDate;
		public bool hasExpirationDate;
		public DateTime expirationDate;
		#endregion

		#region WPF Helpers
		public string wpfEarnedDate { get {
				return "As of: " + earnedDate.ToString("dd MMM yyyy"); } }
		public string wpfExpirationDate {
			get {
				if (hasExpirationDate)
					return "Expires: " + expirationDate.ToString("dd MMM yyyy");
				else
					return "Expires: Never";
			}
		}
		#endregion

		#region Constants
		/// <summary>
		/// A pre-made undesignated rate for users who have not selected
		/// a primary rate.
		/// </summary>
		public static Rate UNDESIGNATED = new Rate() {
			rateId = 0,
			struckId = -1,
			name = "Undesignated Spaceman",
			abbrev = "UD",
			rank = 3
		};
		#endregion

		#region WPF Helpers
		public string classString {
			get {
				if (rank == 1)
					return "1st";
				else if (rank == 2)
					return "2nd";
				else
					return "3rd";
			}
		}

		public string fullName { get {
				return $"{name} {classString} Class"; } }

		public string fullAbbrev { get {
				return $"{abbrev}{rank.ToString()}"; } }

		public string icon { get {
				return $"images/rates/{fullAbbrev}.png"; } }
		#endregion

		#region Constructors
		public Rate() {
			rateId = 0;
			struckId = -1;
			name = "";
			abbrev = "";
			rank = 0;
			hasExpirationDate = false;
		}

		private Rate(Datamodel.Rate r) {
			rateId = r.id;
			struckId = -1;
			name = r.name;
			abbrev = r.abrv;
			rank = 3;
			hasExpirationDate = false;
		}

		private Rate(Datamodel.StruckRate sr) {
			rateId = sr.rate;
			struckId = sr.id;
			rank = sr.rank;

			Datamodel.Rate r = null;
			if (!Datamodel.Rate.FetchById(ref r, rateId))
				throw new ArgumentException(
					"Struck rate does not have a valid rate id");

			name = r.name;
			abbrev = r.abrv;

			earnedDate = 
				DateTimeOffset.FromUnixTimeSeconds(sr.earnedDate).DateTime;
			if(sr.expirationDate != -1) {
				hasExpirationDate = true;
				expirationDate = DateTimeOffset.FromUnixTimeSeconds(
					sr.expirationDate).DateTime;
			} else {
				hasExpirationDate = false;
			}
		}

		/// <summary>
		/// Gets a rate by the ID of that rate
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Rate FetchByRateId(int id) {
			Datamodel.Rate r = null;
			if(Datamodel.Rate.FetchById(ref r, id)) {
				return new Rate(r);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a list of all rates
		/// </summary>
		/// <returns></returns>
		public static List<Rate> FetchAllRates() {
			List<Datamodel.Rate> dbRates = null;
			Datamodel.Rate.FetchAll(ref dbRates);
			return dbRates.ConvertAll<Rate>(
				(a) => { return new ANWI.Rate(a); });
		}

		/// <summary>
		/// Gets a rate struck by a user
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="rateId">The rate ID, NOT the struck ID</param>
		/// <returns></returns>
		public static Rate FetchUsersRate(int userId, int rateId) {
			Datamodel.StruckRate sr = null;
			if(Datamodel.StruckRate.FetchByUserStruckId(ref sr, userId, rateId)) {
				return new Rate(sr);
			} else {
				return Rate.UNDESIGNATED;
			}
		}

		/// <summary>
		/// Gets all of the rates a user has struck
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<Rate> FetchUserRates(int userId) {
			List<Datamodel.StruckRate> dbRates = null;
			if (Datamodel.StruckRate.FetchByUserId(ref dbRates, userId)) {
				return dbRates.ConvertAll<Rate>(
					(a) => { return new ANWI.Rate(a); });
			} else {
				return null;
			}
		}

		#endregion

		#region Equality
		/// <summary>
		/// Equality functions necessary for combobox
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			return rateId.GetHashCode();
		}

		public bool Equals(Rate other) {
			if (other is Rate)
				return other == this;
			else return false;
		}

		public sealed override bool Equals(object other) {
			Rate ro = other as Rate;
			if (ro != null)
				return ro.rateId == this.rateId && ro.rank == this.rank;
			else
				return base.Equals(other);
		}

		public static bool operator ==(Rate a, Rate b) {
			if (ReferenceEquals(a, null)) {
				if (ReferenceEquals(b, null))
					return true;
				else
					return false;
			} else {
				if (ReferenceEquals(b, null))
					return false;
				else
					return a.rateId == b.rateId && a.rank == b.rank;
			}
		}

		public static bool operator !=(Rate a, Rate b) {
			if(ReferenceEquals(a, null)) {
				if (ReferenceEquals(b, null))
					return false;
				else
					return true;
			} else {
				if (ReferenceEquals(b, null))
					return true;
				else
					return a.rateId != b.rateId || 
						(a.rateId == b.rateId && a.rank != b.rank);
			}
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
	public class Rate {
		#region Instance Variables
		public int rateId;
		public int struckId;
		public string name { get; set; }
		public string abbrev { get; set; }
		public int rank { get; set; }
		public DateTime earnedDate;
		public bool hasExpirationDate;
		public DateTime expirationDate;
		#endregion

		#region WPF Helpers
		public string wpfEarnedDate { get { return "As of: " + earnedDate.ToString("dd MMM yyyy"); } }
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

		public string fullName { get { return $"{name} {classString} Class"; } }

		public string fullAbbrev { get { return $"{abbrev}{rank.ToString()}"; } }

		public string icon { get { return $"images/rates/{fullAbbrev}.png"; } }
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
				throw new ArgumentException("Struck rate does not have a valid rate id");

			name = r.name;
			abbrev = r.abrv;

			earnedDate = DateTimeOffset.FromUnixTimeSeconds(sr.earnedDate).DateTime;
			if(sr.expirationDate != -1) {
				hasExpirationDate = true;
				expirationDate = DateTimeOffset.FromUnixTimeSeconds(sr.expirationDate).DateTime;
			} else {
				hasExpirationDate = false;
			}
		}

		public static Rate FetchByRateId(int id) {
			Datamodel.Rate r = null;
			if(Datamodel.Rate.FetchById(ref r, id)) {
				return new Rate(r);
			} else {
				return null;
			}
		}

		public static List<Rate> FetchAllRates() {
			List<Datamodel.Rate> dbRates = null;
			Datamodel.Rate.FetchAll(ref dbRates);
			return dbRates.ConvertAll<Rate>((a) => { return new ANWI.Rate(a); });
		}

		public static Rate FetchUsersRate(int userId, int rateId) {
			Datamodel.StruckRate sr = null;
			if(Datamodel.StruckRate.FetchByUserRate(ref sr, userId, rateId)) {
				return new Rate(sr);
			} else {
				return Rate.UNDESIGNATED;
			}
		}

		public static List<Rate> FetchUserRates(int userId) {
			List<Datamodel.StruckRate> dbRates = null;
			if (Datamodel.StruckRate.FetchByUserId(ref dbRates, userId)) {
				return dbRates.ConvertAll<Rate>((a) => { return new ANWI.Rate(a); });
			} else {
				return null;
			}
		}

		#endregion

		#region Equality
		public override int GetHashCode() {
			return rateId.GetHashCode();
		}

		public bool Equals(Rate other) {
			if (other is Rate)
				return other == this;
			else return false;
		}

		public sealed override bool Equals(object other) {
			if (other is Rate)
				return (other as Rate).rateId == this.rateId;
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
					return a.rateId == b.rateId;
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
					return a.rateId == b.rateId;
			}
		}
		#endregion
	}
}

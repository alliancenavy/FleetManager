using System;
using System.Collections.Generic;
using Datamodel = ANWI.Database.Model;

namespace ANWI {

	/// <summary>
	/// A type of ship
	/// </summary>
	public class Hull {
		#region Instance Variables
		public int id;
		public string name { get; set; }
		public string symbol { get; set; }
		public string manufacturer { get; set; }
		public int ordering;

		private int _roleId;
		public string role { get; set; }

		private int _vendorId;
		public string vendor { get; set; }
		#endregion

		#region WPF Helpers
		public override string ToString() {
			return $"{name} ({symbol})";
		}
		#endregion

		#region Constructors
		public Hull() {
			id = 0;
			role = "";
			name = "";
			symbol = "";
			manufacturer = "";
			ordering = 0;
		}

		private Hull(Datamodel.Hull h) {
			id = h.id;
			name = h.series;
			symbol = h.symbol;
			ordering = h.ordering;

			_roleId = h.role;
			Datamodel.HullRole hr = null;
			if (!Datamodel.HullRole.FetchById(ref hr, _roleId))
				throw new ArgumentException(
					"Hull does not have valid role ID");
			role = hr.name;

			_vendorId = h.vendor;
			Datamodel.HullVendor hv = null;
			if (!Datamodel.HullVendor.FetchById(ref hv, _vendorId))
				throw new ArgumentException(
					"Hull does not have valid vendor ID");
			vendor = hv.name;
		}
		
		/// <summary>
		/// Gets a hull by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static Hull FetchById(int id) {
			Datamodel.Hull h = null;
			if(Datamodel.Hull.FetchById(ref h, id)) {
				return new Hull(h);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets a list of all possible hulls
		/// </summary>
		/// <returns></returns>
		public static List<Hull> FetchAll() {
			List<Datamodel.Hull> all = null;
			Datamodel.Hull.FetchAll(ref all);
			return all.ConvertAll<Hull>((h) => { return new Hull(h); });
		}

		/// <summary>
		/// Gets only large, name-bearing hulls like the Idris
		/// </summary>
		/// <returns></returns>
		public static List<Hull> FetchLarge() {
			List<Datamodel.Hull> all = null;
			Datamodel.Hull.FetchLarge(ref all);
			return all.ConvertAll<Hull>((h) => { return new Hull(h); });
		}

		/// <summary>
		/// Get's only small hulls like fighters and gunships
		/// </summary>
		/// <returns></returns>
		public static List<Hull> FetchSmall() {
			List<Datamodel.Hull> all = null;
			Datamodel.Hull.FetchSmall(ref all);
			return all.ConvertAll<Hull>((h) => { return new Hull(h); });
		}
		#endregion
	}
}

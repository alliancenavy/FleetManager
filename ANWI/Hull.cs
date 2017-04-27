using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datamodel = ANWI.Database.Model;

namespace ANWI {
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

		#region Constructors
		private Hull() {
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
				throw new ArgumentException("Hull does not have valid role ID");
			role = hr.name;

			_vendorId = h.vendor;
			Datamodel.HullVendor hv = null;
			if (!Datamodel.HullVendor.FetchById(ref hv, _vendorId))
				throw new ArgumentException("Hull does not have valid vendor ID");
			vendor = hv.name;
		}
		
		public static Hull FetchById(int id) {
			Datamodel.Hull h = null;
			if(Datamodel.Hull.FetchById(ref h, id)) {
				return new Hull(h);
			} else {
				return null;
			}
		}
		#endregion
	}
}

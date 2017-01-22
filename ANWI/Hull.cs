using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public class Hull {
		public int id;
		public string role { get; set; }
		public string type { get; set; }
		public string symbol { get; set; }
		public string manufacturer;
		public int ordering;

		public static Hull FromDatamodel(ANWI.Database.Model.Hull dmhull) {
			Hull h = new Hull();

			h.id = dmhull.id;
			h.role = dmhull.Role.name;
			h.type = dmhull.series;
			h.symbol = dmhull.symbol;
			h.manufacturer = dmhull.Vendor.name;
			h.ordering = dmhull.ordering;

			return h;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	public struct Hull {
		public int id;
		public string role { get; set; }
		public string type { get; set; }
		public string subtype { get; set; }
		public string symbol { get; set; }
		public string manufacturer;
		public int ordering;
	}
}

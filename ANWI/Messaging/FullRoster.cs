using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class FullRoster : IMessagePayload {
		public List<LiteProfile> members = null;

		public FullRoster() {
			members = null;
		}

		public FullRoster(List<LiteProfile> p) {
			members = p;
		}

		public override string ToString() {
			return "Type: FullRoster. Count: " + members.Count;
		}
	}
}

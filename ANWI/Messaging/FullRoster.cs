using System.Collections.Generic;

namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// A list of all users on the roster
	/// </summary>
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

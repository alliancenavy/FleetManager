using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {
	public class FullProfile : IMessagePayload {
		public Profile profile;

		public FullProfile() {
			profile = null;
		}

		public FullProfile(Profile p) {
			profile = p;
		}

		public override string ToString() {
			return $"Type: FullProfile.";
		}
	}
}

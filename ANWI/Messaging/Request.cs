using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI.Messaging {

	public class Request : MessagePayload {
		public enum Type {
			GetVesselList
		}

		public Type type;

		public Request(Type t) {
			type = t;
		}

		public override string ToString() {
			return "Type: Request.  Request Type: " + type.ToString();
		}
	}
}

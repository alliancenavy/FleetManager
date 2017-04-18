using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace ANWI.Messaging {

	public class Request : IMessagePayload {
		
		public enum Type {
			None = 0,
			GetFleet = 1,
			GetOperations = 2
		}

		public Type type { get; set; }

		public Request() {
			type = Type.None;
		}

		public Request(Type t) {
			type = t;
		}

		public override string ToString() {
			return "Type: Request.  Request Type: " + type.ToString();
		}
	}
}

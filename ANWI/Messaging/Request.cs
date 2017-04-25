using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace ANWI.Messaging {

	public class Request : IMessagePayload {
		
		public enum Type {
			None,
			GetCommonData,
			GetProfile,
			GetFleet,
			GetOperations,
			GetRoster,
			GetShipDetail
		}

		public Type type { get; set; }
		public int id { get; set; }

		public Request() {
			type = Type.None;
		}

		public Request(Type t) {
			type = t;
			id = 0;
		}

		public Request(Type t, int i) {
			type = t;
			id = i;
		}

		public override string ToString() {
			return "Type: Request.  Request Type: " + type.ToString();
		}
	}
}

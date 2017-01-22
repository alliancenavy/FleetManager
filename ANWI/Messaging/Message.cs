using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;

namespace ANWI.Messaging {
	public class Message {

		public struct Routing {
			public enum Target {
				Unknown,
				ServiceRecord,
				VesselReg
			}

			public Target dest;
			public int id;
		}

		public Routing address;

		[MessagePackKnownType("rv", typeof(Request))]
		[MessagePackKnownType("fvr", typeof(FullVesselReg))]
		public MessagePayload payload;

		public Message() {
			address.dest = Routing.Target.Unknown;
			address.id = 0;
			payload = null;
		}

		public Message(Routing addr, MessagePayload data) {
			address = addr;
			payload = data;
		}

		public Message(Routing.Target returnTo, int id, MessagePayload data) {
			address.dest = returnTo;
			address.id = id;
			payload = data;
		}
	}
}

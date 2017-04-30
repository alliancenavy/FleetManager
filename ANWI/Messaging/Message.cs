using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgPack.Serialization;
using WebSocketSharp;
using System.IO;

namespace ANWI.Messaging {
	public class Message {

		public struct Routing {
			public enum Target {
				Unknown,
				Main,
				FleetReg,
				OpDetail
			}

			public Target dest;
			public int id;

			public static readonly Routing NoReturn = new Routing() {
				dest = Target.Unknown,
				id = 0
			};

			public static readonly Routing Main = new Routing() {
				dest = Target.Main,
				id = 0
			};

			public static readonly Routing FleetReg = new Routing() {
				dest = Target.FleetReg,
				id = 0
			};

			public Routing(Target t, int id) {
				dest = t;
				this.id = id;
			}
		}

		public Routing address;
	
		[MessagePackKnownType("rv", typeof(Request))]
		[MessagePackKnownType("fvr", typeof(FullVesselReg))]
		[MessagePackKnownType("cn", typeof(ChangeNickname))]
		[MessagePackKnownType("fol", typeof(FullOperationsList))]
		[MessagePackKnownType("ros", typeof(FullRoster))]
		[MessagePackKnownType("acd", typeof(AllCommonData))]
		[MessagePackKnownType("ar", typeof(AddRate))]
		[MessagePackKnownType("dr", typeof(DeleteRate))]
		[MessagePackKnownType("fpro", typeof(FullProfile))]
		[MessagePackKnownType("ack", typeof(ConfirmUpdate))]
		[MessagePackKnownType("spr", typeof(SetPrimaryRate))]
		[MessagePackKnownType("cr", typeof(ChangeRank))]
		[MessagePackKnownType("fvd", typeof(FullVessel))]
		[MessagePackKnownType("enda", typeof(EndAssignment))]
		[MessagePackKnownType("newa", typeof(NewAssignment))]
		public IMessagePayload payload;

		public Message() {
			payload = null;
		}

		public Message(Routing addr, IMessagePayload data) {
			address = addr;
			payload = data;
		}

		public Message(Routing.Target returnTo, int id, IMessagePayload data) {
			address.dest = returnTo;
			address.id = id;
			payload = data;
		}

		public static void Send(WebSocket sock, Routing returnTo, IMessagePayload data) {
			Message m = new Message(returnTo, data);
			MemoryStream stream = new MemoryStream();
			MessagePackSerializer.Get<Message>().Pack(stream, m);
			sock.Send(stream.ToArray());
		}

		public static Message Receive(byte[] data) {
			MemoryStream stream = new MemoryStream(data);
			return MessagePackSerializer.Get<Message>().Unpack(stream);
		}
	}
}

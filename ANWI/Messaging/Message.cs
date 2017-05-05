using MsgPack.Serialization;
using WebSocketSharp;
using System.IO;

namespace ANWI.Messaging {

	/// <summary>
	/// A message transmitted between the client and server and vice versa
	/// </summary>
	public class Message {

		/// <summary>
		/// Defines where the message is supposed to go when returning
		/// to the client
		/// NOT used on the server
		/// </summary>
		public struct Routing {

			/// <summary>
			/// Destination on the client side
			/// </summary>
			public enum Target {
				Unknown,
				Main,
				FleetReg,
				OpDetail
			}

			public Target dest;
			public int id;

			/// <summary>
			/// Pre-made envelope
			/// This message does not need to be returned.
			/// </summary>
			public static readonly Routing NoReturn = new Routing() {
				dest = Target.Unknown,
				id = 0
			};

			/// <summary>
			/// Pre-made envelope
			/// Return to the main window
			/// </summary>
			public static readonly Routing Main = new Routing() {
				dest = Target.Main,
				id = 0
			};

			/// <summary>
			/// Pre-made envelope
			/// Return to the Fleet window
			/// </summary>
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
	
		/// <summary>
		/// Polymorphic payload
		/// </summary>
		[MessagePackKnownType("logr", typeof(LoginRequest))]
		[MessagePackKnownType("regr", typeof(RegisterRequest))]
		[MessagePackKnownType("lr", typeof(LoginResponse))]
		[MessagePackKnownType("rr", typeof(RegisterResponse))]
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
		[MessagePackKnownType("css", typeof(ChangeShipStatus))]
		[MessagePackKnownType("ns", typeof(NewShip))]
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

		/// <summary>
		/// Serializes the message and sends it to the server
		/// </summary>
		/// <param name="sock"></param>
		/// <param name="returnTo"></param>
		/// <param name="data"></param>
		public static void Send(WebSocket sock, Routing returnTo, 
			IMessagePayload data) {
			Message m = new Message(returnTo, data);
			MemoryStream stream = new MemoryStream();
			MessagePackSerializer.Get<Message>().Pack(stream, m);
			sock.Send(stream.ToArray());
		}

		/// <summary>
		/// Deserializes a message from a byte array
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static Message Receive(byte[] data) {
			MemoryStream stream = new MemoryStream(data);
			return MessagePackSerializer.Get<Message>().Unpack(stream);
		}
	}
}

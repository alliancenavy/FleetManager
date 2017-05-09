using MsgPack.Serialization;
using WebSocketSharp;
using System.IO;

namespace ANWI.Messaging {

	/// <summary>
	/// A message transmitted between the client and server and vice versa
	/// </summary>
	public class Message {

		public int sequence { get; private set; }

		/// <summary>
		/// Polymorphic payload
		/// </summary>
		[MessagePackKnownType("logr", typeof(LoginRequest))]
		[MessagePackKnownType("regr", typeof(RegisterRequest))]
		[MessagePackKnownType("lr", typeof(LoginResponse))]
		[MessagePackKnownType("rr", typeof(RegisterResponse))]
		[MessagePackKnownType("rv", typeof(Request))]
		[MessagePackKnownType("fvr", typeof(FullVesselReg))]
		[MessagePackKnownType("fol", typeof(FullOperationsList))]
		[MessagePackKnownType("ros", typeof(FullRoster))]
		[MessagePackKnownType("acd", typeof(AllCommonData))]
		[MessagePackKnownType("ar", typeof(AddRate))]
		[MessagePackKnownType("fpro", typeof(FullProfile))]
		[MessagePackKnownType("ack", typeof(ConfirmUpdate))]
		[MessagePackKnownType("fvd", typeof(FullVessel))]
		[MessagePackKnownType("enda", typeof(EndAssignment))]
		[MessagePackKnownType("newa", typeof(NewAssignment))]
		[MessagePackKnownType("css", typeof(ChangeShipStatus))]
		[MessagePackKnownType("ns", typeof(NewShip))]
		public IMessagePayload payload;

		public Message() {
			payload = null;
		}

		public Message(int seq, IMessagePayload data) {
			sequence = seq;
			payload = data;
		}

		/// <summary>
		/// Serializes the message and sends it to the server
		/// </summary>
		/// <param name="sock"></param>
		/// <param name="m"></param>
		public static void Send(WebSocket sock, Message m) {
			MemoryStream stream = new MemoryStream();
			MessagePackSerializer.Get<Message>().Pack(stream, m);
			sock.Send(stream.ToArray());
		}

		/// <summary>
		/// Convenience function for sending a message
		/// </summary>
		/// <param name="sock"></param>
		/// <param name="returnTo"></param>
		/// <param name="data"></param>
		public static void Send(WebSocket sock, int sequence, 
			IMessagePayload data) {
			Send(sock, new Message(sequence, data));
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

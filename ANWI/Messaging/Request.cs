using ANWI.Messaging.ReqExp;
using MsgPack.Serialization;

namespace ANWI.Messaging {

	/// <summary>
	/// Client -> Server
	/// A simple request message with a type and integer argument
	/// </summary>
	public class Request : IMessagePayload {

		/// <summary>
		/// Type of request
		/// </summary>
		public enum Type {
			None,

			//
			// Get Requests
			GetCommonData,
			GetProfile,
			GetFleet,
			GetOperations,
			GetRoster,
			GetVesselDetail,
			GetUnassignedRoster,

			//
			// Change requests
			ChangeRank,				// Detail: UserIdPlus
			DeleteRate,				// Detail: UserIdPlus
			SetPrimaryRate,			// Detail: UserIdPlus
			ChangeName				// Detail: IdString
		}

		public Type type { get; set; }
		public int id { get; set; }

		[MessagePackKnownType("uidp", typeof(UserIdPlus))]
		[MessagePackKnownType("idstr", typeof(IdString))]
		public IRequestDetail detail { get; set; }

		public Request() {
			type = Type.None;
		}

		public Request(Type t) {
			type = t;
			id = 0;
			detail = null;
		}

		public Request(Type t, int i) {
			type = t;
			id = i;
			detail = null;
		}

		public Request(Type t, IRequestDetail d) {
			type = t;
			id = 0;
			detail = d;
		}

		public override string ToString() {
			return $"Type: Request ({type.ToString()})";
		}
	}
}

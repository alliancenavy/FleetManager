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
			GetAvailableFleet,
			GetOperations,
			GetRoster,
			GetVesselDetail,
			GetUnassignedRoster,

			//
			// Change requests
			ChangeRank,				// Detail: TwoIDs
			DeleteRate,				// Detail: TwoIDs
			SetPrimaryRate,			// Detail: TwoIDs
			ChangeName,				// Detail: IdString
			AddEquipment,			// Detail: TwoIDs
			RemoveEquipment			// Detail: TwoIDs
		}

		public Type type { get; set; }
		public int id { get; set; }

		[MessagePackKnownType("2ids", typeof(TwoIDs))]
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

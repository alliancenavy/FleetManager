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
			GetCommonData,
			GetProfile,
			GetFleet,
			GetOperations,
			GetRoster,
			GetVesselDetail,
			GetUnassignedRoster
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

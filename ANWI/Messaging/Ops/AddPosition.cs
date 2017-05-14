namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Adds a position to a unit
	/// </summary>
	public class AddPosition : IMessagePayload {
		public string opUUID;
		public string unitUUID;
		public int roleID;

		public AddPosition() {
		}

		public AddPosition(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: AddPosition. Role: {roleID}";
		}
	}
}

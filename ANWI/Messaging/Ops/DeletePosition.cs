namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Removes a position from a unit
	/// </summary>
	public class DeletePosition : IMessagePayload {
		public string opUUID;
		public string posUUID;

		public DeletePosition() {
		}

		public DeletePosition(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: DeletePosition. Role: {posUUID}";
		}
	}
}

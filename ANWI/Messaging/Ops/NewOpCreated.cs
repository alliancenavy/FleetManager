namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Confirms operation creation successful.  Includes UUID of the
	/// op so the client can request the details
	/// </summary>
	public class NewOpCreated : IMessagePayload {
		public string uuid;

		public NewOpCreated() {
			uuid = "";
		}

		public NewOpCreated(string uuid) {
			this.uuid = uuid;
		}

		public override string ToString() {
			return $"Type: NewOpCreated . UUID: {uuid}";
		}
	}
}

namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Returns by the server to the client when an update is made to something
	/// to confirm that the update was successful.
	/// </summary>
	public class ConfirmUpdate : IMessagePayload {
		public bool success;
		public int updatedId;

		public ConfirmUpdate() {
			success = false;
			updatedId = 0;
		}

		public ConfirmUpdate(bool s, int id) {
			success = s;
			updatedId = id;
		}

		public override string ToString() {
			return $"Type: ConfirmUpdate.  Success: {success} Id: {updatedId}";
		}
	}
}

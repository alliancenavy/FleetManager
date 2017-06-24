namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Returns by the server to the client when an update is made to something
	/// to confirm that the update was successful.
	/// </summary>
	public class ConfirmUpdate : IMessagePayload {
		public string originalRequest; // TODO: want to use System.Type but
									   // MsgPack doesn't support it
		public bool success;
		public int updatedId;
		public string errorMessage;

		public ConfirmUpdate() {
			success = false;
			updatedId = 0;
			errorMessage = null;
		}

		public ConfirmUpdate(IMessagePayload req, bool s, int id) {
			originalRequest = req.GetType().ToString();
			success = s;
			updatedId = id;
		}

		public ConfirmUpdate(System.Type req, bool s, int id) {
			originalRequest = req.ToString();
			success = s;
			updatedId = id;
		}

		public ConfirmUpdate(IMessagePayload req, bool s, int id, string msg) {
			originalRequest = req.GetType().ToString();
			success = s;
			updatedId = id;
			errorMessage = msg;
		}

		public ConfirmUpdate(System.Type req, bool s, int id, string msg) {
			originalRequest = req.ToString();
			success = s;
			updatedId = id;
			errorMessage = msg;
		}

		public override string ToString() {
			return $"Type: ConfirmUpdate.  Success: {success} Id: {updatedId}";
		}
	}
}

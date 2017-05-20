namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Updates the clients with the op's settings
	/// </summary>
	public class UpdateSettings : IMessagePayload {
		public bool freeMove;

		public UpdateSettings() {
		}

		public override string ToString() {
			return $"Type: UpdateSettings";
		}
	}
}

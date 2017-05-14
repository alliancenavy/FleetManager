namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Changes variables on a wing unit
	/// </summary>
	public class UpdateWing : IMessagePayload {
		public enum Type {
			SetName,
			SetCallsign,
			ChangeWingCommander
		}

		public string wingUUID;
		public Type type;

		public string str;
		public string boatUUID;

		public UpdateWing() {
		}

		public override string ToString() {
			return $"Type: UpdateWing.";
		}
	}
}

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Changes the critical flag on a position
	/// </summary>
	public class SetPositionCritical : IMessagePayload {
		public string opUUID;
		public string posUUID;
		public bool critical;

		public SetPositionCritical() {
		}

		public SetPositionCritical(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: SetPositionCritical.";
		}
	}
}

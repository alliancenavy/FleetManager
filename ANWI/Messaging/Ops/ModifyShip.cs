namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Changes a ship's properties
	/// </summary>
	public class ModifyShip : IMessagePayload {
		public enum ChangeType {
			Flagship

		}

		public string opUUID;
		public string shipUUID;

		public ChangeType type;
		public bool boolean;

		public ModifyShip() {
		}

		public ModifyShip(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: ModifyShip. Changed: {type}";
		}
	}
}

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Changes a unit's properties
	/// </summary>
	public class ModifyUnit : IMessagePayload {
		public enum ChangeType {
			SetFlagship,
			ChangeName,
			ChangeCallsign,
			SetPositionCritical,
			AddPosition,
			DeletePosition
		}

		public string opUUID;
		public string unitUUID;

		public ChangeType type;
		public bool boolean;
		public int integer;
		public string uuid;
		
		public ModifyUnit() {
		}

		public ModifyUnit(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: ModifyUnit. Changed: {type}";
		}
	}
}

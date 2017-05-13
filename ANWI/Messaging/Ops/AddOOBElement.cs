namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Adds a new ship or wing to the order of battle
	/// </summary>
	public class AddOOBElement : IMessagePayload {
		public enum Type {
			FleetShip,
			CustShip,
			Wing
		}

		public string opUUID;

		public Type type;
		public int shipId;
		public string name;

		public AddOOBElement() {
		}

		public AddOOBElement(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: AddOOBElement. Add: {type}";
		}
	}
}

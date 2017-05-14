namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Adds a new ship or wing to the order of battle
	/// </summary>
	public class AddOOBUnit : IMessagePayload {
		public enum Type {
			FleetShip,
			CustShip,
			Wing,
			Boat
		}

		public string opUUID;

		public Type type;
		public int shipId;
		public string wingUUID;
		public string name;
		public int hullId;

		public AddOOBUnit() {
		}

		public AddOOBUnit(string uuid) {
			opUUID = uuid;
		}

		public override string ToString() {
			return $"Type: AddOOBUnit. Add: {type}";
		}
	}
}

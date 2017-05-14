namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Changes variables on a ship unit
	/// </summary>
	public class UpdateShip : IMessagePayload {
		public enum Type {
			SetFlagship
		}

		public string shipUUID;
		public Type type;

		public UpdateShip() {
		}

		public override string ToString() {
			return $"Type: UpdateShip.";
		}
	}
}

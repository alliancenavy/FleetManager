namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Sets whether the C2 structure is unified or hierarchical
	/// </summary>
	public class SetC2Type : IMessagePayload {
		public string opUUID;
		public bool unified;

		public SetC2Type() {
		}

		public override string ToString() {
			return $"Type: SetC2Type.";
		}
	}
}

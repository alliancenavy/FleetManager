namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Removes a ship or wing from the order of battle
	/// </summary>
	public class DeleteOOBElement : IMessagePayload {
		public string opUUID;
		public string elementUUID;
		
		public DeleteOOBElement() {
		}

		public DeleteOOBElement(string op, string element) {
			opUUID = op;
			elementUUID = element;
		}

		public override string ToString() {
			return $"Type: DeleteOOBElement. Delete: {elementUUID}";
		}
	}
}

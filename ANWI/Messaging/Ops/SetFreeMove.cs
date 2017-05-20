namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Client -> Server
	/// Changes the free move status
	/// </summary>
	public class SetFreeMove : IMessagePayload {
		public string opUUID;
		public bool freeMove;

		public SetFreeMove() {
		}

		public override string ToString() {
			return $"Type: SetFreeMove";
		}
	}
}

namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Contains all of the up-to-date details on an operation.  For loading
	/// the operation initially onto the client.  Most other changes are
	/// handled by delta messages
	/// </summary>
	public class FullOperationSnapshot : IMessagePayload {
		public Operation op;

		public FullOperationSnapshot() {
			op = null;
		}

		public FullOperationSnapshot(Operation op) {
			this.op = op;
		}

		public override string ToString() {
			return $"Type: FullOperationSnapshot. UUID: {op.uuid}";
		}
	}
}

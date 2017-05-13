namespace ANWI.Messaging.Ops {

	/// <summary>
	/// Server -> Client
	/// Informs clients when the op status changes
	/// </summary>
	public class UpdateStatus : IMessagePayload {
		public OperationStatus status;

		public UpdateStatus() {
			status = OperationStatus.CONFIGURING;
		}

		public UpdateStatus(OperationStatus status) {
			this.status = status;
		}

		public override string ToString() {
			return $"Type: UpdateStatus. Status: {status}";
		}
	}
}

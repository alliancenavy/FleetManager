namespace ANWI.Messaging.Ops {
	
	/// <summary>
	/// Client -> Server
	/// Requests the server create a new operation with the given details
	/// </summary>
	public class CreateNewOp : IMessagePayload {
		public string name;
		public OperationType type;
		public int userId;

		public CreateNewOp() {
			name = "";
			type = OperationType.PATROL;
			userId = 0;
		}

		public CreateNewOp(string name, OperationType type, int user) {
			this.name = name;
			this.type = type;
			this.userId = user;
		}

		public override string ToString() {
			return $"Type: CreateNewOp. Name: {name}";
		}
	}
}

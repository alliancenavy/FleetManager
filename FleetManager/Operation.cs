using ANWI;

namespace FleetManager {

	/// <summary>
	/// An active operation
	/// </summary>
	public class Operation {
		#region Instance Members
		public string uuid { get; private set; }

		public string name;
		public OperationType type;
		public OperationStatus status;
		#endregion

		#region Constructors
		public Operation(string uuid, string name, OperationType type) {
			this.uuid = uuid;
			this.name = name;
			this.type = type;
			this.status = OperationStatus.STAGING;
		}

		public LiteOperation ToLite() {
			return new LiteOperation() {
				uuid = uuid,
				name = name,
				type = type,
				status = status
			};
		}
		#endregion
	}
}

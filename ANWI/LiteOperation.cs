namespace ANWI {

	/// <summary>
	/// Basic information on an operation for the list
	/// </summary>
	public class LiteOperation {
		#region Instance Members
		public string uuid;

		public string name { get; set; }
		public OperationType type { get; set; }
		public OperationStatus status { get; set; }
		public int currentMembers { get; set; }
		public int neededMembers { get; set; }
		public int totalSlots { get; set; }
		#endregion

		#region WPF Helpers
		public string typeString { get { return type.ToFriendlyString(); } }
		public string statusString { get { return status.ToFriendlyString(); } }
		#endregion

		#region Constructors
		public LiteOperation() {
			uuid = "";
			name = "";
			type = OperationType.ASSAULT;
			status = OperationStatus.STAGING;
			currentMembers = 0;
			neededMembers = 0;
			totalSlots = 0;
		}
		#endregion
	}
}

namespace ANWI {

	/// <summary>
	/// Describes the status of a vessel
	/// </summary>
	public enum VesselStatus {
		ACTIVE = 0,
		DESTROYED = 1,
		DESTROYED_WAITING_REPLACEMENT = 2,
		DRYDOCKED = 3,
		DECOMMISSIONED = 4
	}

	/// <summary>
	/// Describes the primary purpose of an operation
	/// </summary>
	public enum OperationType {
		PATROL = 0,
		ASSAULT,
		DEFENSE,
		MINING,
		LOGISTICS
	}

	/// <summary>
	/// Current stage in an operation's lifecycle
	/// </summary>
	public enum OperationStatus {
		CONFIGURING = 0,
		STAGING = 1,
		SORTIED = 2,
		DISMISSING = 3
	}

	public static class SharedEnumExtensions {

		/// <summary>
		/// For converting the vessel status into a string for the UI
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		public static string ToFriendlyString(this VesselStatus status) {
			switch (status) {
				case VesselStatus.ACTIVE:
					return "Active";
				case VesselStatus.DESTROYED:
					return "Destroyed";
				case VesselStatus.DESTROYED_WAITING_REPLACEMENT:
					return "Destroyed (Awaiting Replacement)";
				case VesselStatus.DRYDOCKED:
					return "Drydocked";
				case VesselStatus.DECOMMISSIONED:
					return "Decommissioned/Sold";
				default:
					return "Unknown";
			}
		}

		public static string ToFriendlyString(this OperationType type) {
			switch (type) {
				case OperationType.PATROL:
					return "Patrol";
				case OperationType.ASSAULT:
					return "Assault";
				case OperationType.DEFENSE:
					return "Defense";
				case OperationType.MINING:
					return "Mining";
				case OperationType.LOGISTICS:
					return "Logistics";
				default:
					return "";
			}
		}

		public static string ToFriendlyString(this OperationStatus status) {
			switch (status) {
				case OperationStatus.CONFIGURING:
					return "Configuring";
				case OperationStatus.STAGING:
					return "Staging";
				case OperationStatus.SORTIED:
					return "Sortied";
				case OperationStatus.DISMISSING:
					return "Dismissing";
				default:
					return "";
			}
		}

		public static OperationStatus Next(this OperationStatus status) {
			switch(status) {
				case OperationStatus.CONFIGURING:
					return OperationStatus.STAGING;
				case OperationStatus.STAGING:
					return OperationStatus.SORTIED;
				case OperationStatus.SORTIED:
					return OperationStatus.DISMISSING;
				case OperationStatus.DISMISSING:
					return OperationStatus.DISMISSING;
				default:
					return OperationStatus.DISMISSING;
			}
		}

	}
}


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

	}
}


using System;

namespace ANWI.Messaging.Updater {

	/// <summary>
	/// Server -> Client
	/// Returns whether or not an update is required
	/// </summary>
	public class CheckResult : IMessagePayload {
		public bool updateNeeded;
		public long updateSize;

		public CheckResult() {
		}

		public override string ToString() {
			return $"Type: Updater.CheckResult. Needed: {updateNeeded}";
		}
	}
}

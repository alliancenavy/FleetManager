using System;

namespace ANWI.Messaging {

	/// <summary>
	/// Server -> Client
	/// Returns whether or not an update is required
	/// </summary>
	public class UpdateStatus : IMessagePayload {
		public bool updateNeeded;
		public Version ver;
		public int updateSize;

		public UpdateStatus() {
		}

		public override string ToString() {
			return $"Type: UpdateStatus. Needed: {updateNeeded} Version: {ver}";
		}
	}
}

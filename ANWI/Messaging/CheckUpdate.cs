using System;

namespace ANWI.Messaging {

	/// <summary>
	/// Client -> Server
	/// Checks if the client has the latest version
	/// </summary>
	public class CheckUpdate : IMessagePayload {
		public Version ver;

		public CheckUpdate() {
		}

		public CheckUpdate(Version v) {
			ver = v;
		}

		public override string ToString() {
			return $"Type: CheckUpdate. Version: {ver}"; 
		}
	}
}

using System;
using System.Collections.Generic;

namespace ANWI.Messaging.Updater {

	/// <summary>
	/// Client -> Server
	/// Checks if the client has the latest version
	/// </summary>
	public class Check : IMessagePayload {
		public Dictionary<string, string> checksums;

		public Check() {
		}

		public Check(Dictionary<string, string> check) {
			checksums = check;
		}

		public override string ToString() {
			return $"Type: Updater.Check."; 
		}
	}
}

﻿namespace ANWI.Messaging {

	/// <summary>
	/// Client -> Server
	/// Removes a rate from a user
	/// </summary>
	public class DeleteRate : IMessagePayload {
		public int userId;
		public int rateId;

		public DeleteRate() {
			userId = 0;
			rateId = 0;
		}

		public DeleteRate(int uid, int rid) {
			userId = uid;
			rateId = rid;
		}

		public override string ToString() {
			return $"Type: DeleteRate. Strike ID {rateId}";
		}
	}
}

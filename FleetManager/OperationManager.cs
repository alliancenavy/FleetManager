using ANWI;
using System;
using System.Collections.Generic;

namespace FleetManager {

	/// <summary>
	/// Keeps track of all running operations.
	/// </summary>
	public class OperationManager {
		#region Instance Members
		Dictionary<string, Operation> activeOps 
			= new Dictionary<string, Operation>();

		#endregion

		#region Constructors
		public OperationManager() {
			CreateNew();
		}
		#endregion

		#region Message Processing
		public ANWI.Messaging.IMessagePayload 
		DeliverMessage(ANWI.Messaging.IMessagePayload p) {
			return null;
		}
		#endregion

		#region Main Service Access
		public List<LiteOperation> GetAllOperations() {
			List<LiteOperation> ops = new List<LiteOperation>();

			foreach(KeyValuePair<string, Operation> entry in activeOps) {
				ops.Add(entry.Value.ToLite());
			}

			return ops;
		}
		#endregion

		#region Lifecycle
		private void CreateNew() {
			Operation op = new Operation(
				GenerateUUID(),
				"Test Operation",
				OperationType.PATROL);

			activeOps.Add(op.uuid, op);
		}
		#endregion

		#region Subscriptions
		#endregion

		#region Other Helpers
		/// <summary>
		/// Generates a UUID for an Op.
		/// From: http://madskristensen.net/post/generate-unique-strings-and-numbers-in-c
		/// </summary>
		/// <returns></returns>
		private string GenerateUUID() {
			long i = 1;
			foreach (byte b in Guid.NewGuid().ToByteArray()) {
				i *= ((int)b + 1);
			}
			return string.Format("{0:x}", i - DateTime.Now.Ticks);
		}
		#endregion
	}
}

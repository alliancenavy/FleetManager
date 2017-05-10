using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANWI;

namespace FleetManager.Services {

	/// <summary>
	/// Operations Serivce
	/// </summary>
	public class Ops : BaseService {

		#region Instance Members
		Dictionary<string, Operation> activeOps
			= new Dictionary<string, Operation>();

		#endregion

		#region Constructors
		public Ops() : base("Ops Service", true) {
			logger.Info("Started");

			AddProcessor(typeof(ANWI.Messaging.Request), 
				ProcessRequestMessage);

			// Create testing op
			CreateNew("Test Operation");
		}
		#endregion

		#region Abstract Implementations
		/// <summary>
		/// Helper to get auth token from a context
		/// </summary>
		/// <returns></returns>
		protected override string GetTokenCookie() {
			return this.Context.CookieCollection["authtoken"].Value;
		}

		/// <summary>
		/// Helper to get username from a context
		/// </summary>
		/// <returns></returns>
		protected override string GetNameCookie() {
			return this.Context.CookieCollection["name"].Value;
		}

		/// <summary>
		/// Combines cookies for easy identification in the log
		/// </summary>
		/// <returns></returns>
		protected override string GetLogIdentifier() {
			return $"[{GetNameCookie()} ({GetTokenCookie()})]";
		}
		#endregion

		#region Message Processors
		private ANWI.Messaging.IMessagePayload
		ProcessRequestMessage(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Request req = p as ANWI.Messaging.Request;

			switch(req.type) {
				case ANWI.Messaging.Request.Type.GetOperations: {
						List<LiteOperation> ops = GetAllOps();
						return new ANWI.Messaging.FullOperationsList(ops);
					}
			}

			return null;
		}
		#endregion

		#region Op Lifecycle
		private void CreateNew(string name) {
			Operation op = new Operation(
				GenerateUUID(),
				name,
				OperationType.PATROL);

			activeOps.Add(op.uuid, op);

			logger.Info($"Created new operation with UUID {op.uuid}");
			logger.Info($"There are now {activeOps.Count} active operations");
		}
		#endregion


		#region Other Helpers
		/// <summary>
		/// Returns a summary list of all active operations
		/// </summary>
		/// <returns></returns>
		private List<LiteOperation> GetAllOps() {
			List<LiteOperation> ops = new List<LiteOperation>();

			foreach (KeyValuePair<string, Operation> op in activeOps) {
				ops.Add(op.Value.ToLite());
			}

			return ops;
		}

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

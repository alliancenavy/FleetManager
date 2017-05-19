using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ANWI;

namespace FleetManager {
	public class OperationManager {
		#region Instance Members
		Logger logger = LogManager.GetLogger("Ops Manager");

		private Dictionary<string, ActiveOperation> activeOps
			= new Dictionary<string, ActiveOperation>();

		#endregion

		#region Constructors
		private static OperationManager _instance = null;
		public static OperationManager Instance {
			get {
				if (_instance == null)
					_instance = new OperationManager();
				return _instance;
			}
		}

		private OperationManager() {
			// Empty
		}
		#endregion

		#region Interface
		/// <summary>
		/// Creates a new operation in the ActiveOps dictionary
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public string CreateNew(string name, OperationType type) {
			ActiveOperation op = new ActiveOperation(
				ANWI.Utility.UUID.GenerateUUID(),
				name,
				type);

			activeOps.Add(op.uuid, op);

			logger.Info($"Created new operation with UUID {op.uuid}");
			logger.Info($"There are now {activeOps.Count} active operations");

			return op.uuid;
		}

		/// <summary>
		/// Returns a summary list of all active operations that are not in
		/// the configuring stage
		/// </summary>
		/// <returns></returns>
		public List<LiteOperation> GetOpsList() {
			List<LiteOperation> ops = new List<LiteOperation>();

			foreach (KeyValuePair<string, ActiveOperation> op in activeOps) {
				if (op.Value.status != OperationStatus.CONFIGURING)
					ops.Add(op.Value.ToLite());
			}

			return ops;
		}
		
		/// <summary>
		/// Returns an operation with the given UUID
		/// </summary>
		/// <param name="uuid"></param>
		/// <returns></returns>
		public ActiveOperation GetOperation(string uuid) {
			ActiveOperation op;
			if (!activeOps.TryGetValue(uuid, out op)) {
				logger.Error($"Operation {uuid} does not exist");
			}
			return op;
		}
		#endregion
	}
}

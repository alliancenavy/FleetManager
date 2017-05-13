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
		Dictionary<string, ActiveOperation> activeOps
			= new Dictionary<string, ActiveOperation>();

		#endregion

		#region Constructors
		public Ops() : base("Ops Service", true) {
			logger.Info("Started");

			AddProcessor(typeof(ANWI.Messaging.Request), 
				ProcessRequestMessage);
			AddProcessor(typeof(ANWI.Messaging.Ops.CreateNewOp),
				ProcessCreateOp);
			AddProcessor(typeof(ANWI.Messaging.Ops.AddOOBElement),
				ProcessAddOOBElement);
			AddProcessor(typeof(ANWI.Messaging.Ops.DeleteOOBElement),
				ProcessDeleteOOBElement);
			AddProcessor(typeof(ANWI.Messaging.Ops.AssignUser),
				ProcessAssignUser);

			// Create testing op
			string uuid = CreateNew("Test Operation", OperationType.PATROL);
			activeOps[uuid].AdvanceLifecycle();
		}
		#endregion

		#region Cookies
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

				case ANWI.Messaging.Request.Type.GetOperation: {
						ANWI.Messaging.ReqExp.IdString det
							= req.detail as ANWI.Messaging.ReqExp.IdString;

						ActiveOperation op = activeOps[det.str];
						if(op != null) {
							op.SubscribeUser(GetUser(GetTokenCookie()));
							return new ANWI.Messaging.Ops.FullOperationSnapshot(
								op.ToSnapshot());
						} else {
							logger.Error(
								$"{GetLogIdentifier()} requested details of " +
								$"op {det.str} which does not exist.");
							return null;
						}
					}

				case ANWI.Messaging.Request.Type.CloseOperation: {
						ANWI.Messaging.ReqExp.IdString det
							= req.detail as ANWI.Messaging.ReqExp.IdString;

						ActiveOperation op = activeOps[det.str];
						if(op != null) {
							op.UnsubscribeUser(GetTokenCookie());
						} else {
							logger.Error($"{GetLogIdentifier()} requested to " +
								$"close sub to op {det.str} which does not " +
								"exist");
						}

						return null;
					}

				case ANWI.Messaging.Request.Type.AdvanceOpLifecycle: {
						ANWI.Messaging.ReqExp.IdString det
							= req.detail as ANWI.Messaging.ReqExp.IdString;

						ActiveOperation op = activeOps[det.str];
						if (op != null)
							op.AdvanceLifecycle();
						return null;
					}
					
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessCreateOp(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.CreateNewOp cno
				= p as ANWI.Messaging.Ops.CreateNewOp;

			string uuid = CreateNew(cno.name, cno.type);
			return new ANWI.Messaging.Ops.NewOpCreated(uuid);
		}

		private ANWI.Messaging.IMessagePayload
		ProcessAddOOBElement(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.AddOOBElement add
				= p as ANWI.Messaging.Ops.AddOOBElement;

			ActiveOperation op = activeOps[add.opUUID];
			if(op != null) {
				if (add.type == ANWI.Messaging.Ops.AddOOBElement.Type.FleetShip)
					op.AddFleetShip(add.shipId);
				else if (add.type == ANWI.Messaging.Ops.AddOOBElement.Type.Wing)
					op.AddWing();
			} else {
				logger.Error($"Attempted to add fleet ship to op {add.opUUID}" +
					" which does not exist");
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessDeleteOOBElement(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.DeleteOOBElement del
				= p as ANWI.Messaging.Ops.DeleteOOBElement;

			ActiveOperation op = activeOps[del.opUUID];
			if(op != null) {
				op.DeleteFleetElement(del.elementUUID);
			} else {
				logger.Error($"Attempted to delete elem from op {del.opUUID} " +
					"which does not exist");
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessAssignUser(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.AssignUser au
				= p as ANWI.Messaging.Ops.AssignUser;

			ActiveOperation op = activeOps[au.opUUID];
			if(op != null) {
				op.ChangeAssignment(au.elemUUID, au.wingmemberUUID, 
					au.positionUUID, au.userId);
			} else {
				logger.Error("Attempted to change assignment in op " +
					$"{au.opUUID} which does not exist");
			}

			return null;
		}
		#endregion

		#region Op Lifecycle
		private string CreateNew(string name, OperationType type) {
			ActiveOperation op = new ActiveOperation(
				ANWI.Utility.UUID.GenerateUUID(),
				name,
				type);

			activeOps.Add(op.uuid, op);

			logger.Info($"Created new operation with UUID {op.uuid}");
			logger.Info($"There are now {activeOps.Count} active operations");

			return op.uuid;
		}
		#endregion


		#region Other Helpers
		/// <summary>
		/// Returns a summary list of all active operations
		/// </summary>
		/// <returns></returns>
		private List<LiteOperation> GetAllOps() {
			List<LiteOperation> ops = new List<LiteOperation>();

			foreach (KeyValuePair<string, ActiveOperation> op in activeOps) {
				if(op.Value.status != OperationStatus.CONFIGURING)
					ops.Add(op.Value.ToLite());
			}

			return ops;
		}
		#endregion
	}
}

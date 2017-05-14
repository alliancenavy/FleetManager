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
			AddProcessor(typeof(ANWI.Messaging.Ops.AddOOBUnit),
				ProcessAddOOBElement);
			AddProcessor(typeof(ANWI.Messaging.Ops.DeleteOOBElement),
				ProcessDeleteOOBElement);
			AddProcessor(typeof(ANWI.Messaging.Ops.AssignUser),
				ProcessAssignUser);
			AddProcessor(typeof(ANWI.Messaging.Ops.AddPosition),
				ProcessAddPosition);
			AddProcessor(typeof(ANWI.Messaging.Ops.DeletePosition),
				ProcessDeletePosition);
			AddProcessor(typeof(ANWI.Messaging.Ops.SetPositionCritical),
				ProcessSetPositionCritical);

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

						ActiveOperation op = GetOperation(det.str);
						if (op != null) {
							op.SubscribeUser(GetUser(GetTokenCookie()));
							return new ANWI.Messaging.Ops.FullOperationSnapshot(
								op.ToSnapshot());
						} else {
							return null;
						}
					}

				case ANWI.Messaging.Request.Type.CloseOperation: {
						ANWI.Messaging.ReqExp.IdString det
							= req.detail as ANWI.Messaging.ReqExp.IdString;

						ActiveOperation op = GetOperation(det.str);
						if(op != null) {
							op.UnsubscribeUser(GetTokenCookie());
						}

						return null;
					}

				case ANWI.Messaging.Request.Type.AdvanceOpLifecycle: {
						ANWI.Messaging.ReqExp.IdString det
							= req.detail as ANWI.Messaging.ReqExp.IdString;

						ActiveOperation op = GetOperation(det.str);
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
			ANWI.Messaging.Ops.AddOOBUnit add
				= p as ANWI.Messaging.Ops.AddOOBUnit;

			ActiveOperation op = GetOperation(add.opUUID);
			if(op != null) {
				if (add.type == ANWI.Messaging.Ops.AddOOBUnit.Type.FleetShip)
					op.AddFleetShip(add.shipId);
				else if (add.type == ANWI.Messaging.Ops.AddOOBUnit.Type.Wing)
					op.AddWing();
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessDeleteOOBElement(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.DeleteOOBElement del
				= p as ANWI.Messaging.Ops.DeleteOOBElement;

			ActiveOperation op = GetOperation(del.opUUID);
			if(op != null) {
				op.DeleteFleetUnit(del.elementUUID);
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessAssignUser(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.AssignUser au
				= p as ANWI.Messaging.Ops.AssignUser;

			ActiveOperation op = GetOperation(au.opUUID);
			if(op != null) {
				op.ChangeAssignment(au.positionUUID, au.userId);
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessModifyShip(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.ModifyUnit mod
				= p as ANWI.Messaging.Ops.ModifyUnit;

			ActiveOperation op = GetOperation(mod.opUUID);
			if(op != null) {
				op.ModifyUnit(mod);
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessAddPosition(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.AddPosition add
				= p as ANWI.Messaging.Ops.AddPosition;

			ActiveOperation op = GetOperation(add.opUUID);
			if(op != null) {
				op.AddPosition(add.unitUUID, add.roleID);
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessDeletePosition(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.DeletePosition del
				= p as ANWI.Messaging.Ops.DeletePosition;

			ActiveOperation op = GetOperation(del.opUUID);
			if(op != null) {
				op.DeletePosition(del.posUUID);
			}

			return null;
		}

		private ANWI.Messaging.IMessagePayload
		ProcessSetPositionCritical(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Ops.SetPositionCritical set
				= p as ANWI.Messaging.Ops.SetPositionCritical;

			ActiveOperation op = GetOperation(set.opUUID);
			if(op != null) {
				op.SetPositionCritical(set.posUUID, set.critical);
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
		private ActiveOperation GetOperation(string uuid) {
			ActiveOperation op;
			if(!activeOps.TryGetValue(uuid, out op)) {
				logger.Error($"Operation {uuid} does not exist");
			}
			return op;
		}

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

using System;
using System.Collections.Generic;
using ANWI;
using Datamodel = ANWI.Database.Model;
using WebSocketSharp;
using WebSocketSharp.Server;
using NLog;
using System.IO;
using MsgPack.Serialization;

namespace FleetManager.Services {
	public class Main : BaseService {

		private static LoginTracker tracker = new LoginTracker();

		public Main() : base("Main Service") {
			logger.Info("Started");

			// Build the message processor dictionary

			AddProcessor(typeof(ANWI.Messaging.AddRate), 
				ProcessAddRate);
			AddProcessor(typeof(ANWI.Messaging.Request), 
				ProcessRequestMessage);
			AddProcessor(typeof(ANWI.Messaging.NewAssignment), 
				ProcessNewAssignment);
			AddProcessor(typeof(ANWI.Messaging.EndAssignment), 
				ProcessEndAssignment);
			AddProcessor(typeof(ANWI.Messaging.ChangeShipStatus), 
				ProcessChangeShipStatus);
			AddProcessor(typeof(ANWI.Messaging.NewShip), 
				ProcessNewShip);
		}

		#region Websockets
		/// <summary>
		/// 
		/// </summary>
		protected override void OnOpen() {
			base.OnOpen();
			tracker.NewSession(GetAuth0Cookie(), this);
		}

		/// <summary>
		/// Connection ending point
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClose(CloseEventArgs e) {
			tracker.EndSession(GetAuth0Cookie());
			base.OnClose(e);
		}

		/// <summary>
		/// Socket error handler
		/// </summary>
		/// <param name="e"></param>
		protected override void OnError(WebSocketSharp.ErrorEventArgs e) {
			tracker.EndSession(GetAuth0Cookie());
			base.OnError(e);
		}

		public void Terminate() {
			this.Context.WebSocket.Close(CloseStatusCode.PolicyViolation,
				"Account logged in from another location");
		}

		/// <summary>
		/// Helper to get auth token from a context
		/// </summary>
		/// <returns></returns>
		protected override string GetTokenCookie() {
			return this.Context.CookieCollection["authtoken"].Value;
		}

		/// <summary>
		/// Helper to get auth0 id from a context
		/// </summary>
		/// <returns></returns>
		protected string GetAuth0Cookie() {
			return this.Context.CookieCollection["auth0id"].Value;
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
		/// <summary>
		/// Handles the simple request message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		ProcessRequestMessage(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.Request req = p as ANWI.Messaging.Request;

			switch(req.type) {
				case ANWI.Messaging.Request.Type.GetCommonData: {
						ANWI.Messaging.AllCommonData acd 
							= new ANWI.Messaging.AllCommonData();
						acd.ranks = Rank.FetchAll();
						acd.rates = Rate.FetchAllRates();
						acd.assignmentRoles = AssignmentRole.FetchAll();
						acd.largeHulls = Hull.FetchLarge();
						acd.smallHulls = Hull.FetchSmall();
						acd.shipRoles = OperationRole.FetchAllShips();
						acd.boatRoles = OperationRole.FetchAllBoats();
						return acd;
					}

				case ANWI.Messaging.Request.Type.GetProfile: {
						Profile profile = Profile.FetchById(req.id);
						return new ANWI.Messaging.FullProfile(profile);
					}

				case ANWI.Messaging.Request.Type.GetFleet: {
						List<LiteVessel> registry = LiteVessel.FetchRegistry();
						return new ANWI.Messaging.FullVesselReg(registry);
					}

				case ANWI.Messaging.Request.Type.GetAvailableFleet: {
						List<LiteVessel> avail = LiteVessel.FetchAvailable();
						return new ANWI.Messaging.FullVesselReg(avail);
					}

				case ANWI.Messaging.Request.Type.GetOperations: {
						// TODO
						List<LiteOperation> ops = new List<LiteOperation>();
						return new ANWI.Messaging.FullOperationsList(ops);
					}
					
				case ANWI.Messaging.Request.Type.GetRoster: {
						List<LiteProfile> profiles = LiteProfile.FetchAll();
						return new ANWI.Messaging.FullRoster(profiles);
					}

				case ANWI.Messaging.Request.Type.GetVesselDetail: {
						Vessel details = Vessel.FetchById(req.id);
						return new ANWI.Messaging.FullVessel(details);
					}

				case ANWI.Messaging.Request.Type.GetUnassignedRoster: {
						List<LiteProfile> unassigned 
							= LiteProfile.FetchAllUnassigned();
						return new ANWI.Messaging.FullRoster(unassigned);
					}

				case ANWI.Messaging.Request.Type.ChangeRank: {
						ANWI.Messaging.ReqExp.TwoIDs tid
							= req.detail as ANWI.Messaging.ReqExp.TwoIDs;
						return ChangeRank(tid.id1, tid.id2);
					}

				case ANWI.Messaging.Request.Type.DeleteRate: {
						ANWI.Messaging.ReqExp.TwoIDs tid
							= req.detail as ANWI.Messaging.ReqExp.TwoIDs;
						return DeleteRate(tid.id1, tid.id2);
					}

				case ANWI.Messaging.Request.Type.SetPrimaryRate: {
						ANWI.Messaging.ReqExp.TwoIDs tid
							= req.detail as ANWI.Messaging.ReqExp.TwoIDs;
						return SetPrimaryRate(tid.id1, tid.id2);
					}

				case ANWI.Messaging.Request.Type.ChangeName: {
						ANWI.Messaging.ReqExp.IdString ids
							= req.detail as ANWI.Messaging.ReqExp.IdString;
						return ChangeNickname(ids.id, ids.str);
					}

				case ANWI.Messaging.Request.Type.AddEquipment: {
						ANWI.Messaging.ReqExp.TwoIDs tid
							= req.detail as ANWI.Messaging.ReqExp.TwoIDs;
						return AddEquipment(tid.id1, tid.id2);
					}

				case ANWI.Messaging.Request.Type.RemoveEquipment: {
						ANWI.Messaging.ReqExp.TwoIDs tid
							= req.detail as ANWI.Messaging.ReqExp.TwoIDs;
						return RemoveEquipment(tid.id1, tid.id2);
					}
			}

			return null;
		}

		/// <summary>
		/// Handles the AddRate message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		ProcessAddRate(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.AddRate ar = p as ANWI.Messaging.AddRate;

			bool success = false;
			Datamodel.StruckRate sr = null;
			if (Datamodel.StruckRate.FetchByUserRate(
				ref sr, ar.userId, ar.rateId)) {
				sr.rank = ar.rank;
				if (Datamodel.StruckRate.Store(sr)) {
					logger.Info($"Updated ate {ar.rateId} to rank {ar.rank}" +
						$" to user {ar.userId}");
					success = true;
				} else {
					logger.Error($"Failed to update rate {ar.rateId} for user" +
						$" {ar.userId}");
				}
			} else {
				if (Datamodel.StruckRate.Create(
					ref sr, ar.userId, ar.rateId, ar.rank)) {
					logger.Info($"Added rate {ar.rateId} at rank {ar.rank} " +
						$"to user {ar.userId}");
					success = true;
				} else {
					logger.Error($"Failed to add rate {ar.rateId} to " +
						$"user {ar.userId}");
				}
			}

			return new ANWI.Messaging.ConfirmUpdate(success, ar.userId);
		}

		/// <summary>
		/// Handles the NewAssignment message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		ProcessNewAssignment(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.NewAssignment ns = p as ANWI.Messaging.NewAssignment;

			Datamodel.Assignment a = null;
			bool success = Datamodel.Assignment.Create(
				ref a, ns.userId, ns.shipId, ns.roleId);
			if(success) {
				logger.Info($"Started assignment for user {ns.userId} role" +
					$" {ns.roleId} on ship {ns.shipId}");
			} else {
				logger.Error("Failed to start assignment for user" +
					$" {ns.userId} on ship {ns.shipId}");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, ns.shipId);
		}

		/// <summary>
		/// Handles the EndAssignment message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload ProcessEndAssignment(
			ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.EndAssignment es = p as ANWI.Messaging.EndAssignment;

			bool success = Datamodel.Assignment.EndAssignment(
				es.userId, es.assignmentId);
			if (success) {
				logger.Info($"Ended assignment {es.assignmentId} for" +
					$" user {es.userId}");
			} else {
				logger.Error($"Failed to end assignment {es.assignmentId}" +
					$" for user {es.userId}");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, es.shipId);
		}

		/// <summary>
		/// Handles the ChangeShipStatus message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		ProcessChangeShipStatus(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.ChangeShipStatus css 
				= p as ANWI.Messaging.ChangeShipStatus;

			bool success = false;
			Datamodel.UserShip ship = null;
			if(Datamodel.UserShip.FetchById(ref ship, css.shipId)) {
				ship.status = (int)css.status;
				if(Datamodel.UserShip.StoreUpdateStatus(ship)) {
					success = true;
					logger.Info($"Updated ship {css.shipId} status" +
						$" to {css.status}");
				} else {
					logger.Error("Failed to update status for " +
						$"ship {css.shipId}");
				}
			} else {
				logger.Error($"Failed to fetch ship {css.shipId} " +
					"to update status");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, css.shipId);
		}

		/// <summary>
		/// Handles the NewShip message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		ProcessNewShip(ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.NewShip ns = p as ANWI.Messaging.NewShip;

			bool success = false;
			Datamodel.UserShip ship = null;
			int confirmId = 0;
			if(Datamodel.UserShip.Create(ref ship, ns.ownerId, ns.hullId, 
				Convert.ToInt32(ns.LTI), ns.name, (int)VesselStatus.DRYDOCKED)) 
				{
				success = true;
				confirmId = ship.id;
				logger.Info($"Created new ship {ns.name}");
			} else {
				logger.Error($"Failed to create new ship {ns.name}");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, confirmId);
		}
		#endregion

		#region Request Processing Helpers
		/// <summary>
		/// Changes the rank of a user
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload ChangeRank(int uid, int rid) {
			bool success = false;
			Datamodel.User u = null;
			if (Datamodel.User.FetchById(ref u, uid)) {
				u.rank = rid;
				if (Datamodel.User.Store(u)) {
					logger.Info($"Updated rank of user {uid} to {rid}");
					success = true;
				} else {
					logger.Error($"Failed to update rank of user {uid}");
				}
			} else {
				logger.Error("Could not set rank: no user with id {uid} found");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, uid);
		}

		/// <summary>
		/// Deletes a struck rate from a user
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload DeleteRate(int uid, int rid) {
			bool success = false;
			if (Datamodel.StruckRate.DeleteById(rid)) {
				logger.Info($"Deleted rate {rid} from user {uid}");
				success = true;
			} else {
				logger.Error($"Failed to delete rate {rid} from user {uid}");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, uid);
		}

		/// <summary>
		/// Sets a user's primary rate
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		SetPrimaryRate(int uid, int rid) {
			bool success = false;
			Datamodel.User u = null;
			if (Datamodel.User.FetchById(ref u, uid)) {
				u.rate = rid;
				if (Datamodel.User.Store(u)) {
					logger.Info($"Updated primary rate on user {uid} to {rid}");
					success = true;
				} else {
					logger.Error("Failed to update primary rate on user" +
						$" {uid} to {rid}");
				}
			} else {
				logger.Error("Could not set primary rate: no user with" +
					$" id {uid} found");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, uid);
		}

		/// <summary>
		/// Changes a user's name
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		ChangeNickname(int uid, string name) {
			bool success = false;
			Datamodel.User user = null;
			if (Datamodel.User.FetchById(ref user, uid)) {
				user.name = name;
				if (Datamodel.User.Store(user)) {
					logger.Info($"Changed name of user {uid} to {name}");
					success = true;
				} else {
					logger.Error("Failed to update name of user " +
						$"{uid} to {name}.");
				}
			} else {
				logger.Error($"Could not change name: no user with id {uid}.");
			}

			return new ANWI.Messaging.ConfirmUpdate(success, uid);
		}

		/// <summary>
		/// Adds embarked equipment to a ship
		/// </summary>
		/// <param name="shipId"></param>
		/// <param name="hullId"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		AddEquipment(int shipId, int hullId) {
			Datamodel.ShipEquipment e = null;
			bool success 
				= Datamodel.ShipEquipment.Create(ref e, hullId, shipId);

			if (success)
				logger.Info($"Added new equipment {hullId} to ship {shipId}");
			else
				logger.Error($"Failed to add equipment to ship {shipId}");

			return new ANWI.Messaging.ConfirmUpdate(success, shipId);
		}

		/// <summary>
		/// Removes embarked equipment from a ship
		/// </summary>
		/// <param name="shipId"></param>
		/// <param name="hullId"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload 
		RemoveEquipment(int shipId, int hullId) {

			bool success = Datamodel.ShipEquipment.DeleteOneOfHullOnShip(
				hullId, shipId);

			if (success)
				logger.Info($"Removed equipment {hullId} from ship {shipId}");
			else
				logger.Error($"Failed to remove equipment from ship {shipId}");

			return new ANWI.Messaging.ConfirmUpdate(success, shipId);
		}
		#endregion
	}
}

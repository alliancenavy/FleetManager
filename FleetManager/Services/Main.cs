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
	public class Main : WebSocketBehavior {
		private static NLog.Logger logger 
			= LogManager.GetLogger("Main Service");

		private Dictionary<Type, Func<ANWI.Messaging.IMessagePayload, 
			ANWI.Messaging.IMessagePayload>> msgProcessors = null;

		public Main() {
			logger.Info("Started");

			// Build the message processor dictionary
			msgProcessors = new Dictionary<Type, Func<
				ANWI.Messaging.IMessagePayload, 
				ANWI.Messaging.IMessagePayload>>() {
				{ typeof(ANWI.Messaging.Request),
					ProcessRequestMessage },
				{ typeof(ANWI.Messaging.ChangeNickname),
					ProcessChangeNickname },
				{ typeof(ANWI.Messaging.AddRate),
					ProcessAddRate },
				{ typeof(ANWI.Messaging.NewAssignment),
					ProcessNewAssignment },
				{ typeof(ANWI.Messaging.EndAssignment),
					ProcessEndAssignment },
				{ typeof(ANWI.Messaging.ChangeShipStatus),
					ProcessChangeShipStatus },
				{ typeof(ANWI.Messaging.NewShip),
					ProcessNewShip }
			};
		}

		/// <summary>
		/// Helper to get auth token from a context
		/// </summary>
		/// <returns></returns>
		private string GetTokenCookie() {
			return this.Context.CookieCollection["authtoken"].Value;
		}

		/// <summary>
		/// Helper to get username from a context
		/// </summary>
		/// <returns></returns>
		private string GetNameCookie() {
			return this.Context.CookieCollection["name"].Value;
		}

		/// <summary>
		/// Combines cookies for easy identification in the log
		/// </summary>
		/// <returns></returns>
		private string GetLogIdentifier() {
			return $"[{GetNameCookie()} ({GetTokenCookie()})]";
		}

		/// <summary>
		/// Message router.  Sends messages to correct processor function
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMessage(MessageEventArgs e) {
			ANWI.Messaging.Message msg 
				= ANWI.Messaging.Message.Receive(e.RawData);
			
			logger.Info($"Message received from {GetLogIdentifier()}. " +
				$"{msg.payload.ToString()}");

			ANWI.Messaging.IMessagePayload p = 
				msgProcessors[msg.payload.GetType()](msg.payload);

			ANWI.Messaging.Message response 
				= new ANWI.Messaging.Message(msg.address, p);

			if (response != null) {
				using (MemoryStream stream = new MemoryStream()) {
					MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(
						stream, response);
					Send(stream.ToArray());
				}
			}
		}

		/// <summary>
		/// New connection starting point
		/// </summary>
		protected override void OnOpen() {
			base.OnOpen();
			logger.Info("Connection received");
		}

		/// <summary>
		/// Connection ending point
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClose(CloseEventArgs e) {
			base.OnClose(e);
			logger.Info("Connection closed");
		}

		/// <summary>
		/// Socket error handler
		/// </summary>
		/// <param name="e"></param>
		protected override void OnError(WebSocketSharp.ErrorEventArgs e) {
			base.OnError(e);
		}


		#region Message Processors
		/// <summary>
		/// Handles the simple request message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload ProcessRequestMessage(
			ANWI.Messaging.IMessagePayload p) {
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

				case ANWI.Messaging.Request.Type.GetOperations: {
						List<Operation> ops = new List<Operation>();

						//ops.Add(new Operation() {
						//	name = "Daily Homeguard",
						//	type = Operation.Type.PATROL,
						//	status = Operation.Status.SORTIED,
						//	currentMembers = 6,
						//	neededMembers = 5,
						//	totalSlots = 8,
						//	id = 1238456
						//});

						//ops.Add(new Operation() {
						//	name = "Attack Station",
						//	type = Operation.Type.ASSAULT,
						//	status = Operation.Status.STAGING,
						//	currentMembers = 3,
						//	neededMembers = 10,
						//	totalSlots = 20,
						//	id = 1203913
						//});

						//ops.Add(new Operation() {
						//	name = "Unrep ANS Legend of Dave",
						//	type = Operation.Type.LOGISTICS,
						//	status = Operation.Status.DISMISSING,
						//	currentMembers = 2,
						//	neededMembers = 2,
						//	totalSlots = 3,
						//	id = 2357345
						//});

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
						ANWI.Messaging.ReqExp.UserIdPlus uidp
							= req.detail as ANWI.Messaging.ReqExp.UserIdPlus;
						return ChangeRank(uidp.userId, uidp.otherId);
					}

				case ANWI.Messaging.Request.Type.DeleteRate: {
						ANWI.Messaging.ReqExp.UserIdPlus uidp
							= req.detail as ANWI.Messaging.ReqExp.UserIdPlus;
						return DeleteRate(uidp.userId, uidp.otherId);
					}

				case ANWI.Messaging.Request.Type.SetPrimaryRate: {
						ANWI.Messaging.ReqExp.UserIdPlus uidp
							= req.detail as ANWI.Messaging.ReqExp.UserIdPlus;
						return SetPrimaryRate(uidp.userId, uidp.otherId);
					}
			}

			return null;
		}

		/// <summary>
		/// Handles the ChangeNickname message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload ProcessChangeNickname(
			ANWI.Messaging.IMessagePayload p) {
			ANWI.Messaging.ChangeNickname cn 
				= p as ANWI.Messaging.ChangeNickname;

			Datamodel.User user = null;
			if(!Datamodel.User.FetchByAuth0(ref user, cn.auth0_id)) {
				logger.Error("Failed to change name.  Could not select user.");
				return null;
			}

			user.name = cn.newName;
			if(!Datamodel.User.Store(user)) {
				logger.Error("Failed to update name is database.");
				return null;
			}

			logger.Info("Name successfully changed.");
			return null;
		}

		/// <summary>
		/// Handles the AddRate message
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		private ANWI.Messaging.IMessagePayload ProcessAddRate(
			ANWI.Messaging.IMessagePayload p) {
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
		private ANWI.Messaging.IMessagePayload ProcessNewAssignment(
			ANWI.Messaging.IMessagePayload p) {
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
		private ANWI.Messaging.IMessagePayload ProcessChangeShipStatus(
			ANWI.Messaging.IMessagePayload p) {
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
		private ANWI.Messaging.IMessagePayload ProcessNewShip(
			ANWI.Messaging.IMessagePayload p) {
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
		private ANWI.Messaging.IMessagePayload SetPrimaryRate(int uid, 
			int rid) {
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
		#endregion
	}
}

using ANWI;
using ANWI.FleetComp;
using MsgPack.Serialization;
using System.Collections.Generic;
using System.IO;
using WebSocketSharp.Net.WebSockets;
using NLog;

namespace FleetManager {

	/// <summary>
	/// An active operation
	/// </summary>
	public class ActiveOperation {
		private Logger logger = null;

		#region Instance Members
		public string uuid { get; private set; }

		private string name;
		private OperationType type;
		public OperationStatus status { get; private set; }
		private bool freeMove;

		private List<OpParticipant> roster = new List<OpParticipant>();
		public int rosterCount { get { return roster.Count; } }

		private OrderOfBattle fleet = new OrderOfBattle();

		private Dictionary<string, ConnectedUser> subscribed
			= new Dictionary<string, ConnectedUser>();
		public int subscribedCount { get { return subscribed.Count; } }

		private int lastWingNumber = 1;
		#endregion

		#region Constructors
		public ActiveOperation(string uuid, string name, OperationType type) {
			this.uuid = uuid;
			this.name = name;
			this.type = type;
			this.status = OperationStatus.CONFIGURING;
			this.freeMove = true;

			logger = LogManager.GetLogger($"Op {uuid}");
		}

		public LiteOperation ToLite() {
			return new LiteOperation() {
				uuid = uuid,
				name = name,
				type = type,
				status = status,
				currentMembers = rosterCount,
				neededMembers = fleet.TotalCriticalPositions,
				totalSlots = fleet.TotalPositions
			};
		}

		public Operation ToSnapshot() {
			return new Operation() {
				uuid = uuid,
				name = name,
				type = type,
				status = status,
				freeMove = freeMove,
				roster = roster
			};
		}
		#endregion

		#region Helpers
		private OpParticipant GetParticipant(int id) {
			foreach(OpParticipant p in roster) {
				if (p.profile.id == id)
					return p;
			}

			logger.Error($"User {id} is not in the roster");
			return null;
		}
		#endregion

		#region Participant Management
		public void SubscribeUser(ConnectedUser user) {
			subscribed.Add(user.token, user);

			// Join the first subscribing user to the roster
			// This will always be the FC
			if (rosterCount == 0) {
				JoinUser(user.token, false);

				// Add some fake users for testing purposes
				for(int i = 4; i < 9; ++i) {
					string token = ANWI.Utility.UUID.GenerateUUID();
					SubscribeUser(new ConnectedUser(token, i));
					JoinUser(token);
				}
			}
		}

		public void UnsubscribeUser(string token) {
			subscribed.Remove(token);
		}

		public void AdvanceLifecycle() {
			status = status.Next();
			PushToAll(new ANWI.Messaging.Ops.UpdateStatus(status));
		}

		public void JoinUser(string token, bool announce = true) {
			ConnectedUser user = subscribed[token];
			if(user == null) {
				logger.Error("Attempted to join user but they are not subbed");
				return;
			}

			OpParticipant np = new OpParticipant();
			np.profile = user.profile;
			np.position = null;

			// The first person to join will always be the FC
			// They cannot leave
			np.isFC = roster.Count == 0;

			roster.Add(np);

			if(announce)
				PushToAll(new ANWI.Messaging.Ops.UpdateRoster(
						new List<OpParticipant>() { np },
						null
					));
		}

		public void RemoveUser(string token) {
			// TODO
		}
		#endregion

		#region Fleet Composition
		public void AddFleetShip(int id) {
			Ship ship = new Ship();
			ship.uuid = ANWI.Utility.UUID.GenerateUUID();
			ship.v = LiteVessel.FetchById(id);
			ship.isFlagship = false;

			fleet.AddUnit(ship);

			PushToAll(new ANWI.Messaging.Ops.UpdateUnitsShips(
					new List<Ship>() { ship },
					null
				));
		}

		public void AddWing() {
			Wing wing = new Wing() {
				uuid = ANWI.Utility.UUID.GenerateUUID(),
				name = $"New Wing {lastWingNumber}",
				callsign = "No Callsign",
				primaryRole = Wing.Role.CAP
			};

			fleet.AddUnit(wing);

			PushToAll(new ANWI.Messaging.Ops.UpdateUnitsWings(
				new List<Wing>() { wing },
				null));
		}

		public void AddBoat(string wingUUID, int hullID) {
			Boat boat = new Boat() {
				uuid = ANWI.Utility.UUID.GenerateUUID(),
				wingUUID = wingUUID,
				type = Hull.FetchById(hullID),
				isWC = false
			};

			// Add a pilot position by default to make things
			// easy for the FC
			boat.positions.Add(new OpPosition() {
				uuid = ANWI.Utility.UUID.GenerateUUID(),
				unitUUID = boat.uuid,
				critical = false,
				role = new OperationRole() {
					id = 4,
					name = "Pilot",
					rateAbbrev = "FP"
				}
			});

			fleet.AddUnit(boat);

			PushToAll(new ANWI.Messaging.Ops.UpdateUnitsBoats(
				new List<Boat>() { boat },
				null));
		}

		public void DeleteFleetUnit(string uuid) {

			// No need to push assingment changes to users because the OOB
			// class will do it automatically on their end.
			fleet.DeleteUnit(uuid);
			
			PushToAll(new ANWI.Messaging.Ops.UpdateUnitsShips(
				null,
				new List<string>() { uuid }));
		}

		public void ModifyUnit(ANWI.Messaging.Ops.ModifyUnit mod) {
			switch (mod.type) {
				case ANWI.Messaging.Ops.ModifyUnit.ChangeType.SetFlagship:
					fleet.SetFlagship(mod.unitUUID);
					PushToAll(new ANWI.Messaging.Ops.UpdateShip() {
						shipUUID = mod.unitUUID,
						type = ANWI.Messaging.Ops.UpdateShip.Type.SetFlagship
					});
					break;

				case ANWI.Messaging.Ops.ModifyUnit.ChangeType.SetWingCommander:
					fleet.SetWingCommander(mod.unitUUID);
					PushToAll(new ANWI.Messaging.Ops.UpdateWing() {
						type = ANWI.Messaging.Ops.UpdateWing.Type.ChangeWingCommander,
						boatUUID = mod.unitUUID
					});
					break;

				case ANWI.Messaging.Ops.ModifyUnit.ChangeType.ChangeName:
					fleet.SetWingName(mod.unitUUID, mod.str);
					PushToAll(new ANWI.Messaging.Ops.UpdateWing() {
						wingUUID = mod.unitUUID,
						type = ANWI.Messaging.Ops.UpdateWing.Type.SetName,
						str = mod.str
					});
					break;

				case ANWI.Messaging.Ops.ModifyUnit.ChangeType.ChangeCallsign:
					fleet.SetWingCallsign(mod.unitUUID, mod.str);
					PushToAll(new ANWI.Messaging.Ops.UpdateWing() {
						wingUUID = mod.unitUUID,
						type = ANWI.Messaging.Ops.UpdateWing.Type.SetCallsign,
						str = mod.str
					});
					break;

				case ANWI.Messaging.Ops.ModifyUnit.ChangeType.ChangeWingRole:
					fleet.ChangeWingRole(mod.unitUUID, (Wing.Role)mod.integer);
					PushToAll(new ANWI.Messaging.Ops.UpdateWing() {
						wingUUID = mod.unitUUID,
						type = ANWI.Messaging.Ops.UpdateWing.Type.ChangeRole,
						integer = mod.integer
					});
					break;
			}
		}
		#endregion

		#region Positions
		public void AddPosition(string unitUUID, int roleID) {
			OpPosition pos = new OpPosition() {
				uuid = ANWI.Utility.UUID.GenerateUUID(),
				unitUUID = unitUUID,
				critical = false,
				filledById = -1,
				filledByPointer = null,
				role = OperationRole.FetchById(roleID)
			};

			fleet.AddPosition(pos);

			PushToAll(new ANWI.Messaging.Ops.UpdatePositions(
				new List<OpPosition>() { pos },
				null,
				null));
		}

		public void DeletePosition(string uuid) {
			fleet.DeletePosition(uuid);

			PushToAll(new ANWI.Messaging.Ops.UpdatePositions(
				null,
				null,
				new List<string>() { uuid }));
		}

		public void 
		ChangeAssignment(string posUUID, int userID) {
			if (posUUID == "" && userID != -1) {
				OpParticipant user = GetParticipant(userID);
				if (user == null)
					return;

				// Unassign this user
				if (user.position != null) {
					fleet.ClearPosition(user.position.uuid);

					PushToAll(new ANWI.Messaging.Ops.UpdateAssignments(
						null,
						new List<int>() { userID },
						null));
				}
			} else if (userID == -1 && posUUID != "") {
				// Unassign this position
				fleet.ClearPosition(posUUID);

				PushToAll(new ANWI.Messaging.Ops.UpdateAssignments(
					null,
					null,
					new List<string>() { posUUID }));
			} else {
				OpParticipant user = GetParticipant(userID);
				if (user == null)
					return;

				// Assign user to this position
				fleet.AssignPosition(posUUID, user);

				PushToAll(new ANWI.Messaging.Ops.UpdateAssignments(
					new List<System.Tuple<int, string>>() {
						new System.Tuple<int, string>(userID, posUUID),
					},
					null, null));
			}
		}

		public void SetPositionCritical(string uuid, bool crit) {
			OpPosition pos = fleet.GetPosition(uuid);
			if(pos != null) {
				pos.critical = crit;

				PushToAll(new ANWI.Messaging.Ops.UpdatePositions(
					null,
					new List<OpPosition>() { pos },
					null
					));
			}
		}
		#endregion

		private void PushToAll(ANWI.Messaging.IMessagePayload p) {
			MemoryStream stream = new MemoryStream();
			MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(
				stream, new ANWI.Messaging.Message(-1, p));
			byte[] array = stream.ToArray();

			foreach (KeyValuePair<string, ConnectedUser> user in subscribed) {
				if(user.Value.socket != null)
					user.Value.socket.Send(array);
			}
		}
	}
}

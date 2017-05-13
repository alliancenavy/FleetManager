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

		private List<FleetCompElement> fleet = new List<FleetCompElement>();
		public int fleetCount { get { return fleet.Count; } }

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
				status = status
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
			return null;
		}

		private FleetCompElement GetOOBElement(string uuid) {
			foreach(FleetCompElement e in fleet) {
				if (e.uuid == uuid)
					return e;
			}
			return null;
		}

		private OpPosition GetShipPosition(NamedShip ship, string uuid) {
			foreach(OpPosition pos in ship.positions) {
				if (pos.uuid == uuid)
					return pos;
			}
			return null;
		}
		#endregion

		#region Participant Management
		public void SubscribeUser(ConnectedUser user) {
			subscribed.Add(user.token, user);

			// Join the first subscribing user to the roster
			// This will always be the FC
			if (rosterCount == 0)
				JoinUser(user.token, false);
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
			NamedShip ship = new NamedShip();
			ship.uuid = ANWI.Utility.UUID.GenerateUUID();
			ship.v = LiteVessel.FetchById(id);
			ship.isFlagship = false;
			ship.positions.Add(new OpPosition() {
				elemUUID = ship.uuid,
				uuid = ANWI.Utility.UUID.GenerateUUID(),
				role = new AssignmentRole() {
					name = "Pilot"
				},
				critical = true
			});

			fleet.Add(ship);
			PushToAll(new ANWI.Messaging.Ops.UpdateOOBShips(
					new List<NamedShip>() { ship },
					null
				));
		}

		public void AddWing() {
			Wing w = new Wing() {
				uuid = ANWI.Utility.UUID.GenerateUUID(),
				name = $"New Wing {lastWingNumber}",
				callsign = "No Callsign",
				primaryRole = Wing.Role.CAP,
				members = new List<WingMember>()
			};

			PushToAll(new ANWI.Messaging.Ops.UpdateOOBWings(
				new List<Wing>() { w },
				null));
		}

		public void DeleteFleetElement(string uuid) {
			FleetCompElement elem = GetOOBElement(uuid);

			if (elem != null) {
				if (elem is NamedShip) {
					NamedShip ship = elem as NamedShip;

					// Unassign all positions
					List<int> changedPos = new List<int>();
					foreach (OpPosition pos in ship.positions) {
						if (pos.filledById != -1) {
							changedPos.Add(pos.filledById);
							OpPosition.UnassignPosition(pos);
						}
					}

					if (changedPos.Count > 0) {
						PushToAll(new ANWI.Messaging.Ops.UpdateAssignments(
							null, changedPos));
					}
				}

				fleet.Remove(elem);
				PushToAll(new ANWI.Messaging.Ops.UpdateOOBShips(
					null,
					new List<string>() { elem.uuid }));
			}
		}
		#endregion

		#region Positions
		public void 
		ChangeAssignment(string elem, string wingmem, string pos, int user) {
			FleetCompElement unit = GetOOBElement(elem);
			if (unit == null)
				return;

			if(unit is NamedShip) {
				OpPosition job = GetShipPosition(unit as NamedShip, pos);
				OpParticipant member = GetParticipant(user);
				if (job == null && member == null)
					return;

				ANWI.Messaging.Ops.UpdateAssignments update
					= new ANWI.Messaging.Ops.UpdateAssignments();

				update.added
					= new List<ANWI.Messaging.Ops.UpdateAssignments.AssignTo>();
				update.added.Add(
					new ANWI.Messaging.Ops.UpdateAssignments.AssignTo() {
						posUUID = pos,
						shipUUID = elem,
						userId = user
					});

				if (job.filledById != -1) {
					update.removed = new List<int>() { job.filledById };
					OpPosition.UnassignPosition(job);
				}

				OpPosition.AssignPosition(job, member);
				PushToAll(update);
			}
		}

		#endregion

		private void PushToAll(ANWI.Messaging.IMessagePayload p) {
			MemoryStream stream = new MemoryStream();
			MessagePackSerializer.Get<ANWI.Messaging.Message>().Pack(
				stream, new ANWI.Messaging.Message(-1, p));
			byte[] array = stream.ToArray();

			foreach (KeyValuePair<string, ConnectedUser> user in subscribed) {
				user.Value.socket.Send(array);
			}
		}
	}
}

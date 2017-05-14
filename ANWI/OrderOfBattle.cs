using ANWI.FleetComp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANWI {
	/// <summary>
	/// Organizes all ships and positions
	/// </summary>
	public class OrderOfBattle {

		//
		// Ships and wings in the fleet
		private List<FleetUnit> fleetList = new List<FleetUnit>();
		private Dictionary<string, FleetUnit> fleetLookup 
			= new Dictionary<string, FleetUnit>();
		public ReadOnlyCollection<FleetUnit> Fleet {
			get { return fleetList.AsReadOnly(); }
		}
		public int FleetSize { get { return fleetList.Count; } }

		//
		// Easy access position map
		private Dictionary<string, OpPosition> positionsLookup
			= new Dictionary<string, OpPosition>();
		public int TotalPositions { get { return positionsLookup.Count; } }
		public int TotalCriticalPositions { get {
				int count = 0;
				foreach(KeyValuePair<string, OpPosition> p in positionsLookup) {
					if (p.Value.critical)
						count++;
				}
				return count;
			} }

		//
		// Easy access to wing members
		private Dictionary<string, Boat> boatLookup
			= new Dictionary<string, Boat>();
		public int TotalBoats { get { return boatLookup.Count; } }

		public OrderOfBattle() {
			// Empty
		}

		public FleetUnit GetUnit(string uuid) {
			// Check for ships and wings with this ID
			FleetUnit unit;
			if(!fleetLookup.TryGetValue(uuid, out unit)) {
				// If none was found check boats
				Boat boat;
				boatLookup.TryGetValue(uuid, out boat);
				unit = boat;
			}

			return unit;
		}

		public OpPosition GetPosition(string uuid) {
			OpPosition pos;
			positionsLookup.TryGetValue(uuid, out pos);
			return pos;
		}

		/// <summary>
		/// Adds a unit to the fleet
		/// </summary>
		/// <param name="unit"></param>
		public void AddUnit(FleetUnit unit) {
			// Check that this ship is not already in the list
			if (GetUnit(unit.uuid) != null)
				throw new ArgumentException(
					$"Ship {unit.uuid} already in fleet");

			fleetLookup.Add(unit.uuid, unit);
			fleetList.Add(unit);

			// Add relevant members to lookup dictionaries
			if(unit is Ship) {
				Ship ship = unit as Ship;

				// Add all positions
				foreach(OpPosition p in ship.positions) {
					positionsLookup.Add(p.uuid, p);
				}
			} else if(unit is Wing) {
				Wing wing = unit as Wing;

				// Add each boat
				foreach(Boat b in wing.members) {
					boatLookup.Add(b.uuid, b);

					// Add each position to the lookup
					foreach(OpPosition p in b.positions) {
						positionsLookup.Add(p.uuid, p);
					}
				}
			} else if(unit is Boat) {
				Boat boat = unit as Boat;

				Wing wing = GetUnit(boat.wingUUID) as Wing;
				wing.members.Add(boat);

				// Add all positions
				foreach(OpPosition p in boat.positions) {
					positionsLookup.Add(p.uuid, p);
				}

				boatLookup.Add(boat.uuid, boat);
			}
		}

		/// <summary>
		/// Deletes a unit from the fleet
		/// </summary>
		/// <param name="uuid"></param>
		/// <returns>A list of all player IDs which were assigned to
		/// positions on this ship</returns>
		public List<int> DeleteUnit(string uuid) {
			FleetUnit unit = GetUnit(uuid);

			// Clear all lookups
			List<int> removedPos = new List<int>();
			if(unit is Ship) {
				Ship ship = unit as Ship;
				foreach(OpPosition pos in ship.positions) { 
					// Unassign the position from the user
					if(pos.filledById != -1) {
						removedPos.Add(pos.filledById);
						ClearPosition(pos.uuid);
					}

					positionsLookup.Remove(pos.uuid);
				}

				// Remove from the fleet
				fleetLookup.Remove(uuid);
				fleetList.Remove(ship);

			} else if(unit is Wing) {
				// Remove each boat from the lookup
				Wing wing = unit as Wing;
				foreach(Boat boat in wing.members) {
					boatLookup.Remove(boat.uuid);

					// Unassign each position on the board
					foreach(OpPosition pos in boat.positions) {
						if(pos.filledById != -1) {
							removedPos.Add(pos.filledById);
							ClearPosition(pos.uuid);
						}

						positionsLookup.Remove(pos.uuid);
					}
				}

				// Remove from the fleet
				fleetLookup.Remove(uuid);
				fleetList.Remove(wing);

			} else if(unit is Boat) {
				Boat boat = unit as Boat;
				if (boat.wingUUID == "")
					throw new ArgumentException(
						"Boat must have wingUUID to be added");

				// Remove all positions from the lookup
				foreach(OpPosition pos in boat.positions) {
					if(pos.filledById != -1) {
						removedPos.Add(pos.filledById);
						ClearPosition(pos.uuid);
					}

					positionsLookup.Remove(pos.uuid);
				}

				// Remove boat from lookup and wing
				boatLookup.Remove(boat.uuid);
				Wing wing = GetUnit(boat.wingUUID) as Wing;
				wing.members.Remove(boat);
			}

			return removedPos;
		}

		/// <summary>
		/// Adds a position to a ship or boat set in the positions unitUUID
		/// field.
		/// </summary>
		/// <param name="pos"></param>
		public void AddPosition(OpPosition pos) {
			if (pos.unitUUID == "")
				throw new ArgumentException(
					"Position must have a unitUUID to be added");

			FleetUnit unit = GetUnit(pos.unitUUID);

			if (unit is Ship) {
				Ship ship = unit as Ship;
				ship.positions.Add(pos);
				positionsLookup.Add(pos.uuid, pos);
			} else if (unit is Boat) {
				Boat boat = unit as Boat;
				boat.positions.Add(pos);
				positionsLookup.Add(pos.uuid, pos);
			}
		}

		/// <summary>
		/// Removes a position from the fleet
		/// </summary>
		/// <param name="uuid"></param>
		public void DeletePosition(string uuid) {
			OpPosition pos = GetPosition(uuid);
			if(pos != null) {
				// Remove the position from the unit it belongs to
				FleetUnit unit = GetUnit(pos.unitUUID);
				if(unit != null) {
					if(unit is Ship) {
						(unit as Ship).positions.Remove(pos);
					} else if(unit is Boat) {
						(unit as Boat).positions.Remove(pos);
					}
				}

				ClearPosition(uuid);

				// Remove position from the lookup
				positionsLookup.Remove(uuid);
			}
		}

		/// <summary>
		/// Assigns a given participant to a position in the fleet
		/// </summary>
		/// <param name="uuid"></param>
		/// <param name="member"></param>
		public void AssignPosition(string uuid, OpParticipant member) {
			OpPosition job = GetPosition(uuid);
			if (job == null)
				return;

			if (job.filledByPointer != null)
				ClearPosition(uuid);
			if (member.position != null)
				ClearPosition(member.position.uuid);

			member.position = job;
			job.filledByPointer = member;
			job.filledById = member.profile.id;
		}

		/// <summary>
		/// Removes a participant from a position
		/// </summary>
		/// <param name="uuid"></param>
		public void ClearPosition(string uuid) {
			OpPosition job = GetPosition(uuid);
			if (job == null)
				return;

			if (job.filledByPointer != null)
				job.filledByPointer.position = null;
			job.filledByPointer = null;
			job.filledById = -1;
		}

		/// <summary>
		/// Sets a given ship as the flagship for the fleet
		/// </summary>
		/// <param name="uuid"></param>
		public void SetFlagship(string uuid) {
			Ship newFlag = GetUnit(uuid) as Ship;
			if (newFlag == null)
				return;

			newFlag.isFlagship = true;

			// Find the current flagship if there is one
			foreach (FleetUnit unit in fleetList) {
				if(unit is Ship) {
					(unit as Ship).isFlagship = false;
				}
			}	
		}
	}
}

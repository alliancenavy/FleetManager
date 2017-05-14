using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ANWI;
using ANWI.FleetComp;
using System.Collections.Generic;

namespace AFOSTests {
	[TestClass]
	public class OrderOfBattleTests {

		private static OrderOfBattle fleet;
		private static List<OpParticipant> roster;

		[ClassInitialize]
		public static void SetUp(TestContext context) {
			fleet = new OrderOfBattle();

			#region Roster Setup
			roster = new List<OpParticipant>() {
				new OpParticipant() {
				isFC = true,
				profile = new LiteProfile() {
					nickname = "Mazer Ludd",
					rank = new Rank() {
						name = "Captain",
						abbrev = "CAPT",
						ordering = 5
					},
					primaryRate = new Rate() {
						name = "Skipper",
						abbrev = "SK",
						rank = 2
					}
				} },
				new OpParticipant() {
				isFC = false,
				profile = new LiteProfile() {
					nickname = "Spaceman Timmy",
					rank = new Rank() {
						name = "Spacer",
						abbrev = "S",
						ordering = 1
					},
					primaryRate = new Rate() {
						name = "Quartermaster",
						abbrev = "QM",
						rank = 2
					}
				} },
				new OpParticipant() {
				isFC = false,
				profile = new LiteProfile() {
					nickname = "Karen Fielding",
					rank = new Rank() {
						name = "Senior Spacer",
						abbrev = "SS",
						ordering = 2
					},
					primaryRate = new Rate() {
						name = "Fighter Pilot",
						abbrev = "FP",
						rank = 2
					}
				} },
				new OpParticipant() {
				isFC = false,
				profile = new LiteProfile() {
					nickname = "Sinai Zaftig",
					rank = new Rank() {
						name = "Senior Spacer",
						abbrev = "SS",
						ordering = 2
					},
					primaryRate = new Rate() {
						name = "Fighter Pilot",
						abbrev = "FP",
						rank = 2
					}
				} }
			};
			#endregion
		}

		[TestMethod]
		public void AddingUnits() {
			//
			// First add one capital ship with one position
			Ship ship = new Ship() {
				uuid = "klkj4rlkjlkj2-asd-ei2",
				v = new LiteVessel() {
					name = "ANS Legend of Dave"
				},
				isFlagship = false
			};
			ship.positions.Add(new OpPosition() {
				uuid = "409ueomnllknqlknlkn",
				unitUUID = ship.uuid,
				role = new AssignmentRole() {
					name = "Pilot"
				},
				critical = true
			});

			fleet.AddUnit(ship);
			Assert.AreEqual(1, fleet.FleetSize);
			Assert.AreEqual(1, fleet.TotalPositions);

			//
			// Add a wing with one boat
			Wing wing = new Wing() {
				uuid = "a95ujskslkvnskent",
				name = "Fighter CAP",
				callsign = "Dickthunder",
				primaryRole = Wing.Role.CAP
			};
			wing.members.Add(new Boat() {
				uuid = "498osjblj4lksjlkaj",
				wingUUID = wing.uuid,
				callsign = "Dickthunder 1",
				isWC = true
			});
			wing.members[0].positions.Add(new OpPosition() {
				uuid = "4uvlkjlkj3qrlkj",
				unitUUID = wing.uuid,
				critical = true,
				role = new AssignmentRole() {
					name = "Pilot"
				}
			});

			fleet.AddUnit(wing);
			Assert.AreEqual(2, fleet.FleetSize);
			Assert.AreEqual(2, fleet.TotalPositions);
			Assert.AreEqual(2, fleet.TotalCriticalPositions);
			Assert.AreEqual(1, fleet.TotalBoats);

			//
			// Add a boat to the first wing
			Boat boat = new Boat() {
				uuid = "868ekjxj2lk;l3k",
				wingUUID = "a95ujskslkvnskent",
				isWC = false,
				callsign = "Dickthunder 2"
			};
			boat.positions.Add(new OpPosition() {
				uuid = "7848whd2jdalkj3",
				unitUUID = wing.uuid,
				critical = true,
				role = new AssignmentRole() {
					name = "Pilot"
				}
			});

			fleet.AddUnit(boat);
			Assert.AreEqual(2, fleet.TotalBoats);
			Assert.AreEqual(3, fleet.TotalPositions);
			Assert.AreEqual(3, fleet.TotalCriticalPositions);

			Wing checkWing = fleet.GetUnit("a95ujskslkvnskent") as Wing;
			Assert.AreEqual(boat.uuid, checkWing.members[1].uuid);
		}

		[TestMethod]
		public void AddPositions() {

			//
			// Add a Skipper position to the Legend of Dave
			OpPosition one = new OpPosition() {
				uuid = "2jrj2jsnn3kjnksnr",
				unitUUID = "klkj4rlkjlkj2-asd-ei2",
				critical = false,
				role = new AssignmentRole() {
					name = "Skipper"
				}
			};
			fleet.AddPosition(one);

			Assert.AreEqual(4, fleet.TotalPositions);
			Assert.AreEqual(3, fleet.TotalCriticalPositions);

			Ship dave = fleet.GetUnit("klkj4rlkjlkj2-asd-ei2") as Ship;
			Assert.AreEqual(2, dave.positions.Count);
			Assert.AreEqual("2jrj2jsnn3kjnksnr", dave.positions[1].uuid);

			//
			// Add a copilot position to Dickthunder 1
			OpPosition two = new OpPosition() {
				uuid = "4832yufdjn45iua",
				unitUUID = "498osjblj4lksjlkaj",
				critical = false,
				role = new AssignmentRole() {
					name = "Co-Pilot"
				}
			};

			fleet.AddPosition(two);
			Assert.AreEqual(5, fleet.TotalPositions);
			Assert.AreEqual(3, fleet.TotalCriticalPositions);

			Boat d1 = fleet.GetUnit("498osjblj4lksjlkaj") as Boat;
			Assert.AreEqual("4832yufdjn45iua", d1.positions[1].uuid);
		}

		[TestMethod]
		public void AssigningUsers() {
			//
			// Assign Mazer as the Dave's skipper
			// Assign Timmy as the Dave's pilot
			fleet.AssignPosition("2jrj2jsnn3kjnksnr", roster[0]);
			fleet.AssignPosition("409ueomnllknqlknlkn", roster[1]);

			Ship dave = fleet.GetUnit("klkj4rlkjlkj2-asd-ei2") as Ship;
			Assert.AreEqual(roster[0], dave.positions[1].filledByPointer);
			Assert.AreEqual(roster[1], dave.positions[0].filledByPointer);

			//
			// Assign Karen as the Dickthunder 1 Pilot
			fleet.AssignPosition("4uvlkjlkj3qrlkj", roster[2]);

			Boat d1 = fleet.GetUnit("498osjblj4lksjlkaj") as Boat;
			Assert.AreEqual(roster[2], d1.positions[0].filledByPointer);

			//
			// Replace Karen with Sinai
			fleet.AssignPosition("4uvlkjlkj3qrlkj", roster[3]);
			Assert.AreEqual(roster[3], d1.positions[0].filledByPointer);
			Assert.IsNull(roster[2].position);

			//
			// Assign Karen to Dickthunder 1 Co-Pilot
			fleet.AssignPosition("4832yufdjn45iua", roster[2]);
			Assert.AreEqual(roster[2], d1.positions[1].filledByPointer);
		}

		[TestMethod]
		public void DeletePosition() {
			//
			// Delete the co-pilot position for Dickthunder 1
			fleet.DeletePosition("4832yufdjn45iua");

			Assert.AreEqual(4, fleet.TotalPositions);
			Assert.IsNull(roster[2].position);
		}

		[TestMethod]
		public void DeleteUnits() {
			//
			// Delete the Legend of Dave
			// This should remove 2 positions  and leave Mazer
			// and Timmy unassigned
			fleet.DeleteUnit("klkj4rlkjlkj2-asd-ei2");

			Assert.AreEqual(2, fleet.TotalPositions);
			Assert.IsNull(roster[0].position);
			Assert.IsNull(roster[1].position);

			//
			// Delete Dickthunder 2
			fleet.DeleteUnit("868ekjxj2lk;l3k");
			Assert.AreEqual(1, fleet.TotalPositions);
			Assert.AreEqual(1, fleet.TotalBoats);

			//
			// Delete Dickthunder wing.
			// This should remove all remaining positions and 
			// leave Sinai unassigned
			fleet.DeleteUnit("a95ujskslkvnskent");
			Assert.AreEqual(0, fleet.TotalBoats);
			Assert.AreEqual(0, fleet.TotalPositions);
			Assert.AreEqual(0, fleet.TotalCriticalPositions);
			Assert.IsNull(roster[3].position);
		}
	}
}

BEGIN TRANSACTION;

PRAGMA foreign_key = "OFF";

DELETE FROM Assignment;
DELETE FROM AssignmentRole;
DELETE FROM Hull;
DELETE FROM HullRole;
DELETE FROM HullVendor;
DELETE FROM Rank;
DELETE FROM Rate;
DELETE FROM StruckRate;
DELETE FROM User;
DELETE FROM UserShip;

PRAGMA foreign_key = "ON";

---------------------------------------
--Ranks
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (1, "Spacer", "SP", "1", 1);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (2, "Senior Spacer", "SSP", "2", 2);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (3, "Petty Officer", "PO", "3", 3);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (4, "Chief Petty Officer", "CPO", "4", 4);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (5, "Lieutenant", "LT", "5", 5);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (6, "Captain", "CAPT", "6", 6);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (7, "Commodore", "CDRE", "7", 7);

---------------------------------------
--Rate
INSERT INTO Rate (id, name, abrv) VALUES (0, "Undesignated", "UN");
INSERT INTO Rate (id, name, abrv) VALUES (1, "Cargo Pilot", "CP");
INSERT INTO Rate (id, name, abrv) VALUES (2, "Fighter Pilot", "FP");
INSERT INTO Rate (id, name, abrv) VALUES (3, "Dropship Pilot", "DP");
INSERT INTO Rate (id, name, abrv) VALUES (4, "Aerospace Mechanic", "AM");
INSERT INTO Rate (id, name, abrv) VALUES (5, "Aerospace Crewman", "AC");
INSERT INTO Rate (id, name, abrv) VALUES (6, "Quartermaster", "QM");
INSERT INTO Rate (id, name, abrv) VALUES (7, "Gunner's Mate", "GM");
INSERT INTO Rate (id, name, abrv) VALUES (8, "Damage Controlman", "DC");
INSERT INTO Rate (id, name, abrv) VALUES (9, "Skipper", "SK");
INSERT INTO Rate (id, name, abrv) VALUES (10, "Fleet Commander", "FC");
INSERT INTO Rate (id, name, abrv) VALUES (11, "Wing Commander", "WC");
INSERT INTO Rate (id, name, abrv) VALUES (12, "Section Leader", "SL");
INSERT INTO Rate (id, name, abrv) VALUES (13, "Boarding Specialist", "BS");

---------------------------------------
--Assignment roles
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (0, "Commanding Officer", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (1, "First Officer", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (2, "Chief Engineer", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (3, "Commander, Air Group", 0);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (4, "Commander, Marine Detachment", 0);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (5, "Helmsman", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (6, "Gunner", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (7, "Drone Operator", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (8, "Fireman", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (9, "Mechanic", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (10, "Crewman", 1);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (11, "Pilot", 0);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (12, "Air Crewman", 0);
INSERT INTO AssignmentRole (id, name, isCompany) VALUES (13, "Marine", 0);

---------------------------------------
--Operation Roles
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Fleet Commander", 10, 1, 0, 0, 1);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Captain", 9, 1, 0, 0, 1);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Helmsman", 6, 1, 0, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Pilot", 2, 0, 1, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Co-Pilot", 2, 0, 1, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Gunner", 7, 1, 0, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Fireman", 8, 1, 0, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Mechanic", 4, 1, 1, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Crew", 5, 0, 1, 0, 0);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Section Leader", 12, 0, 0, 1, 1);
INSERT INTO OperationRole (name, associatedRate, onShips, onBoats, inSquads, channelCdr) VALUES ("Marine", 13, 0, 0, 1, 0);

---------------------------------------
--Hull Vendors
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (1, "Roberts Space Industries", "RSI", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (2, "Aegis Dynamics", "AEGS", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (3, "Origin Jumpworks", "Origin", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (4, "Musashi Industrial & Starflight Concern", "MISC", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (5, "Esperia", "Esperia", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (6, "Anvil Aerospace", "Anvil", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (7, "Drake Interplanetary", "Drake", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (8, "Crusader Industries", "Crusader", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (9, "Aopoa", "Aopoa", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (10, "Kruger Intergalactic", "Kruger", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (11, "Consolidated Outland", "Consolidated", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (12, "BIRC", "BIRC", "");
INSERT INTO HullVendor (id, name, abrv, icon) VALUES (13, "Banu", "Banu", "");

---------------------------------------
--Hull Roles
INSERT INTO HullRole (id, name, icon) VALUES (0,"Carrier","");
INSERT INTO HullRole (id, name, icon) VALUES (1,"Destroyer","");
INSERT INTO HullRole (id, name, icon) VALUES (2,"Frigate","");
INSERT INTO HullRole (id, name, icon) VALUES (3,"Corvette","");
INSERT INTO HullRole (id, name, icon) VALUES (4,"Cutter","");
INSERT INTO HullRole (id, name, icon) VALUES (5,"Oiler","");
INSERT INTO HullRole (id, name, icon) VALUES (6,"Multirole","");
INSERT INTO HullRole (id, name, icon) VALUES (7,"Recon","");
INSERT INTO HullRole (id, name, icon) VALUES (8,"Hospital","");
INSERT INTO HullRole (id, name, icon) VALUES (9,"Industrial","");
INSERT INTO HullRole (id, name, icon) VALUES (10,"Freighter","");
INSERT INTO HullRole (id, name, icon) VALUES (11,"Transport","");
INSERT INTO HullRole (id, name, icon) VALUES (12,"Fighter","");
INSERT INTO HullRole (id, name, icon) VALUES (13,"Heavy Fighter","");
INSERT INTO HullRole (id, name, icon) VALUES (14,"Racer","");
INSERT INTO HullRole (id, name, icon) VALUES (15,"Gunship","");
INSERT INTO HullRole (id, name, icon) VALUES (16,"Bomber","");
INSERT INTO HullRole (id, name, icon) VALUES (17,"E-War","");
INSERT INTO HullRole (id, name, icon) VALUES (18,"Starter","");
INSERT INTO HullRole (id, name, icon) VALUES (19,"Stealth","");

---------------------------------------
--Hulls
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (0,1,0,"Bengal","CV",1);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (1,1,0,"Pegasus","CVE",2);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (2,2,1,"Javelin","DD",3);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (3,2,2,"Idris-P","FF",4);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (4,2,2,"Idris-M","FF",5);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (5,1,3,"Polaris","K",6);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (6,1,4,"Constellation Andromeda","M",7);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (7,4,5,"Starfarer","AO",8);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (8,4,5,"Starfarer Gemini","AO",9);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (9,7,6,"Caterpillar","AG",10);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (10,4,6,"Endeavor","AG",11);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (11,4,8,"Endeavor Hope","AH",12);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (12,1,9,"Orion","AM",13);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (13,2,9,"Reclaimer","ARS",14);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (14,13,10,"Merchantman","AK",15);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (15,4,10,"Hull E","AK",16);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (16,4,10,"Hull D","AK",17);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (17,1,10,"Constellation Taurus","AK",18);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (18,4,7,"Endeavor Discovery","AGS",19);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (19,6,7,"Carrack","AGS",20);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (20,1,7,"Constellation Aquila","AGS",21);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (21,3,11,"890 Jump","AP",22);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (22,8,11,"Genesis","AP",23);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (23,1,11,"Constellation Phoenix","AP",24);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (24,6,9,"Crucible","ARL",25);

INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (25,3,12,"300i","",150);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (26,3,7,"315p","",151);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (27,3,12,"325a","",152);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (28,3,14,"350r","",153);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (29,1,18,"Aurora","",154);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (30,2,12,"Avenger","",155);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (31,5,12,"Blade","",156);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (32,7,12,"Buccaneer","",157);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (33,7,15,"Cutlass Black","",158);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (34,7,15,"Cutlass Blue","",159);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (35,7,15,"Cutlass Red","",160);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (36,13,12,"Defender","",161);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (37,6,12,"F7A Hornet","",162);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (38,6,12,"F7C Hornet","",163);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (39,6,12,"F7C Hornet WildFire","",164);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (40,6,12,"F7C-M Super Hornet","",165);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (41,6,7,"F7C-R Hornet Tracker","",166);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (42,6,19,"F7C-S Hornet Ghost","",167);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (43,6,12,"F8 Lightning","",168);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (44,4,10,"Freelancer","",169);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (45,4,7,"Freelancer DUR","",170);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (46,4,15,"Freelancer MAX","",171);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (47,4,15,"Freelancer MIS","",172);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (48,6,16,"Gladiator","",173);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (49,2,12,"Gladius","",174);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (50,5,12,"Glaive","",175);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (51,7,17,"Herald","",176);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (52,4,10,"Hull A","",177);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (53,4,10,"Hull B","",178);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (54,4,10,"Hull C","",179);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (55,6,12,"Hurricane","",180);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (56,3,14,"M50","",181);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (57,11,18,"Mustang","",182);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (58,5,15,"Prowler","",183);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (59,2,15,"Redeemer","",184);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (60,4,18,"Reliant","",185);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (61,2,16,"Retaliator","",186);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (62,2,12,"Sabre","",187);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (63,2,12,"Sabre Comet","",188);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (64,5,12,"Scythe","",189);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (65,2,7,"Terrapin","",190);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (66,2,13,"Vanguard Harbinger","",191);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (67,2,13,"Vanguard Sentinel","",192);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (68,2,13,"Vanguard Warden","",193);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (69,1,7,"Zeus","",194);

---------------------------------------
--Users
INSERT INTO User (id, name, auth0, rank, rate) VALUES (0, "Fleet", "", 6, null);

COMMIT TRANSACTION;
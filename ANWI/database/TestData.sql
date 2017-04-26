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

---------------------------------------
--Ranks
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (1, "Spacer", "SP", "1", 1);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (2, "Senior Spacer", "SSP", "2", 2);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (3, "Petty Officer", "PO", "3", 3);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (4, "Lieutenant", "LT", "4", 4);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (5, "Captain", "CAPT", "5", 5);
INSERT INTO Rank (id, name, abrv, icon, ordering) VALUES (6, "Commodore", "CDRE", "6", 6);

---------------------------------------
--Rate
INSERT INTO Rate (id, name, abrv, icon) VALUES (0, "Undesignated", "UN", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (1, "Cargo Pilot", "CP", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (2, "Fighter Pilot", "FP", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (3, "Dropship Pilot", "DP", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (4, "Aerospace Mechanic", "AM", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (5, "Aerospace Crewman", "AC", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (6, "Quartermaster", "QM", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (7, "Gunner's Mate", "GM", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (8, "Damage Controlman", "DC", "");
INSERT INTO Rate (id, name, abrv, icon) VALUES (9, "Skipper", "SK", "");

---------------------------------------
--Test Users
INSERT INTO User (id, name, auth0, rank, rate) VALUES (0, "Fleet", "", 6, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (1, "Mazer Ludd", "auth0|58713654d89baa12399d5000", 5, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (2, "Terbius Occato", "auth0|5884d3bd00e2c5269b40948a", 4, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (3, "Jeremy Duport", "auth0|58713654d89baa12399d5123", 6, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (4, "Seaman Timmy", "auth0|58713654d89baa12399d5124", 1, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (5, "Sinai Zaftig", "auth0|58713654d89baa12399d5125", 2, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (6, "Blaine Banjo", "auth0|58713654d89baa12399d5126", 3, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (7, "Someone Useless", "auth0|58713654d89baa12399d5127", 1, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (8, "Versety Sterling", "auth0|58713654d89baa12399d5128", 1, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (9, "Karen Fielding", "auth0|58713654d89baa12399d5129", 2, null);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (10, "Ronox Templar", "auth0|58713654d89baa12399d5130", 1, null);

---------------------------------------
--Struck rates for Users
INSERT INTO StruckRate (id, user, rate, rank) VALUES (1, 1, 2, 1); -- Mazer has FP1
INSERT INTO StruckRate (id, user, rate, rank) VALUES (2, 1, 8, 3); -- Mazer has DC3
INSERT INTO StruckRate (id, user, rate, rank) VALUES (3, 1, 9, 2); -- Mazer has SK2
INSERT INTO StruckRate (id, user, rate, rank) VALUES (4, 1, 6, 1); -- Mazer has QM1
INSERT INTO StruckRate (id, user, rate, rank) VALUES (5, 2, 6, 1); -- Terbius has QM1
INSERT INTO StruckRate (id, user, rate, rank) VALUES (6, 3, 9, 1); -- Jeremy has SK1
INSERT INTO StruckRate (id, user, rate, rank) VALUES (7, 4, 2, 2); -- Timmy has FP2
INSERT INTO StruckRate (id, user, rate, rank) VALUES (8, 5, 5, 3); -- Sinai has AC3
INSERT INTO StruckRate (id, user, rate, rank) VALUES (9, 6, 7, 2); -- Blaine has GM2
INSERT INTO StruckRate (id, user, rate, rank) VALUES (10, 7, 8, 3); -- Someone has DC3
INSERT INTO StruckRate (id, user, rate, rank) VALUES (11, 8, 5, 2); -- Versety has AC2
INSERT INTO StruckRate (id, user, rate, rank) VALUES (12, 9, 4, 2); -- Karen has AM2


UPDATE User SET rate=3 WHERE id=1;
UPDATE User SET rate=5 WHERE id=2;
UPDATE User SET rate=6 WHERE id=3;
UPDATE User SET rate=7 WHERE id=4;
UPDATE User SET rate=8 WHERE id=5;
UPDATE User SET rate=9 WHERE id=6;
UPDATE User SET rate=10 WHERE id=7;
UPDATE User SET rate=11 WHERE id=8;
UPDATE User SET rate=12 WHERE id=9;

---------------------------------------
--Privs for Users
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (0,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (1,1,1);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (2,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (3,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (4,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (5,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (6,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (7,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (8,0,0);
INSERT INTO UserPrivs (user, canPromote, canCertify) VALUES (9,0,0);

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
--Assignments
INSERT INTO Assignment (id, user, ship, role) VALUES (1, 1, 1, 1);
INSERT INTO Assignment (id, user, ship, role) VALUES (2, 2, 1, 4);
INSERT INTO Assignment (id, user, ship, role) VALUES (3, 3, 1, 0);
INSERT INTO Assignment (id, user, ship, role) VALUES (4, 4, 1, 11);
INSERT INTO Assignment (id, user, ship, role) VALUES (5, 5, 1, 12);
INSERT INTO Assignment (id, user, ship, role) VALUES (6, 6, 1, 3);
INSERT INTO Assignment (id, user, ship, role) VALUES (7, 7, 1, 8);
INSERT INTO Assignment (id, user, ship, role) VALUES (8, 8, 1, 13);
INSERT INTO Assignment (id, user, ship, role) VALUES (9, 9, 1, 2);
INSERT INTO Assignment (id, user, ship, role) VALUES (10, 10, 1, 13);


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

---------------------------------------
--Hull Roles
INSERT INTO HullRole (id, name, abrv, icon) VALUES (1, "Space Superior Fighter", "SSF", "");
INSERT INTO HullRole (id, name, abrv, icon) VALUES (2, "Interceptor", "INT", "");
INSERT INTO HullRole (id, name, abrv, icon) VALUES (3, "Corvette", "K", "");
INSERT INTO HullRole (id, name, abrv, icon) VALUES (4, "Frigate", "FF", "");
INSERT INTO HullRole (id, name, abrv, icon) VALUES (5, "Destroyer", "DD", "");
INSERT INTO HullRole (id, name, abrv, icon) VALUES (6, "Dropship", "DS", "");
INSERT INTO HullRole (id, name, abrv, icon) VALUES (7, "Cutter", "C", "");

---------------------------------------
--Hulls
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (1, 2, 4, "Idris-P", "FF", 10);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (2, 1, 3, "Polaris", "K", 12);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (3, 1, 7, "Constellation Phoenix", "M", 30);

---------------------------------------
--Owned vessels
INSERT INTO UserShip (id, user, hull, insurance, number, name, status, statusDate) VALUES (1, 0, 1, 0, 10, "ANS Legend of Dave", 0, "2017-01-01");
INSERT INTO UserShip (id, user, hull, insurance, number, name, status, statusDate) VALUES (2, 0, 2, 1, 1, "ANS Everlasting Snowmew", 1, date('now'));

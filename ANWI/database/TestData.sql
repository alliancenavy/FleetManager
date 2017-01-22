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
INSERT INTO Rate (id, name, abrv, icon) VALUES (0, "None", "N", "");
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
INSERT INTO User (id, name, auth0, rank, rate) VALUES (0, "Fleet", "", 6, 0);
INSERT INTO User (id, name, auth0, rank, rate) VALUES (1, "Mazer Ludd", "58713654d89baa12399d5000", 5, null);

---------------------------------------
--Struck rates for Users
INSERT INTO StruckRate (id, user, rate, rank) VALUES (1, 1, 2, 1);
INSERT INTO StruckRate (id, user, rate, rank) VALUES (2, 1, 8, 3);
INSERT INTO StruckRate (id, user, rate, rank) VALUES (3, 1, 9, 2);
INSERT INTO StruckRate (id, user, rate, rank) VALUES (4, 1, 6, 1);

UPDATE User SET rate=3 WHERE id=1;

---------------------------------------
--Assignment roles

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
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (3, 1, 7, "Constellation Phoenix", "C", 30);
INSERT INTO Hull (id, vendor, role, series, symbol, ordering) VALUES (2, 1, 3, "Polaris", "K", 12);

---------------------------------------
--Owned vessels
INSERT INTO UserShip (id, user, hull, insurance, number, name, status, statusDate) VALUES (1, 0, 1, 0, 10, "ANS Legend of Dave", 0, "2017-01-01");
INSERT INTO UserShip (id, user, hull, insurance, number, name, status, statusDate) VALUES (2, 0, 2, 1, 1, "ANS Everlasting Snowmew", 1, date('now'));
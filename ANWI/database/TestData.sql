DELETE FROM Assignment;
DELETE FROM StruckRate;
DELETE FROM User WHERE id != 0;
DELETE FROM UserShip;

---------------------------------------
--Test Users
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
--Assignments
INSERT INTO Assignment (id, user, ship, role, start, until) VALUES (0, 1, 2, 2, 1483228800, 1488326400);
INSERT INTO Assignment (id, user, ship, role, start) VALUES (1, 1, 1, 1, 1488326400);
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
--Owned vessels
INSERT INTO UserShip (id, user, hull, insurance, number, name, status, statusDate) VALUES (1, 0, 3, 0, 10, "ANS Legend of Dave", 0, "2017-01-01");
INSERT INTO UserShip (id, user, hull, insurance, number, name, status, statusDate) VALUES (2, 0, 5, 1, 1, "ANS Everlasting Snowmew", 1, date('now'));

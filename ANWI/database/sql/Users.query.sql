-- most basic user insert
insert into Users (name, joined) values (X, Y);

-- common user insert for new users
insert into Users (name, joined, auth0_id) values (X, Y, Z);

-- change a users cached auth0 id by db id
update Users (auth0_id) values (x) where id = y limit 1;

-- change a users cached auth0 id by auth0 id
update Users (auth0_id) values (x) where auth0_id = y limit 1;

-- change a users rank
update Users (rank_id) values (x) where id = y limit 1;

-- change a users preferred job
update Users (primary_rate_id) values (x) where id = y limit 1;

-- change a users assigned ship
update Users (assigned_ship_id) values (x) where id = y limit 1;

-- get user by name
select * from Users where name = X limit 1;

-- get user by auth0 id
select * from Users where auth0_id = X limit 1;
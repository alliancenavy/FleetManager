-- minimal insert
insert into UserShips (user_id, hull_id, is_lti) values (X, Y, Z);

-- insert with name
insert into UserShips (user_id, hull_id, is_lti, name) values (X, Y, Z, W);

-- change name by id
update UserShips (name) values (X) where id = Y limit 1;

-- change name by name
update UserShips (name) values (X) where name = Y limit 1;

-- remove ship (perhaps it died)
delete from UserShips where id = X limit 1;

-- find ships belonging to user
select id, hull_id, is_lti, name from UserShips where user_id = X;

-- find owners of ship type
select id, user_id from UserShips where hull_id = X;
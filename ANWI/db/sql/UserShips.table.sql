drop table if exists UserShips;

create table if not exists UserShips (
 id integer not null primary key autoincrement,
 user_id integer not null references Users(id),
 hull_id integer not null references Hulls(id),
 is_lti integer not null,
 name text
);
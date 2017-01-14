drop table if exists Users;

create table if not exists Users (
 id integer not null primary key autoincrement,
 name text not null unique,
 joined integer not null,
 auth0_id text,
 rank_id integer references Ranks(id),
 primary_rate_id integer references Rates(id),
 assigned_ship_id integer references UserShips(id)
);
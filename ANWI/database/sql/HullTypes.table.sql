drop table if exists HullTypes;

create table if not exists HullTypes (
 id integer not null primary key autoincrement,
 name text not null unique
);
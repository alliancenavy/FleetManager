drop table if exists HullRoles;

create table if not exists HullRoles (
 id integer not null primary key autoincrement,
 name text not null unique,
 abbreviation text not null unique,
 icon_name text
);
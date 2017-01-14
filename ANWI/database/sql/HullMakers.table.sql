drop table if exists HullMakers;

create table if not exists HullMakers (
 id integer not null primary key autoincrement,
 name text not null unique,
 abbreviation text not null unique,
 icon_name text
);
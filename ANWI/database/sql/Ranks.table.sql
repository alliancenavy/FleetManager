drop table if exists Ranks;

create table if not exists Ranks (
 id integer not null primary key autoincrement,
 name text not null unique,
 abbreviation text not null unique,
 ordering integer not null,
 icon_name text
);
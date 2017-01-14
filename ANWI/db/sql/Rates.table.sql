drop table if exists Rates;

create table if not exists Rates (
 id integer not null primary key autoincrement,
 name text not null unique,
 abbreviation text not null unique,
 icon_name text
);
pragma foreign_keys = "1";

drop table if exists HullSeries;

create table if not exists HullSeries (
 id integer not null primary key autoincrement,
 name text not null unique
);

drop table if exists HullVendor;

create table if not exists HullVendor (
 id integer not null primary key autoincrement,
 name text not null unique,
 abrv text not null unique,
 icon text not null unique
);

drop table if exists HullRole;

create table if not exists HullRole (
 id integer not null primary key autoincrement,
 name text not null unique,
 abrv text not null unique,
 icon text not null unique
);

drop table if exists Hull;

create table if not exists Hull (
 id integer not null primary key autoincrement,
 vendor integer not null references HullVendor(id),
 role integer not null references HullRole(id),
 series integer not null references HullSeries(id),
 version text not null default '',
 symbol text not null default '',
 ordering integer not null default 0
);

drop table if exists Rank;

create table if not exists Rank (
 id integer not null primary key autoincrement,
 name text not null unique,
 abrv text not null unique,
 icon text not null default '',
 ordering integer not null default 0
);

drop table if exists Rate;

create table if not exists Rate (
 id integer not null primary key autoincrement,
 name text not null unique,
 abrv text not null unique,
 icon text not null default ''
);

drop table if exists AssignmentRole;

create table if not exists AssignmentRole (
 id integer not null primary key autoincrement,
 name text not null unique,
 rate integer not null references Rate(id)
);

drop table if exists StruckRate;

create table if not exists StruckRate (
 id integer not null primary key autoincrement,
 user integer not null references User(id),
 rate integer not null references Rate(id),
 rank integer not null default 0
);

drop table if exists User;

create table if not exists User (
 id integer not null primary key autoincrement,
 name text not null unique,
 auth0 text not null default '',
 rank integer not null references Rank(id),
 rate integer not null references StruckRate(id)
);

drop table if exists Assignment;

create table if not exists Assignment (
 id integer not null primary key autoincrement,
 user integer not null references User(id),
 ship integer not null references UserShip(id),
 role integer not null references AssignmentRole(id),
 start integer not null default 0,
 until integer not null default 0
);

drop table if exists UserShip;

create table if not exists UserShip (
 id integer not null primary key autoincrement,
 user integer not null references User(id),
 hull integer not null references Hull(id),
 insurance integer not null default 0,
 number integer not null default 0,
 name text not null unique
);
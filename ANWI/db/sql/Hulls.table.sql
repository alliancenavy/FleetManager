drop table if exists Hulls;

create table if not exists Hulls (
 id integer not null primary key autoincrement,
 type_id integer not null references HullTypes(id),
 subtype text,
 role_id integer not null references HullRoles(id),
 manufacturer_id integer not null references HullMakers(id)
);
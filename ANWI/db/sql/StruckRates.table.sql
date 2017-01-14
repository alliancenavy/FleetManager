drop table if exists StruckRates;

create table if not exists StruckRates (
 id integer not null primary key autoincrement,
 user_id integer not null references Users(id),
 rate_id integer not null references Rates(id),
 rank integer not null default 0
);
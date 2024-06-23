create database [performance_pulse];
go

use [performance_pulse];

create login [service] with password = 'LO^9ZpGB8FiA*HMMQyfN';
create user [service] for login [service];
alter role [db_owner] add member [service];

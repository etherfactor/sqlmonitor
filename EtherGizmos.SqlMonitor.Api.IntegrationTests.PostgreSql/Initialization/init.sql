create database performance_pulse;

\c performance_pulse;

create user service with password 'YUx^S7phYwBMiL3QZes&';
alter database performance_pulse owner to service;

create extension if not exists "uuid-ossp";

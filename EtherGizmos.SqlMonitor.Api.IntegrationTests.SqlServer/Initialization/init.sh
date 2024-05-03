#!/bin/bash

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P b@y4PJFfkiOIK1Ma6tXs -d master -i init.sql

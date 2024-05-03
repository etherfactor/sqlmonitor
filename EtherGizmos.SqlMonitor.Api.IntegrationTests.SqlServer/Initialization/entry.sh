#!/bin/bash

set -m
/opt/mssql/bin/sqlservr & /bin/bash init.sh
fg

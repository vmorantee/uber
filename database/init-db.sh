#!/bin/bash
wait_time=30s
sleep $wait_time

echo "Initializing database..."

/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P RideSharing@2026! -C -i /docker-entrypoint-initdb.d/init.sql
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P RideSharing@2026! -C -i /docker-entrypoint-initdb.d/seed.sql

echo "Database initialization complete!"

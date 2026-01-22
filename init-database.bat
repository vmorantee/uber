@echo off
echo Inicjalizacja bazy danych...
echo.

echo Czekam na uruchomienie SQL Server...
timeout /t 15 /nobreak >nul

echo Tworzę bazę danych i tabele...
docker run --rm --network prajetoo_ridesharing-network -v "%CD%/database:/scripts" mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S ridesharing-sqlserver -U sa -P "RideSharing@2026!" -i /scripts/init.sql

echo.
echo Ładuję dane testowe...
docker run --rm --network prajetoo_ridesharing-network -v "%CD%/database:/scripts" mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S ridesharing-sqlserver -U sa -P "RideSharing@2026!" -i /scripts/seed.sql

echo.
echo Baza danych została zainicjalizowana!
echo.
pause

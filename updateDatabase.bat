@echo off

:: Remove the last migration
echo Removing the last migration...
rmdir /s /q .\Migrations

:: Add a new migration
dotnet ef migrations add InitialCreate

:: Delete the database and create a new one
echo Deleting and recreating the database...
mysql -u simao -psimao -h vsgate-s1.dei.isep.ipp.pt -P 10702 -e "DROP DATABASE Sempi5S; CREATE DATABASE Sempi5S;"

:: Update the database
echo Updating the database...
dotnet ef database update

echo Migration update completed successfully!
pause
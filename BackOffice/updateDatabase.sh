#!/bin/bash

cd App
# Remove the last migration
echo "Removing the last migration..."
rm -rf ./Migrations

# Add a new migration
dotnet ef migrations add InitialCreate

# Delete the table
echo "Deleting the table..."
mysql -u root -psCVevokVcWSa -h vsgate-s1.dei.isep.ipp.pt -P 10702 -e "DROP DATABASE Sempi5; CREATE DATABASE Sempi5;"


# Update the database
echo "Updating the database..."
dotnet ef database update

echo "Migration update completed successfully!"

echo "Deleting the data from the MongoDB database..."
mongosh "mongodb://mongoadmin:80845c6c283856e9ef7452e1@vsgate-s1.dei.isep.ipp.pt:10902/admin" --eval "db.allergies.drop(); db.medicalrecords.drop()"

echo "Data deleted successfully!"

cd ..

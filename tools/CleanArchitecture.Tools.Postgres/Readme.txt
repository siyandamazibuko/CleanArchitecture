https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/

Install
dotnet tool install --global dotnet-ef

Add Migrations

Step 1
dotnet ef migrations add InitialCreate -c CleanArchitecture.Infrastructure.Persistence.ApplicationDbContext

Step 2
dotnet ef database update 

Run if you want to remove a migration, before the update
dotnet ef migrations remove

Run if you want to drop a database using the connection string in appsettings.json
dotnet ef database drop

Development

dotnet run -s -d -t

Deployment

dotnet run -s
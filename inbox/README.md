# Provider Inbox — Aspire + SQL Server (Full)

Run:
```bash
dotnet restore
dotnet run --project src/ProviderInbox.AppHost
```

Dev audience scope (single office per session):
Use query params on read endpoints:
`?npi=1234567890&officeNumber=OFFICE-001`

## EF Core migrations (PostgreSQL)
InitialCreate migration is included under `src/ProviderInbox.Service/Infrastructure/Migrations`.
The service runs `db.Database.Migrate()` during seeding; in production, prefer running migrations separately.

Create future migrations:
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <Name> --project src/ProviderInbox.Service --startup-project src/ProviderInbox.Service
dotnet ef database update --project src/ProviderInbox.Service --startup-project src/ProviderInbox.Service
```


## Note on Aspire version
This solution targets **.NET 9** and uses **.NET Aspire 9.1** packages (legacy) so you don't need .NET 10 installed.


## Fix for AppHost load errors
The AppHost uses the **9.x** AppHost project format (`<Sdk Name="Aspire.AppHost.Sdk" Version="9.5.2" />`) so Visual Studio can load it on **.NET 9**.

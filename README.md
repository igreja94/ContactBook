# ContactBook

A contact book REST API built with .NET 10, following Clean Architecture. Data is stored in SQL Server through EF Core, and local development is orchestrated with .NET Aspire.

## Structure

- `Domain`: entities (`User`, `Contact`)
- `Application`: DTOs, services and repository abstractions
- `Infrastructure`: EF Core `DbContext`, migrations, repositories, seed data
- `Webhost`: ASP.NET Core API (controllers)
- `AppHost` and `ServiceDefaults`: Aspire orchestration (SQL Server container + Webhost) and shared defaults
- Test projects (`Application.Tests`, `Infrastructure.Tests`, `Webhost.Tests`): unit and integration tests

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop/) running (SQL Server runs in a container)
- Optional: `dotnet tool install --global dotnet-ef` to manage migrations

## Run

```powershell
dotnet run --project src/AppHost
```

Aspire starts a SQL Server container, then the Webhost, which applies migrations and seeds sample data in the Development environment. Open the Aspire dashboard from the console output to find the Webhost URL; Swagger UI is served at `/swagger`.

The database lives in the `contactbook-sqlserver-data` Docker volume. To reset it, stop the app and run `docker volume rm contactbook-sqlserver-data`.

# ContactBook

A contact book REST API built with .NET 10, following Clean Architecture. Data is stored in SQL Server through EF Core, and local development is orchestrated with .NET Aspire.

## Structure

- `Domain`: entities
- `Application`: DTOs, services, repository abstractions, mappers and application exceptions
- `Infrastructure`: EF Core, migrations, repositories, seed data
- `Webhost`: ASP.NET Core API (controllers and the global exception handlers)
- `AppHost` and `ServiceDefaults`: Aspire orchestration (SQL Server container + Webhost) and shared defaults
- `Tests`: unit and integration tests, grouped by project (`Application`, `Infrastructure`, `Webhost`)

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop/)
- Optional: `dotnet tool install --global dotnet-ef` to manage migrations

## Run

```powershell
dotnet run --project src/AppHost
```

Aspire starts a SQL Server container, then the Webhost, which applies migrations and seeds sample data in the Development environment. Open the Aspire dashboard from the console output to find the Webhost URL; Swagger UI is served at `/swagger`.

The database lives in the `contactbook-sqlserver-data` Docker volume. To reset it, stop the app and run `docker volume rm contactbook-sqlserver-data`.

## Code analysis

The build runs the .NET analyzers, [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) and the code style rules from [src/.editorconfig](src/.editorconfig) and [src/stylecop.json](src/stylecop.json). Any warning fails the build (NuGet vulnerability warnings excepted), and is also reported in the IDE. Documentation rules are disabled. To adjust a rule, change its severity in `.editorconfig`.

Local Debug builds fix violations automatically before compiling, using `dotnet format` ([src/Directory.Build.targets](src/Directory.Build.targets)). It only runs for projects whose files changed, and is skipped on CI so violations fail the pipeline. Disable it with `dotnet build -p:FormatOnBuild=false`.

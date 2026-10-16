var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver");

// Ephemeral mode (used by the integration tests) always starts a brand new container: random name, no data volume.
if (!bool.TryParse(builder.Configuration["SqlServer:Ephemeral"], out var ephemeral) || !ephemeral)
{
    sqlServer
        .WithContainerName("contactbook-sql-server")
        .WithDataVolume("contactbook-sqlserver-data");
}

// The Webhost reads the "DefaultConnection" key, so the reference below maps the resource to that name.
var database = sqlServer.AddDatabase("contactbook-db", "ContactBookDb");

builder.AddProject<Projects.Webhost>("webhost")
    .WithReference(database, connectionName: "DefaultConnection")
    .WaitFor(database)
    .WithExternalHttpEndpoints();

builder.Build().Run();

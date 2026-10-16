var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithContainerName("contactbook-sql-server")
    .WithDataVolume("contactbook-sqlserver-data");

// The Webhost reads the "DefaultConnection" key, so the reference below maps the resource to that name.
var database = sqlServer.AddDatabase("contactbook-db", "ContactBookDb");

builder.AddProject<Projects.Webhost>("webhost")
    .WithReference(database, connectionName: "DefaultConnection")
    .WaitFor(database)
    .WithExternalHttpEndpoints();

builder.Build().Run();

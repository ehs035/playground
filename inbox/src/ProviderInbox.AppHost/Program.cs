using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Local dev SQL Server container
var sql = builder.AddSqlServer("sql")
                .WithDataVolume();

var db = sql.AddDatabase("providerinboxdb");

builder.AddProject<Projects.ProviderInbox_Service>("providerinbox-service")
       .WithReference(db)
       .WithExternalHttpEndpoints();

builder.Build().Run();

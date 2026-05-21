var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("SqlPassword");

var sql = builder
    .AddSqlServer("sql", port: 2026, password: sqlPassword)
    .WithDataVolume("AspireDataVolume")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpointProxySupport(proxyEnabled: false)
    ;

var db = sql
    .AddDatabase("OnlineToestemmingDb", databaseName: "OnlineToestemming");

builder.Build().Run();

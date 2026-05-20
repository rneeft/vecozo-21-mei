using Data.MigrationService;
using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<AllContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineToestemmingDb")));

var host = builder.Build();
host.Run();
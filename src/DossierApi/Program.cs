using DossierApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineToestemming.Data;
using OnlineToestemming.DossierApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AllContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineToestemmingDb")));

builder.Services.AddHttpClient("IdentityApi", client => 
{
    client.BaseAddress = new Uri(builder.Configuration["Services:IdentityApi"]!);
});

builder.Services.AddHttpClient("PseudoniemApi", client => 
{
    client.BaseAddress = new Uri(builder.Configuration["Services:PseudoniemApi"]!);
});

builder.Services.AddScoped<IPseudoniemService, PseudoniemService>();

var jwtKey = builder.Configuration["JwtSettings:SecretSigningKey"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "Online-Toestemming-Workshop-IdentityApi",
            ValidateAudience = true,
            ValidAudience = "Online-Toestemming-Workshop",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("HealthcareCompany", policy =>
        policy.RequireRole("HealthcareCompany"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapDossierEndpoints();

app.Run();
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;
using PatientWebsite.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("IdentityApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:IdentityApi"]!);
});
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AllContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineToestemmingDb")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/SignIn";
        options.LogoutPath = "/SignOut";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapApiEndpoints();

app.Run();

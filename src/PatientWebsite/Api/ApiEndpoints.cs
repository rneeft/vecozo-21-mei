using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;

namespace PatientWebsite.Api;

public static class ApiEndpoints
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapPost("/api/register", async (
            RegisterRequest request,
            AllContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Bsn) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return Results.BadRequest("Bsn, FirstName and LastName are required.");
            }

            var existing = await db.Pseudoniemen
                .FirstOrDefaultAsync(p => p.Bsn == request.Bsn);

            string pseudoniem;

            if (existing is not null)
            {
                pseudoniem = existing.Pseudoniem;

                var patientExists = await db.Patients
                    .AnyAsync(p => p.Pseudoniem == pseudoniem);
                if (patientExists)
                    return Results.Conflict("An account with this BSN already exists.");
            }
            else
            {
                pseudoniem = Guid.NewGuid().ToString();

                db.Pseudoniemen.Add(new BsnPseudoniem
                {
                    Id = 0,
                    Bsn = request.Bsn,
                    Pseudoniem = pseudoniem
                });
            }

            db.Patients.Add(new Patient
            {
                Id = 0,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Pseudoniem = pseudoniem
            });

            await db.SaveChangesAsync();

            return Results.Ok(new { pseudoniem });
        });

        app.MapPost("/api/signin", async (
            SignInRequest request,
            IHttpClientFactory httpClientFactory) =>
        {
            if (string.IsNullOrWhiteSpace(request.Bsn))
                return Results.BadRequest("Bsn is required.");

            var client = httpClientFactory.CreateClient("IdentityApi");

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new { bsn = request.Bsn }),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var token = (await response.Content.ReadAsStringAsync()).Trim('"');
                return Results.Ok(new { token });
            }

            return Results.Json(new { error = "Authentication failed." },
                statusCode: (int)response.StatusCode);
        });

        app.MapGet("/api/patient", async (
            HttpContext httpContext,
            AllContext db) =>
        {
            var pseudoniem = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(pseudoniem))
                return Results.Unauthorized();

            var patient = await db.Patients
                .FirstOrDefaultAsync(p => p.Pseudoniem == pseudoniem);
            if (patient is null)
                return Results.NotFound();

            var companies = await db.Dossiers
                .Where(d => d.Pseudoniem == pseudoniem)
                .Select(d => new { d.healthcareCompany.Id, d.healthcareCompany.CompanyId, d.healthcareCompany.CompanyName })
                .Distinct()
                .ToListAsync();

            return Results.Ok(new
            {
                patient.FirstName,
                patient.LastName,
                patient.Pseudoniem,
                patient.GavePermission,
                CompaniesWithDossier = companies
            });
        }).RequireAuthorization();

        app.MapPost("/api/patient/permission", async (
            PermissionRequest request,
            HttpContext httpContext,
            AllContext db) =>
        {
            var pseudoniem = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(pseudoniem))
                return Results.Unauthorized();

            var patient = await db.Patients
                .FirstOrDefaultAsync(p => p.Pseudoniem == pseudoniem);
            if (patient is null)
                return Results.NotFound();

            patient.GavePermission = request.Approve;
            await db.SaveChangesAsync();

            return Results.Ok(new { patient.GavePermission });
        }).RequireAuthorization();

        return app;
    }
}

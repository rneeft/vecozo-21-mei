using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;
using OnlineToestemming.DossierApi;
using System.Security.Claims;

namespace DossierApi;

public static class DossierEndpoints
{
    public static WebApplication MapDossierEndpoints(this WebApplication app)
    {
        app.MapPost("/company/register", async (RegisterCompanyRequest request, AllContext db) =>
        {
            var existingCompany = await db.HealthcareCompanies.FirstOrDefaultAsync(c => c.CompanyName == request.CompanyName);
            if (existingCompany is not null)
            {
                return Results.Created($"/company/{existingCompany.CompanyId}", new { existingCompany.CompanyId, existingCompany.CompanyName });
            }

            var company = new HealthcareCompany
            {
                Id = 0,
                CompanyId = Guid.NewGuid().ToString(),
                CompanyName = request.CompanyName
            };

            db.HealthcareCompanies.Add(company);
            await db.SaveChangesAsync();

            return Results.Created($"/company/{company.CompanyId}", new { company.CompanyId, company.CompanyName });
        })
        .WithName("RegisterCompany");

        app.MapPost("/dossier", async (CreateDossierRequest request, ClaimsPrincipal user, AllContext db, IPseudoniemService pseudoniemService) =>
        {
            var companyName = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var company = await db.HealthcareCompanies.FirstOrDefaultAsync(c => c.CompanyName == companyName);
            if (company is null)
            {
                return Results.Forbid();
            }

            var pseudoniem = await pseudoniemService.GetPseudoniemForBsnAsync(request.Bsn);
            if (pseudoniem is null)
            {
                return Results.Problem("Failed to retrieve or create pseudoniem.");
            }

            var existing = await db.Dossiers.FirstOrDefaultAsync(d =>
                d.healthcareCompanyId == company.CompanyId && d.Pseudoniem == pseudoniem);
            if (existing is not null)
            {
                // If company already has dossier, that's fine - use normal OK flow
                return Results.Ok(new { existing.Id, CompanyId = existing.healthcareCompanyId, request.Bsn });
            }

            var dossier = new Dossier
            {
                Id = 0,
                healthcareCompanyId = company.CompanyId,
                Pseudoniem = pseudoniem,
                healthcareCompany = company
            };

            db.Dossiers.Add(dossier);
            await db.SaveChangesAsync();

            return Results.Created($"/dossier/{dossier.Id}", new { dossier.Id, CompanyId = dossier.healthcareCompanyId, request.Bsn });
        })
        .RequireAuthorization("HealthcareCompany")
        .WithName("CreateDossier");

        app.MapGet("/dossier/{bsn}/permission", async (string bsn, ClaimsPrincipal user, AllContext db, IPseudoniemService pseudoniemService) =>
        {
            var companyName = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var company = await db.HealthcareCompanies.FirstOrDefaultAsync(c => c.CompanyName == companyName);
            if (company is null)
            {
                return Results.Forbid();
            }

            var pseudoniem = await pseudoniemService.GetPseudoniemForBsnAsync(bsn);
            if (pseudoniem is null)
            {
                return Results.Forbid();
            }

            var dossier = await db.Dossiers.FirstOrDefaultAsync(d => d.healthcareCompanyId == company.CompanyId && d.Pseudoniem == pseudoniem);
            if (dossier is null)
            {
                return Results.Forbid();
            }

            // Company has dossier - check if patient gave permission
            var patient = await db.Patients.FirstOrDefaultAsync(p => p.Pseudoniem == pseudoniem);
            return patient?.GavePermission ?? false
                ? Results.Ok()
                : Results.Forbid();
        })
        .RequireAuthorization("HealthcareCompany")
        .WithName("CheckPatientPermission");

        app.MapDelete("/dossier/{bsn}", async (string bsn, ClaimsPrincipal user, AllContext db, IPseudoniemService pseudoniemService) =>
        {
            var companyName = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var company = await db.HealthcareCompanies.FirstOrDefaultAsync(c => c.CompanyName == companyName);
            if (company is null)
            {
                return Results.Forbid();
            }

            var pseudoniem = await pseudoniemService.GetPseudoniemForBsnAsync(bsn);
            if (pseudoniem is null)
            {
                return Results.NotFound("Patient not found.");
            }

            var dossier = await db.Dossiers.FirstOrDefaultAsync(d =>
                d.healthcareCompanyId == company.CompanyId && d.Pseudoniem == pseudoniem);

            if (dossier is null)
            {
                return Results.NotFound("Dossier not found for this patient and company.");
            }

            db.Dossiers.Remove(dossier);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .RequireAuthorization("HealthcareCompany")
        .WithName("DeleteDossier");

        return app;
    }
}

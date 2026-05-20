using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;
using System.Security.Claims;

namespace PatientWebsite.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AllContext _dbContext;

    public IndexModel(AllContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Patient? Patient { get; set; }
    public List<HealthcareCompany> CompaniesWithDossier { get; set; } = [];
    public string StatusMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        Patient = await GetPatientAsync();
        if (Patient is not null)
        {
            CompaniesWithDossier = await _dbContext.Dossiers
                .Where(d => d.Pseudoniem == Patient.Pseudoniem)
                .Select(d => d.healthcareCompany)
                .Distinct()
                .ToListAsync();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool approve)
    {
        var patient = await GetPatientAsync();
        if (patient is null)
        {
            StatusMessage = "Patient not found.";
            return Page();
        }

        patient.GavePermission = approve;
        await _dbContext.SaveChangesAsync();

        StatusMessage = approve
            ? "You have approved the sharing of your medical data."
            : "You have declined the sharing of your medical data.";

        Patient = patient;
        return Page();
    }

    private async Task<Patient?> GetPatientAsync()
    {
        var pseudoniem = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(pseudoniem))
            return null;

        return await _dbContext.Patients
            .FirstOrDefaultAsync(p => p.Pseudoniem == pseudoniem);
    }
}

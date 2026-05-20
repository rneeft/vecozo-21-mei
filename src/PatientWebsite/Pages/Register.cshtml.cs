using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;
using System.ComponentModel.DataAnnotations;

namespace PatientWebsite.Pages;

public class RegisterModel : PageModel
{
    private readonly AllContext _dbContext;

    public RegisterModel(AllContext dbContext)
    {
        _dbContext = dbContext;
    }

    [BindProperty]
    [Required]
    [Display(Name = "BSN")]
    public string Bsn { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    public string SuccessMessage { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var existing = await _dbContext.Pseudoniemen
            .FirstOrDefaultAsync(p => p.Bsn == Bsn);

        string pseudoniem;

        if (existing is not null)
        {
            pseudoniem = existing.Pseudoniem;

            var patientExists = await _dbContext.Patients
                .AnyAsync(p => p.Pseudoniem == pseudoniem);
            if (patientExists)
            {
                ModelState.AddModelError(string.Empty, "An account with this BSN already exists.");
                return Page();
            }
        }
        else
        {
            pseudoniem = Guid.NewGuid().ToString();

            _dbContext.Pseudoniemen.Add(new BsnPseudoniem
            {
                Id = 0,
                Bsn = Bsn,
                Pseudoniem = pseudoniem
            });
        }

        _dbContext.Patients.Add(new Patient
        {
            Id = 0,
            FirstName = FirstName,
            LastName = LastName,
            Pseudoniem = pseudoniem
        });

        await _dbContext.SaveChangesAsync();

        SuccessMessage = "Account created successfully! You can now sign in.";
        return Page();
    }
}

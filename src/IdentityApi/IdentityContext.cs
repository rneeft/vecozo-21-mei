using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;

namespace OnlineToestemming.IdentityApi;

public class IdentityContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<HealthcareCompany> HealthcareCompanies { get; set; }

    public DbSet<BsnPseudoniem> Pseudoniemen { get; set; }

    public DbSet<Patient> Patients { get; set; }
}

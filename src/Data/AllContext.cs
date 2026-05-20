using Microsoft.EntityFrameworkCore;

namespace OnlineToestemming.Data;

public class AllContext : DbContext
{
    public AllContext()
    {

    }

    public AllContext(DbContextOptions<AllContext> options) : base(options)
    {

    }

    public DbSet<HealthcareCompany> HealthcareCompanies { get; set; }
    public DbSet<BsnPseudoniem> Pseudoniemen { get; set; }
    public DbSet<Dossier> Dossiers { get; set; }
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships for Dossier entity
        modelBuilder.Entity<Dossier>()
            .HasOne(d => d.healthcareCompany)
            .WithMany()
            .HasForeignKey(d => d.healthcareCompanyId)
            .HasPrincipalKey(h => h.CompanyId);

    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnlineToestemming.Data;

public class AllContextFactory : IDesignTimeDbContextFactory<AllContext>
{
    public AllContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AllContext>();
        optionsBuilder.UseSqlServer("Data Source=127.0.0.1,1234;User ID=mitz;TrustServerCertificate=True;");
        return new AllContext(optionsBuilder.Options);
    }
}

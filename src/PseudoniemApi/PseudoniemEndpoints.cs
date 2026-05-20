using Microsoft.EntityFrameworkCore;
using OnlineToestemming.Data;

namespace OnlineToestemming.PseudoniemApi;

public static class PseudoniemEndpoints
{
    public static WebApplication MapPseudoniemEndpoints(this WebApplication app)
    {
        app.MapGet("/pseudoniem/{bsn}", async (string bsn, AllContext db) =>
        {
            var entry = await db.Pseudoniemen.FirstOrDefaultAsync(p => p.Bsn == bsn);

            if (entry is null)
            {
                // Create a new pseudoniem for this BSN
                var newPseudoniem = new BsnPseudoniem
                {
                    Id = 0,
                    Bsn = bsn,
                    Pseudoniem = Guid.NewGuid().ToString()
                };

                db.Pseudoniemen.Add(newPseudoniem);
                await db.SaveChangesAsync();

                return Results.Ok(new { newPseudoniem.Pseudoniem });
            }

            return Results.Ok(new { entry.Pseudoniem });
        })
        .RequireAuthorization("Internal")
        .WithName("GetPseudoniemByBsn");

        return app;
    }
}

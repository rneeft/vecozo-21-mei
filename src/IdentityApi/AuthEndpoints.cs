using System.IdentityModel.Tokens.Jwt;

namespace OnlineToestemming.IdentityApi;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("auth/login", async (UserLoginRequest request, IAuthenticationService authenticationService) =>
        {
            var jwt = await authenticationService.ChallengeUserAsync(request);
            return jwt is null
                ? Results.Unauthorized()
                : Results.Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        });

        app.MapPost("auth/token", async (CompanyTokenRequest request, IAuthenticationService authenticationService) =>
        {
            var jwt = await authenticationService.ChallengeCompanyAsync(request);
            return jwt is null
                ? Results.Unauthorized()
                : Results.Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        });

        app.MapPost("auth/token/internal", (IAuthenticationService authenticationService) =>
        {
            var jwt = authenticationService.CreateInternalToken();
            return Results.Text(new JwtSecurityTokenHandler().WriteToken(jwt));
        });

        return app;
    }
}

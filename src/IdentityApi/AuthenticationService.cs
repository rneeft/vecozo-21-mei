using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineToestemming.IdentityApi;

public interface IAuthenticationService
{
    Task<JwtSecurityToken?> ChallengeCompanyAsync(CompanyTokenRequest request);

    Task<JwtSecurityToken?> ChallengeUserAsync(UserLoginRequest request);

    JwtSecurityToken CreateInternalToken();
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IdentityContext _identityContext;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public AuthenticationService(IdentityContext identityContext, IOptions<JwtSettings> jwtSettings)
    {
        _identityContext = identityContext;
        _jwtSettings = jwtSettings;
    }

    public async Task<JwtSecurityToken?> ChallengeCompanyAsync(CompanyTokenRequest request)
    {
        var company = await _identityContext.HealthcareCompanies
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId);

        return company is null ? null
            : CreateJwtWithClaims(
                new Claim(ClaimTypes.Role, "HealthcareCompany"),
                new Claim(ClaimTypes.NameIdentifier, company.CompanyName)
            );
    }

    public async Task<JwtSecurityToken?> ChallengeUserAsync(UserLoginRequest request)
    {
        var pseudoniem = await _identityContext.Pseudoniemen
            .FirstOrDefaultAsync(x => x.Bsn == request.Bsn);

        if (pseudoniem is null)
            return null;

        var patient = await _identityContext.Patients
            .FirstOrDefaultAsync(x => x.Pseudoniem == pseudoniem.Pseudoniem && x.LastName == request.LastName);

        return patient is null ? null
            : CreateJwtWithClaims(
                new Claim(ClaimTypes.Role, "Patient"),
                new Claim(ClaimTypes.NameIdentifier, pseudoniem.Pseudoniem)
            );
    }

    public JwtSecurityToken CreateInternalToken()
    {
        return CreateJwtWithClaims(
            new Claim(ClaimTypes.Role, "Internal"),
            new Claim(ClaimTypes.NameIdentifier, "internal-system")
        );
    }

    private JwtSecurityToken CreateJwtWithClaims(params Claim[] claims)
    {
        return new JwtSecurityToken(
               issuer: "Online-Toestemming-Workshop-IdentityApi",
               audience: "Online-Toestemming-Workshop",
               claims: claims,
               notBefore: DateTime.UtcNow,
               expires: DateTime.UtcNow.AddMinutes(15),
               signingCredentials: new SigningCredentials(
                   key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretSigningKey)),
                   algorithm: SecurityAlgorithms.HmacSha256
           ));
    }
}
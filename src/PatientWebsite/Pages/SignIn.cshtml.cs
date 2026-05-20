using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PatientWebsite.Pages;

public class SignInModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SignInModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    [Required]
    [Display(Name = "BSN")]
    public string Bsn { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var client = _httpClientFactory.CreateClient("IdentityApi");

        var content = new StringContent(
            JsonSerializer.Serialize(new { bsn = Bsn, lastName = LastName }),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            var token = (await response.Content.ReadAsStringAsync()).Trim('"');
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var claims = jwt.Claims.ToList();
            claims.Add(new Claim("jwt", token));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToPage("/Index");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Authentication failed. Please check your BSN.");
            return Page();
        }
    }
}
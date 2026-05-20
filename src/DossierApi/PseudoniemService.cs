using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OnlineToestemming.DossierApi;

public interface IPseudoniemService
{
    Task<string?> GetPseudoniemForBsnAsync(string bsn);
}

public class PseudoniemService : IPseudoniemService
{
    private readonly HttpClient _identityHttpClient;
    private readonly HttpClient _pseudoniemHttpClient;

    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public PseudoniemService(
        IHttpClientFactory httpClientFactory)
    {
        _identityHttpClient = httpClientFactory.CreateClient("IdentityApi");
        _pseudoniemHttpClient = httpClientFactory.CreateClient("PseudoniemApi");
    }

    public async Task<string?> GetPseudoniemForBsnAsync(string bsn)
    {
        var token = await GetInternalTokenAsync();
        if (token is null)
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/pseudoniem/{bsn}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _pseudoniemHttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("pseudoniem").GetString();
    }

    private async Task<string?> GetInternalTokenAsync()
    {
        if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "/auth/token/internal");
        request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        var response = await _identityHttpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return null;

        _cachedToken = await response.Content.ReadAsStringAsync();
        _tokenExpiry = DateTime.UtcNow.AddMinutes(10);

        return _cachedToken;
    }
}

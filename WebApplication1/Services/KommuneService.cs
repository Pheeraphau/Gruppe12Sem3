namespace WebApplication1.Services;

using System.Text.Json;
using WebApplication1.API_Models;

public class KommuneService
{
    private readonly HttpClient _httpClient;

    public KommuneService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<KommuneInfo?> GetKommuneInfoAsync(string kommunenummer)
    {
        try
        {
            // Hent informasjon om en kommune basert på kommunenummer
            var response = await _httpClient.GetAsync($"/kommuneinfo/v1/kommuner/{kommunenummer}");
            response.EnsureSuccessStatusCode(); // Kaster en unntak hvis responsen ikke er en suksess (200-299)

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<KommuneInfo>(jsonResponse);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Returner null hvis kommunen ikke finnes (404-feil)
            return null;
        }
        catch (Exception)
        {
            // Loggfør eller håndter andre unntak
            throw;
        }
    }
}
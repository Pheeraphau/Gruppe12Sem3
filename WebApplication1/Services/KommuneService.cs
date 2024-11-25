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
            // Update the URL to use kommunenummer instead of kommunenavn
            var response = await _httpClient.GetAsync($"/kommuneinfo/v1/kommuner/{kommunenummer}");
            response.EnsureSuccessStatusCode(); // Throws an exception if the response status is not success (200-299)

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<KommuneInfo>(jsonResponse);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Return null for a 404 error (kommune not found)
            return null;
        }
        catch (Exception)
        {
            // Log or handle other exceptions
            throw;
        }
    }
}

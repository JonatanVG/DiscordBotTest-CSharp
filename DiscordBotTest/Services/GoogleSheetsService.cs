using System.Text.Json;

namespace DiscordBotTest.Services
{
  public class GoogleSheetsService
  {
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public GoogleSheetsService(IHttpClientFactory factory)
    {
      _http = factory.CreateClient("googleSheets");
      _apiKey = Environment.GetEnvironmentVariable("GOOGLE_API")
        ?? throw new InvalidOperationException("GOOGLE_API environment variable is not set.");
    }

    public async Task<DBSheetData?> GetSheetDataAsync(Sheet sheet)
    {
      var url = $"https://sheets.googleapis.com/v4/spreadsheets/{sheet.SheetId}/values/{sheet.Name}!{sheet.Range}?key={_apiKey}";
      try
      {
        using var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<DBSheetData>(content);

        return data;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"BGCError fetching sheet data: {ex.Message}");
        throw;
      }
    }
  }
}

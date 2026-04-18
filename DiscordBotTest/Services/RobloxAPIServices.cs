using System.Text.Json;

namespace DiscordBotTest.Services
{
  public class RobloxAPIServices
  {
    private readonly HttpClient _http = new();

    public RobloxAPIServices()
    {
      _http.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("RBLX_API"));
    }

    public async Task<Dictionary<long, BasicRobloxUser>?> GetUserBasicAsync(string userName)
    {
      if (userName.Length == 0) return null;
      var url = "https://users.roblox.com/v1/usernames/users";
      List<string> userNames = [];
      userNames.Add(userName);
      Dictionary<string, object> payload = [];
      payload.Add("usernames", userNames);
      payload.Add("excludeBannedUsers", false);
      var response = await _http.PostAsJsonAsync(url, payload);

      if (!response.IsSuccessStatusCode)
      {
        Console.WriteLine("GetUserBasicAsync: Response: Failed to fetch users.");
        return null;
      }

      var json = await response.Content.ReadAsStringAsync();
      var data = JsonDocument.Parse(json).RootElement;

      var result = new Dictionary<long, BasicRobloxUser>();

      var users = data.GetProperty("data").EnumerateArray();

      foreach (var user in users)
      {
        var deserializedUser = JsonSerializer.Deserialize<BasicRobloxUser>(user);
        result.Add(deserializedUser!.Id, deserializedUser);
      }

      return result;
    }

    public async Task<List<InventoryItem>?> GetUserBadgesAsync(long userId)
    {
      var url = $"https://apis.roblox.com/cloud/v2/users/{userId}/inventory-items?filter=badges=true&maxPageSize=100";
      List<InventoryItem> badges = [];
      try
      {
        string? pageToken = null;
        do
        {
          var pagedUrl = pageToken != null ? url + $"&pageToken={pageToken}" : url;

          var response = await _http.GetAsync(pagedUrl);
          response.EnsureSuccessStatusCode();

          var json = await response.Content.ReadAsStringAsync();

          var inventoryResponse = JsonSerializer.Deserialize<InventoryResponse>(json);

          if (inventoryResponse?.Data != null)
            foreach (var item in inventoryResponse.Data)
            {
              Console.WriteLine($"Inventory Item: {item}");
              badges.Add(item);
            }

          pageToken = inventoryResponse?.PageToken;

        } while (!string.IsNullOrEmpty(pageToken));
        return badges;
      } 
      catch (HttpRequestException e)
      {
        Console.WriteLine($"\nRequest failed: {e.Message}");
        return null;
      }
    }

    public async Task<RobloxUser?> GetUserInfoAsync(long userId)
    {
      var url = $"https://apis.roblox.com/cloud/v2/users/{userId}";
      try
      {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<RobloxUser>(json);
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine($"GetRobloxUserInfoAsync: Failed: {e.Message}");
        return null;
      }
    }
  }
}

using System.Text.Json;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.Services
{
  public class RobloxAPIServices
  {
    private readonly HttpClient _http = new();
    private readonly HttpClient _legacyHttp = new();

    public RobloxAPIServices()
    {
      _http.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("RBLX_API"));
    }

    private static bool IsValidRobloxUsername(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) return false;
      if (name.Length < 3 || name.Length > 20) return false;
      return name.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    public async Task<Dictionary<string, BasicRobloxUser>?> GetUserBasicAsync(List<string> userNames)
    {
      if (userNames.Count == 0) return null;
      int attempts = 0;
      var url = "https://users.roblox.com/v1/usernames/users";
      var result = new Dictionary<string, BasicRobloxUser>();
      var validNames = userNames.Where(IsValidRobloxUsername).ToList();
      var chunks = SplitIntoChunks(validNames, 200);
      Console.Write($"Chunks: {chunks.Count}\n");
      try
      {
        foreach (var chunk in chunks)
        {
          var payload = new
          {
            usernames = chunk,
            excludeBannedUsers = false
          };
          var response = await _legacyHttp.PostAsJsonAsync(url, payload);
          if (!response.IsSuccessStatusCode)
          {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GetUserBasicAsync: Response: Failed to fetch users: {response.StatusCode}\nException: {error}");
            return null;
          }

          var json = await response.Content.ReadAsStringAsync();
          var data = JsonDocument.Parse(json).RootElement;

          var users = data.GetProperty("data").EnumerateArray();

          foreach (var user in users)
          {
            var deserializedUser = JsonSerializer.Deserialize<BasicRobloxUser>(user);
            if (deserializedUser != null && !result.ContainsKey(deserializedUser.Name))
              result.Add(deserializedUser.Name, deserializedUser);
          }
          attempts += 1;
          Console.WriteLine($"Success ({attempts})");
          await Task.Delay(70000);
        }
        for (var i = 0; i < result.Values.Count; i++)
          Console.WriteLine($"({i}) - {result.Values.ElementAt(i)}");

        List<BasicRobloxUser> basic = [.. result.Values];
        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Caught exception as ex: {ex.Message}");
        return result;
      }
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

    public async Task<UserGroup[]?> GetUserGroupsAsync(long userId)
    {
      var url = $"https://groups.roblox.com/v1/users/{userId}/groups/roles?includeLocked=true";
      try
      {
        var response = await _legacyHttp.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UserGroup[]>(json);
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine($"GetUserGroupsAsync: Failed: {e.Message}");
        return null;
      }
    }

    public async Task<UserFriend[]?> GetUserFriendsAsync(long userId)
    {
      var url = $"https://friends.roblox.com/v1/users/{userId}/friends";
      try
      {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<UserFriend[]>(json);
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine($"GetUserFriendsAsync: Failed: {e.Message}");
        return null;
      }
    }
  }
}

using System.Text.Json;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.Services
{
  public class RobloxAPIServices
  {
    private readonly HttpClient _http;
    private readonly HttpClient _legacyHttp;

    public RobloxAPIServices(IHttpClientFactory factory)
    {
      _http = factory.CreateClient("roblox");
      _legacyHttp = factory.CreateClient("robloxLegacy");
    }

    private static bool IsValidRobloxUsername(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) return false;
      if (name.Length < 3 || name.Length > 20) return false;
      return name.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    public async Task<List<BasicRobloxUser>?> GetUserBasicByIdsAsync(long[] IDs)
    {
      if (IDs.Length == 0) return null;
      int attempts = 0;
      var url = "https://users.roblox.com/v1/users";
      var result = new List<BasicRobloxUser>();
      var chunks = SplitIntoChunks(IDs.ToList(), 200);
      Console.Write($"Chunks: {chunks.Count}\n");
      try
      {
        foreach (var chunk in chunks)
        {
          var payload = new
          {
            userIds = chunk,
            excludeBannedUsers = false
          };
          using var response = await _legacyHttp.PostAsJsonAsync(url, payload);
          response.EnsureSuccessStatusCode();

          var json = await response.Content.ReadAsStringAsync();
          var data = JsonSerializer.Deserialize<BasicUserResponse>(json);

          if (data?.Data is not null) 
            result.AddRange(data.Data);

          attempts++;
          //Console.WriteLine($"GetUserBasicByIdsAsync: Success ({attempts})");
          if (chunks.Count > 1)
            await Task.Delay(70000);
        }

        Console.WriteLine($"GetUserBasicByIdsAsync: Completed: Total users fetched: {result.Count}");

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"GetUserBasicByIdsAsync: Caught exception: {ex.Message}");
        return result;
      }
    }

    public async Task<Dictionary<string, BasicRobloxUser>?> GetUserBasicByUsernamesAsync(List<string> userNames)
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
          using var response = await _legacyHttp.PostAsJsonAsync(url, payload);
          response.EnsureSuccessStatusCode();

          var json = await response.Content.ReadAsStringAsync();
          var data = JsonSerializer.Deserialize<BasicUserResponse>(json);

          if (data?.Data is not null)
            foreach (var user in data.Data)
              result[user.Name] = user;

          attempts++;
          //Console.WriteLine($"GetUserBasicAsync: Success ({attempts})");
          if (chunks.Count > 1)
            await Task.Delay(70000);
        }
        //for (var i = 0; i < result.Values.Count; i++)
        //  Console.WriteLine($"GetUserBasicAsync: User: ({i}) - {result.Values.ElementAt(i)}");

        Console.WriteLine($"GetUserBasicAsync: Completed: Total users fetched: {result.Count}");

        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"GetUserBasicAsync: Caught exception: {ex.Message}");
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

          using var response = await _http.GetAsync(pagedUrl);
          response.EnsureSuccessStatusCode();

          var json = await response.Content.ReadAsStringAsync();

          var inventoryResponse = JsonSerializer.Deserialize<InventoryResponse>(json);

          if (inventoryResponse?.Data != null)
            badges.AddRange(inventoryResponse.Data);

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
        using var response = await _http.GetAsync(url);
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
        using var response = await _legacyHttp.GetAsync(url);
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
        using var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        //Console.WriteLine($"GetUserFriendsAsync: Response JSON: {json}");

        var friendsResponse = JsonSerializer.Deserialize<FriendsResponse>(json);

        return friendsResponse?.Friends;
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine($"GetUserFriendsAsync: Failed: {e.Message}");
        return null;
      }
    }

    public async Task<List<GroupMember>?> GetGroupMembersAsync(long groupId)
    {
      var url = $"https://apis.roblox.com/cloud/v2/groups/{groupId}/memberships?maxPageSize=100";
      List<GroupMember> members = [];
      try
      {
        string? PageToken = null;
        do
        {
          var pagedUrl = PageToken != null ? url + $"&pageToken={PageToken}" : url;

          using var response = await _http.GetAsync(pagedUrl);
          response.EnsureSuccessStatusCode();

          var json = await response.Content.ReadAsStringAsync();

          var membersResponse = JsonSerializer.Deserialize<GroupMembersResponse>(json);

          if (membersResponse?.Members != null)
            members.AddRange(membersResponse.Members);

          PageToken = membersResponse?.PageToken;
        } while (!string.IsNullOrEmpty(PageToken));
        return members;
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine($"GetGroupMembersAsync: Failed: {e.Message}");
        return null;
      }
    }
  }
}

using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DiscordBotTest.Services
{
  public class TrelloService
  {
    private readonly HttpClient _http = new();
    private readonly RobloxAPIServices _robloxApi;
    private readonly DbService _db;

    public TrelloService(RobloxAPIServices robloxApi, DbService db)
    {
      _robloxApi = robloxApi;
      _db = db;
    }

    public async Task<Blacklist?> GetTrelloBlacklist()
    {
      var url = "https://trello.com/b/ov7HU6Pv.json";
      var blacklistedNames = new Blacklist([]);
      var cachedBlacklist = await _db.CallFunctionWithResponse<Blacklist>("get_group_cache", []);
      if (cachedBlacklist != null && cachedBlacklist.Data != null)
      {
        return cachedBlacklist.Data;
      }
      try 
      {
        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
          Console.WriteLine($"GetTrelloBlacklist: Failed GET: Status: {response.StatusCode}");
          return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json).RootElement;

        var cards = data.GetProperty("cards").EnumerateArray().ToList();
        var relevantCards = cards
          .Skip(14)
          .Take(cards.Count - 14 - 7)
          .ToList();
        var lists = data.GetProperty("lists").EnumerateArray();

        List<string> names = [];

        var activeCards = cards
          .Where(c => !c.GetProperty("closed").GetBoolean())
          .Select(c => c.GetProperty("id").GetString())
          .ToHashSet();

        foreach (var card in relevantCards)
        {
          var cardId = card.GetProperty("id").GetString();
          var cardName = card.GetProperty("name").GetString()!.Trim();
          var listId = card.GetProperty("idList").GetString();
          var isArchived = !activeCards.Contains(cardId);

          foreach (var list in data.GetProperty("lists").EnumerateArray())
          {
            if (list.GetProperty("id").GetString() != listId) continue;

            var listName = list.GetProperty("name").GetString()!;
            var statusSuffix = isArchived ? " (Archived/Inactive Punishment" : "";
            var normalized = Regex.Replace(cardName, @"[/\\""()]", " ");
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();
            var name = normalized.Split(' ');

            names.Add(name[0]);
            blacklistedNames.List[name[0]] = new BlacklistEntry(string.Join(", ", name), new BlacklistStatus(listName, statusSuffix));
            break;
          }
        }
        var userIds = await _robloxApi.GetUserBasicAsync(names);
        if (userIds is not null)
          foreach (var user in userIds.Values)
            blacklistedNames.List[user.UserName].Id = user.Id;

        var serializedBlacklist = JsonSerializer.Serialize(blacklistedNames);
        await _db.CallFunctionWithResponse<Blacklist>("register_group_cache", [serializedBlacklist]);
        return blacklistedNames;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception: {ex.Message}");
        return null;
      }
    }
  }

  public class TrelloBlacklistCache(TrelloService t, IMemoryCache cache) : BackgroundService
  {
    private const string CacheKey = "TrelloBlacklist";
    private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        await RefreshCache();
        await Task.Delay(RefreshInterval, stoppingToken);
      }
    }

    private async Task RefreshCache()
    {
      var blacklist = await t.GetTrelloBlacklist();
      cache.Set(CacheKey, blacklist, RefreshInterval + TimeSpan.FromMinutes(30));
    }

    public Blacklist? GetBlacklist()
      => cache.TryGetValue(CacheKey, out Blacklist? data) ? data : null;
  }
}

using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace DiscordBotTest.Services
{
  public class TrelloService
  {
    private readonly HttpClient _http = new();

    public async Task<Blacklist?> GetTrelloBlacklist()
    {
      var url = "https://trello.com/b/ov7HU6Pv.json";
      var response = await _http.GetAsync(url);

      if (!response.IsSuccessStatusCode) return null;

      var json = await response.Content.ReadAsStringAsync();
      var data = JsonDocument.Parse(json).RootElement;

      var blacklistedNames = new Blacklist([]);

      var cards = data.GetProperty("cards").EnumerateArray();
      var lists = data.GetProperty("lists").EnumerateArray();

      var activeCards = cards
        .Where(c => !c.GetProperty("closed").GetBoolean())
        .Select(c => c.GetProperty("id").GetString())
        .ToHashSet();

      foreach (var card in data.GetProperty("cards").EnumerateArray())
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

          blacklistedNames.List[cardName.ToLower()] = new BlacklistEntry(cardName, new BlacklistStatus(listName, statusSuffix));
          break;
        }
      }

      return blacklistedNames;
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

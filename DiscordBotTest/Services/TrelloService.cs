using System.Text.Json;

namespace DiscordBotTest.Services
{
  public class TrelloService
  {
    private readonly HttpClient _http = new();

    public async Task<Dictionary<string, (string Name, string Status)>> GetTrelloBlacklist()
    {
      var url = "https://trello.com/b/ov7HU6Pv.json";
      var response = await _http.GetAsync(url);

      if (!response.IsSuccessStatusCode) return [];

      var json = await response.Content.ReadAsStringAsync();
      var data = JsonDocument.Parse(json).RootElement;

      var blacklistedNames = new Dictionary<string, (string Name, string Status)>();

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

          blacklistedNames[cardName.ToLower()] = (cardName, listName + statusSuffix);
          break;
        }
      }

      return blacklistedNames;
    }
  }
}

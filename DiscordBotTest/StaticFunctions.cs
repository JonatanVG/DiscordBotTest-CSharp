using DiscordBotTest.Services;
using DSharpPlus.Entities;
using ScottPlot;

namespace DiscordBotTest
{
  public static class StaticFunctions
  {
    public static long[] ParseUserPathToID(string[] paths)
    {
      return [.. paths.Select(p =>
      {
        var parts = p.Split('/');
        return parts.Length > 0 && long.TryParse(parts.Last(), out var id) ? id : 0;
      })];
    }

    static List<DiscordEmbed> ComparerError(string title)
    {
      var embeds = new List<DiscordEmbed>();
      embeds.Add(new DiscordEmbedBuilder().WithTitle(title).WithColor(DiscordColor.Red).Build());
      return embeds;
    }

    public static async Task<List<DiscordEmbed>?> CompareDBToGroup(long groupId, BotService s)
    {
      var allMembers = new List<string>();
      var response = new List<DiscordEmbed>();
      var GroupMembers = await s.GetRobloxGroupMembersAsync(groupId);
      if (GroupMembers is null)
        return ComparerError($"Could not get members for group {groupId}");

      string[] MemberPaths = [.. GroupMembers.Select(m => m.User)];

      long[] MemberIds = ParseUserPathToID(MemberPaths);
      if (MemberIds.Length == 0)
        return ComparerError($"No valid member IDs found for group {groupId}");

      var Members = await s.GetBasicRobloxUsersByIdsAsync(MemberIds);
      if (Members is null)
        return ComparerError($"Could not get user info for members of group {groupId}");

      var sheets = await s.GetGroupSheetsAsync((int)groupId);
      if (sheets is null)
        return ComparerError($"Could not get sheets for group {groupId}");
      if (sheets.Data is null || sheets.Data.Length == 0)
        return ComparerError($"No sheets found for group {groupId}");

      foreach (var sheet in sheets.Data)
      {
        var sheetData = await s._google.GetSheetDataAsync(sheet);
        if (sheetData is null)
          return ComparerError($"Could not get data for sheet {sheet.Name} ({sheet.Id})");

        var sheetNames = new HashSet<string>();
        var sheetDuplicates = new List<(string Name, int Row, string Role, string SheetName)>();
        var embed = new DiscordEmbedBuilder();

        var enumerated = sheetData.Values
          .Select((row, index) => (Row: index + sheet.Row, Name: row.ElementAtOrDefault(0), Role: row.ElementAtOrDefault(1), SheetName: sheet.Name))
          .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Role));
        
        foreach (var (Row, Name, Role, SheetName) in enumerated)
        {
          if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Role))
            continue;
          var normalized = Name.Trim().ToLowerInvariant();
          if (!sheetNames.Add(normalized))
            sheetDuplicates.Add((Name, Row, Role, SheetName));
          else
            allMembers.Add(normalized);
        }

        var DuplicateDisplay = sheetDuplicates
          .Take(50)
          .Select(d => $"{d.Name} (Row: {d.Row}, Role: {d.Role}, Sheet: {d.SheetName})");
        if (sheetDuplicates.Count > 50)
          DuplicateDisplay = DuplicateDisplay.Append($"... and {sheetDuplicates.Count - 50} more");

        var desc = $"Total Group Members: {Members.Count}\nTotal Sheet Entries: {sheetNames.Count}\n\nDuplicate DB Entries ({sheetDuplicates.Count}):\n{string.Join("\n", sheetDuplicates.Select(d => $"- {d.Name} (Row: {d.Row}, Role: {d.Role}, Sheet: {d.SheetName})"))}";
        
        response.Add(embed
          .WithTitle($"Check {sheet.Name}")
          .WithDescription(desc)
          .WithColor(DiscordColor.Orange)
          .Build());
      }
      var MissingMembers = Members
        .Where(m => !allMembers.Contains(m.Name.Trim().ToLower()))
        .ToList();
      var MissingDisplay = MissingMembers
        .Take(50)
        .Select(m => $"{m.DisplayName} (@{m.Name}, ID: {m.Id})")
        .ToList();
      response.Add(new DiscordEmbedBuilder()
        .WithTitle($"Missing Members in Sheets")
        .WithDescription($"Total Missing: {MissingMembers.Count}\n\n{string.Join("\n", MissingDisplay)}{(MissingMembers.Count > 50 ? $"\n... and {MissingMembers.Count - 50} more" : "")}")
        .WithColor(DiscordColor.Red)
        .Build());

      return response;
    }
    
    public static List<List<T>> SplitIntoChunks<T>(List<T> source, int chunkSize = 100)
    {
      var chunks = new List<List<T>>();

      for (var i = 0; i < source.Count; i += chunkSize)
      {
        var length = Math.Min(chunkSize, source.Count - i);
        chunks.Add(source.GetRange(i, length));
      }

      return chunks;
    }
    
    static BGCResult BGCError(string title)
    {
      var result = new BGCResult();
      result.Embeds.Add(new DiscordEmbedBuilder().WithTitle(title).WithColor(DiscordColor.Red).Build());
      return result;
    }

    public static async Task<BGCResult> BGCFunction(List<string> username, BotService s, bool graph, bool mode)
    {
      var response = new BGCResult();
      var embed = new DiscordEmbedBuilder();
      var User = await s.PostGetRobloxUsersAsync(username);
      if (User is null)
        return BGCError($"User {username} does not exist.");
      //Console.WriteLine($"Fetched user {username[0]} with ID {User.Values.First().Id}");

      var user = User.Values.First();
      var desc = $"**Name: {user.DisplayName} (@{user.Name})**\nID: {user.Id}\nVerified: {user.IsVerified}\nRequest Name: {user.UserName}";

      var Badges = await s.GetRobloxUserBadgesAsync(user.Id);
      if (Badges is null)
        return BGCError($"Could not get badges for user {username}");
      //Console.WriteLine($"Fetched badges for user {username[0]} with ID {user.Id}");

      var userInfo = await s.GetRobloxUserInfoAsync(user.Id);
      if (userInfo is null)
        return BGCError($"Could not get userInfo for user {username}");
      //Console.WriteLine($"Fetched userInfo for user {username[0]} with ID {user.Id}");

      var userFriends = await s.GetRobloxUserFriendsAsync(user.Id);
      if (userFriends is null)
        return BGCError($"Could not get friends for user {username}");
      //Console.WriteLine($"Fetched friends for user {username[0]} with ID {user.Id}");
      var friendIds = userFriends.Select(f => f.Id).ToList();

      var blacklist = s.GetTrelloBlacklist();
      if (blacklist is null)
        return BGCError("Could not fetch Trello blacklist");
      //Console.WriteLine($"Fetched Trello blacklist with {blacklist.List.Count} entries");
      var isBlacklisted = blacklist.List.ContainsKey(user.Id.ToString());

      desc += $"\nJoinDate: {userInfo.CreateTime}\nLocale: {userInfo.Locale}\nHasPremium: {userInfo.IsPremium}\nIsIdVerified: {userInfo.IsIdVerified}"; // \nIsBlacklisted: {isBlacklisted}

      var blacklistedFriends = blacklist.List.Values.Where(x => friendIds.Contains(x.Id)).ToList();

      if (blacklistedFriends.Count > 0)
      {
        desc += $"\n\n**Blacklisted Friends ({blacklistedFriends.Count}):**\n";
        desc += string.Join("\n", blacklistedFriends.Select(x => $"- {x.Name}({x.Id}) → {x.Status.Status} {x.Status.Suffix}"));
      }

      var awardDates = graph ? GetAwardDates(Badges) : null;
      //if (graph) Console.WriteLine($"Extracted award dates for user {username[0]} with ID {user.Id}: {string.Join(", ", awardDates!)}");
      var imageBytes = graph ? PlotCumulativeBadges(user.Name, user.Id, awardDates!, mode) : null;
      //if (graph) Console.WriteLine($"Generated badge graph for user {username[0]} with ID {user.Id}, image size: {imageBytes!.Length} bytes");
      var stream = graph ? new MemoryStream(imageBytes!) : null;
      //if (graph) Console.WriteLine($"Created memory stream for badge graph for user {username[0]} with ID {user.Id}, stream length: {stream!.Length} bytes");

      if (graph) embed.WithImageUrl("attachment://badges.png");

      response.Embeds.Add(embed
        .WithTitle($"Check: {username[0]}")
        .WithDescription(desc + $"\nBadgeCount: {Badges.Count}")
        .WithColor(DiscordColor.Blurple)
        .Build());
      if (graph) 
      {
        response.Files.Add(("badges.png", stream));
        //Console.WriteLine($"Added badge graph to response for user {username[0]} with ID {user.Id}");
      }

      //Console.WriteLine($"BGCFunction: Completed for user {username[0]} with ID {user.Id}");
      return response;
    }

    public static byte[] PlotCumulativeBadges(string username, long userId, DateTimeOffset[] dates, bool mode)
    {
      var sorted = dates.Order().ToList();

      double[] xValues = [.. sorted.Select(d => d.UtcDateTime.ToOADate())];
      double[] yValues = [.. Enumerable.Range(1, sorted.Count).Select(i => (double)i)];

      using var plt = new Plot();
      plt.Title($"Badges over time for {username} ({userId})");
      plt.XLabel("Badge Earned Date");
      plt.YLabel("Total Badges");

      var scatter = plt.Add.Scatter(xValues, yValues);
      scatter.MarkerStyle.Shape = MarkerShape.OpenCircle;
      scatter.MarkerStyle.Size = 10;
      scatter.LineWidth = 0;

      plt.Axes.DateTimeTicksBottom();

      plt.Add.Annotation($"Badge Count: {sorted.Count}");

      if (!mode)
      {
        plt.FigureBackground.Color = new("#101021");
        plt.Axes.Color(new("#888888"));
        plt.Grid.XAxisStyle.FillColor1 = new Color("#888888").WithAlpha(10);
        plt.Grid.YAxisStyle.FillColor1 = new Color("#888888").WithAlpha(10);
        plt.Grid.XAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(0.75);
        plt.Grid.YAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(0.75);
        plt.Grid.XAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(0.25);
        plt.Grid.YAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(0.25);
        plt.Grid.XAxisStyle.MajorLineStyle.Width = 1;
        plt.Grid.YAxisStyle.MajorLineStyle.Width = 1;
      }
      else
      {
        plt.FigureBackground.Color = new("#f0f0f0");
        plt.Axes.Color(new("#999999"));
        plt.Grid.XAxisStyle.FillColor1 = new Color("#999999").WithAlpha(10);
        plt.Grid.YAxisStyle.FillColor1 = new Color("#999999").WithAlpha(10);
        plt.Grid.XAxisStyle.MajorLineStyle.Color = Colors.Black.WithAlpha(0.75);
        plt.Grid.YAxisStyle.MajorLineStyle.Color = Colors.Black.WithAlpha(0.75);
        plt.Grid.XAxisStyle.MajorLineStyle.Color = Colors.Black.WithAlpha(0.25);
        plt.Grid.YAxisStyle.MajorLineStyle.Color = Colors.Black.WithAlpha(0.25);
        plt.Grid.XAxisStyle.MajorLineStyle.Width = 1;
        plt.Grid.YAxisStyle.MajorLineStyle.Width = 1;
      }

      return plt.GetImageBytes(800, 600, ImageFormat.Png);
    }

    public static DateTimeOffset[] GetAwardDates(List<InventoryItem> items) => [.. items.Select(x => x.AddTime)];
    
    public static string[] ParseArgs(string input)
    {
      var args = new List<string>();
      var current = new System.Text.StringBuilder();
      bool inQuotes = false;

      foreach (char c in input)
      {
        if (c == '"')
        {
          inQuotes = !inQuotes;
        }
        else if (c == ' ' && !inQuotes)
        {
          if (current.Length > 0)
          {
            args.Add(current.ToString());
            current.Clear();
          }
        }
        else
        {
          current.Append(c);
        }
      }

      if (current.Length > 0)
        args.Add(current.ToString());

      return [.. args];
    }
  }
}

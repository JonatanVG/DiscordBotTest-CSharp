using DiscordBotTest.Services;
using DSharpPlus.Entities;
using ScottPlot;

namespace DiscordBotTest
{
  public static class StaticFunctions
  {
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
    
    static BGCResult Error(string title)
    {
      var result = new BGCResult();
      result.Embeds.Add(new DiscordEmbedBuilder().WithTitle(title).WithColor(DiscordColor.Red).Build());
      return result;
    }

    public async static Task<BGCResult> BGCFunction(List<string> username, BotService s, bool graph, bool mode)
    {
      var response = new BGCResult();
      var embed = new DiscordEmbedBuilder();
      var User = await s.PostGetRobloxUsersAsync(username);
      if (User is null)
        return Error($"User {username} does not exist.");
      
      var user = User.Values.First();
      var desc = $"**Name: {user.DisplayName} (@{user.Name})**\nID: {user.Id}\nVerified: {user.IsVerified}\nRequest Name: {user.UserName}";

      var Badges = await s.GetRobloxUserBadgesAsync(user.Id);
      if (Badges is null)
        return Error($"Could not get badges for user {username}");

      var userInfo = await s.GetRobloxUserInfoAsync(user.Id);
      if (userInfo is null)
        return Error($"Could not get userInfo for user {username}");

      desc += $"\nJoinDate: {userInfo.CreateTime}\nLocale: {userInfo.Locale}\nHasPremium: {userInfo.IsPremium}\nIsIdVerified: {userInfo.IsIdVerified}";

      var awardDates = graph ? GetAwardDates(Badges) : null;
      var imageBytes = graph ? PlotCumulativeBadges(user.Name, user.Id, awardDates!, mode) : null;
      using var stream = graph ? new MemoryStream(imageBytes!) : null;

      if (graph) embed.WithImageUrl("attachment://badges.png");

      response.Embeds.Add(embed
        .WithTitle($"Check: {username}")
        .WithDescription(desc + $"\nBadgeCount: {Badges.Count}")
        .WithColor(DiscordColor.Blurple)
        .Build());
      if (graph) response.Files.Add(("badges.png", stream));

      return response;
    }

    public static byte[] PlotCumulativeBadges(string username, long userId, DateTimeOffset[] dates, bool mode)
    {
      var sorted = dates.Order().ToList();

      double[] xValues = [.. sorted.Select(d => d.UtcDateTime.ToOADate())];
      double[] yValues = [.. Enumerable.Range(1, sorted.Count).Select(i => (double)i)];

      var plt = new Plot();
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

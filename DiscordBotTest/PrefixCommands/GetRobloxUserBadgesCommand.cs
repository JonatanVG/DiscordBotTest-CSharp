using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetRobloxUserBadgesCommand : IPrefixCommand
  {
    public string Name => "GetBadges";
    public string[] Aliases => ["GB"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var user = args[0];
      if (user == null) return;
      var userId = long.Parse(user);
      var badges = await s.GetRobloxUserBadgesAsync(userId);
      var response = string.Join("\n", [.. badges
        .Select(x => $"**Path: {x.Path}**\nBadgeID: {x.BadgeDetails.BadgeId}\nAwardDate: {x.AddTime}")]);
      var embed = new DiscordEmbedBuilder()
        .WithTitle($"Badges for {userId}")
        .WithDescription(response)
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

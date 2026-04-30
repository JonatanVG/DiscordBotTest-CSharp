using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetRobloxUserBadgesCommand : IPrefixCommand
  {
    public string Name => "GetBadges";
    public string[] Aliases => ["GB"];
    public string Usage => "GetBadges Usage\nFields: <userId>\nOptional Fields: N/A\n\nExample Usage:\nGetBadges 1234567890";
    public string Category => "Security Checks";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder();
      if (args.Length < 1)
      {
        embed
          .WithTitle("Invalid Input")
          .WithDescription(Usage)
          .WithColor(DiscordColor.Red)
          .Build();
        await m.RespondAsync(embed);
        return;
      }
      var user = args[0];
      if (user == null) return;
      var userId = long.Parse(user);
      var badges = await s.GetRobloxUserBadgesAsync(userId);
      var response = string.Join("\n", [.. badges
        .Select(x => $"**Path: {x.Path}**\nBadgeID: {x.BadgeDetails.BadgeId}\nAwardDate: {x.AddTime}")]);
      embed
        .WithTitle($"Badges for {userId}")
        .WithDescription(response)
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

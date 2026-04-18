using DiscordBotTest.Services;
using DSharpPlus.Entities;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.PrefixCommands
{
  public class RunBGCCommand : IPrefixCommand
  {
    public string Name => "BGC";
    public string[] Aliases => ["RunBGC", "run_bgc", "DoBGC"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder();
      if (args.Length == 0)
      {
        embed.WithTitle("Incorrect Input")
        .WithDescription("Usage: BGC <username1,username2,...> <Graph?''> <Light/Dark?''")
        .Build();
        await m.RespondAsync(embed);
        return;
      }
      var users = args[0];
      if (users.Contains(',')) users = users.Split(',')[0];
      var graph = args.ElementAtOrDefault(1);
      bool type = graph?.Length > 0;
      var color = args.ElementAtOrDefault(2);
      bool mode = color?.FirstOrDefault() == 'L';
      var Users = await s.PostGetRobloxUsersAsync(users);
      if (Users.Count <= 0)
      {
        embed.WithTitle($"User {users} does not exist.")
        .Build();
        await m.RespondAsync(embed);
        return;
      }
      var user = Users.Values.First();
      var desc = $"**Name: {user.DisplayName} (@{user.Name})**\nID: {user.Id}\nVerified: {user.IsVerified}\nRequest Name: {user.UserName}";
      var Badges = await s.GetRobloxUserBadgesAsync(user.Id);
      var userInfo = await s.GetRobloxUserInfoAsync(user.Id);
      desc += $"\nJoinDate: {userInfo.CreateTime}\nLocale: {userInfo.Locale}\nHasPremium: {userInfo.IsPremium}\nIs IdVerified: {userInfo.IsIdVerified}";
      var AwardDates = type ? GetAwardDates(Badges) : null;
      var imageBytes = type ? PlotCumulativeBadges(user.Name, user.Id, AwardDates, mode) : null;
      using var stream = type ? new MemoryStream(imageBytes) : null;
      embed.WithTitle($"Check: {users}")
        .WithDescription(desc + $"\nBadge Count: {Badges.Count}")
        .WithColor(DiscordColor.Blurple);
      if (type) embed.WithImageUrl("attachment://badges.png");
      var attachment = new DiscordMessageBuilder()
        .WithEmbed(embed.Build());
      if (type) attachment.AddFile("badges.png", stream);
      await m.RespondAsync(attachment);
    }
  }
}

using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class UniversalBlacklistCommand : IPrefixCommand
  {
    public string Name => "BotBlacklist";
    public string[] Aliases => ["BBL", "Blacklist", "BList"];
    public string Usage => "BotBlacklist Usage\nFields: <Add/Remove> <User ID/Mention>\nOptional Fields: N/A\n\nExample Usage:\nBotBlacklist Add 1234567890\nBotBlacklist Remove 1234567890";
    public string Category => "Bot Management";
    public SecurityLevel SecurityLevel => SecurityLevel.Owner;

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder().WithTitle("Bot Blacklist Management");
      if (args.Length < 2)
      {
        await m.RespondAsync(embed
          .WithDescription("Success: False\nReason: Incorrect input\nUsage: BotBlacklist <Add/Remove> <User ID/Mention>")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      var action = string.Equals(args[0].ToLower(), "add");
      DiscordUser? user = null;
      Logging.DebugLog($"BotBlacklist Command: Action={action}, User={args[1]}");
      if (args[1].StartsWith("<@") && args[1].EndsWith('>'))
      {
        var idStr = ulong.Parse(args[1][2..^1].TrimStart('!'));
        user = await s.Client.GetUserAsync(idStr);
      }
      else if (ulong.TryParse(args[1], out var userId))
        user = await s.Client.GetUserAsync(userId);
      if (user == null)
      {
        await m.RespondAsync(embed
          .WithDescription("Success: False\nReason: Invalid user ID or mention")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      if (action)
      {
        var post = await s.PostBlacklsitedUserAsync(user.Username, user.Id.ToString());
        await m.RespondAsync(embed
          .WithDescription($"Success: {post?.Success}\nReason: {post?.Message}")
          .WithColor(post != null && post.Success ? DiscordColor.Green : DiscordColor.Red)
          .Build());
        if (post != null && post.Success)
          s._blacklist[user.Id.ToString()] = user.Username;
        return;
      }
      var remove = await s.RemoveBlacklistedUserAsync(user.Id.ToString());
      await m.RespondAsync(embed
        .WithDescription($"Success: {remove?.Success}\nReason: {remove?.Message}")
        .WithColor(remove != null && remove.Success ? DiscordColor.Green : DiscordColor.Red)
        .Build());
      if (remove != null && remove.Success)
        s._blacklist.Remove(user.Id.ToString());
    }
  }
}

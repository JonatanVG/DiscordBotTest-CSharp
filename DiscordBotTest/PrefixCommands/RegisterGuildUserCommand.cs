using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterGuildUserCommand : IPrefixCommand
  {
    public string Name => "GuildRegisterUser";
    public string[] Aliases => ["GRU", "GRegisterUser", "GuildRUser", "GuildRegisterU", "GRUser", "GuildRU", "GRegisterU", "GuildRUser"];
    public string Usage => "GuildRegisterUser Usage\nFields: <userId or @mention>\nOptional Fields: N/A\n\nExample Usage:\nGuildRegisterUser 1234567890\nGuildRegisterUser @User";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder();
      if (args.Length < 1)
      {
        embed
          .WithTitle("Invalid Usage")
          .WithDescription(Usage)
          .WithColor(DiscordColor.Orange)
          .Build();
        await m.RespondAsync(embed);
        return;
      }
      if (s.IsOwner(m.Author.Id)) return;
      var guild = m.Channel.Guild;
      if (guild == null) return;
      DiscordUser? user = null;
      if (args[0].StartsWith("<@") && args[0].EndsWith('>'))
      {
        var userId = ulong.Parse(args[0][2..^1].TrimStart('!'));
        user = await s.Client.GetUserAsync(userId);
      }
      else if (ulong.TryParse(args[0], out var rawId))
        user = await s.Client.GetUserAsync(rawId);
      if (user == null) return;
      var response = await s.PostUserAsync(user.Username, user.Id.ToString(), guild.Id.ToString());
      embed
        .WithTitle("User Registration")
        .WithDescription($"Success: {response?.Success}\nMessage: {response?.Message}\nRecordID: {response?.Data?.Id}\nCreated at: {response?.Data?.CreatedAt}")
        .WithColor(response?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}
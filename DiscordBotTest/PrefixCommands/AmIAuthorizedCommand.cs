using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class AmIAuthorizedCommand : IPrefixCommand
  {
    public string Name => "CheckAuth";
    public string[] Aliases => ["CA", "AIA"];
    public string Usage => "CheckAuth Usage\nFields: N/A\nOptional Fields: N/A\nExample Usage:\nCheckAuth\nCA\nAIA";
    public string Category => "User Management";
    public SecurityLevel SecurityLevel => SecurityLevel.Public;

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var authorized = await s.IsGuildAuthorized(m.Author, m.Channel.Guild);
      var owner = await s.IsGuildOwner(m.Author, m.Channel.Guild);
      var embed = new DiscordEmbedBuilder()
        .WithDescription($"Guild Authorized?: {authorized}\nGuild Owner?: {owner}\nBot Admin?: {s.IsBotAdmin(m.Author.Id)}\nBot Owner?: {s.IsOwner(m.Author.Id)}\nBot Blacklisted?: {s.IsBotBlacklisted(m.Author.Id)}")
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

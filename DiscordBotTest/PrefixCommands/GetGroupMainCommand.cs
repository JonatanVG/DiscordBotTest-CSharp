using DiscordBotTest.Services;
using DSharpPlus.Entities;
using System.Text.Json;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGroupMainCommand : IPrefixCommand
  {
    public string Name => "GuildMain";
    public string[] Aliases => ["Main", "GuildM", "GM"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var guild = await s.GetDefaultGuildAsync(m.Channel.Guild.Id);
      if (guild == null)
      {
        await m.RespondAsync("This group has no default guild.");
        return;
      }
      var embed = new DiscordEmbedBuilder()
        .WithTitle(guild.GuildName)
        .WithDescription($"GuildID: {guild.GuildId}\nSheetID: {guild.GoogleSheetId}\nLeadership: {string.Join(", ", guild.IgnoreRoles)}\nMainGroup: {guild.MainGroup}")
        .WithColor(DiscordColor.SpringGreen)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

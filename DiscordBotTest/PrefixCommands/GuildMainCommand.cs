using DiscordBotTest.Services;
using DSharpPlus.Entities;
using System.Text.Json;

namespace DiscordBotTest.PrefixCommands
{
  public class GuildMainCommand : IPrefixCommand
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
        .WithDescription($"ID: {guild.GuildId}\nSHEET_ID: {guild.GoogleSheetId}\nIGNORE: {string.Join(", ", guild.IgnoreRoles)}\nMAIN_GROUP: {guild.MainGroup}")
        .WithColor(DiscordColor.SpringGreen)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

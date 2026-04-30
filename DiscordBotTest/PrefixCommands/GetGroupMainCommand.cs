using DiscordBotTest.Services;
using DSharpPlus.Entities;
using System.Text.Json;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGroupMainCommand : IPrefixCommand
  {
    public string Name => "GuildMain";
    public string[] Aliases => ["Main", "GuildM", "GM"];
    public string Usage => "GuildMain Usage\nFields: N/A\nOptional Fields: N/A\n\nExample Usage:\nGuildMain";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var guild = await s.GetDefaultGuildAsync(m.Channel.Guild.Id);
      if (guild == null)
      {
        await m.RespondAsync("This group has no default guild.");
        return;
      }
      var embed = new DiscordEmbedBuilder()
        .WithTitle(guild.Data.GuildName)
        .WithDescription($"GuildID: {guild.Data.GuildId}\nSheetID: {guild.Data.GoogleSheetId}\nLeadership: {string.Join(", ", guild.Data.IgnoreRoles)}\nMainGroup: {guild.Data.MainGroup}")
        .WithColor(DiscordColor.SpringGreen)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

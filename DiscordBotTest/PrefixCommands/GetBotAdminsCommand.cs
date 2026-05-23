using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetBotAdminsCommand : IPrefixCommand
  {
    public string Name => "BotAdmins";
    public string[] Aliases => ["BA"];
    public string Usage => "";
    public string Category => "Bot Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Bot Admins");

      var desc  = "";
      foreach (var dict in s._auth["0"])
      {
        desc += $"{dict.Value}({dict.Key})\n";
      }
      await m.RespondAsync(embed
        .WithDescription(desc)
        .WithColor(DiscordColor.Blurple));
    }
  }
}

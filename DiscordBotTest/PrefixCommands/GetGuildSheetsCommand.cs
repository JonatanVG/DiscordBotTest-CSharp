using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGuildSheetsCommand : IPrefixCommand
  {
    public string Name => "GuildSheets";
    public string[] Aliases => ["Sheets", "GuildS", "GS"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (args.Length < 1)
      {
        await m.RespondAsync("Please provide a guild ID.");
        return;
      }
      var sheets = await s.GetGuildSheetsAsync(int.Parse(args[0]));
      string[] ssheets = [.. sheets
        .Select(x => 
          $"**Name: {x.Name}**\nSheet: {x.SheetId}\nRange: {x.Range}\n")];
      var embed = new DiscordEmbedBuilder()
        .WithTitle($"Sheets for {args[0]}")
        .WithDescription(string.Join("\n", ssheets))
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

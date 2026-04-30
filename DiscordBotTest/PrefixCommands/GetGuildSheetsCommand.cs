using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGuildSheetsCommand : IPrefixCommand
  {
    public string Name => "GuildSheets";
    public string[] Aliases => ["Sheets", "GuildS", "GS"];
    public string Usage => "GuildSheets Usage\nFields: <guildId>\nOptional Fields: N/A\n\nExample Usage:\nGuildSheets 1234567890";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder();
      if (args.Length < 1)
      {
        await m.RespondAsync(embed
          .WithTitle("Invalid Input")
          .WithDescription(Usage)
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      var sheets = await s.GetGuildSheetsAsync(int.Parse(args[0]));
      if (sheets is null)
      {
        await m.RespondAsync(embed
          .WithTitle($"Failed Sheets Fetch")
          .WithDescription("Success: False\nReason: Unexpected cause, not handled.")
          .Build());
        return;
      }
      string[] ssheets = [.. sheets.Data
        .Select(x => $"**Name: {x.Name}**\nSheet: {x.SheetId}\nRange: {x.Range}\n")];  
      await m.RespondAsync(embed
        .WithTitle($"Sheets for {args[0]}")
        .WithDescription(string.Join("\n", ssheets))
        .WithColor(DiscordColor.Blurple)
        .Build());
    }
  }
}

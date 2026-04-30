using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GuildSheetCommand : IPrefixCommand
  {
    public string Name => "Sheet";
    public string[] Aliases => ["GuildSheet", "GS", "S"];
    public string Usage => "Sheet Usage\nFields: <guildId> <sheetName> <sheetId> <sheetRange> <startRow>\nOptional Fields: N/A\n\nExample Usage:\nSheet 1234567890 MySheet ac33dfv343-32bjb435jkbk43 A1:C10 1";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id)) return;
      var response = await s.PostSheetAsync(int.Parse(args[0]), args[1], args[2], args[3], int.Parse(args[4]));
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Sheet Registration")
        .WithDescription($"Success: {response?.Success}\nMessage: {response?.Message}\nRecordID: {response?.Data?.Id}\nCreated at: {response?.Data?.CreatedAt}")
        .WithColor(response?.Success == true ? DiscordColor.SpringGreen : DiscordColor.DarkRed)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

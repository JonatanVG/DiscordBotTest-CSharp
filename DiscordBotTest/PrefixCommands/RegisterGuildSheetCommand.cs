using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterGuildSheetCommand : IPrefixCommand
  {
    public string Name => "AddSheet";
    public string[] Aliases => ["RegisterSheet", "RS", "AS", "AddS", "RegisterS"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id)) return;
      var response = await s.PostSheetAsync(int.Parse(args[0]), args[1], args[2], args[3], int.Parse(args[4]));
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Sheet Registration")
        .WithDescription($"Success: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data.Id}\nCreated at: {response.Data.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.DarkRed)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

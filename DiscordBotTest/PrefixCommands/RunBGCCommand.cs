using DiscordBotTest.Services;
using DSharpPlus.Entities;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.PrefixCommands
{
  public class RunBGCCommand : IPrefixCommand
  {
    public string Name => "BGC";
    public string[] Aliases => ["RunBGC", "run_bgc", "DoBGC"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (args.Length == 0)
      {
        await m.RespondAsync(new DiscordEmbedBuilder()
          .WithTitle("Incorrect Input")
          .WithDescription("Usage: BGC <username1,username2,...> <Graph?''> <Light/Dark?''")
          .Build());
        return;
      }
      var username = args[0];
      if (username.Contains(',')) username = username.Split(',')[0];
      var graph = args.ElementAtOrDefault(1);
      bool type = graph != null && graph.Length > 0;
      var color = args.ElementAtOrDefault(2);
      bool mode = color != null && color.FirstOrDefault() == 'L';
      var result = await BGCFunction(username, s, type, mode);

      var message = new DiscordMessageBuilder()
        .AddEmbeds(result.Embeds);

      foreach (var file in result.Files)
        message.AddFile(file.Name, file.Stream);

      await m.RespondAsync(message);
    }
  }
}

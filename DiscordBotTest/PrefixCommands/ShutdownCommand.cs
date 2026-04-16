using DSharpPlus.Entities;
using DiscordBotTest.Services;

namespace DiscordBotTest.PrefixCommands
{
  public class ShutdownCommand : IPrefixCommand
  {
    public string Name => "Shutdown";
    public string[] Aliases => ["Stop"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id)) return;
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Shutting down...")
        .WithColor(DiscordColor.DarkRed)
        .Build();
      await m.RespondAsync(embed);
      await Task.Delay(500);
      await s.StopAsync(CancellationToken.None);
    }
  }
}
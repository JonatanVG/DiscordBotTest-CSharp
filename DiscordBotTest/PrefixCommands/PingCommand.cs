using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class PingCommand : IPrefixCommand
  {
    public string Name => "Ping";
    public string[] Aliases => ["P"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (m.Author.Id != m.Channel.Guild.OwnerId)
      {
        await m.RespondAsync("Not guild owner");
        return;
      }
      var embed = new DiscordEmbedBuilder()
        .WithTitle($"Ping!")
        .WithDescription($"Latency: {s.Client.Ping}ms")
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

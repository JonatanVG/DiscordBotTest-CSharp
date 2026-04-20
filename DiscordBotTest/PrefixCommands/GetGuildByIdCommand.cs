using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGuildByIdCommand : IPrefixCommand
  {
    public string Name => "GetGuild";
    public string[] Aliases => ["GG"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      await m.RespondAsync($"You're in the guild: {m.Channel.Guild.Name} (@{m.Channel.GuildId})");
    }
  }
}

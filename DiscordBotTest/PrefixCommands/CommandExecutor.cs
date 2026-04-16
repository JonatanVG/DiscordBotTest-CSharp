using DiscordBotTest.Services;
using DSharpPlus.Entities;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.PrefixCommands
{
  public class CommandExecutor
  {
    private readonly CommandRegistry _registry;
    private readonly char _prefix;
    private readonly BotService _botService;

    public CommandExecutor(CommandRegistry registry, char prefix, BotService botService)
    {
      _registry = registry;
      _prefix = prefix;
      _botService = botService;
    }

    public async Task HandleAsync(DiscordMessage m)
    {
      if (m.Author.IsBot) return;
      if (m.Content[0] != _prefix) return;

      var p = ParseArgs(m.Content[1..]);

      var client = _botService.Client;

      if (!p[0].Equals(client.CurrentUser.Username, StringComparison.OrdinalIgnoreCase)) return;

      if (p.Length < 2) return;
      var n = p[1].ToLower();
      var a = p[2..];

      var c = _registry.GetCommand(n);
      if (c is null) return;

      await c.ExecuteAsync(_botService, m, a);
    }
  }
}

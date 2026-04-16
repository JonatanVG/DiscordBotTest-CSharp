namespace DiscordBotTest.PrefixCommands
{
  public class CommandRegistry
  {
    private readonly Dictionary<string, IPrefixCommand> _commands = [];

    public void Register(IPrefixCommand c)
    {
      _commands[c.Name.ToLower()] = c;
      foreach (var alias in c.Aliases)
        _commands[alias.ToLower()] = c;
    }

    public IPrefixCommand? GetCommand(string name) =>
      _commands.TryGetValue(name.ToLower(), out var cmd) ? cmd : null;
  }
}

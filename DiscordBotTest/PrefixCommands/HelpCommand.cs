using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class HelpCommand : IPrefixCommand
  {
    private readonly CommandRegistry _registry;

    public HelpCommand(CommandRegistry registry)
    {
      _registry = registry;
    }

    public string Name => "Help";
    public string[] Aliases => ["H"];
    public string Usage => "Help Usage\nFields: N/A\nOptional Fields: <CommandName>\n\nExample Usage:\nHelp\nHelp Ping";
    public string Category => "Bot Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder
      {
        Title = "Help",
        Description = "Use `Help <CommandName>` to get more information about a specific command.",
        Color = DiscordColor.Blurple
      };
      if (args.Length != 0)
      {
        var command = _registry.GetCommand(args[0]);
        if (command is null)
        {
          await m.RespondAsync("Command not found.");
          return;
        }
        embed.Title = command.Name;
        embed.Description = $"Aliases: {string.Join(", ", command.Aliases)}\n\n{command.Usage}";
        await m.RespondAsync(embed.Build());
        return;
      }
      var commands = _registry.GetAllCommands();
      List<string> GuildCommands = [];
      List<string> UserCommands = [];
      List<string> SecurityCommands = [];
      List<string> BotManagementCommands = [];
      foreach (var command in commands)
      {
        if (command.Category == "Guild Management")
          GuildCommands.Add(command.Name);
        else if (command.Category == "User Management")
          UserCommands.Add(command.Name);
        else if (command.Category == "Security Checks")
          SecurityCommands.Add(command.Name);
        else if (command.Category == "Bot Management")
          BotManagementCommands.Add(command.Name);
      }
      if (GuildCommands.Count != 0)
        embed.AddField("Guild Management", string.Join("\n", GuildCommands));
      if (UserCommands.Count != 0)
        embed.AddField("User Management", string.Join("\n", UserCommands));
      if (SecurityCommands.Count != 0)
        embed.AddField("Security", string.Join("\n", SecurityCommands));
      if (BotManagementCommands.Count != 0)
        embed.AddField("Bot Management", string.Join("\n", BotManagementCommands));
      await m.RespondAsync(embed.Build());
    }
  }
}

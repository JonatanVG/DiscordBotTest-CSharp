using DiscordBotTest.PrefixCommands;
using DiscordBotTest.Services;
using DSharpPlus;
using System.Text.RegularExpressions;

namespace DiscordBotTest
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddSingleton<RobloxAPIServices>();
      //builder.Services.AddSingleton(sp =>
      //{
      //  return new DiscordClient(new DiscordConfiguration
      //  {
      //    Token = Environment.GetEnvironmentVariable("TOKEN"),
      //    TokenType = TokenType.Bot,
      //    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
      //  });
      //});

      builder.Services.AddSingleton<DbService>();

      //builder.Services.AddSingleton(sp =>
      //{
      //  var registry = new CommandRegistry();

      //  registry.Register(new PingCommand());
      //  registry.Register(new ShutdownCommand());
      //  registry.Register(new GetGuildSheetsCommand());
      //  registry.Register(new GetGroupMainCommand());
      //  registry.Register(new RegisterGuildCommand());
      //  registry.Register(new GuildSheetCommand());
      //  registry.Register(new RegisterGroupCommand());
      //  registry.Register(new GuildRoleCommand());
      //  registry.Register(new GetGuildRolesCommand());
      //  registry.Register(new GetGuildUsersCommand());
      //  registry.Register(new RegisterGuildUserCommand());
      //  registry.Register(new UniversalUserCommand());
      //  registry.Register(new RunBGCCommand());
      //  registry.Register(new GetRobloxUserBadgesCommand());

      //  return registry;
      //});

      builder.Services.AddMemoryCache();
      builder.Services.AddSingleton<TrelloService>();
      builder.Services.AddSingleton<TrelloBlacklistCache>();
      builder.Services.AddHostedService(sp => sp.GetRequiredService<TrelloBlacklistCache>());

      //builder.Services.AddSingleton<BotService>();
      //builder.Services.AddHostedService(sp => sp.GetRequiredService<BotService>());
      //builder.Services.AddHostedService<UptimePingerService>();

      var app = builder.Build();

      app.MapGet("/trello", static (TrelloBlacklistCache t) =>
      {
        var b = t.GetBlacklist();
        if (b is null) return "No blacklisted items found or unable to fetch data";

        var result = "Trello Blacklist:\n\n";
        int count = 0;
        foreach (var entry in b.List.Values)
        {
          var Status = entry.Status;
          count++;
          if (count <= 13) continue;
          if (Status.Suffix.Contains('('))
          {
            result += $"({count}) [ARCHIVED] - {Status.Status} {Status.Suffix}\n";
            continue;
          }
          result += $"({count}) [@{entry.Id}] {entry.Name} - {Status.Status}\n";
        }
        return result;
      });

      app.MapGet("/archived", static (TrelloBlacklistCache t) =>
      {
        var b = t.GetBlacklist();
        if (b is null) return "No blacklisted items found or unable to fetch data";

        var result = "Archived Punishments:\n\n";
        int count = 0;
        foreach (var entry in b.List.Values)
        {
          var Status = entry.Status;
          count++;
          if (count <= 13) continue;
          if (!Status.Suffix.Contains('(')) continue;
          result += $"({count}) [@{entry.Id}] {entry.Name} - {Status.Status} {Status.Suffix}\n";
        }
        return result;
      });

      app.MapGet("/", () => "Bot is running!");

      await app.RunAsync();
    }
  }
}
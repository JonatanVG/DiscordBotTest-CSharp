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
      builder.Services.AddSingleton<DiscordClient>(sp =>
      {
        return new DiscordClient(new DiscordConfiguration
        {
          Token = Environment.GetEnvironmentVariable("TOKEN"),
          TokenType = TokenType.Bot,
          Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        });
      });
      Console.WriteLine("Builder: Build: DiscordClient singleton created!");

      builder.Services.AddSingleton<DbService>();
      Console.WriteLine("Builder: Build: DbService singleton created!");

      builder.Services.AddSingleton<CommandRegistry>(sp =>
      {
        var registry = new CommandRegistry();

        registry.Register(new PingCommand());
        registry.Register(new ShutdownCommand());
        //registry.Register(new GetGuildSheetsCommand());
        //registry.Register(new GetGroupMainCommand());
        //registry.Register(new RegisterGuildCommand());
        //registry.Register(new RegisterGuildSheetCommand());
        //registry.Register(new RegisterGroupCommand());
        //registry.Register(new RegisterGuildRoleCommand());
        //registry.Register(new GetGuildRolesCommand());
        //registry.Register(new GetGuildUsersCommand());
        //registry.Register(new RegisterGuildUserCommand());
        //registry.Register(new RegisterUniversalUserCommand());
        registry.Register(new RunBGCCommand());
        registry.Register(new GetRobloxUserBadgesCommand());

        return registry;
      });

      builder.Services.AddHostedService<BotService>();
      Console.WriteLine("Builder: Build: BotService hosted service created!");
      //builder.Services.AddHostedService<UptimePingerService>();
      //Console.WriteLine("Builder: Build: UptimePingerService hosted service created!");

      //builder.Services.AddMemoryCache();
      //builder.Services.AddSingleton<TrelloService>();
      //builder.Services.AddSingleton<TrelloBlacklistCache>();
      //builder.Services.AddHostedService(sp => sp.GetRequiredService<TrelloBlacklistCache>());

      var app = builder.Build();

      //app.MapGet("/trello", static (TrelloBlacklistCache t) =>
      //{
      //  var b = t.GetBlacklist();
      //  if (b is null || b.Count == 0) return "No blacklisted items found or unable to fetch data";

      //  var result = "Trello Blacklist:\n\n";
      //  int count = 0;
      //  foreach (var (Name, Status) in b.Values)
      //  {
      //    count++;
      //    if (count <= 13) continue;
      //    if (Status.Contains('('))
      //    {
      //      result += $"({count}) [ARCHIVED] - {Status}\n";
      //      continue;
      //    }
      //    var name = Name;
      //    if (name.Contains('/') || name.Contains('('))
      //      name = string.Join(", ", Regex.Split(name, @"\s*[/()]\s*|\s+")
      //        .Where(s => !string.IsNullOrWhiteSpace(s)));
      //    result += $"({count}) {name} - {Status}\n";
      //  }
      //  return result;
      //});

      //app.MapGet("/archived", static (TrelloBlacklistCache t) =>
      //{
      //  var b = t.GetBlacklist();
      //  if (b is null || b.Count == 0) return "No blacklisted items found or unable to fetch data";

      //  var result = "Archived Punishments:\n\n";
      //  int count = 0;
      //  foreach (var (Name, Status) in b.Values)
      //  {
      //    count++;
      //    if (count <= 13) continue;
      //    if (!Status.Contains('(')) continue;
      //    var name = Name;
      //    name = string.Join(", ", Regex.Split(name, @"\s*[/()]\s*|\s+")
      //      .Where(s => !string.IsNullOrWhiteSpace(s)));
      //    result += $"({count}) {name} - {Status}\n";
      //  }
      //  return result;
      //});

      app.MapGet("/", () => "Bot is running!");

      app.Run();
    }
  }
}
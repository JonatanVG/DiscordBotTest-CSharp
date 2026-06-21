using DiscordBotTest.PrefixCommands;
using DiscordBotTest.Services;
using DSharpPlus;

namespace DiscordBotTest
{
  public static class Logging
  {
    private static ILogger? _logger;

    public static void Initialize(ILogger logger)
    {
      _logger = logger;
    }

    public static void DebugLog(string message)
    {
      _logger?.LogDebug("[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Message}",
        DateTime.Now, message);
    }
  }
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddHttpClient("roblox", c =>
        c.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("RBLX_API")));
      builder.Services.AddHttpClient("robloxLegacy");
      builder.Services.AddHttpClient("trello");
      builder.Services.AddHttpClient("uptime");
      builder.Services.AddHttpClient("googleSheets");

      builder.Services.AddSingleton<RobloxAPIServices>();
      builder.Services.AddSingleton(sp =>
      {
        return new DiscordClient(new DiscordConfiguration
        {
          Token = Environment.GetEnvironmentVariable("TOKEN"),
          TokenType = TokenType.Bot,
          Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
        });
      });

      builder.Services.AddSingleton<DbService>();

      builder.Services.AddSingleton(sp =>
      {
        var registry = new CommandRegistry();

        registry.Register(new AmIAuthorizedCommand());
        registry.Register(new DatabaseComparerCommand());
        registry.Register(new GetBotAdminsCommand());
        registry.Register(new GetGroupMainCommand());
        registry.Register(new GetGuildByIdCommand());
        registry.Register(new GetGuildRolesCommand());
        registry.Register(new GetGroupSheetsCommand());
        registry.Register(new GetGuildUsersCommand());
        registry.Register(new GetRobloxUserBadgesCommand());
        registry.Register(new GroupManageCommand());
        registry.Register(new GuildRoleCommand());
        registry.Register(new GuildSheetCommand());
        registry.Register(new HelpCommand(registry));
        registry.Register(new PingCommand());
        registry.Register(new RegisterGroupCommand());
        registry.Register(new RegisterGuildCommand());
        registry.Register(new RunBGCCommand());
        registry.Register(new ShutdownCommand());
        registry.Register(new UniversalBlacklistCommand());
        registry.Register(new UniversalUserCommand());

        return registry;
      });

      builder.Services.AddMemoryCache();
      builder.Services.AddSingleton<TrelloService>();
      builder.Services.AddSingleton<TrelloBlacklistCache>();
      builder.Services.AddHostedService(sp => sp.GetRequiredService<TrelloBlacklistCache>());

      builder.Services.AddSingleton<GoogleSheetsService>();

      builder.Services.AddSingleton<BotService>();
      builder.Services.AddHostedService(sp => sp.GetRequiredService<BotService>());
      builder.Services.AddRazorPages();
      builder.Services.AddHostedService<UptimePingerService>();

      var app = builder.Build();
      Logging.Initialize(app.Logger);

      app.MapRazorPages();

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

      await app.RunAsync();
    }
  }
}
using DiscordBotTest.PrefixCommands;
using DiscordBotTest.Services;
using DSharpPlus;

namespace DiscordBotTest
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

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
        registry.Register(new SheetsCommand());
        registry.Register(new GuildMainCommand());
        registry.Register(new RegisterGuildCommand());
        registry.Register(new RegisterSheetCommand());
        registry.Register(new RegisterGroupCommand());
        registry.Register(new RegisterRoleCommand());
        registry.Register(new GetGuildRolesCommand());
        registry.Register(new GetGuildUsersCommand());
        registry.Register(new RegisterGuildUserCommand());

        return registry;
      });

      builder.Services.AddHostedService<BotService>();
      Console.WriteLine("Builder: Build: BotService hosted service created!");
      builder.Services.AddHostedService<UptimePingerService>();
      Console.WriteLine("Builder: Build: UptimePingerService hosted service created!");

      var app = builder.Build();

      app.MapGet("/", () => "Bot is running!");

      app.Run();
    }
  }
}
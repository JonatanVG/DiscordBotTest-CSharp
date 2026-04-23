using DiscordBotTest.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.SlashCommands
{
  public class SlashCommandsModule : ApplicationCommandModule
  {
    private readonly BotService _bot;

    public SlashCommandsModule(BotService bot)
    {
      _bot = bot;
    }

    [SlashCommand("bgc_run", "Run a background check on a Roblox user")]
    public async Task BGC(InteractionContext ctx, 
      [Option("username", "The Roblox username to check")] string username,
      [Choice("True", 1)]
      [Choice("False", 0)]
      [Option("graph", "Whether to include a badge graph")] long graph = 0,
      [Choice("True", 1)]
      [Choice("False", 0)]
      [Option("light", "Whether to use light mode for the graph")] long light = 0)
    {
      await ctx.DeferAsync();

      List<string> usernames = [username];
      var response = await BGCFunction(usernames, _bot, graph == 1, light == 1);

      await ctx.EditResponseAsync(new DiscordWebhookBuilder()
        .AddEmbeds(response.Embeds)
        .AddFile(response.Files.First().Name, response.Files.First().Stream));
    }
  }
}

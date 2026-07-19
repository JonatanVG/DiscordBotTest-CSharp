using DiscordBotTest.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DiscordBotTest.PrefixCommands.HelperFunctions;
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

      var builder = new DiscordWebhookBuilder();

      if (await CommandSecurityCheckClass.SecurityCheck(SecurityLevel.Guild, _bot, ctx.Guild, ctx.User))
      {
        await ctx.EditResponseAsync(builder.AddEmbed(
          _bot.NotAuthorizedError()
        ));
        return;
      }

      List<string> usernames = [username];
      var response = await BGCFunction(usernames, _bot, graph == 1, light == 1);

      builder.AddEmbeds(response.Embeds);
      if (response.Files.Count > 0)
        builder.AddFile(response.Files.First().Name, response.Files.First().Stream);

      await ctx.EditResponseAsync(builder);

      foreach (var file in response.Files)
        file.Stream.Dispose();
    }

    [SlashCommand("Compare", "Compare the current guilds google sheet(s) with the current guilds main group memberlist.")]
    public async Task Compare(InteractionContext ctx,
      [Option("guildId", "The guild to check.")] string? guildId = null)
    {
      await ctx.DeferAsync();

      var builder = new DiscordWebhookBuilder();

      if (await _bot.IsGuildAuthorized(ctx.User, ctx.Guild))
      {
        await ctx.EditResponseAsync(builder.AddEmbed(
          _bot.NotAuthorizedError()
        ));
        return;
      }

      var guild = guildId != null ? ulong.Parse(guildId) : ctx.Guild.Id;

      var MainGroup = await _bot.GetDefaultGroupAsync(guild);
      var GroupIgnores = MainGroup?.Data?.IgnoreRoles ?? [];
      //foreach (var ignore in GroupIgnores)
      //{
      //  Console.WriteLine($"IgnoreRole: {ignore}");
      //}
      var GroupID = MainGroup?.Data?.GuildId;
      if (GroupID is null)
      {
        var response = new DiscordEmbedBuilder()
          .WithTitle("No group found")
          .WithDescription("No group found for this guild.")
          .WithColor(DiscordColor.Red);
        await ctx.EditResponseAsync(builder.AddEmbed(response));
        return;
      }
      var result = await CompareDBToGroup(GroupID.Value, _bot, GroupIgnores);

      if (result is null)
      {
        var response = new DiscordEmbedBuilder()
          .WithTitle("Error")
          .WithDescription("An error occurred while comparing the database to the group. Please try again later.")
          .WithColor(DiscordColor.Red);
        await ctx.EditResponseAsync(builder.AddEmbed(response));
        return;
      }

      await ctx.EditResponseAsync(builder.AddEmbeds(result));
    }
  }
}

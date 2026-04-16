using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGuildUsersCommand : IPrefixCommand
  {
    public string Name => "GetGuildUsers";
    public string[] Aliases => ["GGU", "GetGuildU", "GetGU"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var guild = m.Channel.Guild;
      if (guild == null) return;
      var users = await s.GetUsersAsync(guild.Id.ToString());
      string[] lUsers = [.. users
        .Select(x => $"**Name: {x.Name}**\nUserID: {x.UserId}\nGroupID: {x.GroupId}\nCreated at: {x.CreatedAt}")];
      var embed = new DiscordEmbedBuilder()
        .WithTitle($"Authorized Users ({guild.Id})")
        .WithDescription(string.Join("\n", lUsers))
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

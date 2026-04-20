using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GetGuildRolesCommand : IPrefixCommand
  {
    public string Name => "GetRoles";
    public string[] Aliases => ["GR", "GetR", "GRoles", "RolesGet", "RolesG", "ShowRoles", "SRoles", "RolesShow", "RolesS", "ShowR"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var guild = m.Channel.Guild;
      if (guild is null) return;
      var embed = new DiscordEmbedBuilder();
      var roles = await s.GetRolesAsync(guild.Id.ToString());
      if (roles is null)
      {
        await m.RespondAsync(embed
          .WithTitle($"Failed to fetch authorized roles for ({guild.Id})")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      string[] lRoles = [.. roles.Data
        .Select(x => $"**Name: {x.Name}**\nRoleId: {x.RoleId}\nGroup: {x.GroupId}\nCreated at: {x.CreatedAt}\n")];
      await m.RespondAsync(embed
        .WithTitle($"Authorized Roles ({guild.Id})")
        .WithDescription(string.Join("\n", lRoles))
        .WithColor(DiscordColor.Blurple)
        .Build());
    }
  }
}

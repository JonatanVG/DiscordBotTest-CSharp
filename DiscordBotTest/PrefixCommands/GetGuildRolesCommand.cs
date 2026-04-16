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
      if (guild == null) return;
      var roles = await s.GetRolesAsync(guild.Id.ToString());
      string[] lRoles = [.. roles
        .Select(x => $"**Name: {x.Name}**\nRoleId: {x.RoleId}\nGroup: {x.GroupId}\nCreated at: {x.CreatedAt}\n")];
      var embed = new DiscordEmbedBuilder()
        .WithTitle($"Authorized Roles ({guild.Id})")
        .WithDescription(string.Join("\n", lRoles))
        .WithColor(DiscordColor.Blurple)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

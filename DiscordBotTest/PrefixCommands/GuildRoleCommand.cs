using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GuildRoleCommand : IPrefixCommand
  {
    public string Name => "RoleManage";
    public string[] Aliases => ["RM"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder().WithTitle("RoleRecord Managment");
      if (args.Length <= 1)
      {
        await m.RespondAsync(embed
          .WithDescription("Success: False\nReason: Incorrect input\nUsage: RoleManage <Add/Remove> <RoleID/RoleMention>"));
      }
      if (!s.IsOwner(m.Author.Id)) return;
      var guild = m.Channel.Guild;
      if (guild is null)
      {
        await m.RespondAsync(embed
          .WithDescription("Success: False\nReason: Command used outside of a guild.")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      DiscordRole? role = null;
      if (args[0].StartsWith("<@&") && args[0].EndsWith('>'))
      {
        var roleId = ulong.Parse(args[0][3..^1]);
        role = guild.GetRole(roleId);
      }
      else if (ulong.TryParse(args[0], out var roleId))
        role = guild.GetRole(roleId);
      if (role is null)
      {
        await m.RespondAsync(embed
          .WithDescription("Success: False\nReason: RoleRecord does not exist or arguement invalid.")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      var response = await s.PostRoleAsync(role.Name, role.Id.ToString(), guild.Id.ToString());
      if (response is null)
      {
        await m.RespondAsync(embed
          .WithDescription("Success: False\nReason: Response is null")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      await m.RespondAsync(embed
        .WithDescription($"Success: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data?.Id}\nCreated at: {response.Data?.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build());
    }
  }
}

using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GroupManageCommand : IPrefixCommand
  {
    public string Name => "ManageGroup";
    public string[] Aliases => ["MG"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Guild Managment")
        .WithColor(DiscordColor.Red);
      if (args.Length < 3)
      {
        await m.RespondAsync(embed
          .WithTitle("Invalid Input")
          .WithDescription("Usage: ManageGroup <Add/Remove> <RoleRecord/User/Sheet> <(User/RoleRecord ID/Mention)/SheetName")
          .Build());
        return;
      }
      var action = string.Equals(args[0], "Add");
      var type   = args[1];
      var target = args[2];
      var group  = m.Channel.Guild;
      DiscordUser? user = null;
      DiscordRole? role = null;
      if (type is "RoleRecord")
      {
        if (target.StartsWith("<@&") && target.EndsWith('>'))
        {
          var roleId = ulong.Parse(target[3..^1]);
          role = group.GetRole(roleId);
        }
        else if (ulong.TryParse(target, out var roleId))
          role = group.GetRole(roleId);
        if (role is null)
        {
          await m.RespondAsync(embed
            .WithDescription($"Success: False\nReason: Failed to get RoleRecord.\nInput: {target}")
            .Build());
          return;
        }
        if (action)
        {
          var response = await s.PostRoleAsync(role.Name, role.Id.ToString(), group.Id.ToString());
          if (response is null)
          {
            await m.RespondAsync(embed
              .WithDescription("Success: False\nReason: Failed to post to Database. Returned null.")
              .Build());
            return;
          }
          await m.RespondAsync(embed
            .WithDescription($"Action: {args[0]}({args[1]})\nSuccess: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data?.Id}\nCreatedAt: {response.Data?.CreatedAt}")
            .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
            .Build());
          return;
        }
        var remove = await s.RemoveGroupRoleAsync(role.Id.ToString());
        if (remove is null)
        {
          await m.RespondAsync(embed
            .WithDescription("Success: False\nReason: Failed to remove from Database. Returned null.")
            .Build());
          return;
        }
        await m.RespondAsync(embed
          .WithDescription($"Action: {args[0]}({args[1]})\nSuccess: {remove.Success}\nMessage: {remove.Message}\nRecordID: {remove.Data.Id}\nRoleName: {remove.Data.Name}\nRoleID: {remove.Data.Id}\nGroupID: {remove.Data.GroupId}\nCreatedAt: {remove.Data.CreatedAt}")
          .WithColor(remove.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
          .Build());
        return;
      }
    }
  }
}

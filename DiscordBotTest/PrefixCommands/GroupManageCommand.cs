using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class GroupManageCommand : IPrefixCommand
  {
    public string Name => "ManageGroup";
    public string[] Aliases => ["MG"];
    public string Usage => "ManageGroup Usage\nFields: <Add/Remove> <Role/User/Sheet> <(User/Role ID/Mention)/SheetName>\nOptional Fields: N/A\n\nExample Usage:\nManageGroup Add Role @Role\nManageGroup Remove User 1234567890\nManageGroup Add Sheet SheetName 1234567890 SheetID Range StartRow";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Guild Managment")
        .WithColor(DiscordColor.Red);
      if (args.Length < 3)
      {
        await m.RespondAsync(embed
          .WithTitle("Invalid Input")
          .WithDescription(Usage)
          .Build());
        return;
      }
      var action = string.Equals(args[0], "Add");
      var type   = args[1];
      var target = args[2];
      var group  = m.Channel.Guild;
      DiscordUser? user = null;
      DiscordRole? role = null;
      if (type is "Role")
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
            .WithDescription($"Success: False\nReason: Failed to get Role.\nInput: {target}")
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
            .WithDescription($"Action: {args[0]}<{type}>({target})\nSuccess: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data?.Id}\nCreatedAt: {response.Data?.CreatedAt}")
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
          .WithDescription($"Action: {args[0]}({args[1]})\nSuccess: {remove?.Success}\nMessage: {remove?.Message}\nRecordID: {remove?.Data?.Id}\nRoleName: {remove?.Data?.Name}\nRoleID: {remove?.Data?.Id}\nGroupID: {remove?.Data?.GroupId}\nCreatedAt: {remove?.Data?.CreatedAt}")
          .WithColor(remove?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
          .Build());
        return;
      }
      else if (type is "User")
      {
        if (target.StartsWith("<@") && target.EndsWith('>'))
        {
          var userId = ulong.Parse(target[2..^1]);
          user = await s.Client.GetUserAsync(userId);
        }
        else if (ulong.TryParse(target, out var userId))
          user = await s.Client.GetUserAsync(userId);
        if (user is null)
        {
          await m.RespondAsync(embed
            .WithDescription($"Success: False\nReason: Failed to get User.\nInput: {action} {type} {target}")
            .Build());
          return;
        }
        if (action)
        {
          var response = await s.PostUserAsync(user.Username, user.Id.ToString(), group.Id.ToString());
          if (response is null)
          {
            await m.RespondAsync(embed
              .WithDescription($"Success: False\nReason: Failed to post to Database. Returned null.")
              .Build());
            return;
          }
          await m.RespondAsync(embed
            .WithDescription($"Action: {args[0]}<{type}>({target})\nSuccess: {response?.Success}\nMessage: {response?.Message}\nRecordID: {response?.Data?.Id}\nCreatedAt: {response?.Data?.CreatedAt}")
            .WithColor(response?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
            .Build());
          return;
        }
        var remove = await s.RemoveGroupUserAsync(user.Id.ToString());
        if (remove is null)
        {
          await m.RespondAsync(embed
            .WithDescription("Success: False\nReason: Failed to remove from Database. Returned null.")
            .Build());
          return;
        }
        await m.RespondAsync(embed
          .WithDescription($"Action: {args[0]}({args[1]})\nSuccess: {remove?.Success}\nMessage: {remove?.Message}\nRecordID: {remove?.Data?.Id}\nUsername: {remove?.Data?.Name}\nUserID: {remove?.Data?.Id}\nGroupID: {remove?.Data?.GroupId}\nCreatedAt: {remove?.Data?.CreatedAt}")
          .WithColor(remove?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
          .Build());
        return;
      }
      else if (type is "Sheet")
      {
        if (args.Length < 3)
        {
          await m.RespondAsync(embed
            .WithTitle("Invalid Input")
            .WithDescription("Usage: ManageGroup Remove Sheet <SheetName>")
            .Build());
          return;
        }
        if (action)
        {
          if (args.Length < 7)
          {
            await m.RespondAsync(embed
              .WithTitle("Invalid Input")
              .WithDescription("Usage: ManageGroup Add Sheet <SheetName> <RobloxGroupID> <SheetID> <Range(EX: C7:D200)> <StartRow>")
              .Build());
            return;
          }
          var GroupID = int.Parse(args[3]);
          var startRow = int.Parse(args[6]);
          var response = await s.PostSheetAsync(GroupID, target, args[4], args[5], startRow);
          if (response is null)
          {
            await m.RespondAsync(embed
              .WithDescription($"Success: False\nReason: Failed to post to Database. Returned null.")
              .Build());
            return;
          }
          await m.RespondAsync(embed
            .WithDescription($"Action: {args[0]}<{type}>({target})\nSuccess: {response?.Success}\nMessage: {response?.Message}\nRecordID: {response?.Data?.Id}\nCreatedAt: {response?.Data?.CreatedAt}")
            .WithColor(response?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
            .Build());
          return;
        }
        var remove = await s.RemoveGroupSheetAsync(target);
        if (remove is null)
        {
          await m.RespondAsync(embed
            .WithDescription("Success: False\nReason: Failed to remove from Database. Returned null.")
            .Build());
          return;
        }
        await m.RespondAsync(embed
          .WithDescription($"Action: {args[0]}({args[1]})\nSuccess: {remove?.Success}\nMessage: {remove?.Message}\nRecordID: {remove?.Data?.Id}\nUsername: {remove?.Data?.Name}\nUserID: {remove?.Data?.Id}\nGuildID: {remove?.Data?.GuildId}\nCreatedAt: {remove?.Data?.CreatedAt}")
          .WithColor(remove?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
          .Build());
        return;
      }
    }
  }
}

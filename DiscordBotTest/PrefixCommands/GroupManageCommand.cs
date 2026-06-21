using DiscordBotTest.Services;
using DSharpPlus.Entities;
using DiscordBotTest.PrefixCommands.HelperFunctions;

namespace DiscordBotTest.PrefixCommands
{
  public class GroupManageCommand : IPrefixCommand
  {
    public string Name => "ManageGroup";
    public string[] Aliases => ["MG"];
    public string Usage => "ManageGroup Usage\nFields: <Add/Remove> <Role/User/Sheet> <(User/Role ID/Mention)/SheetName>\nOptional Fields: N/A\n\nExample Usage:\nManageGroup Add Role @Role\nManageGroup Remove User 1234567890\nManageGroup Add Sheet SheetName 1234567890 SheetID Range StartRow";
    public string Category => "Guild Management";
    public SecurityLevel SecurityLevel => SecurityLevel.GuildOwner;

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
      var action = string.Equals(args[0].ToLower(), "add");
      var type   = args[1];
      var target = args[2];
      var guild  = m.Channel.Guild;
      if (type.ToLower() is "role")
      {
        if (action)
        {
          await m.RespondAsync(await AddGuildRoleClass.AddRoleAsyncFunc(s, target, guild));
          return;
        }
        await m.RespondAsync(await RemoveGuildRoleClass.RemoveRoleAsyncFunc(s, target, guild));
        return;
      }
      else if (type.ToLower() is "user")
      {
        if (action)
        {
          await m.RespondAsync(await AddGuildUserClass.AddGuildUserAsyncFunc(s, target, guild));
          return;
        }
        await m.RespondAsync(await RemoveGuildUserClass.RemoveGuildUserAsyncFunc(s, target, guild));
        return;
      }
      else if (type.ToLower() is "sheet")
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

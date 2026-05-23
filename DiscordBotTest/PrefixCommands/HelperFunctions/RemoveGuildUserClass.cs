using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands.HelperFunctions
{
  public class RemoveGuildUserClass
  {
    public static async Task<DiscordEmbed> RemoveGuildUserAsyncFunc(BotService s, string target, DiscordGuild guild)
    {
      var result = new DiscordEmbedBuilder()
        .WithTitle("Guild Managment");
      DiscordMember? user = null;
      if (ulong.TryParse(target, out var userId))
        user = await guild.GetMemberAsync(userId);
      else if (target.StartsWith("<@") && target.EndsWith('>'))
      {
        userId = ulong.Parse(target[2..^1]);
        user = await guild.GetMemberAsync(userId);
      }
      if (user == null)
      {
        return result
          .WithDescription($"Error: Invalid Input\nUser: ({target}) is not a valid user(ID) for this guild.")
          .WithColor(DiscordColor.DarkRed)
          .Build();
      }
      var response = await s.RemoveGuildUserAsync(user.Id.ToString());
      if (response is null)
      {
        return result
          .WithDescription("Success: False\nReason: Failed to post to Database. Returned null.")
          .Build();
      }
      await s.AuditAction(DateTimeOffset.Now.ToString(), "User Action", "Remove<User>", $"{target}");
      s._auth[guild.Id.ToString()].Remove(userId.ToString());
      return result
        .WithDescription($"Action: Remove<User>({target})\nSuccess: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data?.Id}\nCreatedAt: {response.Data?.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
    }
  }
}

using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands.HelperFunctions
{
  public class AddGuildRoleClass
  {
    public static async Task<DiscordEmbed> AddRoleAsyncFunc(BotService s, string target, DiscordGuild guild) 
    {
      var result = new DiscordEmbedBuilder()
        .WithTitle("Guild Managment");
      DiscordRole? role = null;
      if (ulong.TryParse(target, out var roleId))
        role = guild.GetRole(roleId);
      else if (target.StartsWith("<@&") && target.EndsWith('>'))
      {
        roleId = ulong.Parse(target[3..^1]);
        role = guild.GetRole(roleId);
      }
      if (role == null)
      {
        return result
          .WithDescription($"Error: Invalid Input\nRole: ({target}) is not a valid role(ID) for this guild.")
          .WithColor(DiscordColor.DarkRed)
          .Build();
      }
      var response = await s.PostGuildRoleAsync(role.Name, role.Id.ToString(), guild.Id.ToString());
      if (response is null)
      {
        return result
          .WithDescription("Success: False\nReason: Failed to post to Database. Returned null.")
          .Build();
      }
      await s.AuditAction(DateTimeOffset.Now.ToString(), "User Action", "Add<Role>", $"{response.Data}");
      s._auth[guild.Id.ToString()][role.Id.ToString()] = role.Name.ToString();
      return result
        .WithDescription($"Action: Add<Role>({target})\nSuccess: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data?.Id}\nCreatedAt: {response.Data?.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
    }
  }
}

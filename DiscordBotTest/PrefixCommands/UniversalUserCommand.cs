using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class UniversalUserCommand : IPrefixCommand
  {
    public string Name => "Admin";
    public string[] Aliases => ["A", "Administrator", "BotAdmin", "BAdmin"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id))
      {
        await m.RespondAsync("You are not the owner of this bot.");
        return;
      }
      var response = new DiscordEmbedBuilder().WithTitle("Admin Registration");
      DiscordUser? user = null;
      var action = string.Equals(args[0].ToLower(), "add");
      Console.WriteLine($"Usermention: {args[1]}");
      if (args[1].StartsWith("<@") && args[1].EndsWith('>'))
      {
        var userId = ulong.Parse(args[1][2..^1].TrimStart('!'));
        user = await s.Client.GetUserAsync(userId);
      }
      else if (ulong.TryParse(args[1], out var userId))
      {
        user = await s.Client.GetUserAsync(userId);
      }
      if (user is null)
      {
        await m.RespondAsync(response
          .WithDescription($"Success: False\nMessage: Could not find user with ID ({args[1]})")
          .WithColor(DiscordColor.Red)
          .Build());
        return;
      }
      if (action) 
      {
        var post = await s.PostAdminUserAsync(user.Username, user.Id.ToString());
        await m.RespondAsync(response
          .WithDescription($"Success: {post!.Success}\nMessage: {post.Message}\nRecordID: {post.Data!.Id}\nCreated at: {post.Data.CreatedAt}")
          .WithColor(post.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
          .Build());
        return;
      }
      var remove = await s.RemoveAdminUserAsync(user.Id.ToString());
      await m.RespondAsync(response
        .WithDescription($"Success: {remove!.Success}\nMessage: {remove.Message}\n\n**RemovedUser:**\nRecordID: {remove.Data!.Id}\nUsername: {remove.Data.Name}\nUserID: {remove.Data.UserId}\nCreatedAt: {remove.Data.CreatedAt}")
        .WithColor(remove.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build());
    }
  }
}
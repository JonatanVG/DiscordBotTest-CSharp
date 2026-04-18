using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterUniversalUserCommand : IPrefixCommand
  {
    public string Name => "AddAdmin";
    public string[] Aliases => ["AA", "Admin", "Administrator", "BotAdmin", "BAdmin"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id))
      {
        await m.RespondAsync("You are not the owner of this bot.");
        return;
      }
      DiscordUser? user = null;
      Console.WriteLine($"Usermention: {args[0]}");
      if (args[0].StartsWith("<@") && args[0].EndsWith('>'))
      {
        var userId = ulong.Parse(args[0][2..^1].TrimStart('!'));
        user = await s.Client.GetUserAsync(userId);
      }
      else if (ulong.TryParse(args[0], out var rawId))
        user = await s.Client.GetUserAsync(rawId);
      if (user == null) return;
      var response = await s.PostAdminUserAsync(user.Username, user.Id.ToString());
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Admin Registration")
        .WithDescription($"Success: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data.Id}\nCreated at: {response.Data.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}
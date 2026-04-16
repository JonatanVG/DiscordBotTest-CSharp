using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterGuildUserCommand : IPrefixCommand
  {
    public string Name => "GuildRegisterUser";
    public string[] Aliases => ["GRU", "GRegisterUser", "GuildRUser", "GuildRegisterU", "GRUser", "GuildRU", "GRegisterU", "GuildRUser"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (s.IsOwner(m.Author.Id)) return;
      var guild = m.Channel.Guild;
      if (guild == null) return;
      DiscordUser? user = null;
      if (args[0].StartsWith("<@") && args[0].EndsWith('>'))
      {
        var userId = ulong.Parse(args[0].Substring(2, args[0].Length - 3));
        user = await s.Client.GetUserAsync(userId);
      }
      if (user == null) return;
      var response = await s.PostUserAsync(user.Username, user.Id.ToString(), guild.Id.ToString());
      var embed = new DiscordEmbedBuilder()
        .WithTitle("User Registration")
        .WithDescription($"Success: {response.Success}\nMessage: {response.Message}\nRecord ID: {response.Data.Id}\nCreated at: {response.Data.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}
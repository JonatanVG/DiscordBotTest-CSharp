using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterGuildCommand : IPrefixCommand
  {
    public string Name => "RegisterGuild";
    public string[] Aliases => ["RegisterG", "RG"];
    public string Usage => "RegisterGuild Usage\nFields: <GuildName> <GuildId> <SheetId>\nOptional Fields: N/A\n\nExample Usage:\nRegisterGuild";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id)) return;
      var guild = m.Channel.Guild;
      if (guild == null) return;
      var mainGroup = guild.Id.ToString()!;
      string[] ignores = args[3].Split(',') ?? [];
      var response = await s.PostGuildAsync(args[0], int.Parse(args[1]), args[2], ignores, mainGroup);
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Guild Registration")
        .WithDescription($"Success: {response?.Success}\nMessage: {response?.Message}\nRecordID: {response?.Data?.Id}\nCreated at: {response?.Data?.CreatedAt}")
        .WithColor(response?.Success == true ? DiscordColor.SpringGreen : DiscordColor.DarkRed)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

using DiscordBotTest.Services;
using DSharpPlus.Entities;
using static DiscordBotTest.StaticFunctions;

namespace DiscordBotTest.PrefixCommands
{
  public class DatabaseComparerCommand : IPrefixCommand
  {
    public string Name => "dbcompare";
    public string[] Aliases => ["dbc"];
    public string Usage => "dbcompare Usage\nFields: N/A\nOptional Fields: N/A\n\nExample Usage:\ndbcompare\ndbcompare 6769420";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      var MainGroup = await s.GetDefaultGroupAsync(m.Channel.Guild.Id);
      var GroupID = MainGroup?.Data?.GuildId;
      if (GroupID is null)
      {
        await m.RespondAsync("No group found for this guild.");
        return;
      }
      var result = await CompareDBToGroup(GroupID.Value, s);

      if (result is null)
      {
        await m.RespondAsync("An error occurred while comparing the database to the group. Please try again later.");
        return;
      }

      await m.RespondAsync(new DiscordMessageBuilder().WithContent($"**{result.Count}** embeds found.").AddEmbeds(result));
    }
  }
}

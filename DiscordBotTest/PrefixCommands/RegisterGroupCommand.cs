using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterGroupCommand : IPrefixCommand
  {
    public string Name => "RegisterGroup";
    public string[] Aliases => ["RGR", "RegisterGr", "RGroup", "GroupR", "GRegister"];
    public string Usage => "RegisterGroup Usage\nFields: N/A\nOptional Fields: N/A\n\nExample Usage:\nRegisterGroup";
    public string Category => "Guild Management";

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id)) return;
      var guild     = m.Channel.Guild;
      if (guild == null) return;
      var groupName = guild.Name;
      var groupId   = guild.Id.ToString();
      var ownerId   = guild.OwnerId.ToString();
      var ownerName = guild.Owner.Username;
      var response = await s.PostGroupAsync(groupName, groupId, ownerId, ownerName);
      var embed = new DiscordEmbedBuilder()
        .WithTitle("GroupRecord Registration")
        .WithDescription($"Success: {response?.Success}\nMessage: {response?.Message}\nRecordID: {response?.Data?.Id}\nCreated at: {response?.Data?.CreatedAt}")
        .WithColor(response?.Success == true ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

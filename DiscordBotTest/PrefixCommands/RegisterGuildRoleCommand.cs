using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands
{
  public class RegisterGuildRoleCommand : IPrefixCommand
  {
    public string Name => "RegisterRole";
    public string[] Aliases => ["RR", "RegisterR", "RRole", "RoleRegister", "RoleR"];

    public async Task ExecuteAsync(BotService s, DiscordMessage m, string[] args)
    {
      if (!s.IsOwner(m.Author.Id)) return;
      var guild = m.Channel.Guild;
      if (guild == null) return;
      DiscordRole? role = null;
      if (args[0].StartsWith("<@&") && args[0].EndsWith('>'))
      {
        var roleId = ulong.Parse(args[0].Substring(3, args[0].Length - 4));
        role = guild.GetRole(roleId);
      }
      if (role == null) return;
      var response = await s.PostRoleAsync(role.Name, role.Id.ToString(), guild.Id.ToString());
      var embed = new DiscordEmbedBuilder()
        .WithTitle("Role Registration")
        .WithDescription($"Success: {response.Success}\nMessage: {response.Message}\nRecordID: {response.Data.Id}\nCreated at: {response.Data.CreatedAt}")
        .WithColor(response.Success ? DiscordColor.SpringGreen : DiscordColor.Red)
        .Build();
      await m.RespondAsync(embed);
    }
  }
}

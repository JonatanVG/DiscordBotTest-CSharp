using DiscordBotTest.Services;
using DSharpPlus.Entities;

namespace DiscordBotTest.PrefixCommands.HelperFunctions
{
  public class CommandSecurityCheckClass
  {
    public static async Task<bool> SecurityCheck(SecurityLevel SecurityLevel, BotService s, DiscordGuild g, DiscordUser u)
    {
      switch (SecurityLevel)
      {
        case SecurityLevel.Public:
          return true;
        case SecurityLevel.Guild:
          if (g == null)
            return s.IsOwner(u.Id) || s.IsBotAdmin(u.Id);
          return await s.IsGuildAuthorized(u, g);
        case SecurityLevel.GuildOwner:
          if (g == null)
            return s.IsOwner(u.Id) || s.IsBotAdmin(u.Id);
          return await s.IsGuildOwner(u, g) || s.IsBotAdmin(u.Id);
        case SecurityLevel.Admin:
          return s.IsBotAdmin(u.Id) || s.IsOwner(u.Id);
        case SecurityLevel.Owner:
          return s.IsOwner(u.Id);
      }
      return false;
    }
  }
}

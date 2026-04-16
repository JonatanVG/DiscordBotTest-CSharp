using DSharpPlus;
using System.Text.RegularExpressions;

namespace DiscordBotTest
{
  public class ActionResolver
  {
    private static readonly List<(Regex Pattern, Action<Match> Handler, DiscordClient Caller)> _handlers = [];

    public static void Register(string pattern, Action<Match> handler, DiscordClient caller)
    {
      _handlers.Add((new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled), handler, caller));
    }

    public static bool TryExecute(string action)
    {
      foreach(var (pattern, handler, caller) in _handlers)
      {
        var match = pattern.Match(action);
        
        if (match.Success)
        {
          handler(match);
          return true;
        }
      }
      return false;
    }
  }
}

namespace DiscordBotTest
{
  public static class StaticFunctions
  {
    public static string[] ParseArgs(string input)
    {
      var args = new List<string>();
      var current = new System.Text.StringBuilder();
      bool inQuotes = false;

      foreach (char c in input)
      {
        if (c == '"')
        {
          inQuotes = !inQuotes;
        }
        else if (c == ' ' && !inQuotes)
        {
          if (current.Length > 0)
          {
            args.Add(current.ToString());
            current.Clear();
          }
        }
        else
        {
          current.Append(c);
        }
      }

      if (current.Length > 0)
        args.Add(current.ToString());

      return [.. args];
    }
  }
}

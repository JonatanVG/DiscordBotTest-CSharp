using ScottPlot;
using ScottPlot.Palettes;

namespace DiscordBotTest
{
  public static class StaticFunctions
  {
    public static byte[] PlotCumulativeBadges(string username, long userId, DateTimeOffset[] dates, bool mode)
    {
      var sorted = dates.Order().ToList();

      double[] xValues = [.. sorted.Select(d => d.UtcDateTime.ToOADate())];
      double[] yValues = [.. Enumerable.Range(1, sorted.Count).Select(i => (double)i)];

      var plt = new Plot();
      plt.Title($"Badges over time for {username} ({userId})");
      plt.XLabel("Badge Earned Date");
      plt.YLabel("Total Badges");
      if (!mode)
      {
        var pltStle = new PlotStyle
        {
          FigureBackgroundColor = Colors.Black,
          DataBackgroundColor = Colors.Black,
          GridMajorLineColor = Colors.White,
          AxisColor = Colors.White
        };
        plt.SetStyle(pltStle);
      }

      var scatter = plt.Add.Scatter(xValues, yValues);
      scatter.MarkerStyle.Shape = MarkerShape.OpenCircle;
      scatter.MarkerStyle.Size = 5;
      scatter.LineWidth = 0;

      plt.Axes.DateTimeTicksBottom();

      plt.Add.Annotation($"Badge Count: {sorted.Count}");

      return plt.GetImageBytes(800, 600, ImageFormat.Png);
    }

    public static DateTimeOffset[] GetAwardDates(List<InventoryItem> items) => [.. items.Select(x => x.AddTime)];
    
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

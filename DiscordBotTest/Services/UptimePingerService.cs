namespace DiscordBotTest.Services
{
  public class UptimePingerService : BackgroundService
  {
    private readonly IHttpClientFactory _factory;
    private readonly DbService _db;
    private readonly BotService _bot;
    private string? _url;

    public UptimePingerService(DbService db, IHttpClientFactory factory, BotService bot)
    {
      _db = db;
      _bot = bot;
      _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var data = await _db.CallFunctionWithResponse<string>("get_koyeb_url", []);
      _url = data?.Data ?? null;
      if (string.IsNullOrEmpty(_url)) return;

      while (!stoppingToken.IsCancellationRequested)
      {
        using var http = _factory.CreateClient("uptime");
        using var response = await http.GetAsync(_url, stoppingToken);
        await _db.CallFunction("audit_botstart", [DateTimeOffset.Now.ToString(), "Bot Started/Restarted", _bot.Client.CurrentApplication.Name]);
        await _db.CallFunction2("audit_botstart", [_bot.Client.CurrentApplication.Name, "Bot Started/Restarted", DateTimeOffset.Now.ToString()]);
        await Task.Delay(TimeSpan.FromMinutes(45), stoppingToken);
      }
    }
  }
}

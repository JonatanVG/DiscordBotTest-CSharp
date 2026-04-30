namespace DiscordBotTest.Services
{
  public class UptimePingerService : BackgroundService
  {
    private readonly HttpClient _http = new();
    private readonly DbService _db;
    private string? _url;

    public UptimePingerService(DbService db)
    {
      _db = db;
      _url = "2";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var data = await _db.CallFunctionWithResponse<string>("get_koyeb_url", []);
      _url = data?.Data ?? null;
      if (string.IsNullOrEmpty(_url)) return;

      while (!stoppingToken.IsCancellationRequested)
      {
        await _http.GetAsync(_url, stoppingToken);
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
      }
    }
  }
}

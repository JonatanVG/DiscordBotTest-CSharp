namespace DiscordBotTest.Services
{
  public class UptimePingerService : BackgroundService
  {
    private readonly HttpClient _http = new();
    private readonly string? _url;

    public UptimePingerService(IConfiguration config)
    {
      _url = config["UptimePinger:Url"];
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      if (string.IsNullOrEmpty(_url)) return;

      while (!stoppingToken.IsCancellationRequested)
      {
        await _http.GetAsync(_url, stoppingToken);
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
      }
    }
  }
}

using Npgsql;
using System.Text.Json;

namespace DiscordBotTest.Services
{
  public class DbService
  {
    private readonly NpgsqlDataSource _dataSource;

    public DbService()
    {
      var _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
        ?? throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");

      _dataSource = NpgsqlDataSource.Create(_connectionString);
    }

    public async Task<ApiResponse<T>?> CallFunctionWithResponse<T>(string func, params object?[] args) where T : class
    {
      await using var conn = await _dataSource.OpenConnectionAsync();
      var argsP = string.Join(", ", args.Select((_, i) => $"${i + 1}"));
      var sql = $"SELECT * FROM {func}({argsP})";
      await using var cmd = new NpgsqlCommand(sql, conn);
      foreach (var arg in args)
        cmd.Parameters.AddWithValue(arg!);
      await using var reader = await cmd.ExecuteReaderAsync();
      while (await reader.ReadAsync())
      {
        var success = reader.GetBoolean(0);
        var message = reader.GetString(1);
        var dat = reader.GetValue(2).ToString();

        return new ApiResponse<T>
        {
          Success = success,
          Message = message,
          Data = string.IsNullOrEmpty(dat) ? null : JsonSerializer.Deserialize<T>(dat)
        };
      }
      return null;
    }
  }
}

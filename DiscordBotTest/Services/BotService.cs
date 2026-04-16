using DiscordBotTest.PrefixCommands;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBotTest.Services
{
  public class BotService : IHostedService
  {
    private readonly DiscordClient _client;
    private readonly CommandExecutor _executor;
    private readonly DbService _db;

    public BotService(DiscordClient client, DbService db, CommandRegistry registry)
    {
      _client = client;
      _db = db;
      _executor = new(registry, '!', this);
    }

    public DiscordClient Client => _client;

    public async Task<Guild> GetGuildAsync(int guildId) => await _db.CallFunction<Guild>("get_guild_by_id", guildId);

    public async Task<Guild> GetDefaultGuildAsync(ulong groupId) => await _db.CallFunction<Guild>("get_guild_by_group", groupId.ToString());

    public async Task<Sheet[]> GetGuildSheetsAsync(int guildId) => await _db.CallFunction<Sheet[]>("get_guild_sheets", guildId);

    public async Task<ApiResponse<Guild>> PostGuildAsync(string guildName, int guildId, string sheetId, string[] ignoreRoles, string mainGroup) => await _db.CallFunctionWithResponse<Guild>("register_guild", [guildName, guildId, sheetId, ignoreRoles, mainGroup]);

    public async Task<ApiResponse<Sheet>> PostSheetAsync(int guildId, string sheetName, string sheetId, string range, int startRow) => await _db.CallFunctionWithResponse<Sheet>("register_sheet_with_guild", [sheetName, sheetId, guildId, range, startRow]);

    public async Task<ApiResponse<Group>> PostGroupAsync(string groupName, string groupId, string ownerId, string ownerName) => await _db.CallFunctionWithResponse<Group>("register_group", [groupName, groupId, ownerId, ownerName]);

    public async Task<ApiResponse<Role>> PostRoleAsync(string roleName, string roleId, string groupId) => await _db.CallFunctionWithResponse<Role>("register_role_with_group", [roleName, roleId, groupId]);

    public async Task<Role[]> GetRolesAsync(string groupId) => await _db.CallFunction<Role[]>("get_group_roles", groupId);

    public async Task<User[]> GetUsersAsync(string groupId) => await _db.CallFunction<User[]>("get_group_users", groupId);

    public async Task<ApiResponse<User>> PostUserAsync(string userName, string userId, string groupId) => await _db.CallFunctionWithResponse<User>("register_user_with_group", [userName, userId, groupId]);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      _client.MessageCreated += async (s, e) => await _executor.HandleAsync(e.Message);
      
      await _client.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await _client.DisconnectAsync();
      _client.Dispose();
      Environment.Exit(0);
    }

    public bool IsOwner(ulong id) => id == _client.CurrentApplication.Owners.FirstOrDefault()!.Id;
  }
}
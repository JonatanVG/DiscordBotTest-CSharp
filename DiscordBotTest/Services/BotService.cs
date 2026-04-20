using DiscordBotTest.PrefixCommands;
using DiscordBotTest.SlashCommands;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace DiscordBotTest.Services
{
  public class BotService : IHostedService
  {
    private readonly DiscordClient _client;
    private readonly CommandExecutor _executor;
    private readonly DbService _db;
    private readonly RobloxAPIServices _robloxApi;
    private readonly TrelloService _trello;
    private readonly IServiceProvider _serviceProvider;

    public BotService(DiscordClient client, DbService db, CommandRegistry registry, RobloxAPIServices robloxApi, TrelloService trello, IServiceProvider serviceProvider)
    {
      _client = client;
      _db = db;
      _executor = new(registry, '!', this);
      _robloxApi = robloxApi;
      _trello = trello;
      _serviceProvider = serviceProvider;
    }

    public DiscordClient Client => _client;

    //// Database API calls
    /// <summary>
    /// Retrieves a guild record from the database matching the specified guildId.
    /// </summary>
    /// <param name="guildId">The guildId to search for.</param>
    /// <returns>A ApiResponse object containg a Guild object containing the guild records data.</returns>
    public async Task<ApiResponse<Guild>?> GetGuildAsync(int guildId) => await _db.CallFunctionWithResponse<Guild>("get_guild_by_id", guildId);

    /// <summary>
    /// Retrieves a guild record from the database matching the specified groupId.
    /// </summary>
    /// <param name="groupId">The groupId to search for.</param>
    /// <returns>A ApiResponse object containg a Guild object containing the guild records data.</returns>
    public async Task<ApiResponse<Guild>?> GetDefaultGuildAsync(ulong groupId) => await _db.CallFunctionWithResponse<Guild>("get_guild_by_group", groupId.ToString());

    /// <summary>
    /// Retrieves a list of a guilds recorded sheets by its guildId.
    /// </summary>
    /// <param name="guildId">The guildId to search with.</param>
    /// <returns>A ApiResponse object containing a list of Sheet objects containing the data of each recorded sheet.</returns>
    public async Task<ApiResponse<Sheet[]>?> GetGuildSheetsAsync(int guildId) => await _db.CallFunctionWithResponse<Sheet[]>("get_guild_sheets", guildId);

    /// <summary>
    /// Gets a list of Role records registered with the Group relating to the specified groupId.
    /// </summary>
    /// <param name="groupId">The groupId to search with.</param>
    /// <returns>A ApiResponse object containing a list of Role objects containing the Role records data.</returns>
    public async Task<ApiResponse<RoleRecord[]>?> GetRolesAsync(string groupId) => await _db.CallFunctionWithResponse<RoleRecord[]>("get_group_roles", groupId);

    /// <summary>
    /// Gets a list of User records registered with the Group relating to the specified groupId.
    /// </summary>
    /// <param name="groupId">The groupId to search with.</param>
    /// <returns>A ApiResponse object containing a list of User objects containing the User records data.</returns>
    public async Task<ApiResponse<User[]>?> GetUsersAsync(string groupId) => await _db.CallFunctionWithResponse<User[]>("get_group_users", groupId);

    /// <summary>
    /// Registers a new User record into the Databases GROUP_SPECIFIC_AUTHORIZED_USERS table using the data parameters.
    /// This registers the User as a administrative user granting it access to use specific access-locked commands in the specific group.
    /// To be used when you want to only grant an individual User administrative access rather everyone with a specific Role.
    /// </summary>
    /// <param name="userName">The name of the User to record.</param>
    /// <param name="userId">The id of the User to record.</param>
    /// <param name="groupId">The id of the Group the User is to be recorded with.</param>
    /// <returns>A ApiResponse object containing a User object containing the new records data.</returns>
    public async Task<ApiResponse<User>?> PostUserAsync(string userName, string userId, string groupId) => await _db.CallFunctionWithResponse<User>("register_user_with_group", [userName, userId, groupId]);

    /// <summary>
    /// Registers a new User record into the Datbases AUTHORIZED_BOT_USERS table using the data parameters.
    /// This registers the specific User as a Admin over the bot allowing them to use otherwise restricted commands.
    /// </summary>
    /// <param name="userName">The name of the User to record.</param>
    /// <param name="userId">The id of the User to record.</param>
    /// <returns>A ApiResponse object containing a User object containing the new records data.</returns>
    public async Task<ApiResponse<User>?> PostAdminUserAsync(string userName, string userId) => await _db.CallFunctionWithResponse<User>("register_admin_user", [userName, userId]);

    /// <summary>
    /// Registers a new Guild record into the Databases REGISTERED_GUILDS table using the data parameters.
    /// </summary>
    /// <param name="guildName">The name of the guild to record.</param>
    /// <param name="guildId">The id of the guild to record.</param>
    /// <param name="sheetId">The sheet of the guild to record.</param>
    /// <param name="ignoreRoles">The roles in the guild that should not be used in the Comparison command when comparing databases to guild memberlists.</param>
    /// <param name="mainGroup">Which group should automatically use this guild record for commands when no guild record has been specified.</param>
    /// <returns>A ApiResponse object containing a Guild object containing the new records data.</returns>
    public async Task<ApiResponse<Guild>?> PostGuildAsync(string guildName, int guildId, string sheetId, string[] ignoreRoles, string mainGroup) => await _db.CallFunctionWithResponse<Guild>("register_guild", [guildName, guildId, sheetId, ignoreRoles, mainGroup]);

    /// <summary>
    /// Registers a new Sheet record into the Databases REGISTERED_GUILD_SHEETS table using the data parameters.
    /// </summary>
    /// <param name="guildId">The id of the Guild the Sheet to record belongs to.</param>
    /// <param name="sheetName">The name of the Sheet to record.</param>
    /// <param name="sheetId">The id of the Sheet to record.</param>
    /// <param name="range">The range the bot should use when fetching from the Sheet to record.</param>
    /// <param name="startRow">The starting row the bot should use when fetching from the Sheet to record.</param>
    /// <returns>A ApiResponse object containing a Sheet object containing the new records data.</returns>
    public async Task<ApiResponse<Sheet>?> PostSheetAsync(int guildId, string sheetName, string sheetId, string range, int startRow) => await _db.CallFunctionWithResponse<Sheet>("register_sheet_with_guild", [sheetName, sheetId, guildId, range, startRow]);

    /// <summary>
    /// Registers a new Group record into the Databases REGISTERED_GROUPS table using the data parameters.
    /// </summary>
    /// <param name="groupName">The name of the Group to record.</param>
    /// <param name="groupId">The id of the Group to record.</param>
    /// <param name="ownerId">The id of the owner of the Group to record.</param>
    /// <param name="ownerName">The name of the owner of the Group to record.</param>
    /// <returns>A ApiResponse object containing a Group object containing the new records data.</returns>
    public async Task<ApiResponse<GroupRecord>?> PostGroupAsync(string groupName, string groupId, string ownerId, string ownerName) => await _db.CallFunctionWithResponse<GroupRecord>("register_group", [groupName, groupId, ownerId, ownerName]);

    /// <summary>
    /// Registers a new Role record into the Databases GROUP_SPECIFIC_AUTORIZED_ROLES table using the data parameters.
    /// This registers the Role as a administrative role granting it access to use specific access-locked commands in the specific group.
    /// </summary>
    /// <param name="roleName">The name of the Role to record.</param>
    /// <param name="roleId">The id of the Role to record.</param>
    /// <param name="groupId">The id of the Group the Role is to be recorded with.</param>
    /// <returns>A ApiResponse object containing a Role object containing the new records data.</returns>
    public async Task<ApiResponse<RoleRecord>?> PostRoleAsync(string roleName, string roleId, string groupId) => await _db.CallFunctionWithResponse<RoleRecord>("register_role_with_group", [roleName, roleId, groupId]);

    /// <summary>
    /// Deletes a recorded User from the Databases AUTHORIZED_BOT_USERS table by the specific userId.
    /// This removes the specific Users Admin privileges over the Bot.
    /// </summary>
    /// <param name="userId">The userId to search for and delete the relating record of.</param>
    /// <returns>A ApiResponse object containing a User object containing the deleted records data.</returns>
    public async Task<ApiResponse<User>?> RemoveAdminUserAsync(string userId) => await _db.CallFunctionWithResponse<User>("remove_admin_user", [userId]);

    public async Task<ApiResponse<User>?> RemoveGroupUserAsync(string userId) => await _db.CallFunctionWithResponse<User>("deauthorize_user_from_group", [userId]);


    public async Task<ApiResponse<RoleRecord>?> RemoveGroupRoleAsync(string roleId) => await _db.CallFunctionWithResponse<RoleRecord>("deauthorize_role_from_group", [roleId]);


    public async Task<ApiResponse<Sheet>?> RemoveGroupSheetAsync(string sheetName) => await _db.CallFunctionWithResponse<Sheet>("remove_sheet_from_group", [sheetName]);


    public async Task<ApiResponse<GroupRecord>?> RemoveGroupAsync(string groupId) => await _db.CallFunctionWithResponse<GroupRecord>("remove_group_registration", [groupId]);


    public async Task<ApiResponse<Guild>?> RemoveGuildAsync(string guildId) => await _db.CallFunctionWithResponse<Guild>("remove_guild_registration", [guildId]);
    

    //// Roblox API calls
    /// <summary>
    /// Retrieves basic information for Roblox users matching the specified username.
    /// </summary>
    /// <param name="userName">The username to search for.</param>
    /// <returns>A dictionary mapping user IDs to basic Roblox user information, or null if no users are found.</returns>
    public async Task<Dictionary<long, BasicRobloxUser>?> PostGetRobloxUsersAsync(string userName) => await _robloxApi.GetUserBasicAsync(userName);

    /// <summary>
    /// Retrieves badges for a Roblox user matching the specified userId.
    /// </summary>
    /// <param name="userId">The userId to search with.</param>
    /// <returns>A list containing all the users badges.</returns>
    public async Task<List<InventoryItem>?> GetRobloxUserBadgesAsync(long userId) => await _robloxApi.GetUserBadgesAsync(userId);

    /// <summary>
    /// Retrieves advanced information for a Roblox user matching the specified userId.
    /// </summary>
    /// <param name="userId">The userId to search with.</param>
    /// <returns>A object containing the roblox users advanced info that is set to be visible by them.</returns>
    public async Task<RobloxUser?> GetRobloxUserInfoAsync(long userId) => await _robloxApi.GetUserInfoAsync(userId);

    /// <summary>
    /// Retrieves a list of groups a Roblox user is in matching the specified userId.
    /// </summary>
    /// <param name="userId">The userId to search with.</param>
    /// <returns>A UserGroup list containing the Groups the Roblox user is a member of.</returns>
    public async Task<UserGroup[]?> GetRobloxUserGroupsAsync(long userId) => await _robloxApi.GetUserGroupsAsync(userId);


    //// Trello API calls
    public async Task<Blacklist?> GetTrelloBlacklistAsync() => await _trello.GetTrelloBlacklist();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      var slash = _client.UseSlashCommands(new SlashCommandsConfiguration
      {
        Services = _serviceProvider
      });
      slash.RegisterCommands<SlashCommandsModule>(1346784451455356948);

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
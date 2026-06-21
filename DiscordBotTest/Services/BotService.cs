using DiscordBotTest.PrefixCommands;
using DiscordBotTest.SlashCommands;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBotTest.Services
{
  public class BotService : IHostedService
  {
    private readonly DiscordClient _client;
    private readonly CommandExecutor _executor;
    private readonly DbService _db;
    public readonly GoogleSheetsService _google;
    private readonly RobloxAPIServices _robloxApi;
    private readonly TrelloBlacklistCache _trello;
    private readonly IServiceProvider _serviceProvider;
    public Tuple<string, string>? _owner;
    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> _auth = [];
    public Dictionary<string, string> _blacklist = [];

    public BotService(DiscordClient client, DbService db, GoogleSheetsService google, CommandRegistry registry, RobloxAPIServices robloxApi, TrelloBlacklistCache trello, IServiceProvider serviceProvider)
    {
      _client = client;
      _db = db;
      _google = google;
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
    public async Task<ApiResponse<Guild>?> GetDefaultGroupAsync(ulong groupId) => await _db.CallFunctionWithResponse<Guild>("get_guild_by_group", groupId.ToString());

    /// <summary>
    /// Retrieves a list of a guilds recorded sheets by its guildId.
    /// </summary>
    /// <param name="groupId">The groupId to search with.</param>
    /// <returns>A ApiResponse object containing a list of Sheet objects containing the data of each recorded sheet.</returns>
    public async Task<ApiResponse<Sheet[]>?> GetGroupSheetsAsync(int groupId) => await _db.CallFunctionWithResponse<Sheet[]>("get_guild_sheets", groupId);

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
    /// Gets a list of User records registered as blacklisted in the database.
    /// </summary>
    /// <returns>A ApiResponse object containing a list of User objects containing the blacklisted User records data.</returns>
    public async Task<ApiResponse<User[]>?> GetBotBlacklistAsync() => await _db.CallFunctionWithResponse<User[]>("get_universal_blacklist");

    /// <summary>
    /// Registers a new User record into the Databases GUILD_SPECIFIC_AUTHORIZED_USERS table using the data parameters.
    /// This registers the User as a administrative user granting it access to use specific access-locked commands in the specific guild.
    /// To be used when you want to only grant an individual User administrative access rather everyone with a specific Role.
    /// </summary>
    /// <param name="userName">The name of the User to record.</param>
    /// <param name="userId">The id of the User to record.</param>
    /// <param name="guildId">The id of the Guild the User is to be recorded with.</param>
    /// <returns>A ApiResponse object containing a User object containing the new records data.</returns>
    public async Task<ApiResponse<User>?> PostGuildUserAsync(string userName, string userId, string guildId) => await _db.CallFunctionWithResponse<User>("register_user_with_group", [userName, userId, guildId]);

    /// <summary>
    /// Registers a new User record into the Databases AUTHORIZED_BOT_USERS table using the data parameters.
    /// This registers the specific User as a Admin over the bot allowing them to use otherwise restricted commands.
    /// </summary>
    /// <param name="userName">The name of the User to record.</param>
    /// <param name="userId">The id of the User to record.</param>
    /// <returns>A ApiResponse object containing a User object containing the new records data.</returns>
    public async Task<ApiResponse<User>?> PostAdminUserAsync(string userName, string userId) => await _db.CallFunctionWithResponse<User>("register_admin_user", [userName, userId]);

    /// <summary>
    /// Registers a new User record into the Databases BLACKLISTED_BOT_USERS table using the data parameters.
    /// </summary>
    /// <param name="userName">The name of the User to record.</param>
    /// <param name="userId">The id of the User to record.</param>
    /// <returns>A ApiResponse object containing a User object containing the new records data.</returns>
    public async Task<ApiResponse<User>?> PostBlacklsitedUserAsync(string userName, string userId) => await _db.CallFunctionWithResponse<User>("register_blacklisted_user", [userName, userId]);

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
    /// <param name="guildId">The id of the Group the Role is to be recorded with.</param>
    /// <returns>A ApiResponse object containing a Role object containing the new records data.</returns>
    public async Task<ApiResponse<RoleRecord>?> PostGuildRoleAsync(string roleName, string roleId, string guildId) => await _db.CallFunctionWithResponse<RoleRecord>("register_role_with_group", [roleName, roleId, guildId]);

    /// <summary>
    /// Deletes a recorded User from the Databases AUTHORIZED_BOT_USERS table by the specific userId.
    /// This removes the specific Users Admin privileges over the Bot.
    /// </summary>
    /// <param name="userId">The userId to search for and delete the relating record of.</param>
    /// <returns>A ApiResponse object containing a User object containing the deleted records data.</returns>
    public async Task<ApiResponse<User>?> RemoveAdminUserAsync(string userId) => await _db.CallFunctionWithResponse<User>("remove_admin_user", [userId]);

    /// <summary>
    /// Deletes a recorded User from the Databases BLACKLISTED_BOT_USERS table by the specific userId.
    /// </summary>
    /// <param name="userId">The userId to search for and delete the relating record of.</param>
    /// <returns>A ApiResponse object containing a User object containing the deleted records data.</returns>
    public async Task<ApiResponse<User>?> RemoveBlacklistedUserAsync(string userId) => await _db.CallFunctionWithResponse<User>("remove_blacklisted_user", [userId]);

    /// <summary>
    /// Deletes a recorded User from the Databases GROUP_SPECIFIC_AUTHORIZED_USERS table by the specific userId.
    /// </summary>
    /// <param name="userId">The userId to search for and delete the relating record of.</param>
    /// <returns>A ApiResponse object containing a User object containing the deleted records data.</returns>
    public async Task<ApiResponse<User>?> RemoveGuildUserAsync(string userId) => await _db.CallFunctionWithResponse<User>("remove_group_user", [userId]);


    public async Task<ApiResponse<RoleRecord>?> RemoveGuildRoleAsync(string roleId) => await _db.CallFunctionWithResponse<RoleRecord>("remove_group_role", [roleId]);


    public async Task<ApiResponse<Sheet>?> RemoveGroupSheetAsync(string sheetName) => await _db.CallFunctionWithResponse<Sheet>("remove_group_sheet", [sheetName]);


    public async Task<ApiResponse<GroupRecord[]>?> GetAllRegisteredGroups() => await _db.CallFunctionWithResponse<GroupRecord[]>("get_all_registered_groups");


    public async Task<ApiResponse<User[]>?> GetBotAdmins() => await _db.CallFunctionWithResponse<User[]>("get_universal_admins");


    public async Task AuditAction(string date, string reason, string category, string item) => await _db.CallFunction("audit", [date, reason, category, item]);


    //// Roblox API calls
    /// <summary>
    /// Retrieves basic information for Roblox users matching the specified username.
    /// </summary>
    /// <param name="userName">The username to search for.</param>
    /// <returns>A dictionary mapping user IDs to basic Roblox user information, or null if no users are found.</returns>
    public async Task<Dictionary<string, BasicRobloxUser>?> PostGetRobloxUsersAsync(List<string> userName) => await _robloxApi.GetUserBasicByUsernamesAsync(userName);

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

    /// <summary>
    /// Retrieves a list of friends a Roblox user matching the specified userId has.
    /// </summary>
    /// <param name="userId">The userId to search for.</param>
    /// <returns>A UserFriend list containing the friends the Roblox user has.</returns>
    public async Task<UserFriend[]?> GetRobloxUserFriendsAsync(long userId) => await _robloxApi.GetUserFriendsAsync(userId);

    /// <summary>
    /// Retrieves a list of members in a Roblox group matching the specified groupId.
    /// </summary>
    /// <param name="groupId">The groupId to search with.</param>
    /// <returns>A list containing the members of the Roblox group.</returns>
    public async Task<List<GroupMember>?> GetRobloxGroupMembersAsync(long groupId, string[] roleIds) => await _robloxApi.GetGroupMembersAsync(groupId, roleIds);

    /// <summary>
    /// Retrieves basic information for Roblox users matching the specified userIds.
    /// </summary>
    /// <param name="userIds">An array of userIds to search for.</param>
    /// <returns>A list containing the basic information of the Roblox users.</returns>
    public async Task<List<BasicRobloxUser>?> GetBasicRobloxUsersByIdsAsync(long[] userIds) => await _robloxApi.GetUserBasicByIdsAsync(userIds);

    /// <summary>
    /// Retrieves a list of roles in a Roblox group matching the specified groupId.
    /// </summary>
    /// <param name="groupId">The groupId to search with.</param>
    /// <returns>A list containing the roles of the Roblox group.</returns>
    public async Task<List<GroupRole>?> GetRobloxGroupRolesAsync(long groupId) => await _robloxApi.GetGroupRolesAsync(groupId);


    //// Trello API calls
    public Blacklist? GetTrelloBlacklist() => _trello.GetBlacklist();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      var slash = _client.UseSlashCommands(new SlashCommandsConfiguration
      {
        Services = _serviceProvider
      });
      if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        slash.RegisterCommands<SlashCommandsModule>(1346784451455356948);
      else
        slash.RegisterCommands<SlashCommandsModule>();

      _client.MessageCreated += async (s, e) => await _executor.HandleAsync(e.Message);

      await _client.ConnectAsync();

      var groups = await GetAllRegisteredGroups();
      _owner = new(_client.CurrentApplication.Owners.FirstOrDefault()!.Id.ToString(), _client.CurrentApplication.Owners.FirstOrDefault()!.Username);

      var blacklist = await GetBotBlacklistAsync();

      foreach (var user in blacklist?.Data ?? [])
      {
        _blacklist[user.UserId] = user.Name;
        Logging.DebugLog($"Blacklisted User: {user.Name}({user.UserId})");
      }

      if (groups?.Data is not null)
      {
        _auth["Admin"] = [];
        _auth["Admin"]["0"] = [];
        _auth["Guild"] = [];
        _auth["Guild Owners"] = [];
        var adminUsers = await GetBotAdmins();
        if (adminUsers?.Data is not null)
        {
          foreach (var group in groups.Data)
          {
            _auth["Guild"][group.GroupId] = [];
            _auth["Guild Owners"][group.GroupId] = [];
            Logging.DebugLog($"Group Registration: {group.Name}({group.GroupId})");
            var roles = await GetRolesAsync(group.GroupId);
            if (roles?.Data is not null)
            {
              foreach (var role in roles.Data)
              {
                Logging.DebugLog($"   Role: {role.Name}({role.RoleId})");
                _auth["Guild"][group.GroupId][role.Id.ToString()] = role.Name;
              }
            }
            var users = await GetUsersAsync(group.GroupId);
            if (users?.Data is not null)
            {
              foreach (var user in users.Data)
              {
                Logging.DebugLog($"   User: {user.Name}({user.UserId})");
                _auth["Guild"][group.GroupId][user.UserId] = user.Name;
                if (user.UserId == group.OwnerId)
                  _auth["Guild Owners"][group.GroupId][user.UserId] = user.Name;
              }
            }
          }
          foreach (var admin in adminUsers.Data) _auth["Admin"]["0"][admin.UserId] = admin.Name;
        }
        _auth["Admin"]["0"][_owner.Item1] = _owner.Item2;
      }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await _client.DisconnectAsync();
      _client.Dispose();
      Environment.Exit(0);
    }

    /// <summary>
    /// Whether the specified userId is the owner of the bot application, granting them full administrative privileges over the bot regardless of their status in any specific guild.
    /// </summary>
    /// <param name="id">The userId to check.</param>
    /// <returns>True if the userId is the owner of the bot application, otherwise false.</returns>
    public bool IsOwner(ulong id) => id == _client.CurrentApplication.Owners.FirstOrDefault()!.Id;

    public async Task<bool> IsGuildAuthorized(DiscordUser user, DiscordGuild? guild)
    {
      List<ulong> ids = [];
      var guildId = "0";
      if (guild != null)
      {
        var author = await guild.GetMemberAsync(user.Id);
        ids = [.. author.Roles.Select(r => r.Id)];
        guildId = guild.Id.ToString();
      }
      ids.Add(user.Id);
      foreach (var id in ids)
      {
        if (_auth["Guild"][guildId].ContainsKey(id.ToString())) return true;
        if (_auth["Guild Owners"][guildId].ContainsKey(id.ToString())) return true;
      }
      return false;
    }

    public async Task<bool> IsGuildOwner(DiscordUser user, DiscordGuild? guild)
    {
      if (guild == null) return false;
      var author = await guild.GetMemberAsync(user.Id);
      return _auth["Guild Owners"][guild.Id.ToString()].ContainsKey(author.Id.ToString());
    }

    public bool IsBotAdmin(ulong id) => _auth["Admin"]["0"].ContainsKey(id.ToString());

    public bool IsBotBlacklisted(ulong id) => _blacklist.ContainsKey(id.ToString());

    public DiscordEmbed NotAuthorizedError()
    {
      return new DiscordEmbedBuilder()
        .WithTitle("Auth: Failed")
        .WithDescription("Error: PermissionError: User lacks administrative permissions")
        .WithColor(DiscordColor.DarkRed)
        .Build();
    }

    public DiscordEmbed BlacklistedError()
    {
      return new DiscordEmbedBuilder()
        .WithTitle("Auth: Failed")
        .WithDescription("Error: PermissionError: User is blacklisted from using the bot.")
        .WithColor(DiscordColor.DarkRed)
        .Build();
    }
  }
}
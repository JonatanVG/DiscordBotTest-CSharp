using DiscordBotTest.Services;
using DSharpPlus.Entities;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

#nullable disable
namespace DiscordBotTest
{
  public interface IPrefixCommand
  {
    string Name { get; }
    string[] Aliases { get; }
    Task ExecuteAsync(BotService c, DiscordMessage m, string[] args);
  }

  public class ApiResponse<T>
  {
    public bool Success { get; set; }
    public string Message { get; set; }
    #nullable enable
    public T? Data { get; set; }
    #nullable disable
  }

  public sealed record Blacklist(
    Dictionary<string, BlacklistEntry> List
  );

  public sealed record BlacklistEntry(
    string Name,
    BlacklistStatus Status
  );

  public sealed record BlacklistStatus(
    string Status,
    string Suffix
  );

  public sealed record BGCResult()
  {
    public List<DiscordEmbed> Embeds { get; init; } = [];
    public List<(string Name, Stream Stream)> Files { get; init; } = [];
  };

  public sealed record UserFriend(
    [property: JsonPropertyName("isOnline")] bool IsOnline = false,
    [property: JsonPropertyName("presenceType")] int PresenceType = 0,
    [property: JsonPropertyName("isDeleted")] bool IsDeleted = false,
    [property: JsonPropertyName("friendFrequentScore")] int FrequentScore = 0,
    [property: JsonPropertyName("friendFrequentRank")] int FrequentRank = 0,
    [property: JsonPropertyName("hasVerifiedBadge")] bool IsVerified = false,
    [property: JsonPropertyName("description")] string Desc = null,
    [property: JsonPropertyName("created")] DateTimeOffset? CreatedAt = null,
    [property: JsonPropertyName("isBanned")] bool Banned = false,
    [property: JsonPropertyName("externalAppDisplayName")] string AppDisplayName = null,
    [property: JsonPropertyName("id")] long Id = 0,
    [property: JsonPropertyName("name")] string Name = null,
    [property: JsonPropertyName("displayName")] string DisplayName = null
  );

  public sealed record InventoryResponse(
    [property: JsonPropertyName("inventoryItems")] InventoryItem[] Data,
    [property: JsonPropertyName("nextPageToken")] string PageToken
  );

  public sealed record InventoryItem(
    [property: JsonPropertyName("path")] string Path,
    #nullable enable
    [property: JsonPropertyName("assetDetails")] AssetDetails? AssetDetails,
    [property: JsonPropertyName("badgeDetails")] BadgeDetails? BadgeDetails,
    [property: JsonPropertyName("gamePassDetails")] GamePassDetails? GamePassDetails,
    [property: JsonPropertyName("privateServerDetails")] PrivateServerDetails? PrivateServerDetails,
    #nullable disable
    [property: JsonPropertyName("addTime")] DateTimeOffset AddTime
  );

  public sealed record BadgeDetails(
    [property: JsonPropertyName("badgeId")] string BadgeId
  );

  public sealed record GamePassDetails(
    [property: JsonPropertyName("gamePassId")] string GamePassId
  );

  public sealed record PrivateServerDetails(
    [property: JsonPropertyName("privateServerId")] string PrivateServerId
  );

  public sealed record AssetDetails(
    [property: JsonPropertyName("assetId")] string AssetId,
    [property: JsonPropertyName("inventoryItemAssetType")]  string AssetType,
    [property: JsonPropertyName("instanceId")] string InstanceId,
    #nullable enable
    [property: JsonPropertyName("collectibleDetails")] CollectibleDetails? CollectibleDetails
    #nullable disable
  );

  public sealed record CollectibleDetails(
    [property: JsonPropertyName("itemId")] string ItemId,
    [property: JsonPropertyName("instanceId")] string InstanceId,
    [property: JsonPropertyName("instanceState")] string State,
    [property: JsonPropertyName("serialNumber")]  int SerialNumber
  );

  public sealed record BasicRobloxUser(
    [property: JsonPropertyName("requestedUsername")] string UserName,
    [property: JsonPropertyName("hasVerifiedBadge")] bool IsVerified,
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("displayName")] string DisplayName
  );

  public sealed record RobloxUser(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("createTime")] string CreateTime,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("displayName")] string DisplayName,
    [property: JsonPropertyName("about")] string About,
    [property: JsonPropertyName("locale")] string Locale,
    #nullable enable
    [property: JsonPropertyName("premium")] bool? IsPremium = false,
    [property: JsonPropertyName("idVerified")] bool? IsIdVerified = false,
    [property: JsonPropertyName("socialNetworkProfiles")] SocialNetworkProfiles? SocialNetworkProfiles = null
    #nullable disable
  );

  #nullable enable
  public sealed record SocialNetworkProfiles(
    [property: JsonPropertyName("facebook")] string? Facebook,
    [property: JsonPropertyName("twitter")] string? Twitter,
    [property: JsonPropertyName("youtube")] string? YouTube,
    [property: JsonPropertyName("twitch")] string? Twitch,
    [property: JsonPropertyName("guilded")] string? Guilded,
    [property: JsonPropertyName("visibility")] string? Visibility
  );
  #nullable disable

  public sealed record User(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("USERNAME")] string Name,
    [property: JsonPropertyName("USER_ID")]  string UserId,
    #nullable enable
    [property: JsonPropertyName("GROUP_ID")] string? GroupId,
    #nullable disable
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt
  );

  public sealed record RoleRecord(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("ROLE_NAME")] string Name,
    [property: JsonPropertyName("ROLE_ID")] string RoleId,
    [property: JsonPropertyName("GROUP_ID")]  string GroupId,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt
  );

  public sealed record UserGroup(
    [property: JsonPropertyName("group")] Group Group,
    [property: JsonPropertyName("role")] GroupRole Role,
    [property: JsonPropertyName("isPrimaryGroup")] bool UserPrimary = false
  );

  public sealed record Group(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Desc,
    [property: JsonPropertyName("owner")] GroupUser Owner,
    #nullable enable
    [property: JsonPropertyName("shout")] GroupShout? Shout,
    #nullable disable
    [property: JsonPropertyName("memberCount")] long MemberCount,
    [property: JsonPropertyName("isBuildersClubOnly")] bool ClubOnly,
    [property: JsonPropertyName("publicEntryAllowed")] bool PublicEntry,
    [property: JsonPropertyName("isLocked")] bool IsLocked,
    [property: JsonPropertyName("hasVerifiedBadge")] bool IsVerified,
    [property: JsonPropertyName("hasSocialModules")] bool HasSocials
  );

  public sealed record GroupUser(
    [property: JsonPropertyName("buildersClubMembershipType")] int? ClubType,
    [property: JsonPropertyName("hasVerifiedBadge")] bool Verified,
    [property: JsonPropertyName("userId")] long Id,
    [property: JsonPropertyName("username")] string Name,
    [property: JsonPropertyName("displayName")] string DisplayName
  );

  public sealed record GroupShout(
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("poster")] GroupUser Poster,
    [property: JsonPropertyName("created")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated")] DateTimeOffset? UpdatedAt = null
  );

  public sealed record GroupRecord(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("GROUP_NAME")] string Name,
    [property: JsonPropertyName("GROUP_ID")] string GroupId,
    [property: JsonPropertyName("OWNER_ID")] string OwnerId,
    [property: JsonPropertyName("OWNER_NAME")] string OwnerName,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt
  );

  public sealed record GroupRole(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Desc = "",
    [property: JsonPropertyName("rank")] int Rank = 0,
    [property: JsonPropertyName("memberCount")] long HasRoleCount = 0,
    [property: JsonPropertyName("isBase")] bool Base = false,
    [property: JsonPropertyName("color")] int Color = 0
  );

  public sealed record Guild(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("IGNORE")] List<string> IgnoreRoles,
    [property: JsonPropertyName("GUILD_ID")] long GuildId,
    [property: JsonPropertyName("GUILD_NAME")] string GuildName,
    [property: JsonPropertyName("MAIN_GROUP")] string MainGroup,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("GUILD_SHEET_ID")] string GoogleSheetId
  );

  public sealed record Sheet(
     [property: JsonPropertyName("id")] int Id,
     [property: JsonPropertyName("SHEET_NAME")] string Name,
     [property: JsonPropertyName("SHEET_ID")] string SheetId,
     [property: JsonPropertyName("GUILD_ID")] int GuildId,
     [property: JsonPropertyName("SHEET_RANGE")] string Range,
     [property: JsonPropertyName("START_ROW")] int Row,
     [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt
  );
}

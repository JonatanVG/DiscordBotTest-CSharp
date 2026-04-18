using DiscordBotTest.Services;
using DSharpPlus.Entities;
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
    public T Data { get; set; }
  }

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

  public sealed record Role(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("ROLE_NAME")] string Name,
    [property: JsonPropertyName("ROLE_ID")] string RoleId,
    [property: JsonPropertyName("GROUP_ID")]  string GroupId,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt
  );

  public sealed record Group(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("GROUP_NAME")] string Name,
    [property: JsonPropertyName("GROUP_ID")] string GroupId,
    [property: JsonPropertyName("OWNER_ID")] string OwnerId,
    [property: JsonPropertyName("OWNER_NAME")] string OwnerName,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt
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

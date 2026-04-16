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

  public class User
  {
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("USERNAME")]
    public string Name { get; set; }
    [JsonPropertyName("USER_ID")]
    public string UserId { get; set; }
    [JsonPropertyName("GROUP_ID")]
    public string GroupId { get; set; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
  }

  public class Role
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("ROLE_NAME")]
    public string Name { get; set; }
    [JsonPropertyName("ROLE_ID")]
    public string RoleId { get; set; }
    [JsonPropertyName("GROUP_ID")]
    public string GroupId { get; set; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
  }

  public class Group
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("GROUP_NAME")]
    public string Name { get; set; }
    [JsonPropertyName("GROUP_ID")]
    public string GroupId { get; set; }
    [JsonPropertyName("OWNER_ID")]
    public string OwnerId { get; set; }
    [JsonPropertyName("OWNER_NAME")]
    public string OwnerName { get; set; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
  }

  public class Guild
  {
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("IGNORE")]
    public List<string> IgnoreRoles { get; set; }
    [JsonPropertyName("GUILD_ID")]
    public long GuildId { get; set; }
    [JsonPropertyName("GUILD_NAME")]
    public string GuildName { get; set; }
    [JsonPropertyName("MAIN_GROUP")]
    public string MainGroup { get; set; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("GUILD_SHEET_ID")]
    public string GoogleSheetId { get; set; }
  }

  public class Sheet
  {
     [JsonPropertyName("id")]
     public int Id { get; set; }
     [JsonPropertyName("SHEET_NAME")]
     public string Name { get; set; }
     [JsonPropertyName("SHEET_ID")]
     public string SheetId { get; set; }
     [JsonPropertyName("GUILD_ID")]
     public int GuildId { get; set; }
     [JsonPropertyName("SHEET_RANGE")]
     public string Range { get; set; }
     [JsonPropertyName("START_ROW")]
     public int Row { get; set; }
     [JsonPropertyName("created_at")]
     public DateTimeOffset CreatedAt { get; set; }
  }
}

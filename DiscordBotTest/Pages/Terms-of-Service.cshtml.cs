using DiscordBotTest.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiscordBotTest.Pages
{
  public class Terms_of_ServiceModel : PageModel
  {
    private readonly DbService _db;
    public WebsiteItem[] Items { get; set; } = [];
    public List<WebsitePage> Pages { get; set; } = [];

    public Terms_of_ServiceModel(DbService db)
    {
      _db = db;
    }

    public async Task OnGetAsync()
    {
      var response = await _db.CallFunctionWithRows<WebsiteItem>("get_website_items", [2]);
      Items = [.. response!];
      Pages = await _db.CallFunctionWithRows<WebsitePage>("get_website_pages", []) ?? [];
      ViewData["Pages"] = Pages;
    }
  }
}
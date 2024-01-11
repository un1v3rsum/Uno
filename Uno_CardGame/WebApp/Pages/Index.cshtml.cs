using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _ctx;
    //NO FUNNY BUSINESS ON HOME PAGE
    public IndexModel(ILogger<IndexModel> logger, AppDbContext ctx)
    {
        _ctx = ctx;
    }
    

    public void OnGet()
    {
    }
}
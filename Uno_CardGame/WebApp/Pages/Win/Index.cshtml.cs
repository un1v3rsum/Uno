using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UnoEngine;

namespace WebApp.Pages.Win;

public class Index : PageModel
{
    private readonly DAL.AppDbContext _context;
    private readonly IGameRepository _gameRepository = default!;
    public GameEngine GameEngine { get; set; } = default!;

    public Index(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEF(_context);
    }
    //attribute to get the Id values
    [BindProperty(SupportsGet = true)]public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)]public Guid PlayerId { get; set; }
    
    public void OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        GameEngine = new GameEngine(_gameRepository)
        {
            State = gameState
        };
    }
}
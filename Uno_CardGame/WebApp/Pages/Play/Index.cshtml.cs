using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UnoEngine;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly DAL.AppDbContext _context;
    private readonly IGameRepository _gameRepository = default!;
    public GameEngine GameEngine { get; set; } = default!;

    public Index(AppDbContext context)
    {
        _context = context;
        _gameRepository = new GameRepositoryEf(_context);
    }
    //attribute to get the Id values
    [BindProperty(SupportsGet = true)]public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)]public Guid PlayerId { get; set; }
    [BindProperty] public int SelectedCardIndex { get; set; }
    [BindProperty]public string SelectedCardColor { get; set; } 
    [BindProperty]public bool ColorSelection { get; set; } 
    [BindProperty]public bool CardPlayed { get; set; } 
    [BindProperty]public bool Uno { get; set; } 
    [BindProperty]public bool DrawCard { get; set; } 
    [BindProperty]public bool EndMove { get; set; } 
    
    public void OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        GameEngine = new GameEngine(_gameRepository)
        {
            State = gameState
        };
        
        DrawCard = false;
        ColorSelection = false;
        CardPlayed = false;
        Uno = false;
        EndMove = false;

        if (GameEngine.GetActivePlayer().PlayerType == EPlayerType.Ai)
        {
            GameEngine.AiMove();

            if (!GameEngine.IsHandFinished())
            {
                GameEngine.NextPlayer();
            }
        }
        _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);
        
        if (!GameEngine.IsGameOver())
        {
            GameEngine.UpdateGame();
        }

    }

    public void OnPost()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        
        GameEngine = new GameEngine(_gameRepository)
        {
            State = gameState
        };
                
        if (DrawCard)
        {
            GameEngine.DrewCard(1);
        }
        
        if (CardPlayed)
        {
            GameEngine.PlayCard(SelectedCardIndex);
            if (DrawCard)
            {
                DrawCard = false;
            }
            CardPlayed = false;
            
            if (GameEngine.State.DiscardedCards.Last().CardColor != ECardColor.Wild)
            {
                GameEngine.NextPlayer();
            }
        }
        
        if (ColorSelection)
        {
            GameEngine.DeclareColor(SelectedCardColor);
            ColorSelection = false;
            GameEngine.NextPlayer();
        }
        
        if (EndMove)
        {
            EndMove = false;
            GameEngine.NextPlayer();
        }
        
        _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);

    }
}
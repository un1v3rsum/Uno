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
        _gameRepository = new GameRepositoryEF(_context);
        
    }
    //attribute to get the Id values
    [BindProperty(SupportsGet = true)]public Guid GameId { get; set; }
    [BindProperty(SupportsGet = true)]public Guid PlayerId { get; set; }
    [BindProperty] public int SelectedCardIndex { get; set; } = default!;
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
        
        Console.WriteLine("turnresult: " + GameEngine.State.TurnResult);
        
        DrawCard = false;
        ColorSelection = false;
        CardPlayed = false;
        Uno = false;
        EndMove = false;
        
        if (GameEngine.GetActivePlayer().PlayerType == EPlayerType.Ai && !GameEngine.GameDone)
        {
            Console.WriteLine("do we get to AIMove?");
            GameEngine.AiMove();
            if (GameEngine.State.DiscardedCards.Last().CardColor == ECardColor.Wild)
            {
                var random = new Random();
                var colorChoice = random.Next(1, 5).ToString();
                GameEngine.DeclareColor(colorChoice);
            }
        
            if (!GameEngine.IsHandFinished())
            {
                GameEngine.NextPlayer();
            }
            else
            {
                if (!GameEngine.IsGameOver())
                {
                    Console.WriteLine("do we update game?");
                    GameEngine.UpdateGame();
                }
                else
                {
                    GameEngine.GameDone = true;
                }
            }
        }
        GameEngine.State.TurnResult = ETurnResult.OnGoing;
        _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);
    }

    public IActionResult OnPost()
    {
        var gameState = _gameRepository.LoadGame(GameId);
        GameEngine = new GameEngine(_gameRepository)
        {
            State = gameState
        };
        Console.WriteLine("turnresult: " + GameEngine.State.TurnResult);

        if (DrawCard)
        {
            GameEngine.DrewCard(1);
        }
        
        if (CardPlayed)
        {
            Console.WriteLine("OnPost() CardPlayed who is here: " + GameEngine.GetActivePlayer().NickName);
            GameEngine.PlayCard(SelectedCardIndex);
        
            if (GameEngine.State.DiscardedCards.Last().CardColor != ECardColor.Wild)
            {
                GameEngine.NextPlayer();
            }
            Console.WriteLine("what is CardPlayed value: " + CardPlayed);
        }
    
        if (ColorSelection)
        {
            GameEngine.DeclareColor(SelectedCardColor);
            GameEngine.NextPlayer();
            Console.WriteLine("what is colorselection boolean value: " + ColorSelection);
        }
        
        
        if (GameEngine.IsHandFinished())
        {
            if (!GameEngine.IsGameOver())
            {
                Console.WriteLine("do we update game?");
                GameEngine.UpdateGame();
            }
            else
            {
                GameEngine.GameDone = true;
            }
        }

        _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);
        return Redirect("/Play?GameId=" + GameEngine.State.Id + "&PlayerId=" + PlayerId);
    }
}
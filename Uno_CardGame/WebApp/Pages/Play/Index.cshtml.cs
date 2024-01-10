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
        
        Console.WriteLine("turnresult: " + GameEngine.State.TurnResult);
        
        DrawCard = false;
        ColorSelection = false;
        CardPlayed = false;
        Uno = false;
        EndMove = false;

        if (!GameEngine.IsHandFinished())
        { 
            //<<<<<================ IF PREVIOUS CARDS WERE ACTION CARDS ===============================>>>>>
                
        //<<<<<================ SKIP ===================>>>>>
        if (GameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Skip)
        {
            //if previous player made the move and didn't draw a card, then next player is skipped
            // & it's not a loadgame
            if (GameEngine.State.TurnResult != ETurnResult.DrewCard && 
                GameEngine.State.TurnResult != ETurnResult.LoadGame)
            {
                //move on to next player
                GameEngine.NextPlayer();
            }
        }
                //<<<<<================ REVERSE ===================>>>>>
        if (GameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Reverse)
        {
            //if previous player made the move and didn't draw a card and next player in line is changed
            if (GameEngine.State.TurnResult != ETurnResult.DrewCard && 
                GameEngine.State.TurnResult != ETurnResult.LoadGame)
            {
                //then gameDirection is reversed
                GameEngine.SetGameDirection();
                //and next player is picked
                //since PlayerMove() already went over to next player before gamedirection was set, 
                //then we go back to the player before Reverse card player
                GameEngine.NextPlayer();
                GameEngine.NextPlayer();
            }
        }
        //<<<<<================ DRAW-TWO ===================>>>>>
        if (GameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawTwo)
        {
            //if previous player made the move and didn't draw a card
            if (GameEngine.State.TurnResult != ETurnResult.DrewCard && 
                GameEngine.State.TurnResult != ETurnResult.LoadGame)
            {
                GameEngine.DrewCard(2);
            }
        }
        
                //<<<<<================ WILD + DRAW FOUR ===================>>>>>
        if (GameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawFour)
        {
            //if it is the first draw of the game
            if (GameEngine.State.TurnResult == ETurnResult.GameStart)
            {
                //first card in discardpile is put back into the deck 
                GameEngine
                    .State
                    .CardDeck
                    .Cards.Add(
                        GameEngine
                            .State
                            .DiscardedCards.First());
                
                //remove the card from discardpile
                GameEngine
                    .State
                    .DiscardedCards.RemoveAt(0);
                
                //initialize new discard pile
                GameEngine.InitializeDiscardPile();
            }
            //if it is not a draw card or the beginning of the loadGame
            if (GameEngine.State.TurnResult != ETurnResult.DrewCard && 
                GameEngine.State.TurnResult != ETurnResult.LoadGame)
            {
                //and draws 4 cards
                GameEngine.DrewCard(4);
            }
        }
        //<<<<<================ AI MOVE ===================>>>>>
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

            if (GameEngine.IsHandFinished())
            {
                if (GameEngine.IsGameOver())
                {
                    GameEngine.GameDone = true;
                }
                else
                {
                    Console.WriteLine("do we update game?");
                    GameEngine.UpdateGame();
                }
            }
        }
        GameEngine.State.TurnResult = ETurnResult.OnGoing;
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
            Console.WriteLine("what is CardPlayed value: " + CardPlayed);
        }
    
        if (ColorSelection)
        {
            GameEngine.DeclareColor(SelectedCardColor);
            GameEngine.NextPlayer();
            Console.WriteLine("what is colorselection boolean value: " + ColorSelection);
        }

        _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);
        return Redirect("/Play?GameId=" + GameEngine.State.Id + "&PlayerId=" + PlayerId);
    }
}
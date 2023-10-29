using Domain;
using UnoEngine;

namespace UnoConsoleUI;

public class GameController<TKey>
{
    private readonly GameEngine<TKey> _gameEngine;
    public GameController(GameEngine<TKey> gameEngine)
    {
        _gameEngine = gameEngine;
    }
    public string? MainLoop()
    {
        //@start of the game clears the console from prev menusystem
        Console.Clear();
        Console.WriteLine(_gameEngine.State.TurnResult == ETurnResult.LoadGame
        //loading previous game
            ? "<======== LOADING PREVIOUS GAME ========>"
            //new game
            : "<============ NEW GAME ============>");
        //first level loop for the game
        while (!_gameEngine.GameDone)
        {
            if (_gameEngine.State.TurnResult != ETurnResult.LoadGame)
            {
                //loading new hand
                Console.WriteLine("<======== STARTING NEW HAND =======>");
                Console.WriteLine("First card in discard-pile: " + 
                                  _gameEngine.State.DiscardedCards.First());
            }
            else
            {
                //loading previous game from load game
                Console.WriteLine("<====== RELOADING PREVIOUS HAND =====>");
            }
            //second level loop for the hand - remains true if one of the next attributes is false
            while (_gameEngine is { HandDone: false, GameDone: false })
            {
               
                //if the card on top of the discardpile is SKIP
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Skip)
                {
                    //if previous player made the move and didn't draw a card, then next player is skipped
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard)
                    {
                        //if previous player made the move and didn't draw a card and next player in line is changed
                        Console.WriteLine("Last card played was SKIP.");
                        Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                          + $"misses his turn! ");
                        _gameEngine.NextPlayer();
                    }
                    //otherwise the active player makes their move
                    _gameEngine.PlayerMove();
                }
                //if the card on top of the discardpile is REVERSE
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Reverse)
                {
                    //if previous player made the move and didn't draw a card and next player in line is changed
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard)
                    {
                        //then gameDirection is reversed
                        Console.WriteLine("Last card played was REVERSE. Direction will be set to counterclockwise!");
                        _gameEngine.SetGameDirection();
                        _gameEngine.NextPlayer();
                    }
                    //active player makes a move
                    _gameEngine.PlayerMove();
                }
                //if the card on top of the discardpile is DRAWTWO
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawTwo)
                {
                    //if previous player made the move and didn't draw a card
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard)
                    {
                        //then next player takes two cards and is skipped
                        Console.WriteLine("Last card played was DRAW-TWO.");
                        Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                          + $" takes 2 cards and misses his turn! ");
                        _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].
                            PlayerHand.AddRange(_gameEngine.State.CardDeck.Draw(2));
                        _gameEngine.State.TurnResult = ETurnResult.DrewCard;
                        _gameEngine.NextPlayer();
                    }
                    //active player makes a move
                    _gameEngine.PlayerMove();
                }
                //if the card on top of the discardpile is REGULAR WILD CARD
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Wild)
                {
                    //if it is the start of the game, then first player picks the color & makes their move
                    if (_gameEngine.State.TurnResult == ETurnResult.GameStart)
                    {
                        _gameEngine.State.TurnResult = ETurnResult.OnGoing;
                        _gameEngine.DeclareColor();
                    }
                    //if it is an ongoing game then color declaration was done by the previous player
                    _gameEngine.PlayerMove();
                }
                //if the card on top of the discardpile is WILD DRAW FOUR
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawFour)
                {
                    //if it is the first draw of the game
                    if (_gameEngine.State.TurnResult == ETurnResult.GameStart)
                    {
                        //card is put back into the deck 
                        Console.WriteLine("FIRST DRAW IN THE GAME, CARD IS PUT BACK TO THE DECK!");
                        _gameEngine.State.CardDeck.Cards.Add(_gameEngine.State.DiscardedCards.First());
                        _gameEngine.State.DiscardedCards.RemoveAt(0);
                        _gameEngine.InitializeDiscardPile();
                    }
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard && 
                        _gameEngine.State.TurnResult != ETurnResult.GameStart)
                    {
                        //if previous player made this move then next player takes 4 cards and is skipped
                        Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].NickName } " 
                                          + $" takes 4 cards and misses his turn! ");
                        _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].
                            PlayerHand.AddRange(_gameEngine.State.CardDeck.Draw(4));
                        _gameEngine.State.TurnResult = ETurnResult.DrewCard;
                        _gameEngine.NextPlayer();
                    }
                    //if previous player drew a card then next player makes their move
                    _gameEngine.PlayerMove();
                }
                //if it is a number card
                if (_gameEngine.State.DiscardedCards.Last().CardValue != ECardValues.Wild 
                    && _gameEngine.State.DiscardedCards.Last().CardValue != ECardValues.DrawFour 
                    && _gameEngine.State.DiscardedCards.Last().CardValue != ECardValues.DrawTwo 
                    && _gameEngine.State.DiscardedCards.Last().CardValue != ECardValues.Reverse 
                    && _gameEngine.State.DiscardedCards.Last().CardValue != ECardValues.Skip
                    && !_gameEngine.IsHandFinished())
                {
                    //modify turnResult and make a move
                    _gameEngine.State.TurnResult = ETurnResult.OnGoing;
                    _gameEngine.PlayerMove();
                }
                //TODO new hand and game generation
                _gameEngine.HandDone = _gameEngine.IsHandFinished();
                _gameEngine.GameDone = _gameEngine.IsGameOver();

            }
            //check if gameOver
            if (_gameEngine.GameDone)
            {
                Console.WriteLine("Game is closing!");
                break;
            }
        }
        return null;
    }
}
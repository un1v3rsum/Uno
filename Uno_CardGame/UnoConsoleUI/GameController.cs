using DAL;
using Domain;
using UnoEngine;

namespace UnoConsoleUI;

public class GameController
{
    //declare gamecontroller attributes (engine & repo)
    private readonly GameEngine _gameEngine;
    private readonly IGameRepository _gameRepository;
    //constructor takes 2 parameters
    public GameController(GameEngine gameEngine, IGameRepository gameRepository)
    {
        _gameEngine = gameEngine;
        _gameRepository = gameRepository;
    }
    //game mainloop
    public string? MainLoop()
    {
        //<<<<<================ VISUALIZE START OF GAME ===================>>>>>
        ConsoleVizualisation.StartGame(_gameEngine.State);
        
        //first level loop for the game
        while (!_gameEngine.GameDone)
        {
            //<<<<<================ VISUALIZE START OF HAND ===================>>>>>
            if (_gameEngine.State.TurnResult != ETurnResult.LoadGame)
            {
                //update all the cards and player hands
                _gameEngine.UpdateGame();
            }
            
            //start game hand, waiter players confirmation
            ConsoleVizualisation.StartHand(_gameEngine.State);
            Console.WriteLine("Press any key to continue..");
            Console.ReadLine();
            
            //second level loop for the hand - remains true if one of the next attributes is false
            while (!_gameEngine.HandDone)
            {
                //clear console, so that other players wouldn't see previous players cards on hand
                Console.Clear();
                
                //<<<<<================ IF PREVIOUS CARDS WERE ACTION CARDS ===============================>>>>>
                
                //<<<<<================ SKIP ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Skip)
                {
                    //if previous player made the move and didn't draw a card, then next player is skipped
                    // & it's not a loadgame
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard && 
                        _gameEngine.State.TurnResult != ETurnResult.LoadGame)
                    {
                        //if previous player made the move and didn't draw a card and next player in line is changed
                        Console.WriteLine("Last card played was SKIP.");
                        Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo]
                                                            .NickName } " + $"misses his turn! ");
                        _gameEngine.NextPlayer();
                    }
                }
                //<<<<<================ REVERSE ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Reverse)
                {
                    //if previous player made the move and didn't draw a card and next player in line is changed
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard && 
                        _gameEngine.State.TurnResult != ETurnResult.LoadGame)
                    {
                        //then gameDirection is reversed
                        _gameEngine.SetGameDirection();
                        //and next player is picked
                        //since PlayerMove() already went over to next player before gamedirection was set, 
                        //then we go back to the player before Reverse card player
                        _gameEngine.NextPlayer();
                        _gameEngine.NextPlayer();
                    }
                }
                //<<<<<================ DRAW-TWO ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawTwo)
                {
                    //if previous player made the move and didn't draw a card
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard && 
                        _gameEngine.State.TurnResult != ETurnResult.LoadGame)
                    {
                        //then next player takes two cards and is skipped
                        Console.WriteLine("Last card played was DRAW-TWO.");
                        Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo]
                                                        .NickName} " + $"misses his turn! ");
                        _gameEngine.DrewCard(2);
                        _gameEngine.NextPlayer();
                    }
                }
                //<<<<<================ WILD ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.Wild)
                {
                    //if it is the start of the game, then first player picks the color & makes their move
                    if (_gameEngine.State.TurnResult == ETurnResult.GameStart)
                    {
                        _gameEngine.DeclareColor();
                    }
                    //if it is an ongoing game then color declaration was already done by the previous player
                }
                //<<<<<================ WILD + DRAW FOUR ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawFour)
                {
                    //if it is the first draw of the game
                    if (_gameEngine.State.TurnResult == ETurnResult.GameStart)
                    {
                        //card is put back into the deck 
                        Console.WriteLine("FIRST DRAW IN THE GAME, CARD IS PUT BACK TO THE DECK!");
                        _gameEngine.State.CardDeck.Cards.Add(_gameEngine.State.DiscardedCards.First());
                        _gameEngine.State.DiscardedCards.RemoveAt(0);
                        //initialize new discard pile
                        _gameEngine.InitializeDiscardPile();
                    }
                    //if previous player made this move then next player takes 4 cards and is skipped
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard && 
                        _gameEngine.State.TurnResult != ETurnResult.LoadGame)
                    {
                        Console.WriteLine("Last card played was WILD-DRAW-FOUR.");
                        Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo]
                                                .NickName} " + $"misses his turn! ");
                        _gameEngine.DrewCard(4);
                        _gameEngine.NextPlayer();
                    }
                }
                //<<<<<================ IF PREVIOUS CARDS WERE ACTION CARDS ===============================>>>>>
                
                
                //next players turn, wait for confirmation
                Console.WriteLine($"{_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo]
                    .NickName} 's " + $"turn! Press any key to continue..");
                Console.ReadLine();
                Console.Clear();
                
                //vizualise next players hand
                ConsoleVizualisation.StartPlayerMove(_gameEngine.State);
                
                //define active players handsize
                var handSize = _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].PlayerHand.Count;
                //boolean for loop
                var correctInput = false;
                var choice = "";
                
                //<<<<<================ AI PLAYER MAKES A CHOICE ===================>>>>>
                if (_gameEngine.State.Players[_gameEngine.State.ActivePlayerNo].PlayerType == EPlayerType.AI)
                {
                    //choice is made by AI
                    choice = _gameEngine.AIMove();
                }
                //<<<<<================ HUMAN PLAYER MAKES A CHOICE ===================>>>>>
                else
                {
                    //in a loop
                    do
                    {
                        //read in the choice from console
                        choice = Console.ReadLine()!.ToLower().Trim();
                        //if player wants to draw or quit then break the loop
                        if (choice is "d" or "q")
                        {
                            correctInput = true;
                        }
                        else // otherwise check if the pressed key was a number from 1 to the number of cards on hand
                        {
                            if (int.TryParse(choice, out var position) && position > 0 && position <= handSize)
                            {
                                //check if it is a legal move and break the loop
                                if (_gameEngine.CheckValidity(_gameEngine.State.DiscardedCards.Last(),
                                        _gameEngine.State.Players[_gameEngine.State.ActivePlayerNo]
                                            .PlayerHand[position - 1]))
                                {
                                    correctInput = true;
                                }
                                else //not a valid card, continue looping
                                {
                                    Console.WriteLine("This card cant be played!");
                                }
                            }
                            else //if choice is not a valid number from 1 to the number of cards on hand
                            {
                                Console.WriteLine("Undefined shortcut (playermove)!");
                                //and continue looping
                            }
                        }
                    } while (!correctInput);
                }
                //<<<<<================ PLAYER MAKES AN ACTUAL MOVE ===================>>>>>
                _gameEngine.MakeAMove(choice);
                
                //<<<<<================ SAVE GAME AFTER EVERY MOVE ===================>>>>>
                _gameRepository.SaveGame(_gameEngine.State.Id, _gameEngine.State);
            }
        }
        return null;
    }
}
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
                
                //<<<<<================ IF PLAYER DREW A CARD ===================>>>>>
                if (_gameEngine.State.TurnResult == ETurnResult.DrewCard)
                {
                    //draws one card, checks its validity and plays if allowed
                    _gameEngine.DrewCard(1);
                }
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
                        //active player misses their turn
                        Console.WriteLine($"{
                            _gameEngine
                                .State
                                .Players[_gameEngine.State.ActivePlayerNo]
                                .NickName } " + $"misses his turn! ");
                        //move on to next player
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
                        //active player misses their turn
                        Console.WriteLine($"{
                            _gameEngine
                                .State
                                .Players[_gameEngine.State.ActivePlayerNo]
                                .NickName} " + $"misses his turn! ");
                        
                        _gameEngine.DrewCard(2);
                    }
                }
                //<<<<<================ WILD ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardColor == ECardColor.Wild)
                {
                    //get color input from player
                    var colorChoice = "";
                    var input = false;
                    do
                    {
                        ConsoleVizualisation.ShowDeclaringAColor(_gameEngine.State);
                        //if human declares a color
                        if (_gameEngine
                                .State
                                .Players[_gameEngine.State.ActivePlayerNo]
                                .PlayerType == EPlayerType.Human)
                        {
                            //read in the choice from console
                            colorChoice = Console.ReadLine()!.ToLower().Trim();
                        }
                        //if AI declares a color
                        else
                        {
                            //picks a random color
                            Random random = new Random();
                            colorChoice = random.Next(1, 5).ToString();
                        }

                        if (colorChoice is "1" or "2" or "3" or "4"  )
                        {
                            input = true;
                        }
                        else
                        {
                            Console.WriteLine("Undefined shortcut (declare color)!");
                        }
                        
                    } while (input == false);
                    //declare the color
                    _gameEngine.DeclareColor(colorChoice);
                    ConsoleVizualisation.ShowNewDeclaredColor(_gameEngine.State);
                    //move on to next player
                    _gameEngine.NextPlayer();
                }
                //<<<<<================ WILD + DRAW FOUR ===================>>>>>
                if (_gameEngine.State.DiscardedCards.Last().CardValue == ECardValues.DrawFour)
                {
                    //if it is the first draw of the game
                    if (_gameEngine.State.TurnResult == ETurnResult.GameStart)
                    {
                        Console.WriteLine("FIRST DRAW IN THE GAME, CARD IS PUT BACK TO THE DECK!");
                        //first card in discardpile is put back into the deck 
                        _gameEngine
                            .State
                            .CardDeck
                            .Cards.Add(
                                _gameEngine
                                    .State
                                    .DiscardedCards.First());
                        
                        //remove the card from discardpile
                        _gameEngine
                            .State
                            .DiscardedCards.RemoveAt(0);
                        
                        //initialize new discard pile
                        _gameEngine.InitializeDiscardPile();
                    }
                    //if it is not a draw card or the beginning of the loadGame
                    if (_gameEngine.State.TurnResult != ETurnResult.DrewCard && 
                        _gameEngine.State.TurnResult != ETurnResult.LoadGame)
                    {
                        //active player misses their turn
                        Console.WriteLine("Last card played was WILD-DRAW-FOUR.");
                        Console.WriteLine($"{
                            _gameEngine
                                .State
                                .Players[_gameEngine.State.ActivePlayerNo]
                                .NickName} " + $"misses his turn! ");
                        //and draws 4 cards
                        _gameEngine.DrewCard(4);
                    }
                }
                //<<<<<================ IF PREVIOUS CARDS WERE NUMBER CARDS ===============================>>>>>
                
                //next players turn, wait for confirmation
                Console.WriteLine($"{
                    _gameEngine
                        .State
                        .Players[_gameEngine.State.ActivePlayerNo]
                        .NickName} 's " + $"turn! Press any key to continue..");
                Console.ReadLine();
                Console.Clear();
                
                //define active players handsize
                var handSize = _gameEngine
                                        .State
                                        .Players[_gameEngine.State.ActivePlayerNo]
                                        .PlayerHand
                                        .Count;
                //boolean for loop
                var correctInput = false;
                var choice = "";
                
                //<<<<<================ AI PLAYER MAKES A CHOICE ===================>>>>>
                if (_gameEngine
                        .State
                        .Players[_gameEngine.State.ActivePlayerNo]
                        .PlayerType == EPlayerType.Ai)
                {
                    //choice is made by AI
                    choice = _gameEngine.AiChoice();
                }
                //<<<<<================ HUMAN PLAYER MAKES A CHOICE ===================>>>>>
                else
                {
                    //in a loop
                    do
                    {
                        //vizualise next players hand
                        ConsoleVizualisation.StartPlayerMove(_gameEngine.State);
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
                                if (_gameEngine
                                    .CheckValidity(
                                        _gameEngine
                                            .State
                                            .DiscardedCards.Last(),
                                        _gameEngine
                                                .State
                                                .Players[_gameEngine.State.ActivePlayerNo]
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
                
                //<<<<<================ DO WE MOVE ON TO THE NEXT PLAYER? ===================>>>>>
                
                // if player hand is empty
                if (_gameEngine.IsHandFinished())
                {
                    //if player hand was empty, then check if the game is over
                    _gameEngine.GameDone = _gameEngine.IsGameOver();
                }

            }
        }
        return null;
    }
}
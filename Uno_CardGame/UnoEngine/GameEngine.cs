using DAL;
using Domain;
namespace UnoEngine;

//gameEngine class
public class GameEngine
{//gameEngine attributes: gameRepo, State and initialHandSize of the player
    public IGameRepository GameRepository { get; set; }
    public GameState State { get; set; } = new GameState();
    //modify starting hand size
    private const int InitialHandSize = 7;
    public bool GameDone { get; set; }
    public bool HandDone { get; set; }

    //gameEngine constructor
    public GameEngine(IGameRepository repository)
    {//create repository, cardDeck, default Players, playerHand and discardPile
        GameRepository = repository;
        State.ActivePlayerNo = 0;
        InitializeFullDeck();
        InitializePlayers();
        InitializePlayerHand();
        InitializeDiscardPile();
    }
//method for updating States - if game parameters are changed (no of players, deck size, players names/types)
    public void UpdateGame()
    {//create new cardDeck, playerHand and discardPile
        InitializeFullDeck();
        InitializePlayerHand();
        InitializeDiscardPile();
        HandDone = false;
    }
//method for initializing a new carddeck
    private void InitializeFullDeck()
    {//create a new carddeck with preferred size
        State.CardDeck = new CardDeck(State.CardDeck.Size,State.CardDeck.DeckType);
        //shuffle the cards
        State.CardDeck.Shuffle();
    }
    //default method for creating players
    private void InitializePlayers()
    {
        State.Players = new List<Player>()
        {
            new Player()
            {
                NickName = "Puny human",
                PlayerType = EPlayerType.Human,
                Score = 0,
            },
            new Player()
            {
                NickName = "Mighty AI",
                PlayerType = EPlayerType.AI,
                Score = 0,
            },
        };
    }
    //method for dealing cards to the player
    private void InitializePlayerHand()
    {//create a new list of gamecards for every player
        foreach (var t in State.Players)
        {
            t.PlayerHand = new List<GameCard>();
        }
        //in a double loop deal an even number of cards to every player
        //total of cards dealt = initialHandSize * player count
        int dealtCards = 0;
        while(dealtCards < InitialHandSize * State.Players.Count)
        {
            foreach (var player in State.Players)
            {
                player.PlayerHand.Add(State.CardDeck.Cards.First());
                State.CardDeck.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
    }
    //method for creating a discard pile
    public void InitializeDiscardPile()
    {
        //new list of discardedCards 
        State.DiscardedCards = new List<GameCard>();
        //Add a single card to the discard pile
        State.DiscardedCards.Add(State.CardDeck.Cards.First());
        //remove it from carddeck
        State.CardDeck.Cards.RemoveAt(0);
        State.DeclaredColor = State.DiscardedCards.Last().CardColor;
    }

    //playerMove method
    public void PlayerMove()
    {
        //define boolean for the loop and playerchoice
        var correctInput = false;
        var choice = "";
        //ask for the active players move in a loop
        do
        {
            StartPlayerMove();
            //define player handSize
            var handSize = State.Players[State.ActivePlayerNo].PlayerHand.Count;
            //read in the choice from console
            choice = Console.ReadLine()!.ToLower().Trim();
            //if player chooses to quit, then break the loop
            if (choice == "q")
            {
                Console.WriteLine("Game shuts down!");
                break;
            }
            //if player wants to draw
            if (choice == "d")
            {
                DrewCard(1);
                //check if they are able to play the card
                if (CheckValidity(State.DiscardedCards.Last(),
                        State.Players[State.ActivePlayerNo].PlayerHand.Last()))
                {
                    //actually play the card
                    PlayCard(State.Players[State.ActivePlayerNo].PlayerHand.Count - 1);
                    //check for wild card and declare new color if TRUE
                    if (State.DiscardedCards.Last().CardColor == ECardColor.Wild)
                    {
                        DeclareColor();
                    }
                }
                correctInput = true;
            }
            //if player wrote something else in console (except "q" or "d")
            else
            {
                //check if it was a number and smaller or equal than the amount of cards on hand
                if (int.TryParse(choice, out var position) && position > 0 && position <= handSize)
                {
                    //check if it is a legal move
                    if (!CheckValidity(State.DiscardedCards.Last(),
                            State.Players[State.ActivePlayerNo].PlayerHand[position - 1]))
                    {
                        Console.WriteLine("This card cant be played!");
                    }
                    else
                    {
                        //play the card and declare color if it is a wild card
                        PlayCard(position - 1);
                        
                        if (State.DiscardedCards.Last().CardColor == ECardColor.Wild)
                        {
                            DeclareColor();
                        }
                    }
                    correctInput = true;
                }
                //no valid moves were made, start again
                else
                {
                    Console.WriteLine("Undefined shortcut....");
                }
            }
        } while (!correctInput);
        //if player chose quit then both hand and game will be over
        if (choice == "q")
        {
            HandDone = true;
            GameDone = true;
        }
        //if player drew or played a card
        else
        {
            //if hand is not finished then continue with another player
            if (!IsHandFinished())
            {
                NextPlayer();
            }
            //if hand is finished then check if the game is over
            else
            {
                HandDone = true;
                GameDone = IsGameOver();
            }
        }
    }
//method to show playerHand on console
    public void ShowPlayerHand()
    {
        //Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + "'s turn. Cards on hand: ");
        for (int i = 0; i < State.Players[State.ActivePlayerNo].PlayerHand.Count; i++)
        {
            Console.WriteLine((i+1) + ") |" + State.Players[State.ActivePlayerNo].PlayerHand[i] + "| ");
        }
        Console.WriteLine("d) draw a card");
        Console.WriteLine("q) quit & save game");
    }
    //show discardPile on console
    public void ShowDiscardPile()
    {
        Console.WriteLine("Card on top of discard pile: |" + State.DiscardedCards.Last() + "|");
    }
  //method for declaring the color after a wild card is played
    public void DeclareColor()
    {
        bool correctInput = false;
        do
        {
            State.TurnResult = ETurnResult.OnGoing;
            Console.WriteLine("WILD card was played!");
            Console.WriteLine("1) " + ECardColor.Blue.ToString());
            Console.WriteLine("2) " + ECardColor.Red.ToString());
            Console.WriteLine("3) " + ECardColor.Green.ToString());
            Console.WriteLine("4) " + ECardColor.Yellow.ToString());
            Console.Write($"Player ({State.Players[State.ActivePlayerNo].NickName }) " +$"declare a color: ");
            var colorChoice = Console.ReadLine()!.ToLower().Trim();
            switch (colorChoice)
            {
                case "1":
                    State.DiscardedCards.Last().CardColor = ECardColor.Blue;
                    correctInput = true;
                    break;
                case "2":
                    State.DiscardedCards.Last().CardColor = ECardColor.Red;
                    correctInput = true;
                    break;
                case "3":
                    State.DiscardedCards.Last().CardColor = ECardColor.Green;
                    correctInput = true;
                    break;
                case "4":
                    State.DiscardedCards.Last().CardColor = ECardColor.Yellow;
                    correctInput = true;
                    break;
            }

            if (!correctInput)
            {
                Console.WriteLine("Undefined shortcut...");
            }
        } while (correctInput == false);
    }
    //method for playing the card
    public void PlayCard(int position)
    {
        var playedCard = State.Players[State.ActivePlayerNo].PlayerHand[position];
        State.Players[State.ActivePlayerNo].PlayerHand.RemoveAt(position);
        State.DiscardedCards.Add(playedCard);
        Console.WriteLine(State.Players[State.ActivePlayerNo].NickName 
                          + " plays the new card:  "
                          + playedCard.ToString2());
        //modify turnResult 
        State.TurnResult = ETurnResult.OnGoing;
        
    }
    //method for valid move check
    public bool CheckValidity(GameCard discarded, GameCard choice)
    {
        if (discarded.CardValue == choice.CardValue ||
            discarded.CardColor == choice.CardColor ||
            choice.CardValue == ECardValues.Wild)
        {
            return true;
        }
        //WILD DRAW FOUR
        //else loop through playerHand and check if they have any cards that would match the discardpile
        if (choice.CardValue != ECardValues.DrawFour) return false;
        foreach (var card in State.Players[State.ActivePlayerNo].PlayerHand)
        {
            if (discarded.CardValue == card.CardValue || 
                discarded.CardColor == card.CardColor)
            {
                return false;
            }
        }
        return true;
    }
    //calculates the score for every card that other player are left in their hand
    public void CalculateScore()
    {
        foreach (var player in State.Players)
        {
            foreach (var card in player.PlayerHand)
            {
                State.Players[State.ActivePlayerNo].Score += card.Score;
            }
        }
        Console.WriteLine(State.Players[State.ActivePlayerNo].NickName +
                          " wins the hand! Score: " + State.Players[State.ActivePlayerNo].Score);
    }
    //game is over if a player has a score of 500 points
    public bool IsGameOver()
    {
        if (State.Players[State.ActivePlayerNo].Score >= 500)
        {
            Console.WriteLine(State.Players[State.ActivePlayerNo].NickName +
                              " wins the game! Score: " + State.Players[State.ActivePlayerNo].Score);
            return true;
        }
        return false;
    }
    //hand is finished when a player has 0 cards in their hand or cardDeck is empty
    public bool IsHandFinished()
    {
        if (State.Players[State.ActivePlayerNo].PlayerHand.Count == 0)
        {
            Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + " has 0 cards left! ");
            CalculateScore();
            State.TurnResult = ETurnResult.GameStart;
            return true;
        }
        if (State.CardDeck.Size == 0)
        {
            Console.WriteLine("Card deck is empty!  ");
            CalculateScore();
            State.TurnResult = ETurnResult.GameStart;
            return true;
        }
        return false;
    }
    
    //method for setting the next player
    public void NextPlayer()
    {   //if game goes clockwise and the activeplayerNo is less than the total of players
        if (State.GameDirection == EGameDirection.ClockWise 
            && State.ActivePlayerNo < State.Players.Count - 1)
        {//increase the number
            State.ActivePlayerNo++;
        }
        //if game goes clockwise and the activeplayerNo is equal to the total of players (last player in list)
        else if (State.GameDirection == EGameDirection.ClockWise 
            && State.ActivePlayerNo == State.Players.Count - 1)
        {//set the number back to zero (go back to the first player)
            State.ActivePlayerNo = 0;
        }
        //if game goes counterclockwise and the activeplayerNo is bigger than zero (not the first player)
        if (State is { GameDirection: EGameDirection.CounterClockWise, ActivePlayerNo: > 0 })
        {//decrease the number
            State.ActivePlayerNo--;
        }
        //if game goes counterclockwise and the activeplayerNo is equal to zero (first player)
        else if(State is { GameDirection: EGameDirection.CounterClockWise, ActivePlayerNo: 0 })
        {//go back to the end of the players list
            State.ActivePlayerNo = State.Players.Count - 1;
        }
    }
    //method for changing the game direction
    public void SetGameDirection()
    {
        State.GameDirection = (State.GameDirection == EGameDirection.ClockWise)
            ? EGameDirection.CounterClockWise
            : EGameDirection.ClockWise;
        Console.WriteLine("Last card played was REVERSE. Direction will be set to " + State.GameDirection + "!");
        State.TurnResult = ETurnResult.DrewCard;
    }

    public void DrewCard(int noOfCards)
    {
        Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + " drew " + noOfCards + " card(s)!");
        State.Players[State.ActivePlayerNo].PlayerHand.AddRange(State.CardDeck.Draw(noOfCards));
        //modify turnResult
        State.TurnResult = ETurnResult.DrewCard;
    }

    public void StartPlayerMove()
    {
        Console.WriteLine($"<======== {State.Players[State.ActivePlayerNo].NickName }" + $"'s TURN ========>");
        //show discard pile and players hand
        ShowDiscardPile();
        //show player hand
        Console.WriteLine($"{State.Players[State.ActivePlayerNo].NickName }" + $"'s cards on hand: ");
        ShowPlayerHand();
        Console.WriteLine("<===================>");
        Console.Write("Choose your card: ");
    }
    //method for loading new game text on console
    public void StartGame()
    {
        //@start of the game clears the console from prev menusystem
        Console.Clear();
        Console.WriteLine(State.TurnResult == ETurnResult.LoadGame
            //loading previous game
            ? "<======== LOADING PREVIOUS GAME ========>"
            //new game
            : "<============ NEW GAME ============>");
    }
    //method for loading new hand text on console
    public void StartHand()
    {
        if (State.TurnResult != ETurnResult.LoadGame)
        {
            UpdateGame();
            //loading new hand
            Console.WriteLine("<======== STARTING NEW HAND =======>");
            Console.WriteLine("First card in discard-pile: " + 
                              State.DiscardedCards.First());
        }
        else
        {
            //loading previous game from load game
            Console.WriteLine("<====== RELOADING PREVIOUS HAND =====>");
        }
    }
    
}
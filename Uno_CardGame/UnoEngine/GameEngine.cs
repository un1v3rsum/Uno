using DAL;
using Domain;
namespace UnoEngine;

//gameEngine class
public class GameEngine<TKey>
{//gameEngine attributes: gameRepo, State and initialHandSize of the player
    public IGameRepository<TKey> GameRepository { get; set; }
    public GameState State { get; set; } = new GameState();
    private const int InitialHandSize = 7;
    
    //gameEngine constructor
    public GameEngine(IGameRepository<TKey> repository)
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
    private void InitializeDiscardPile()
    {
        //new list of discardedCards 
        State.DiscardedCards = new List<GameCard>();
        //Add a single card to the discard pile
        State.DiscardedCards.Add(State.CardDeck.Cards.First());
        //remove it from carddeck
        State.CardDeck.Cards.RemoveAt(0);

        //Game rules do not allow the first discard to be a wildDrawFour or newer wild cards.
        while(State.DiscardedCards.First().CardValue == ECardValues.DrawFour ||
              State.DiscardedCards.First().CardValue == ECardValues.Customizable ||
              State.DiscardedCards.First().CardValue == ECardValues.ShuffleHands)
        {
            State.DiscardedCards.Insert(0, State.CardDeck.Cards.First());
            State.CardDeck.Cards.RemoveAt(0);
        }

        State.DeclaredColor = State.DiscardedCards.Last().CardColor;
    }
    //method to save a game
    //@this moment id = fileName
    //while saving and id = null, then fileName = uno-" + DateTime.Now.ToFileTime() + ".json"
    public void SaveGame()
    {
        GameRepository.SaveGame(1, State);
    }
    //method to load a game
    public GameState LoadGame()
    {
        return GameRepository.LoadGame(1);
    }
    //playerMove method
    public void PlayerMove()
    {
        if (State.TurnResult == ETurnResult.GameStart)
        {
            State.TurnResult = ETurnResult.OnGoing;
        }
        //define player handSize
        var handSize = State.Players[State.ActivePlayerNo].PlayerHand.Count;
        bool correctInput = false;
        do
        {   
            Console.WriteLine($"<======== {State.Players[State.ActivePlayerNo].NickName }" + $"'s TURN ========>");
            //in a loop shows the activePlayer 
            ShowDiscardPile();
            Console.WriteLine($"{State.Players[State.ActivePlayerNo].NickName }" + $"'s cards on hand: ");
            ShowPlayerHand();
            Console.WriteLine("<===================>");
            Console.Write("Choose your card: ");
            
            var choice = Console.ReadLine().ToLower().Trim();
            if (choice == "q")
            {
                //TODO quit game mid-session
                Console.WriteLine("Game shuts down!");
                correctInput = true;
                SaveGame();
                
                IsGameOver();
            }
            if (choice == "d")
            {
                Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + " drew a new card!");
                State.Players[State.ActivePlayerNo].PlayerHand.
                    AddRange(State.CardDeck.Draw(1));
                
                if (CheckValidity(State.DiscardedCards.Last(), 
                        State.Players[State.ActivePlayerNo].PlayerHand.Last()))
                {
                    Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + " plays the new card:  " 
                        + State.Players[State.ActivePlayerNo].PlayerHand.Last().ToString2());
                    
                    PlayCard(State.Players[State.ActivePlayerNo].PlayerHand.Count - 1);
                    
                    if (State.DiscardedCards.Last().CardColor == ECardColor.Wild)
                    {
                        DeclareColor();
                    }
                }
                if (!IsHandFinished())
                {
                    NextPlayer();
                }
                correctInput = true;
            }
            if (int.TryParse(choice,out var position) && position > 0 && position <= handSize)
            {
                if (!CheckValidity(State.DiscardedCards.Last(), 
                        State.Players[State.ActivePlayerNo].PlayerHand[position - 1]))
                {
                    Console.WriteLine("This card cant be played!");
                }
                else
                {
                    Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + " played " 
                        + State.Players[State.ActivePlayerNo].PlayerHand[position - 1].ToString2());
                    
                    PlayCard(position-1);
                    
                    //check if played card is wild
                    if (State.DiscardedCards.Last().CardColor == ECardColor.Wild )
                    {
                        DeclareColor();
                    }

                    if (!IsHandFinished())
                    {
                        NextPlayer();
                    }
                }
                correctInput = true;
            }
            else
            {
                Console.WriteLine("Undefined shortcut....");
            }
        } while (correctInput == false);
    }
//method to show playerHand on console
    public void ShowPlayerHand()
    {
        //Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + "'s turn. Cards on hand: ");
        for (int i = 0; i < State.Players[State.ActivePlayerNo].PlayerHand.Count; i++)
        {
            Console.WriteLine((i+1) + ") |" + State.Players[State.ActivePlayerNo].PlayerHand[i].ToString() + "| ");
        }
        Console.WriteLine("d) draw a card");
        Console.WriteLine("q) quit & save game");
    }

    public void ShowDiscardPile()
    {
        Console.WriteLine("Card on top of discard pile: |" + State.DiscardedCards.Last() + "|");
    }
  
    public void DeclareColor()
    {
        bool correctInput = false;
        do
        {
            Console.WriteLine("WILD card was played!");
            Console.WriteLine("1) " + ECardColor.Blue.ToString());
            Console.WriteLine("2) " + ECardColor.Red.ToString());
            Console.WriteLine("3) " + ECardColor.Green.ToString());
            Console.WriteLine("4) " + ECardColor.Yellow.ToString());
            Console.Write($"Player ({State.Players[State.ActivePlayerNo].NickName }) " +$"declare a color: ");
            var colorChoice = Console.ReadLine().ToLower().Trim();
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
        ShowDiscardPile();
    }

    public void PlayCard(int position)
    {
        var playedCard = State.Players[State.ActivePlayerNo].PlayerHand[position];
        State.Players[State.ActivePlayerNo].PlayerHand.RemoveAt(position);
        State.DiscardedCards.Add(playedCard);
        SaveGame();
    }

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
        foreach (var t in State.Players[State.ActivePlayerNo].PlayerHand)
        {
            if (discarded.CardValue == t.CardValue || 
                discarded.CardColor == t.CardColor)
            {
                return false;
            }
        }
        return true;
    }

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
    //TODO 
    public bool IsHandFinished()
    {
        if (State.Players[State.ActivePlayerNo].PlayerHand.Count == 0)
        {
            Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + " has 0 cards left! ");
            return true;
        }
        if (State.CardDeck.Size == 0)
        {
            Console.WriteLine("Card deck is empty!  ");
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
    }
    
}
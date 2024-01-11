using DAL;
using Domain;
namespace UnoEngine;

//gameEngine class
public class GameEngine
{//gameEngine attributes: gameRepo, State and initialHandSize of the player
    public IGameRepository GameRepository { get; set; }
    public GameState State { get; set; } = new GameState();
    //modify starting hand size
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
    {
        //create new cardDeck, playerHand and discardPile
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
                PlayerType = EPlayerType.Ai,
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
        while(dealtCards < State.InitialHandSize * State.Players.Count)
        {
            foreach (var player in State.Players)
            {
                //for each player add a card to their hand
                player
                    .PlayerHand
                    .Add(
                        State
                            .CardDeck
                            .Cards
                            .First());
                //and remove that card from carddeck
                State
                    .CardDeck
                    .Cards
                    .RemoveAt(0);
                
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
    
    //AI choice
    public string AiChoice()
    {
        //create container for valid cards
        var validCardPositions = new List<int>();
        var choice = "";
        //loops through player cards
        for (var i = 0; i < State.Players[State.ActivePlayerNo].PlayerHand.Count; i++)
        {
            //created container because:
            //*AI choice got lost in loop and wasn't accessible in lower lines*
                    
            //finds valid card indexes and adds to container
            if (CheckValidity(
                    State
                        .DiscardedCards
                        .Last(),
                    State
                        .Players[State.ActivePlayerNo]
                        .PlayerHand[i]))
            {
                validCardPositions.Add(i);
            }
        }
        //if no valid cards, then draw card
        if (validCardPositions.Count == 0)
        {
            Console.WriteLine("AI has no valid moves, DRAWS a CARD!");
            choice = "d";
        }
        else
        {
            //picks a random valid card
            var random = new Random();
            var randomValidCardPosition = validCardPositions[random.Next(0, validCardPositions.Count)];
            choice = (randomValidCardPosition + 1).ToString();
        }
        return choice;
    }

    public void AiMove()
    {
        MakeAMove(AiChoice());
        
        if (State.TurnResult == ETurnResult.DrewCard)
        {
            DrewCard(1);
            //check if they are able to play the card
        }
    }
    //method for making a valid move
    public void MakeAMove(string choice)
    {
        switch (choice)
        {
            //if player chose to quit the game
            case "q":
                Console.WriteLine("Game shuts down!");
                HandDone = true;
                GameDone = true;
                break;
            case "d":
                //if player wants to draw then change the TurnResult
                State.TurnResult = ETurnResult.DrewCard;
                Console.WriteLine(State.Players[State.ActivePlayerNo] + " draws a card");
                break;
            default:
                //play the actual card
                PlayCard(int.Parse(choice) - 1);
                break;
        }
    }
  //method for declaring the color after a wild card is played
    public void DeclareColor(string colorChoice)
    {
        switch (colorChoice)
        {
            case "1":
                State.DiscardedCards.Last().CardColor = ECardColor.Blue;
                break;
            case "2":
                State.DiscardedCards.Last().CardColor = ECardColor.Red;
                break;
            case "3":
                State.DiscardedCards.Last().CardColor = ECardColor.Green;
                break;
            case "4":
                State.DiscardedCards.Last().CardColor = ECardColor.Yellow;
                break;
        }
    }
    //method for playing the card
    public void PlayCard(int position)
    {
        var playedCard = State.Players[State.ActivePlayerNo].PlayerHand[position];
        State
            .Players[State.ActivePlayerNo]
            .PlayerHand
            .RemoveAt(position);
        
        State.DiscardedCards.Add(playedCard);
        Console.WriteLine(State
                .Players[State.ActivePlayerNo]
                .NickName + " plays card:  " + playedCard.ToString2());
        
        //modify turnResult 
        State.TurnResult = ETurnResult.OnGoing;
                    
        //after every card played, check if hand is finished
        HandDone = IsHandFinished();
        
        //<<<<<================ DO WE MOVE ON TO THE NEXT PLAYER? ===================>>>>>
        //If the played card was not a wild card then move on to next player
        //if it was a wild card, then we want the active player to declare new color
        //TODO maybe nextPlayer selection should be outside of this method...
        if (State.DiscardedCards.Last().CardColor != ECardColor.Wild)
        {
            NextPlayer();
        }
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
    //opposite boolean for HTML button disable function
    public bool CheckValidityHtml(bool boolean){
        if (boolean) {
            return false;
        } return true;
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
        Console.WriteLine(State
                              .Players[State.ActivePlayerNo]
                              .NickName 
                          + " wins the hand! Score: " + 
                          State.Players[State.ActivePlayerNo].Score);
    }
    //game is over if a player has a score of 500 points
    public bool IsGameOver()
    {
        if (State.Players[State.ActivePlayerNo].Score < 500) return false;
        Console.WriteLine(State
                              .Players[State.ActivePlayerNo]
                              .NickName 
                          + " wins the game! Score: " + 
                          State.Players[State.ActivePlayerNo].Score);
        return true;
    }
    //hand is finished when a player has 0 cards in their hand or cardDeck is empty
    public bool IsHandFinished()
    {
        if (State.Players[State.ActivePlayerNo].PlayerHand.Count == 0)
        {
            Console.WriteLine(State
                .Players[State.ActivePlayerNo]
                .NickName 
                              + " has 0 cards left! ");
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
    {   
        //if game goes clockwise and the activeplayerNo is less than the total of players
        if (State.GameDirection == EGameDirection.ClockWise && 
            State.ActivePlayerNo < State.Players.Count - 1)
        {//increase the number
            State.ActivePlayerNo++;
        }
        //if game goes clockwise and the activeplayerNo is equal to the total of players (last player in list)
        else if (State.GameDirection == EGameDirection.ClockWise && 
                 State.ActivePlayerNo == State.Players.Count - 1)
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
    //method for drawing a card
    public void DrewCard(int noOfCards)
    {
        Console.WriteLine(State
            .Players[State.ActivePlayerNo]
            .NickName 
                          + " drew " + noOfCards + " card(s)!");
        //draw cards from the carddeck
        State
            .Players[State.ActivePlayerNo]
            .PlayerHand
            .AddRange(State.CardDeck.Draw(noOfCards));
        //if previous player played action card and active player drew cards
        if (State.DiscardedCards.Last().CardValue == ECardValues.DrawTwo || 
            State.DiscardedCards.Last().CardValue == ECardValues.DrawFour)
        {
            State.TurnResult = ETurnResult.DrewCard;
            NextPlayer();
        }
        else
        {
            //check if they are able to play the card
            if (CheckValidity(
                    State
                        .DiscardedCards
                        .Last(), 
                    State
                        .Players[State.ActivePlayerNo]
                        .PlayerHand.Last()))
            {
                //actually play the card
                PlayCard(State.Players[State.ActivePlayerNo].PlayerHand.Count - 1);
            }
            //if player drew a card and couldnt play it, then game moves on to next player
            else
            {
                State.TurnResult = ETurnResult.DrewCard;
                NextPlayer();
            }
        }
    }
    
    //method for getting the active player
    public Player GetActivePlayer()
    {
        return State.Players[State.ActivePlayerNo];
    }
    //method for getting the player index
    public int GetPlayerIndex(Guid playerId)
    {
        int idx = default!;
        for (var i = 0; i < State.Players.Count; i++)
        {
            if (State.Players[i].Id == playerId)
            {
                idx = i;
            }
        }
        return idx;
    }
    //method for getting the card position
    public int GetCardPositionInPlayerHand(GameCard card, Guid playerId)
    {
        int idx = default!;
        for (var i = 0; i < State.Players.Count; i++)
        {
            if (State.Players[i].Id == playerId)
            {
                idx = State.Players[i].PlayerHand.IndexOf(card);
            }
        }
        return idx;
    }

//method for changing the deck type CURRENTLY NOT AVAILABLE
    public string? SetDeckType()
    {
        State.CardDeck.DeckType = (State.CardDeck.DeckType == ECardDeckType.Modern) 
            ? ECardDeckType.Original 
            : ECardDeckType.Modern;
        UpdateGame();
        return null;
    }

    public void SetPlayerCount(int humanCount, int aiCount)
    {
        //create new list of Players, initially all human
        State.Players = new List<Player>();
        for (int i = 0; i < humanCount; i++)
        {
            State.Players.Add(new Player()
            {
                NickName   = "Human " + (i+1),
                PlayerType = EPlayerType.Human,
                Score = 0,
            });
        }
        for (int i = 0; i < aiCount; i++)
        {
            State.Players.Add(new Player()
            {
                NickName   = "AI " + (i+1),
                PlayerType = EPlayerType.Ai,
                Score = 0,
            });
        }
    }
    //method for changing the deck size
    public void SetDeckSize(int size)
    {
        State.CardDeck.Size = size;
    }
    public void SetHandSize(int size)
    {
        State.InitialHandSize = size;
    }
    //======================== CONSOLE METHODS ============================================//
    
    //method for changing the deck size on console
    public string? SetHandSizeConsole()
    {
        Console.WriteLine("Starting player hand can from 1 to 10 cards.");
        bool correctCount = false;
        do
        {
            Console.Write("Insert nr of cards:"); 
            var countStr = Console.ReadLine();
            if (int.Parse(countStr) >= 1 && int.Parse(countStr) <= 10)
            {
                State.InitialHandSize = int.Parse(countStr);
                correctCount = true;
            }
            else
            {
                Console.WriteLine("ERROR! You have to insert an integer between 1 - 10.");
            }
        } while (correctCount == false);
        UpdateGame();
        return null;
    }
    
    
    
    //method for changing the deck size on console
    public string? SetDeckSizeConsole()
    {
        Console.WriteLine("Max 3 packs of cards can be used.");
        bool correctCount = false;
        do
        {
            Console.Write("Insert nr of packs:"); 
            var countStr = Console.ReadLine();
            if (countStr is "1" or "2" or "3")
            {
                State.CardDeck.Size = int.Parse(countStr);
                correctCount = true;
            }
            else
            {
                Console.WriteLine("ERROR! You have to insert an integer between 1 - 3.");
            }
        } while (correctCount == false);
        UpdateGame();
        return null;
    }
    //method for setting playerCount on console
    public string SetPlayerCountConsole()
    {
        Console.WriteLine("Game can have 2 - 10 players.");
        bool correctCount = false;
        int count;
        do
        {
            Console.Write("Insert player count:"); 
            var countStr = Console.ReadLine();
            int.TryParse(countStr,out count);
            if (count is < 2 or > 10)
            {
                Console.WriteLine("ERROR! You have to insert an integer between 2 - 10.");
            }
            else
            {
                correctCount = true;
            }
        } while (correctCount == false);
        //create new list of Players, initially all human
        State.Players = new List<Player>();
        for (int i = 0; i < count; i++)
        {
            State.Players.Add(new Player()
            {
                NickName   = "Player " + (i+1),
                PlayerType = EPlayerType.Human,
                Score = 0,
            });
        }
        UpdateGame();
        return null;
    }
    //method for changing players names and types on console
    public string? EditPlayerNamesAndTypesConsole()
    {
        for (var i = 0; i < State.Players.Count; i++)
        {
            var realType = false;
            Console.Write("Enter " + (i+1) + ". player name: ");
            State.Players[i].NickName = Console.ReadLine();
            do
            { 
                Console.Write("Is player " + (i+1) + " human (press: h) or AI (press: a)?: ");
                var answer = Console.ReadLine();
                if (answer != "h" && answer != "a")
                {
                    Console.WriteLine("ERROR! Press the letter 'h' or 'a' on your keyboard.");
                }
                switch (answer)
                {
                    case "h":
                        State.Players[i].PlayerType = EPlayerType.Human;
                        realType = true;
                        break;
                    case "a":
                        State.Players[i].PlayerType = EPlayerType.Ai;
                        realType = true;
                        break;
                }
            } while (realType == false);
        }
        UpdateGame();
        return null;
    }
}
﻿using DAL;
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
    {
        Console.WriteLine("do we update game?");
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
        while(dealtCards < InitialHandSize * State.Players.Count)
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
        Console.WriteLine("AI no of card: " +  State.Players[State.ActivePlayerNo].PlayerHand.Count);
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
        Console.WriteLine("valid move count: " + validCardPositions.Count);
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
            Console.WriteLine("AI chosen index was: " + randomValidCardPosition);
            Console.WriteLine("now we add + 1 to " + randomValidCardPosition);
            choice = (randomValidCardPosition + 1).ToString();
            Console.WriteLine("input sent by the AI to makeAmove(): "+choice);
        }
        return choice;
    }

    public void AiMove()
    {
        var choice = AiChoice();
        Console.WriteLine("do we get aiChoice: " + choice);
        MakeAMove(choice);
        
        if (State.TurnResult == ETurnResult.DrewCard)
        {
            DrewCard(1);
            //check if they are able to play the card
        }
    }
    //method for making a valid move
    public void MakeAMove(string choice)
    {
        Console.WriteLine(State.Players[State.ActivePlayerNo] + " gave input to MakeAMove(): " + choice);
        //if player chose to quit the game
        if (choice == "q")
        {
            Console.WriteLine("Game shuts down!");
            HandDone = true;
            GameDone = true;
        }
        else
        //check if player choice was to draw or play a card
        {
            if (choice == "d")
            {
                //if player wants to draw then change the TurnResult
                State.TurnResult = ETurnResult.DrewCard;
                Console.WriteLine(State.Players[State.ActivePlayerNo] + " draws a card");
            }
            else
            {
                Console.WriteLine("now should " + choice + " - 1");
                Console.WriteLine("input that reaches for playCard(): " + (int.Parse(choice)-1));
                //play the actual card
                PlayCard(int.Parse(choice) - 1);
            }
        }
    }
  //method for declaring the color after a wild card is played
    public void DeclareColor(string colorChoice)
    {
        Console.WriteLine("do we declare color?");
        Console.WriteLine("who declares color: " + State.Players[State.ActivePlayerNo].NickName);
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
        Console.WriteLine("who plays card: " + State.Players[State.ActivePlayerNo].NickName);
        Console.WriteLine("playcard() got the input: " + position);
        Console.WriteLine("input is: "+position);
        Console.WriteLine("playerhand length: " + State.Players[State.ActivePlayerNo].PlayerHand.Count);
        var playedCard = State.Players[State.ActivePlayerNo].PlayerHand[position];
        Console.WriteLine("the chosen card is: " + playedCard.ToString());
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
    }
    //method for valid move check
    public bool CheckValidity(GameCard discarded, GameCard choice)
    {
        Console.WriteLine("turnresult: " + State.TurnResult);
        Console.WriteLine("do we control validity?");
        Console.WriteLine("who checks validity: " + State.Players[State.ActivePlayerNo].NickName);
        Console.WriteLine("discarded card was: " + discarded.ToString2());
        Console.WriteLine("what card validity: " + choice.ToString2());
        if (discarded.CardValue == choice.CardValue ||
            discarded.CardColor == choice.CardColor ||
            choice.CardValue == ECardValues.Wild)
        {
            Console.WriteLine("is VALID");
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
        Console.WriteLine("do we control gameOver?");
        if (State.Players[State.ActivePlayerNo].Score >= 500)
        {
            Console.WriteLine(State
                                  .Players[State.ActivePlayerNo]
                                  .NickName 
                              + " wins the game! Score: " + 
                              State.Players[State.ActivePlayerNo].Score);
            return true;
        }
        return false;
    }
    //hand is finished when a player has 0 cards in their hand or cardDeck is empty
    public bool IsHandFinished()
    {
        Console.WriteLine("do we control handFinish?");
        Console.WriteLine("how many cards on hand: " + State.Players[State.ActivePlayerNo].PlayerHand.Count);
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
        Console.WriteLine("do we move on to next player");
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
            
            //If the played card was not a wild card then move on to next player
            //if it was a wild card, then we want the active player to declare new color
            if (State.DiscardedCards.Last().CardColor != ECardColor.Wild)
            {
                NextPlayer();
            }
        }
        //if player drew a card and couldnt play it, then game moves on to next player
        else
        {
            NextPlayer();
        }
    }
    
    //method for getting the active player
    public Player GetActivePlayer()
    {
        return State.Players[State.ActivePlayerNo];
    }

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
    
}
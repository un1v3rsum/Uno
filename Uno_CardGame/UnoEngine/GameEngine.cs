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
                Position = 0
            },
            new Player()
            {
                NickName = "Mighty AI",
                PlayerType = EPlayerType.AI,
                Position = 1
            },
        };
    }
    //method for dealing cards to the player
    private void InitializePlayerHand()
    {//create a new list of gamecards for every player
        foreach (var player in State.Players)
        {
            State.Players[player.Position].PlayerHand = new List<GameCard>();
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
    {   //new list of discardedCards 
        State.DiscardedCards = new List<GameCard>();
        //Add a single card to the discard pile
        State.DiscardedCards.Add(State.CardDeck.Cards.First());
        //remove it from carddeck
        State.CardDeck.Cards.RemoveAt(0);

        //Game rules do not allow the first discard to be a wild.
        while(State.DiscardedCards.First().CardValue == ECardValues.Wild || 
              State.DiscardedCards.First().CardValue == ECardValues.WildDrawFour ||
              State.DiscardedCards.First().CardValue == ECardValues.WildCustomizable ||
              State.DiscardedCards.First().CardValue == ECardValues.WildShuffleHands)
        {
            State.DiscardedCards.Insert(0, State.CardDeck.Cards.First());
            State.CardDeck.Cards.RemoveAt(0);
        }
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
//method to show playerHand on console
    public void ShowPlayerHand()
    {
        Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + "'s turn. Cards on hand: ");
        for (int i = 0; i < State.Players[State.ActivePlayerNo].PlayerHand.Count; i++)
        {
            Console.Write(" |" + State.Players[State.ActivePlayerNo].PlayerHand[i].ToString() + "| ");
        }
        Console.WriteLine();
    }

    public bool IsGameOver()
    {
        return false;
    }

    public bool IsItNewMove()
    {
        return true;
    }
    
}
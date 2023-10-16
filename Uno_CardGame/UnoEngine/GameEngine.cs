using DAL;
using Domain;
namespace UnoEngine;

public class GameEngine<TKey>
{
    public IGameRepository<TKey> GameRepository { get; set; }
    public GameState State { get; set; } = new GameState();
    private const int InitialHandSize = 7;
    //private const int InitialDeckSize = 1;
    public GameEngine(IGameRepository<TKey> repository)
    {
        GameRepository = repository;
        InitializeFullDeck();
        InitializePlayers();
        InitializePlayerHand();
        InitializeDiscardPile();
    }

    public void UpdateGame()
    {
        InitializeFullDeck();
        InitializePlayerHand();
        InitializeDiscardPile();
    }

    private void InitializeFullDeck()
    {
        new CardDeck(State.CardDeck.Size);
        State.CardDeck.Shuffle();
    }
    
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

    private void InitializePlayerHand()
    {
        int dealtCards = 0;
        while(dealtCards < InitialHandSize * State.Players.Count)
        {
            for(int i = 0; i < State.Players.Count; i ++)
            {
                State.Players[i].PlayerHand.Add(State.CardDeck.Cards.First());
                State.CardDeck.Cards.RemoveAt(0);
                dealtCards++;
            }
        }
    }

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
    //@this moment id = fileName
    //while saving and id = null, then fileName = uno-" + DateTime.Now.ToFileTime() + ".json"
    public void SaveGame()
    {
        GameRepository.SaveGame(2, State);
    }
    public GameState LoadGame()
    {
        return GameRepository.LoadGame(2);
    }
    
}
using DAL;
using Domain;
namespace UnoEngine;

public class GameEngine<TKey>
{
    public IGameRepository<TKey> GameRepository { get; set; }
    public GameState State { get; set; } = new GameState();
    private const int InitialHandSize = 7;
    private const int InitialDeckSize = 1;
    public GameEngine(IGameRepository<TKey> repository)
    {
        GameRepository = repository;
        InitializeFullDeck();
        InitializePlayers();
    }

    private void InitializeFullDeck()
    {
        new CardDeck(InitialDeckSize);
        CardDeck.Shuffle();
    }
    
    private void InitializePlayers()
    {
        State.Players = new List<Player>()
        {
            new Player()
            {
                NickName = "Puny human",
                PlayerType = EPlayerType.Human
            },
            new Player()
            {
                NickName = "Mighty AI",
                PlayerType = EPlayerType.AI
            },
        };
    }

    public void SaveGame()
    {
        GameRepository.SaveGame(null, State);
    }

    
}
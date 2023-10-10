using Domain;

namespace UnoEngine;
//gameengine class, if constructed, creates a list with players: ATM 1 human + 1 AI
public class GameEngine
{//TODO 
    //public DeckOfCards DeckOfCards { get; set; }
    //random engine declaration
    public List<GameCard> DiscardedCards { get; set; }
    public CardDeck CardDeck { get; set; }
    private Random rnd { get; set; } = new Random();
    public int ActivePlayerNo { get; set; } = 0;//TODO: how to choose the starter?
    public List<Player> Players { get; set; } = new List<Player>()
    {
        new Player()
        {
            NickName = "Human",
            PlayerType = EPlayerType.Human
        },
        new Player()
        {
            NickName = "AI",
            PlayerType = EPlayerType.AI
        },
    };

}
using Domain;
namespace UnoEngine;
//gameEngine class, if constructed, creates a list with players: ATM 1 human + 1 AI
public class GameEngine
{
    public List<GameCard> DiscardedCards { get; set; }
    public CardDeck CardDeck { get; set; } = new CardDeck(1);
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
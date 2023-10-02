namespace UnoEngine;
//gameengine class, if constructed, creates a list with players: ATM 1 human + 1 AI
public class GameEngine
{
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
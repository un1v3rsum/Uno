namespace UnoEngine;

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
using Domain;
namespace UnoEngine;

//player class with a string (nickname), enum (playertype), int (score) & list of gamecards (playerHand) attributes
public class Player
{
    public string NickName { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    public int Score { get; set; }
    public List<GameCard> PlayerHand { get; set; }
    public override string ToString()
    {
        return NickName + " " + "(" + PlayerType.ToString() + ")";
    }
}
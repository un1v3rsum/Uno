using Domain;

namespace UnoEngine;
//player class with a string (nickname), enum (playertype) & list of gamecards (playerHand) attributes
public class Player
{
    public string NickName { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    
    public List<GameCard> PlayerHand { get; set; } = new List<GameCard>();
}
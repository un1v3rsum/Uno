using Domain;
namespace UnoEngine;
//player class with a string (nickname), enum (playertype), int (position) & list of gamecards (playerHand) attributes
public class Player
{
    public string NickName { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }
    //starting order in the game
    public string Position { get; set; }
    
    public List<GameCard> PlayerHand { get; set; } = new List<GameCard>();
}
using UnoEngine;

namespace Domain;
public class GameState
{
    public List<Player> Players { get; set; } 
    //public int ActivePlayerNo { get; set; } = 0;//TODO: how to choose the starter?
    public CardDeck CardDeck { get; set; } = new CardDeck(1);
   public List<GameCard> DiscardedCards { get; set; } 
}
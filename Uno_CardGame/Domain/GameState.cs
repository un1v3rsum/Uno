using UnoEngine;

namespace Domain;
public class GameState
{
    public List<Player> Players { get; set; }
    public int ActivePlayerNo { get; set; }
    public CardDeck CardDeck { get; set; } = new CardDeck(1, ECardDeckType.Original);
   public List<GameCard> DiscardedCards { get; set; } 
}
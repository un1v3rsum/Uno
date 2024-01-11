using UnoEngine;

namespace Domain;
//gamestate class for saving the game
public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public List<Player> Players { get; set; }
    public int InitialHandSize { get; set; } = 7;
    public int ActivePlayerNo { get; set; }
    //carddeck default size as 1
    public CardDeck CardDeck { get; set; } = new CardDeck(1, ECardDeckType.Original);
   public List<GameCard> DiscardedCards { get; set; }
   public ECardColor DeclaredColor { get; set; }
   //turnResult status -> @ start of the game, first card in discardpile has a bit of different outcome
   public ETurnResult TurnResult { get; set; } = ETurnResult.GameStart;
   //gameDirection starts playing a role if there is >2 players
   public EGameDirection GameDirection { get; set; } = EGameDirection.ClockWise;
   
}
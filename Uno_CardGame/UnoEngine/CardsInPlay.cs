namespace UnoEngine;

public class CardsInPlay
{//TODO 
    public List<GameCard> ReceivedCards { get; set; } = new List<GameCard>();

    void Reset()
    {
        ReceivedCards = new List<GameCard>();
    }
}
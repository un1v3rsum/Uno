namespace Domain;

public class CardDeck
{//TODO 
    public List<GameCard> Cards { get; set; }

    public CardDeck()
    {
        Cards = new List<GameCard>();
    }

    //void Reset()
    //{
        //ReceivedCards = new List<GameCard>();
    //}
}
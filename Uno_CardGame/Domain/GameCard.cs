using UnoEngine;

namespace Domain;

public class GameCard
{
    public ECardColor CardColor { get; set; }
    public ECardValues CardValue { get; set; }
    public int score { get; set; }
}
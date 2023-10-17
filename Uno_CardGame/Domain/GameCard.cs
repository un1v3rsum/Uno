using UnoEngine;
namespace Domain;

//gameCard class with three attributes: color, value, score
public class GameCard
{
    public ECardColor CardColor { get; set; }
    public ECardValues CardValue { get; set; }
    public int Score { get; set; }
    //method for combining CardColor and CardValue
    public override string ToString()
    {
        return CardColorToString() + CardValueToString();
    }
    //method for giving string emoji values for CardColors
    private string CardColorToString() =>
        CardColor switch
        {
            ECardColor.Blue => "🟦",
            ECardColor.Green => "🟩",
            ECardColor.Red => "🟥",
            ECardColor.Yellow => "🟨",
            ECardColor.Wild => "☢️"
            //ECardColor.Wild => "◼️"
        };
//method for giving string values to CardValues
    private string CardValueToString() =>
        CardValue switch
        {
            ECardValues.Zero => "0️",
            ECardValues.One => "1",
            ECardValues.Two => "2",
            ECardValues.Three => "3",
            ECardValues.Four => "4",
            ECardValues.Five => "5",
            ECardValues.Six => "6",
            ECardValues.Seven => "7",
            ECardValues.Eight => "8",
            ECardValues.Nine => "9",
            ECardValues.Skip => "SKIP",
            ECardValues.Reverse => "REVERSE",
            ECardValues.DrawTwo => "+2",
            ECardValues.Wild => "",
            ECardValues.WildDrawFour => "+4",
            ECardValues.WildShuffleHands => "SHUFFLE",
            ECardValues.WildCustomizable => "CUSTOMIZABLE"
        };
    
}
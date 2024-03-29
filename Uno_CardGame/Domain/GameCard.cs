using UnoEngine;
namespace Domain;

//gameCard class with three attributes: color, value, score
public class GameCard
{
    public ECardColor CardColor { get; set; }
    public ECardValues CardValue { get; set; }
    public int Score { get; set; }
    //methods for combining CardColor and CardValue as colored cards in console or plain text 
    public override string ToString()
    {
        return CardColorToString() + CardValueToString();
    }
    public string ToString2()
    {
        return CardColor.ToString() + CardValueToString();
    }
    //method for giving string emoji values for CardColors
    public string CardColorToString() =>
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
    public string CardValueToString() =>
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
            ECardValues.Skip => "✘",
            ECardValues.Reverse => "↺",
            ECardValues.DrawTwo => "+2",
            ECardValues.Wild => "☢️",
            ECardValues.DrawFour => "☢️4",
            ECardValues.ShuffleHands => "SHUFFLE",
            ECardValues.Customizable => "CUSTOMIZABLE"
        };
    
}
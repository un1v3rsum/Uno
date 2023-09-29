namespace Uno_CardGame;
public enum CardValue
{
    Zero, 
    One, 
    Two, 
    Three, 
    Four, 
    Five, 
    Six, 
    Seven, 
    Eight, 
    Nine, 
    Reverse, 
    Skip, 
    DrawTwo, 
    Wild, 
    WildDrawFour
}
public enum CardColor
{
    Yellow, 
    Blue, 
    Green, 
    Red, 
    Wild
}
public class Card
{
    public CardColor Color { get; set; }
    public CardValue Value { get; set; }
    public int Score { get; set; }
}
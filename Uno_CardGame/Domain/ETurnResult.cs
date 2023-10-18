namespace Domain;

public enum ETurnResult
{
    GameStart, 
    OnGoing,
    PlayedCard, 
    Skip, 
    DrawTwo, 
    Attacked, 
    DrawCard, 
    DrawCardAndPlayIt, 
    WildCard, 
    WildDrawFour, 
    Reversed, 
    
    //TODO modernDeckType 
    WildShuffleHands, 
    WildCustomizable
}
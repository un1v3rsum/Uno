namespace Domain;

public enum ETurnResult
{
    GameStart, 
    PlayedCard, 
    Skip, 
    DrawTwo, 
    Attacked, 
    ForceDraw, 
    ForceDrawPlay, 
    WildCard, 
    WildDrawFour, 
    Reversed, 
    
    //TODO modernDeckType 
    WildShuffleHands, 
    WildCustomizable
}
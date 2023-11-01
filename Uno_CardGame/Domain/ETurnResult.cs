namespace Domain;
//turnResult enum, defines if it's the first playerTurn in the game
public enum ETurnResult
{
    GameStart, 
    OnGoing,
    //so that action card on top of discard-pile wouldn't affect additional players if someone draws a card
    DrewCard,
    //so that action card on top of discard-pile wouldn't affect additional players if game is loaded
    LoadGame
}
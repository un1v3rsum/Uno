namespace UnoConsoleUI;
using Domain;

public static class ConsoleVizualisation
{
    //method for loading new game text on console
    public static void StartGame(GameState State)
    {
        //@start of the game clears the console from prev menusystem
        Console.Clear();
        Console.WriteLine(State.TurnResult == ETurnResult.LoadGame
            //loading previous game
            ? "<======== LOADING PREVIOUS GAME ========>"
            //new game
            : "<============ NEW GAME ============>");
    }
    //method for loading new hand text on console
    public static void StartHand(GameState State)
    {
        if (State.TurnResult != ETurnResult.LoadGame)
        {
            //loading new hand
            Console.WriteLine("<======== STARTING NEW HAND =======>");
            Console.WriteLine("First card in discard-pile: " + 
                              State.DiscardedCards.First());
        }
        else
        {
            //loading previous game from load game
            Console.WriteLine("<====== RELOADING PREVIOUS HAND =====>");
        }
    }
    public static void StartPlayerMove(GameState state)
    {
        Console.WriteLine($"<======== {state.Players[state.ActivePlayerNo].NickName }" + $"'s TURN ========>");
        //show discard pile and players hand
        ShowDiscardPile(state);
        //show player hand
        Console.WriteLine($"{state.Players[state.ActivePlayerNo].NickName }" + $"'s cards on hand: ");
        ShowPlayerHand(state);
        Console.WriteLine("<===================>");
        Console.Write("Choose your card: ");
    }
    
    //show discardPile on console
    public static void ShowDiscardPile(GameState state)
    {
        Console.WriteLine("Card on top of discard pile: |" + state.DiscardedCards.Last() + "|");
    }
    //method to show playerHand on console
    public static void ShowPlayerHand(GameState state)
    {
        //Console.WriteLine(State.Players[State.ActivePlayerNo].NickName + "'s turn. Cards on hand: ");
        for (int i = 0; i < state.Players[state.ActivePlayerNo].PlayerHand.Count; i++)
        {
            Console.WriteLine((i+1) + ") |" + state.Players[state.ActivePlayerNo].PlayerHand[i] + "| ");
        }
        Console.WriteLine("d) draw a card");
        Console.WriteLine("q) quit & save game");
    }
}

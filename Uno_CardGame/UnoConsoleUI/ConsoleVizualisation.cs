using UnoEngine;

namespace UnoConsoleUI;
using Domain;

public static class ConsoleVizualisation
{
    //method for loading new game text on console
    public static void StartGame(GameState state)
    {
        //@start of the game clears the console from prev menusystem
        Console.Clear();
        Console.WriteLine(state.TurnResult == ETurnResult.LoadGame
            //loading previous game
            ? "<======== LOADING PREVIOUS GAME ========>"
            //new game
            : "<============ NEW GAME ============>");
    }
    //method for loading new hand text on console
    public static void StartHand(GameState state)
    {
        if (state.TurnResult != ETurnResult.LoadGame)
        {
            //loading new hand
            Console.WriteLine("<======== STARTING NEW HAND =======>");
            Console.WriteLine("First card in discard-pile: " + 
                              state.DiscardedCards.First());
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
    //vizualise color declaration on console
    public static void ShowDeclaringAColor(GameState state)
    {
        state.TurnResult = ETurnResult.OnGoing;
        Console.WriteLine("WILD card was played!");
        Console.WriteLine("1) " + ECardColor.Blue.ToString());
        Console.WriteLine("2) " + ECardColor.Red.ToString());
        Console.WriteLine("3) " + ECardColor.Green.ToString());
        Console.WriteLine("4) " + ECardColor.Yellow.ToString());
        Console.Write($"Player ({state.Players[state.ActivePlayerNo].NickName }) " +$"declare a color: ");
    }
    //vizualise new declared color on console
    public static void ShowNewDeclaredColor(GameState state)
    {
        Console.WriteLine($"Player ({state.Players[state.ActivePlayerNo].NickName }) " 
                          +$"chose new color as: " + state.DiscardedCards.Last().CardColor);
        Console.WriteLine("Press any key to continue: ");
        Console.ReadLine();
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

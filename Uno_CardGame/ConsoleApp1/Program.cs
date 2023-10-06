using UnoEngine;
using MenuSystem;
//define gameEngine
var game = new GameEngine();

//method for setting playerCount
string? SetPlayerCount()
{
    Console.Write("Player count?");
    var countStr = Console.ReadLine()?.Trim();
    var count = int.Parse(countStr);

    game.Players = new List<Player>();
    for (int i = 0; i < count; i++)
    {
        game.Players.Add(new Player()
        {
            NickName   = "Human " + i,
            PlayerType = EPlayerType.Human,
        });
    }
   //TODO only allow numbers to be inserted
    
    return null;
}
//method for picking New Game
string? runNewGameMenu()
{
    //constructing new menu with items
    var startNewGameMenu = new Menu(EMenuLevel.Second, "New Game", new List<MenuItem>()
        {
            new MenuItem(
                "Player count: "+game.Players.Count, 
                "c", 
                SetPlayerCount),
            new MenuItem(
                "Players names and types: ", 
                "t", 
                null),
            new MenuItem(
                "Start the game of UNO", 
                "s", 
                null),
        }
    );
    //returns and activates new menu
    return startNewGameMenu.Run();
}
//Initial mainMenu 
var mainMenu = new Menu(EMenuLevel.First,">> UNO <<", new List<MenuItem>()
{
    new MenuItem(
        "Start a new game", 
        "s", 
        runNewGameMenu),
    new MenuItem(
        "Load game", 
        "l", 
        null),
    new MenuItem(
        "Options", 
        "o", 
        null),
});
//run the consoleApp 
var userChoice = mainMenu.Run();
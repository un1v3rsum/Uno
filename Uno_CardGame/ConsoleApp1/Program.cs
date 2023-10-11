using UnoEngine;
using MenuSystem;
//define gameEngine
var game = new GameEngine();
//method for setting playerCount 
string setPlayerCount(string? i1)
{
    Console.WriteLine("Game can have 2 - 10 players.");
    bool correctCount = false;
    int count;
    do
    {
        Console.Write("Insert player count:"); 
        var countStr = Console.ReadLine();
        int.TryParse(countStr,out count);
        if (count is < 2 or > 10)
        {
            Console.WriteLine("ERROR! You have to insert an integer between 2 - 10.");
        }
        else
        {
            correctCount = true;
        }
    } while (correctCount == false);
    //create new list of Players, initially all human
    game.Players = new List<Player>();
    for (int i = 0; i < count; i++)
    {
        game.Players.Add(new Player()
        {
            NickName   = "Human " + i,
            PlayerType = EPlayerType.Human,
        });
    }
    return null;
}
//method to change the name or type of the player
string? changePlayerNameAndType(string? i)
{
    var playerMenu = new Menu(EMenuLevel.Other, game.Players[int.Parse(i)].NickName, new List<MenuItem>()
        {
            new MenuItem(
                "Change NickName", 
                "n", 
                null),
            new MenuItem(
                "Change PlayerType", 
                "t", 
                showPlayersNamesAndTypes),
        }
    );
    
    //returns and activates new menu
    return playerMenu.Run();;
    return null;
}
//method to show players names and types
string? showPlayersNamesAndTypes(string? i1)
{
    var playersAsMenuItems = new List<MenuItem>();
    for (int i = 0; i < game.Players.Count; i++)
    {
        var strNr = i.ToString();
        playersAsMenuItems.Add(
            new MenuItem(
                game.Players[i].NickName,
                strNr,
                changePlayerNameAndType(strNr)//PROBLEEM!!
            ));
    }
    var playersMenu = new Menu(EMenuLevel.Other, "Players List", playersAsMenuItems);
    
    return playersMenu.Run();
}
//method for picking New Game
string? runNewGameMenu(string? i)
{
    //constructing new menu with items
    var startNewGameMenu = new Menu(EMenuLevel.Second, "New Game", new List<MenuItem>()
        {
            new MenuItem(
                "Player count: "+game.Players.Count, 
                "c", 
                setPlayerCount),
            new MenuItem(
                "Show players", 
                "t", 
                showPlayersNamesAndTypes),
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
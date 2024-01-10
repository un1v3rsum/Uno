using System.Text;
using DAL;
using Domain;
using UnoEngine;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using UnoConsoleUI;

//DATABASES lectures 20.10 (sql statements and logger) & 27.10 (game db)
//*entity framework has entity tracking attribute (has a reference to an object "somewhere" and tracks it )
//if we fetched some kind of data from db then for a while some data is stored in memory
//sometimes smth works and sometimes it doesn't (tracking can be switched off, makes code a bit faster)

//encoding for emoji output
Console.OutputEncoding = Encoding.UTF8;

//<<<================== LOCAL SAVE OPTIONS ===============>>>
// //define gameRepository (local)
// //*comment out*
// var gameRepository = new GameRepositoryFileSystem(); 
//<<<================== LOCAL SAVE OPTIONS ===============>>>


//<<<====================== DATABASE SAVE OPTIONS ====================>>>

//create connectionstring, so that it would work in everybodys computer
var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());

//define gameRepository (db)
//create contextOptions
var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableDetailedErrors()
    .Options;
//if used as "using var db" then system disposes db connection automatically
//no need for ->  db.dispose() afterwards
using var db = new AppDbContext(contextOptions);
//create database and do the migrations if database doesnt already exist
//db.Database.Migrate();
//create gamerepository in db
var gameRepository = new GameRepositoryEF(db);
//<<<====================== DATABASE SAVE OPTIONS ====================>>>

//<<<====================== CREATE THE GAME ====================>>>
//create gameEngine
var game = new GameEngine(gameRepository);
//create gameController
var gameController = new GameController(game,gameRepository);

//Options MENU
string? RunOptionsMenu()
{
    //constructing new optionsMenu with items
    var optionsGameMenu = new Menu(EMenuLevel.Second, "Options", new List<MenuItem>()
        {
            new MenuItem(
                "No of card-packs used: "+ game.State.CardDeck.Size, 
                (() => "No of card-packs used: "+game.State.CardDeck.Size.ToString()),//anonym. func
                "p", 
                game.SetDeckSizeConsole
                ),
            //TODO modernDeckType UNO after year 2018
            // new MenuItem(
            //     "Type of deck used: "+ game.State.CardDeck.DeckType, 
            //     (() => "Type of deck used: "+game.State.CardDeck.DeckType.ToString()),
            //     "t", 
            //     setDeckType
            // ),
            new MenuItem(
                "Type of deck used: "+ game.State.CardDeck.DeckType, 
                null,
                "t", 
                null
            ),
        }
    );
    
    //returns and activates new menu
    return optionsGameMenu.Run();;
}
//New Game MENU
string? RunNewGameMenu()
{
    //constructing new menu with items
    var startNewGameMenu = new Menu(EMenuLevel.Second, "New Game", new List<MenuItem>()
        {
            new MenuItem(
                "Player count: "+game.State.Players.Count, 
                (() => "Player count: "+game.State.Players.Count.ToString()),
                "c", 
                game.SetPlayerCountConsole),
            new MenuItem(
                "Edit players: " + string.Join(", ", game.State.Players), 
                (() => "Edit players: "+ string.Join(", ", game.State.Players)),
                "t", 
                game.EditPlayerNamesAndTypesConsole),
            new MenuItem(
                "Start the game of UNO", 
                null,
                "s", 
                gameController.MainLoop),
        }
    );
    //returns and activates new menu
    return startNewGameMenu.Run();
}
//Main Menu
var mainMenu = new Menu(EMenuLevel.First,">> UNO <<", new List<MenuItem>()
{
    new MenuItem(
        "Start a new game", 
        null,
        "s", 
        RunNewGameMenu),
    new MenuItem(
        "Load game", 
        null,
        "l", 
        LoadGame),
    new MenuItem(
        "Options", 
        null,
        "o", 
        RunOptionsMenu),
});

//method for loading saved games
string? LoadGame()
{
    var saveGameList = gameRepository.GetSaveGames();
    var saveGameListDisplay = saveGameList.Select((s, i) => (i + 1) + " - " + s).ToList();

    if (saveGameListDisplay.Count == 0)
    {
        Console.WriteLine("No saved games! Press any key to return!");
        Console.ReadLine();
        return null;
    }
    
    Console.WriteLine("Saved games");
    Guid gameId;
    //ask the player what game to load until they insert valid choice
    while (true)
    {
        Console.WriteLine(string.Join("\n", saveGameListDisplay));
        Console.Write($"Select game to load (1..{saveGameListDisplay.Count}):");
        var userChoiceStr = Console.ReadLine();
        if (int.TryParse(userChoiceStr, out var userChoice))
        {
            if (userChoice < 1 || userChoice > saveGameListDisplay.Count)
            {
                Console.WriteLine("Not in range...");
                continue;
            }

            gameId = saveGameList[userChoice - 1].id;
            Console.WriteLine($"Loading file: {gameId}");

            break;
        }

        Console.WriteLine("Parse error...");
    }
    //generate a gamestate from selected json file
    var gameState = gameRepository.LoadGame(gameId);
    //generate a gameengine from previously generated state
    var gameEngine = new GameEngine(gameRepository)
    {
        State = gameState
    };
    gameEngine.State.TurnResult = ETurnResult.LoadGame;
    //generate gamecontroller from gameengine and gamerepository
    var gameController = new GameController(gameEngine, gameRepository);
    //run the game
    gameController.MainLoop();
    return null;
}

//<<<====================== START THE GAME from MAIN MENU ====================>>>
mainMenu.Run();



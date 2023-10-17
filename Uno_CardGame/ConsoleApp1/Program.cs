using System.Text;
using DAL;
using Domain;
using UnoEngine;
using MenuSystem;
using UnoConsoleUI;

//encoding for emoji output
Console.OutputEncoding = Encoding.UTF8;
//define gameRepository
var gameRepository = new GameRepositoryFileSystem();
//define gameEngine
var game = new GameEngine<string>(gameRepository);
//define gameController
var gameController = new GameController<string>(game);
//method for setting playerCount 
string setPlayerCount()
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
    game.State.Players = new List<Player>();
    for (int i = 0; i < count; i++)
    {
        game.State.Players.Add(new Player()
        {
            NickName   = "Player " + (i+1),
            PlayerType = EPlayerType.Human,
            Position = i
        });
    }
    game.UpdateGame();
    return null;
}

string? editPlayerNamesAndTypes()
{
    for (int i = 0; i < game.State.Players.Count; i++)
    {
        bool realType = false;
        Console.Write("Enter " + (i+1) + ". player name: ");
        game.State.Players[i].NickName = Console.ReadLine();
        do
        { 
            Console.Write("Is player " + (i+1) + " human (press: h) or AI (press: a)?: ");
            var answer = Console.ReadLine();
            if (answer != "h" && answer != "a")
            {
                Console.WriteLine("ERROR! Press the letter 'h' or 'a' on your keyboard.");
            }
            switch (answer)
            {
                case "h":
                    game.State.Players[i].PlayerType = EPlayerType.Human;
                    realType = true;
                    break;
                case "a":
                    game.State.Players[i].PlayerType = EPlayerType.AI;
                    realType = true;
                    break;
            }
        } while (realType == false);
    }
    game.UpdateGame();
    return null;
}

/*string? openPlayerMenu(string shortCut)
{
    var position = int.Parse(shortCut) - 1;
    var playerMenu = new Menu(EMenuLevel.Other, game.State.Players[position].NickName, new List<MenuItem>()
        {
            new MenuItem(
                "Edit name: " + game.State.Players[position].NickName,
                null,
                "n",
                null),
            new MenuItem(
                "Edit type: " + game.State.Players[position].PlayerType,
                null,
                "t",
                null)
        }
    );
    return null;
}*/

//method to show players names and types
string? showPlayersNamesAndTypes()
{
    var playersAsMenuItems = new List<MenuItem>();
    for (int i = 0; i < game.State.Players.Count; i++)
    {
        playersAsMenuItems.Add(
            new MenuItem(
                game.State.Players[i].NickName + " " + game.State.Players[i].PlayerType,
                null,
                (i+1).ToString(),
                null
            ));
    }
    playersAsMenuItems.Add(
        new MenuItem(
            "EDIT players",
            null,
            "e",
            editPlayerNamesAndTypes
            ));
    
    var playersMenu = new Menu(EMenuLevel.Other, "Players List", playersAsMenuItems);
    return playersMenu.Run();
}

string? setDeckSize()
{
    Console.WriteLine("Max 3 packs of cards can be used.");
    bool correctCount = false;
    do
    {
        Console.Write("Insert nr of packs:"); 
        var countStr = Console.ReadLine();
        if (countStr is "1" or "2" or "3")
        {
            game.State.CardDeck.Size = int.Parse(countStr);
            correctCount = true;
        }
        else
        {
            Console.WriteLine("ERROR! You have to insert an integer between 1 - 3.");
        }
    } while (correctCount == false);
    game.UpdateGame();
    return null;
}

string? setDeckType()
{
    game.State.CardDeck.DeckType = 
        (game.State.CardDeck.DeckType == ECardDeckType.Modern) ?
            ECardDeckType.Original : ECardDeckType.Modern;
    //game.State.CardDeck.DeckType += 1;
    game.UpdateGame();
    return null;
}

string? runOptionsMenu()
{
    //constructing new optionsMenu with items
    var optionsGameMenu = new Menu(EMenuLevel.Second, "Options", new List<MenuItem>()
        {
            new MenuItem(
                "No of card-packs used: "+ game.State.CardDeck.Size, 
                (() => "No of card-packs used: "+game.State.CardDeck.Size.ToString()),//anonym. func
                "p", 
                setDeckSize
                ),
            //TODO modernDeckType 
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
//method for picking New Game
string? runNewGameMenu()
{
    //constructing new menu with items
    var startNewGameMenu = new Menu(EMenuLevel.Second, "New Game", new List<MenuItem>()
        {
            new MenuItem(
                "Player count: "+game.State.Players.Count, 
                (() => "Player count: "+game.State.Players.Count.ToString()),
                "c", 
                setPlayerCount),
            new MenuItem(
                "Show players: " + string.Join(", ", game.State.Players), 
                (() => "Show players: "+ string.Join(", ", game.State.Players)),
                "t", 
                editPlayerNamesAndTypes),
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
//Initial mainMenu 
var mainMenu = new Menu(EMenuLevel.First,">> UNO <<", new List<MenuItem>()
{
    new MenuItem(
        "Start a new game", 
        null,
        "s", 
        runNewGameMenu),
    new MenuItem(
        "Load game", 
        null,
        "l", 
        loadGame),
    new MenuItem(
        "Options", 
        null,
        "o", 
        runOptionsMenu),
});

string loadGame()
{
    game.State = game.LoadGame();
    return null;
}
//run the consoleApp 
mainMenu.Run();
game.ShowPlayerHand();
//game.State.Players[0].PlayerHand.AddRange(game.State.CardDeck.Draw(3));
//game.ShowPlayerHand();
Console.WriteLine(game.State.CardDeck.DeckType);
Console.WriteLine(game.State.CardDeck.Cards.Count + 14 + 1);


//game.SaveGame();


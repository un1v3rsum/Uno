﻿using System.Text;
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
var game = new GameEngine(gameRepository);
//define gameController
var gameController = new GameController(game,gameRepository);
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
            Score = 0,
        });
    }
    game.UpdateGame();
    return null;
}
//method for changing players names and types
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

//method for changing the deck size
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
//TODO 
string? setDeckType()
{
    game.State.CardDeck.DeckType = (game.State.CardDeck.DeckType == ECardDeckType.Modern) 
            ? ECardDeckType.Original 
            : ECardDeckType.Modern;
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
                "Edit players: " + string.Join(", ", game.State.Players), 
                (() => "Edit players: "+ string.Join(", ", game.State.Players)),
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
        LoadGame),
    new MenuItem(
        "Options", 
        null,
        "o", 
        runOptionsMenu),
});

//method for loading locally saved games
string? LoadGame()
{
    //get locally saved json files as a list and show them on console one bye one
    Console.WriteLine("Saved games");
    var saveGameList = gameRepository.GetSaveGames();
    var saveGameListDisplay = saveGameList.Select((s, i) => (i + 1) + " - " + s).ToList();

    if (saveGameListDisplay.Count == 0) return null;
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
//run the mainMenu 
mainMenu.Run();



﻿using Domain;
using UnoEngine;
using MenuSystem;
//define gameEngine
var game = new GameEngine();
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
    game.Players = new List<Player>();
    for (int i = 0; i < count; i++)
    {
        game.Players.Add(new Player()
        {
            NickName   = "Human " + (i+1),
            PlayerType = EPlayerType.Human,
        });
    }
    return null;
}
//method to change the name or type of the player
string changePlayerNameAndType()
{
    var playerMenu = new Menu(EMenuLevel.Other, game.Players[int.Parse("1")].NickName, new List<MenuItem>()
        {
            new MenuItem(
                "Change NickName", 
                "n", 
                null),
            new MenuItem(
                "Change PlayerType", 
                "t", 
                null),
        }
    );
    
    //returns and activates new menu
    return playerMenu.Run();;
    return null;
}

string? editPlayerNamesAndTypes()
{
    for (int i = 0; i < game.Players.Count; i++)
    {//TODO namecheck
        bool realType = false;
        Console.Write("Enter " + (i+1) + ". player name: ");
        game.Players[i].NickName = Console.ReadLine();
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
                    game.Players[i].PlayerType = EPlayerType.Human;
                    realType = true;
                    break;
                case "a":
                    game.Players[i].PlayerType = EPlayerType.AI;
                    realType = true;
                    break;
            }
        } while (realType == false);
    }

    return null;
}
//method to show players names and types
string? showPlayersNamesAndTypes()
{
    var playersAsMenuItems = new List<MenuItem>();
    for (int i = 0; i < game.Players.Count; i++)
    {
        playersAsMenuItems.Add(
            new MenuItem(
                game.Players[i].NickName + " " + game.Players[i].PlayerType,
                (i+1).ToString(),
                null//TODO kuidas kasutada ühe sisendparameetriga meetodit !!
            ));
    }
    playersAsMenuItems.Add(
        new MenuItem(
            "EDIT players",
            "e",
            editPlayerNamesAndTypes
            ));
    
    var playersMenu = new Menu(EMenuLevel.Other, "Players List", playersAsMenuItems);
    return playersMenu.Run();
}

string? setDeckSize()//TODO
{
    Console.WriteLine("Max 3 packs of cards can be used.");
    bool correctCount = false;
    do
    {
        Console.Write("Insert nr of packs:"); 
        var countStr = Console.ReadLine();
        if (int.Parse(countStr) is < 1 or > 3)
        {
            Console.WriteLine("ERROR! You have to insert an integer between 2 - 10.");
        }
        else
        {
            game.CardDeck.Size = int.Parse(countStr);
            correctCount = true;
        }
    } while (correctCount == false);
    return null;
}
string? runOptionsMenu()
{
    //constructing new menu with items
    var optionsGameMenu = new Menu(EMenuLevel.Second, "Options", new List<MenuItem>()
        {
            new MenuItem(
                "Cards used: "+game.CardDeck.Size, 
                "p", 
                setDeckSize),
        }
    );
    //returns and activates new menu
    return optionsGameMenu.Run();
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
        runOptionsMenu),
});
//run the consoleApp 
mainMenu.Run();
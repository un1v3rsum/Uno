namespace MenuSystem;
//menu class
public class Menu
{
    private readonly EMenuLevel _level;
    public string? Title { get; set; }
    //container for menuItems
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new();
    private const string MenuSeparator = "=======================";
    private const string ShortcutExit = "x";
    private const string ShortcutBack = "b";
    private const string ShortcutReturnToMain = "r";
    //private menuItems, not accessible by the player 
    private readonly MenuItem _menuItemExit = new MenuItem("Exit", ShortcutExit, null);
    private readonly MenuItem _menuItemBack = new MenuItem("Back", ShortcutBack, null);
    private readonly MenuItem _menuItemReturnToMain = new MenuItem("Return to Main",  ShortcutReturnToMain, null);
    
    public Menu(EMenuLevel level, string? title, List<MenuItem> menuItems)
    {
        _level = level;
        Title = title;
        foreach (var menuItem in menuItems)
        {   
            //exception for repeating shortcuts
            if (MenuItems.ContainsKey(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException(
                    $"Menu shortcut '{menuItem.Shortcut.ToLower()}' is already registered!");
            }
            //loop through all menuItems and add to dict
            MenuItems.Add(menuItem.Shortcut, menuItem);
            
        }
        //BACK not shown in the first level
        if (_level != EMenuLevel.First)
            MenuItems.Add(ShortcutBack, _menuItemBack);
        //RETURN TO MAIN only shown in lower levels (not 1. & 2.)
        if (_level == EMenuLevel.Other)
            MenuItems.Add(ShortcutReturnToMain, _menuItemReturnToMain);
        //EXIT seen in every level
        MenuItems.Add(ShortcutExit, _menuItemExit);
    }
    //method for drawing the menusystem
    private void Draw()
    {
        //Console.WriteLine("size of deck: "+ CardDeck.Cards.Count); //*check for the deckSize befor shuffle
        //foreach (var card in CardDeck.Cards){Console.WriteLine(card.CardColor + ""+ card.CardValue);}//*SHOW CARDS
        //CardDeck.Shuffle();//*SHUFFLE DECK
        //foreach (var card in CardDeck.Cards){Console.WriteLine(card.CardColor + ""+ card.CardValue);}//*SHOW CARDS
        //Console.WriteLine("size of deck: "+ CardDeck.Cards.Count); //*check for the deckSize after shuffle
        if (!string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine(MenuSeparator);
        }
        //loop through all the menuItems and write them on console
        foreach (var menuItem in MenuItems)
        {
            Console.Write(menuItem.Key);
            Console.Write(") ");
            Console.WriteLine(menuItem.Value.MenuLabel);
        }
        Console.WriteLine(MenuSeparator);
        Console.Write("Your choice:");
    }
    //method for running the menuSystem
    public string Run()
    {   
        //clear the buffer
        Console.Clear();
        //boolean for do-while loop
        var menuDone = false;
        var userChoice = "";
        do//loop
        {
            //draw menuItems
            Draw();
            userChoice = Console.ReadLine()?.ToLower().Trim();
            if (MenuItems.ContainsKey(userChoice))
            {
                string? result = null;
                if (MenuItems[userChoice].MethodToRun != null)
                {
                    result = MenuItems[userChoice].MethodToRun!();
                }

                if (userChoice == ShortcutBack)
                {
                    menuDone = true;
                }

                if (userChoice == ShortcutExit || result == ShortcutExit)
                {
                    //if result != null -> uses result, else uses userChoice
                    userChoice = result ?? userChoice;
                    menuDone = true;
                }

                if ((userChoice == ShortcutReturnToMain || result == ShortcutReturnToMain) && _level != EMenuLevel.First)
                {
                    //if result != null -> uses result, else uses userChoice
                    userChoice = result ?? userChoice;
                    menuDone = true;
                }
            }
            else //*error*
            {
                Console.WriteLine("Undefined shortcut....");
            }

            Console.WriteLine();
            //menuSystem stops running if menuDone = true
        } while (menuDone == false);
        
        return userChoice;
    }
}

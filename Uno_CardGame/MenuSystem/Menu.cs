namespace MenuSystem;

public class Menu
{
    public string? Title { get; set; }
    public Dictionary<string, MenuItem> MenuItems { get; set; } = new();

    private const string MenuSeparator = "=======================";
    private static readonly HashSet<string> ReservedShortcuts = new() {"x", "b", "r"};

    public Menu(string? title, List<MenuItem> menuItems)
    {
        Title = title;
        foreach (var menuItem in menuItems)
        {
            if (ReservedShortcuts.Contains(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException(
                    $"Menu shortcut '{menuItem.Shortcut.ToLower()}' in not allowed list!");
            }

            if (MenuItems.ContainsKey(menuItem.Shortcut.ToLower()))
            {
                throw new ApplicationException(
                    $"Menu shortcut '{menuItem.Shortcut.ToLower()}' is already registered!");
            }

            MenuItems[menuItem.Shortcut.ToLower()] = menuItem;
        }
    }

    private void Draw()
    {
        if (!string.IsNullOrWhiteSpace(Title))
        {
            Console.WriteLine(Title);
            Console.WriteLine(MenuSeparator);
        }

        foreach (var menuItem in MenuItems)
        {
            Console.Write(menuItem.Key);
            Console.Write(") ");
            Console.WriteLine(menuItem.Value.MenuLabel);
        }

        // TODO: should not be there in the main level
        Console.WriteLine("b) Back");
        // TODO: should not be there in the main and second level
        Console.WriteLine("r) Return to main");

        Console.WriteLine("x) eXit");

        Console.WriteLine(MenuSeparator);
        Console.Write("Your choice:");
    }

    public string Run(EMenuLevel menuLevel = EMenuLevel.First)
    {
        Console.Clear();

        var userChoice = "";
        do
        {
            Draw();
            userChoice = Console.ReadLine()?.Trim();

            if (MenuItems.ContainsKey(userChoice?.ToLower()))
            {
                if (MenuItems[userChoice!.ToLower()].SubMenuToRun != null)
                {
                    var result = "";
                    if (menuLevel == EMenuLevel.First)
                    {
                         result = MenuItems[userChoice!.ToLower()].SubMenuToRun!(EMenuLevel.Second);
                    }
                    else
                    {
                        result = MenuItems[userChoice!.ToLower()].SubMenuToRun!(EMenuLevel.Other);
                    }
                    // TODO:  handle result - b,x,r
                    
                }
                else if (MenuItems[userChoice!.ToLower()].MethodToRun != null)
                {
                    var result = MenuItems[userChoice!.ToLower()].MethodToRun!();
                    if (result?.ToLower() == "x")
                    {
                        userChoice = "x";
                    }
                }
            }
            else if (!ReservedShortcuts.Contains(userChoice?.ToLower()))
            {
                Console.WriteLine("Undefined shortcut....");
            }

            Console.WriteLine();
        } while (!ReservedShortcuts.Contains(userChoice));

        return userChoice;
    }
}

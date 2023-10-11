﻿namespace MenuSystem;
//menuitem class with 2 string attributes (label & shortcutkey) and one method 
public class MenuItem
{
    public string MenuLabel { get; set; } = default!;
    public string Shortcut { get; set; } = default!;
    public Func<string?,string?>? MethodToRun { get; set; }
    
    //menuItem constructor
    public MenuItem(string title, string shortcut, Func<string?,string?>? methodToRun)
    {
        MenuLabel = title;
        Shortcut = shortcut;
        MethodToRun = methodToRun;
    }

}
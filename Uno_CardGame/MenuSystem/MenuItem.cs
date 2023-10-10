﻿namespace MenuSystem;
//menuitem class with 2 string attributes (label & shortcutkey) and 2 methods (delegates)
public class MenuItem
{
    public string MenuLabel { get; set; } = default!;
    public string Shortcut { get; set; } = default!;
    public Func<string?>? MethodToRun { get; set; } = null;
    //TODO SubMenuToRun delegate ex!?
    //public Func<EMenuLevel, string?>? SubMenuToRun { get; set; } = null;
    
    //menuItem constructor
    public MenuItem(string title, string shortcut, Func<string>? methodToRun )
    {
        MenuLabel = title;
        Shortcut = shortcut;
        MethodToRun = methodToRun;
    }

}
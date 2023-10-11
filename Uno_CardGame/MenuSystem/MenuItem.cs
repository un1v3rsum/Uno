namespace MenuSystem;
//menuitem class with 2 string attributes (label & shortcutkey) and one method 
public class MenuItem
{
    public string MenuLabel { get; set; } = default!;
    public string Shortcut { get; set; } = default!;
    public Func<string?>? MethodToRun { get; set; } = null;
    
    //menuItem constructor
    public MenuItem(string title, string shortcut, Func<string>? methodToRun )
    {
        MenuLabel = title;
        Shortcut = shortcut;
        MethodToRun = methodToRun;
    }

}
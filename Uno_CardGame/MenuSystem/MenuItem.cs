namespace MenuSystem;
//menuitem class with 2 string attributes (label & shortcutkey) and two methods  
public class MenuItem
{
    public string MenuLabel { get; set; } = default!;
    //menuLabel function for dynamic label changing
    public Func<string>? MenuLabelFunction { get; set; }
    public string Shortcut { get; set; } = default!;
    public Func<string?>? MethodToRun { get; set; } 
    
    
    //menuItem constructor
    public MenuItem(string title, Func<string>? menuLabelFunction,string shortcut, Func<string>? methodToRun)
    {
        MenuLabel = title;
        MenuLabelFunction = menuLabelFunction;
        Shortcut = shortcut;
        MethodToRun = methodToRun;
    }

}
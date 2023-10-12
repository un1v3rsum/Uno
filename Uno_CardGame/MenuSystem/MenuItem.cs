namespace MenuSystem;
//menuitem class with 2 string attributes (label & shortcutkey) and one method 
public class MenuItem
{
    public string MenuLabel { get; set; } = default!;
    public string Shortcut { get; set; } = default!;
    public Func<string>? MethodToRun { get; set; }
    //public Func<string,string>? MethodToRun { get; set; } //tahaks, et tulevikus määratud meetod võtaks sisendparameetri,
    //kuid selline func ei tööta
    
    //menuItem constructor
    public MenuItem(string title, string shortcut, Func<string>? methodToRun)
    {
        MenuLabel = title;
        Shortcut = shortcut;
        MethodToRun = methodToRun;
    }

}
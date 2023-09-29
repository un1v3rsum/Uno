namespace MenuSystem;

public class MenuItem
{
    public string MenuLabel { get; set; } = default!;
    public string Shortcut { get; set; } = default!;
    public Func<string?>? MethodToRun { get; set; } = null;
    public Func<EMenuLevel, string?>? SubMenuToRun { get; set; } = null;
}
namespace Domain;

public class History
{   
    //PK <classname> + Id
    public int HistoryId { get; set; }
    
    public string GameState { get; set; } = default!;
    
    public DateTime UpdatedAt { get; set; }
    
    //will be FK <classname> + Id
    public int GameId { get; set; }
    public Game? Game { get; set; }
}
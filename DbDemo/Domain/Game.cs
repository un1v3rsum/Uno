using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Game
{
    //Id-s should actually be Guid and not int type
    public int GameId { get; set; }
    
    [MaxLength(128)]
    public string GameName { get; set; } = default!;
    
    public ICollection<History> Histories { get; set; }
    
}
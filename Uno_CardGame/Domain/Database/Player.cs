using System.ComponentModel.DataAnnotations;
using UnoEngine;

namespace Domain.Database;

public class Player : BaseEntity
{
    
    //reflection based utility like entity framework needs parameterless constructors to work its magic
    //if we construct an object and hardcode the parameters then EntityFramework doesnt work
    
    [MaxLength(128)]
    public string NickName { get; set; } = default!;
    
    public EPlayerType PlayerType { get; set; }
    //GameId can't be null because every player has to have 1 game relationship
    public Guid GameId { get; set; }
    public Game? Game { get; set; } 
    //nullability decides relationship type
}
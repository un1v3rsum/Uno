using System.ComponentModel.DataAnnotations;
using UnoEngine;

namespace Domain.Database;

public class Player
{
    [MaxLength(128)]
    public string NickName { get; set; } = default!;
    
    public EPlayerType PlayerType { get; set; }
    //GameId can't be null because every player has to have 1 game relationship
    public Guid GameId { get; set; }
    public Game? Game { get; set; } 
}
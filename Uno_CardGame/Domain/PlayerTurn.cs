using UnoEngine;

namespace Domain;

public class PlayerTurn
{
    public GameCard Card { get; set; }
    public ECardColor DeclaredColor { get; set; }
    public ETurnResult Result { get; set; }
    
}
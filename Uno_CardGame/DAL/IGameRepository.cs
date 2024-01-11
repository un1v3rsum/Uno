using Domain;

namespace DAL;
//GAMEREPOSITOR INTERFACE
public interface IGameRepository
{
    //method for saving the game
    void SaveGame(Guid id, GameState state);
    //tuple of locally saved game Guids
    List<(Guid id, DateTime dt)> GetSaveGames();
    //method for saving the game
    GameState LoadGame(Guid id);
}
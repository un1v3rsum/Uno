using Domain;

namespace DAL;
//saving game locally
public interface IGameRepository
{
    void SaveGame(Guid id, GameState state);
    //tuple of locally saved game Guids
    List<(Guid id, DateTime dt)> GetSaveGames();
    GameState LoadGame(Guid id);
}
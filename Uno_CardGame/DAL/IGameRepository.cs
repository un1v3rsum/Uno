using Domain;

namespace DAL;
//saving game locally
public interface IGameRepository
{
    void SaveGame(Guid id, GameState game);
    //list of locally saved games
    List<(Guid id, DateTime dt)> GetSaveGames();
    GameState LoadGame(Guid id);
}
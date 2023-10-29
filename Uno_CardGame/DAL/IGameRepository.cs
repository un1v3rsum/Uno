using Domain;

namespace DAL;
//saving game locally
public interface IGameRepository<TKey>
{
    TKey SaveGame(object? id, GameState game);
    GameState LoadGame(object? id);
}
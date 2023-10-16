using Domain;

namespace DAL;

public interface IGameRepository<TKey>
{
    TKey SaveGame(object? id, GameState game);
    GameState LoadGame(TKey id);
}
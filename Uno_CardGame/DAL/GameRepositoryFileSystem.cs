using System.Text.Json;
using Domain;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository<string>
{
    private readonly string _filePrefix = "." + System.IO.Path.DirectorySeparatorChar;

    public string SaveGame(object? id, GameState game)
    {
        var fileName = (string?)id  ?? "uno-" + DateTime.Now.ToFileTime() + ".json";
        System.IO.File.WriteAllText(_filePrefix + fileName, JsonSerializer.Serialize(game));
        return fileName;
    }

    public GameState LoadGame(string id)
    {
        return JsonSerializer.Deserialize<GameState>(
            System.IO.File.ReadAllText(_filePrefix + id)
        )!;
    }

}
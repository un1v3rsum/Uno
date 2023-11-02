using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

namespace DAL;
public class GameRepositoryFileSystem : IGameRepository
{
    private readonly string _filePrefix = "." + System.IO.Path.DirectorySeparatorChar;
    private const string SaveLocation = "/Users/vladi/UnoGames";

    //public string SaveGame(object? id, GameState game)
    public void SaveGame(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);

        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }
        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);

       // var fileName = "";
        //if (id != null)
        //{ 
         //   fileName = id.ToString() + ".json";
        //}
        //else
        //{
         //   fileName = "uno-" + DateTime.Now.ToFileTime() + ".json";
        //}
        //System.IO.File.WriteAllText(_filePrefix + fileName, JsonSerializer.Serialize(game));
        //return fileName;
    }

    //return a list tuples of saveGames with the last write time 
    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        //get list of file names
        var data = Directory.EnumerateFiles(SaveLocation);
        var res = data
            .Select(
                path => (
                    Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                    File.GetLastWriteTime(path)
                )
            ).ToList();
        
        return res;

    }

    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        var jsonStr = File.ReadAllText(Path.Combine(SaveLocation, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;

    }

    //public GameState LoadGame(object? id)
    //{
       // return JsonSerializer.Deserialize<GameState>(
        //    System.IO.File.ReadAllText(_filePrefix + id + ".json")
      //  )!;
    //}
}
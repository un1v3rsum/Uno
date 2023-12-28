using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

namespace DAL;
public class GameRepositoryFileSystem : IGameRepository
{
    //find the savelocation from locally accessible folder
    static string SaveLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    //combine found path with "folderName"
    private string saveGamesPath = System.IO.Path.Combine(SaveLocation, "UnoSaveGames");
    
    public void SaveGame(Guid id, GameState state)
    {
        //transform the state into json
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);   
        //create filename
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(saveGamesPath))
        {
            Directory.CreateDirectory(saveGamesPath);
        }
        File.WriteAllText(Path.Combine(saveGamesPath, fileName), content);
    }

    //return a list tuples of saveGames with the last write time 
    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        if (!Path.Exists(saveGamesPath))
        {
            Directory.CreateDirectory(saveGamesPath);
        }
        //get list of file names
        var data = Directory.EnumerateFiles(saveGamesPath);
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

        var jsonStr = File.ReadAllText(Path.Combine(saveGamesPath, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;
    }
    
}
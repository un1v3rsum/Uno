using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

namespace DAL;
//<<========================= LOCAL GAMEREPOSITORY ==============================>>
public class GameRepositoryFileSystem : IGameRepository
{
    //find the savelocation from locally accessible folder
    static string _saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    //combine found path with "folderName"
    private string _saveGamesPath = System.IO.Path.Combine(_saveLocation, "UnoSaveGames");
    
    public void SaveGame(Guid id, GameState state)
    {
        //transform the state into json
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);   
        //create filename
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(_saveGamesPath))
        {
            Directory.CreateDirectory(_saveGamesPath);
        }
        File.WriteAllText(Path.Combine(_saveGamesPath, fileName), content);
    }

    //return a list tuples of saveGames with the last write time 
    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        if (!Path.Exists(_saveGamesPath))
        {
            Directory.CreateDirectory(_saveGamesPath);
        }
        //get list of file names
        var data = Directory.EnumerateFiles(_saveGamesPath);
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

        var jsonStr = File.ReadAllText(Path.Combine(_saveGamesPath, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelpers.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;
    }
    
}
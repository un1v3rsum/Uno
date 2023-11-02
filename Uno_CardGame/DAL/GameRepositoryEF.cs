using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;

namespace DAL;

public class GameRepositoryEF : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepositoryEF(AppDbContext context)
    {
        _context = context;
    }
    public void SaveGame(Guid id, GameState state)
    {
        //creates database repository
        //if there are no games in db, then creates it and adds it to list
        var game = _context.Games.FirstOrDefault(g => g.Id == state.Id);
        if (game == null)
        {
            game = new Game()
            {
                Id = state.Id,
                State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions),
                //for every player that is in the state, we want to convert it into a database player
                Players = state.Players.Select(p => new Domain.Database.Player()
                {
                    NickName = p.NickName,
                    PlayerType = p.PlayerType
                }).ToList()
            };
            _context.Games.Add(game);
        }
        //if there is a game entry with that Id then updates it
        else
        {
            game.UpdatedAtDt = DateTime.Now;
            game.State = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);
        }

        var changeCount = _context.SaveChanges();
        Console.WriteLine("savechanges: " + changeCount);
    }
    //return games from db as a list tuples
    public List<(Guid id, DateTime dt)> GetSaveGames()
    {
        return _context.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (g.Id, g.UpdatedAtDt))
            .ToList();
    } 
    //load a game from tuple
    public GameState LoadGame(Guid id)
    {
        var game = _context.Games.First(g => g.Id == id);
        return JsonSerializer.Deserialize<GameState>(game.State, JsonHelpers.JsonSerializerOptions);
    }
}
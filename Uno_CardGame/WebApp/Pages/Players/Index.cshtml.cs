using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Database;
using UnoEngine;
using Player = Domain.Database.Player;

namespace WebApp.Pages_Players
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        private readonly IGameRepository _gameRepository = default!;
        public GameEngine GameEngine { get; set; } = default!;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
            _gameRepository = new GameRepositoryEF(_context);
        }
        public IList<Game> Games { get;set; } = default!;
        public IList<Player> Players { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Players != null)
            {
                Players = await _context.Players
                .Include(p => p.Game).ToListAsync();
            }
            if (_context.Games != null)
            {
                Games = await _context.Games
                    .Include(g => g.Players)
                    .OrderByDescending(g => g.UpdatedAtDt)
                    .ToListAsync();
            }
            foreach (var game in Games)
            {

                var gameState = _gameRepository.LoadGame(game.Id);
                GameEngine = new GameEngine(_gameRepository)
                {
                    State = gameState
                };
                
                foreach (var player in Players)
                {
                    foreach (var p in GameEngine.State.Players)
                    {
                        if (player.Id == p.Id)
                        {
                            p.NickName = player.NickName;
                            p.PlayerType = player.PlayerType;
                        }
                    }

                    _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);
                }
            }
        }
    }
}

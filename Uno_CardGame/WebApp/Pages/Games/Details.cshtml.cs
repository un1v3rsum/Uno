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

namespace WebApp.Pages_Games
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        private readonly IGameRepository _gameRepository = default!;
        public GameEngine GameEngine { get; set; } = default!;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
            _gameRepository = new GameRepositoryEF(_context);
        }
        //attribute to get the Id values
        [BindProperty(SupportsGet = true)]public Guid GameId { get; set; }
        [BindProperty(SupportsGet = true)]public Guid PlayerId { get; set; }
        public Game Game { get; set; } = default!;
        public int playerCount = default!;


        
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game =  await _context.Games
                .Include(g => g.Players)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            Game = game;
            return Page();
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL;
using Domain.Database;
using UnoEngine;

namespace WebApp.Pages_Games
{
    public class CreateModel : PageModel
    {
        private readonly DAL.AppDbContext _context;
        private readonly IGameRepository _gameRepository = default!;
        public GameEngine GameEngine { get; set; } = default!;

        [BindProperty]public int Number1 { get; set; }
        [BindProperty]public int Number2 { get; set; }
        [BindProperty]public int DeckSize { get; set; }
        [BindProperty]public int HandSize { get; set; }
        public CreateModel(DAL.AppDbContext context)
        {
            _context = context;
            _gameRepository = new GameRepositoryEF(_context);
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] public Game Game { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (_context.Games == null || Game == null)
          {
              return Page();
          }

          GameEngine = new GameEngine(_gameRepository);
          GameEngine.SetPlayerCount(Number1,Number2);
          GameEngine.SetDeckSize(DeckSize);
          GameEngine.SetHandSize(HandSize);
          GameEngine.UpdateGame();
          _gameRepository.SaveGame(GameEngine.State.Id, GameEngine.State);
          
            return RedirectToPage("./Index");
        }
    }
}

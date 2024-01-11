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

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        public Game Game { get; set; } = default!;


        
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }
            //gets the game
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

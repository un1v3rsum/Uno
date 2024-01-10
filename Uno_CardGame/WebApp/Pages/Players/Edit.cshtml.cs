using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Database;
using UnoEngine;
using Player = Domain.Database.Player;

namespace WebApp.Pages_Players
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
        {
            _context = context;
           
        }
        [BindProperty]public Player Player { get; set; } = default!;
        [BindProperty]public IList<Game> Games { get;set; } = default!;
        [BindProperty]public IList<Player> Players { get;set; } = default!;
        
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player =  await _context.Players.FirstOrDefaultAsync(m => m.Id == id);
            
            if (player == null)
            {
                return NotFound();
            }
            
            if (_context.Games != null)
            {
                Games = await _context.Games
                    .Include(g => g.Players)
                    .OrderByDescending(g => g.UpdatedAtDt)
                    .ToListAsync();
            }
            
            if (_context.Players != null)
            {
                Players = await _context.Players
                    .Include(p => p.Game).ToListAsync();
            }
            
            Player = player;
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "State");

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Attach(Player).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(Player.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return RedirectToPage("./Index");
        }

        private bool PlayerExists(Guid id)
        {
          return (_context.Players?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

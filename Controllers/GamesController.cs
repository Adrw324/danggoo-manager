using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using danggoo_manager.Data;
using danggoo_manager.Models;

namespace danggoo_manager.Controllers
{
    public class GamesController : Controller
    {
        private readonly dmContext _context;

        public GamesController(dmContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
              return _context.Game != null ? 
                          View(await _context.Game.ToListAsync()) :
                          Problem("Entity set 'dmContext.Game'  is null.");
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Table_Num,Date,Start,End,Playtime,Fee,finished")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Table_Num,Date,Start,End,Playtime,Fee,finished")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Game == null)
            {
                return Problem("Entity set 'dmContext.Game'  is null.");
            }
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
          return (_context.Game?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        [HttpPost, HttpDelete, HttpGet]
        public async Task<IActionResult> Save(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }

            var game = await _context.Game.FindAsync(id);
            if(game == null || game.finished == false){
                return NotFound();
            }
            
            var newRecord = new Record
            {
                Table_Num = game.Table_Num,
                Date = game.Date,
                Start = game.Start,
                End = game.End,
                Playtime = game.Playtime,
                Fee = game.Fee
            };

            _context.Game.Remove(game);
            _context.Record.Add(newRecord);
            _context.SaveChanges();

            return RedirectToAction("Index", "Records");
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest("Invalid game data");
            }

            var existingGame = await _context.Game.FirstOrDefaultAsync(g =>
                g.Table_Num == game.Table_Num && g.Start == game.Start);

            if (existingGame == null)
            {
                _context.Game.Add(game);
            }
            else
            {
                // Update existing game with new data
                existingGame.End = game.End;
                existingGame.Playtime = game.Playtime;
                existingGame.Fee = game.Fee;
                existingGame.finished = game.finished;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Records");
        }
            
        
    }
}

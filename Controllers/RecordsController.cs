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
    public class RecordsController : Controller
    {

        public class RecordSaveData
        {
            public Record? record { get; set; }
            public int UtcOffsetMinutes { get; set; }
        }

        private readonly dmContext _context;

        public RecordsController(dmContext context)
        {
            _context = context;
        }

        // GET: Records
        public async Task<IActionResult> Index(string searchString, int? tableNumber, int? year, int? month, DateTime? day)
        {
            var records = from r in _context.Record
                          select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                records = records.Where(r => r.Table_Num.ToString().Contains(searchString));
            }

            if (tableNumber.HasValue)
            {
                records = records.Where(r => r.Table_Num == tableNumber);
            }

            if (year.HasValue)
            {
                records = records.Where(r => r.Date.Year == year);
            }

            if (month.HasValue)
            {
                records = records.Where(r => r.Date.Month == month);
            }

            if (day.HasValue)
            {
                records = records.Where(r => r.Date.Date == day.Value.Date);
            }

            return View(await records.ToListAsync());
        }


        // GET: Records/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Record == null)
            {
                return NotFound();
            }

            var @record = await _context.Record
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // GET: Records/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Records/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Table_Num,Date,Start,End,Playtime,Fee")] Record @record)
        {
            if (ModelState.IsValid)
            {
                @record.Date = @record.Date.Date; // 날짜만 추출
                @record.Start = @record.Start.ToLocalTime(); // 클라이언트로부터 전송된 시간을 로컬 시간으로 변환
                @record.End = @record.End.ToLocalTime(); //
                _context.Add(@record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@record);
        }

        // GET: Records/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Record == null)
            {
                return NotFound();
            }

            var @record = await _context.Record.FindAsync(id);
            if (@record == null)
            {
                return NotFound();
            }
            return View(@record);
        }

        // POST: Records/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Table_Num,Date,Start,End,Playtime,Fee")] Record @record)
        {
            if (id != @record.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordExists(@record.Id))
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
            return View(@record);
        }

        // GET: Records/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Record == null)
            {
                return NotFound();
            }

            var @record = await _context.Record
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // POST: Records/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Record == null)
            {
                return Problem("Entity set 'dmContext.Record'  is null.");
            }
            var @record = await _context.Record.FindAsync(id);
            if (@record != null)
            {
                _context.Record.Remove(@record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
         [HttpPost]
        public async Task<IActionResult> SaveRecord([FromBody] RecordSaveData data)
        {
            int utcOffsetMinutes = data.UtcOffsetMinutes;

            DateTime clientDate = data.record.Date.AddMinutes(-utcOffsetMinutes);
            DateTime clientStart = data.record.Start.AddMinutes(-utcOffsetMinutes);
            DateTime clientEnd = data.record.End.AddMinutes(-utcOffsetMinutes);

            Console.WriteLine(clientDate);
            Console.WriteLine(clientStart);
            Console.WriteLine(clientEnd);

            Record record = new Record
            {
                Table_Num = data.record.Table_Num,
                Date = clientDate,
                Start = clientStart,
                End = clientEnd,
                Playtime = data.record.Playtime,
                Fee = data.record.Fee
            };
            
            if (ModelState.IsValid)
            {
                _context.Add(record);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Record saved successfully." });
            }

            return Json(new { success = false, message = "Failed to save record." });
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
                // Update other properties as needed
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool RecordExists(int id)
        {
            return (_context.Record?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

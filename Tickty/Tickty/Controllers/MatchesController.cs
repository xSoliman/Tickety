using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tickty.Models;
using Tickty.ViewModels;

namespace Tickty.Controllers
{
    public class MatchesController : Controller
    {
        private readonly TicktyContext _context;

        public MatchesController(TicktyContext context)
        {
            _context = context;
        }

        // GET: Matches
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var ticktyContext = _context.Matches.Include(m => m.Stadium);
            return View(await ticktyContext.ToListAsync());
        }



        // GET: Matches/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var stadiums = _context.Stadiums.Select(s => new {
                Id = s.Id,
                NameWithId = $"{s.Name} (ID:{s.Id})"
            }).ToList();

            ViewData["Stadiums"] = new SelectList(stadiums, "Id", "NameWithId");
            return View();
        }

        // POST: Matches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Team1,Team2,Referee,Description,Date,StartTime,StadiumId")] Match match)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.Remove(nameof(match.Stadium));
            ModelState.Remove(nameof(match.Tickets));
 

            if (ModelState.IsValid)
            {
                _context.Add(match);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StadiumId"] = new SelectList(_context.Stadiums, "Id", "Id", match.StadiumId);
            return View(match);
        }

        // GET: Matches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null || _context.Matches == null)
            {
                return NotFound();
            }

            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            var stadiums = _context.Stadiums.Select(s => new {
                Id = s.Id,
                NameWithId = $"{s.Name} (ID:{s.Id})"
            }).ToList();

            ViewData["Stadiums"] = new SelectList(stadiums, "Id", "NameWithId");
            return View(match);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Team1,Team2,Referee,Description,Date,StartTime,StadiumId")] Match match)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != match.Id)
            {
                return NotFound();
            }


            ModelState.Remove(nameof(match.Stadium));
            ModelState.Remove(nameof(match.Tickets));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(match);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchExists(match.Id))
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
            ViewData["StadiumId"] = new SelectList(_context.Stadiums, "Id", "Id", match.StadiumId);
            return View(match);
        }

     

        // POST: Matches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (_context.Matches == null)
            {
                return Problem("Entity set 'TicktyContext.Matches'  is null.");
            }
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MatchExists(int id)
        {
          return (_context.Matches?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

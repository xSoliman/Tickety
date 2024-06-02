using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tickty.Models;

namespace Tickty.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TicktyContext _context;

        public TicketsController(TicktyContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var ticktyContext = _context.Tickets.Include(t => t.Match);
            return View(await ticktyContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Match)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        // GET: Tickets/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var matches = _context.Matches.Select(s => new {
                Id = s.Id,
                TeamsWithId = $"{s.Team1} vs {s.Team2} (ID:{s.Id})"
            }).ToList();

            ViewData["Matches"] = new SelectList(matches, "Id", "TeamsWithId");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Price,Description,TicketCount,Type,MatchId")] Ticket ticket)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.Remove(nameof(ticket.Match));
            ModelState.Remove(nameof(ticket.Bills));

            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MatchId"] = new SelectList(_context.Matches, "Id", "Id", ticket.MatchId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var matches = _context.Matches.Select(s => new {
                Id = s.Id,
                TeamsWithId = $"{s.Team1} vs {s.Team2} (ID:{s.Id})"
            }).ToList();

            ViewData["Matches"] = new SelectList(matches, "Id", "TeamsWithId");

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Price,Description,TicketCount,Type,MatchId")] Ticket ticket)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != ticket.Id)
            {
                return NotFound();
            }
            ModelState.Remove(nameof(ticket.Match));
            ModelState.Remove(nameof(ticket.Bills));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            ViewData["MatchId"] = new SelectList(_context.Matches, "Id", "Id", ticket.MatchId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Match)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'TicktyContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
          return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

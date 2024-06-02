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
    public class BillsController : Controller
    {
        private readonly TicktyContext _context;

        public BillsController(TicktyContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role!="Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var bills = await _context.Bills
                .Include(b => b.Ticket)
                    .ThenInclude(t => t.Match)
                        .ThenInclude(m => m.Stadium)
                .Include(u=>u.User)
                .OrderByDescending(b => b.Ticket.Match.Date)
                .ThenByDescending(b => b.Ticket.Match.StartTime)
                .ToListAsync();

            return View(bills);
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Bills == null)
            {
                return NotFound();
            }

            var bill = _context.Bills
                                  .Include(b => b.Ticket)        // Include the associated ticket
                                      .ThenInclude(t => t.Match) // Include the associated match details
                                          .ThenInclude(m => m.Stadium)
                                  .Include(u => u.User)
                                  .FirstOrDefault(b => b.Id == id);

            if (bill == null || bill.Ticket == null)
            {
                // Handle the case where the bill or ticket is not found
                TempData["ErrorMessage"] = "Ticket not found.";
                return RedirectToAction("Index", "Home");
            }

            // Create a TicketDetailsViewModel object to pass ticket details to the view
            var ticketDetailsViewModel = new TicketDetails
            {
                BillId = bill.Id,
                BillDate = bill.Date,
                MatchId = bill.Ticket.MatchId,
                Team1 = bill.Ticket.Match.Team1,
                Team2 = bill.Ticket.Match.Team2,
                Referee = bill.Ticket.Match.Referee,
                MatchDate = bill.Ticket.Match.Date,
                StartTime = bill.Ticket.Match.StartTime,
                StadiumName = bill.Ticket.Match.Stadium.Name,
                StadiumLocation = bill.Ticket.Match.Stadium.Location,
                GmapLocation = bill.Ticket.Match.Stadium.GmapLocation,
                QrCode = bill.QrCode,
                TicketId = bill.TicketId,
                TicketType = bill.Ticket.Type,
                UserId = bill.UserId,
                UserEmail = bill.User.Email

            };

            return View(ticketDetailsViewModel);
        }

      
        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.Bills == null)
            {
                return Problem("Entity set 'TicktyContext.Bills'  is null.");
            }
            var bill = await _context.Bills.FindAsync(id);
            if (bill != null)
            {
                _context.Bills.Remove(bill);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
          return (_context.Bills?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

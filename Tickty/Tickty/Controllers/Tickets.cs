using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickty.Models;
using System.Linq;
using System.Threading.Tasks;
using Tickty.ViewModels;

namespace Tickty.Controllers
{
    public class TicketsController : Controller
    {
        private readonly TicktyContext _db;

        public TicketsController(TicktyContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> UserTickets()
        {
            int? userId = HttpContext.Session.GetInt32("Id");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Please login first.";
                return RedirectToAction("Login", "Authentication");
            }

            var user = _db.Users
          .Include(u => u.Bills)
          .ThenInclude(b => b.Ticket)
          .ThenInclude(t => t.Match)
          .ThenInclude(s => s.Stadium)
          .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var tickets = user.Bills.Select(b => b.Ticket)
                .OrderByDescending(t => t.Match.Date)
                .ThenByDescending(t => t.Match.StartTime)
            .ToList();

            ViewBag.BillId = tickets.SelectMany(t => t.Bills)
                                       .FirstOrDefault(b => b.UserId == userId)?.Id;

            return View(tickets);
        }


        public IActionResult TicketDetails(int billId)
        {
            // Retrieve the bill corresponding to the provided bill ID including its associated ticket and match details
            var bill = _db.Bills
                           .Include(b => b.Ticket)        // Include the associated ticket
                               .ThenInclude(t => t.Match) // Include the associated match details
                                   .ThenInclude(m => m.Stadium) // Include the associated stadium details
                           .FirstOrDefault(b => b.Id == billId);

            if (bill == null || bill.Ticket == null)
            {
                // Handle the case where the bill or ticket is not found
                TempData["ErrorMessage"] = "Ticket not found.";
                return RedirectToAction("Index", "Home");
            }

            // Create a TicketDetailsViewModel object to pass ticket details to the view
            var ticketDetailsViewModel = new TicketDetails
            {
                Team1 = bill.Ticket.Match.Team1,
                Team2 = bill.Ticket.Match.Team2,
                Referee = bill.Ticket.Match.Referee,
                MatchDescription = bill.Ticket.Match.Description,
                Date = bill.Ticket.Match.Date,
                StartTime = bill.Ticket.Match.StartTime,
                StadiumName = bill.Ticket.Match.Stadium.Name,
                StadiumLocation = bill.Ticket.Match.Stadium.Location,
                QrCode = bill.QrCode,
                TicketType = bill.Ticket.Type,
                TicketDescription = bill.Ticket.Description
            };

            return View(ticketDetailsViewModel);
        }


    }
}

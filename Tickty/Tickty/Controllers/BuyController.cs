using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickty.Models;
using Tickty.ViewModels;
using QRCoder;
using System.IO;


namespace Tickty.Controllers;

public class BuyController : Controller
{ 
    private readonly TicktyContext _context;

    public BuyController(TicktyContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var matchDetails = _context.Matches
            .Include(m => m.Stadium)
            .Include(t => t.Tickets)
            .Select(m => new MatchDetails
            {
                matchId = m.Id,
                Team1 = m.Team1,
                Team2 = m.Team2,
                Description = m.Description,
                Date = m.Date,
                StartTime = m.StartTime,
                TicketCount = m.Tickets.Sum(t => t.TicketCount),
                LowestTicket = m.Tickets.Min(t => t.Price),
                StadiumName = m.Stadium.Name,
                StadiumLocation = m.Stadium.Location,
                GmapLocation = m.Stadium.GmapLocation,
                StadiumImage = m.Stadium.Image
            })
            .ToList();

        return View(matchDetails);
    }

    public IActionResult DisplayMatch(int id)
    {
        var match = _context.Matches
            .Include(m => m.Stadium)
             .Include(t => t.Tickets)
            .FirstOrDefault(m => m.Id == id);

        var matchTickets = _context.Tickets
            .Where(t => t.MatchId == id);

        decimal? lowestTicketPrice = null;

        if (matchTickets.Any())
        {
            lowestTicketPrice = matchTickets.Min(t => t.Price);
        }

        var matchDetails = new MatchDetails
        {
            matchId = match.Id,
            Team1 = match.Team1,
            Team2 = match.Team2,
            Referee = match.Referee,
            Description = match.Description,
            Date = match.Date,
            LowestTicket = lowestTicketPrice,
            TicketCount = match.Tickets.Sum(t => t.TicketCount),
            StartTime = match.StartTime,
            StadiumName = match.Stadium?.Name,
            StadiumLocation = match.Stadium?.Location,
            GmapLocation = match.Stadium.GmapLocation,
            StadiumImage = match.Stadium?.Image
        };
        matchDetails.StartTimeFormatted = DateTime.Today.Add(matchDetails.StartTime ?? TimeSpan.Zero).ToString("h:mm tt");

        return View(matchDetails);
    }

    public IActionResult BookMatch(int matchId, string wrongMessage)
    {
        var id = HttpContext.Session.GetInt32("Id");

        if (id == null)
        {
            return RedirectToAction("Login", "Authentication");
        }
        var tickets = _context.Tickets
            .Where(t => t.MatchId == matchId)
            .OrderBy(t => t.Price)
            .ToList();

        var match = _context.Matches
        .FirstOrDefault(m => m.Id == matchId);

        ViewBag.Team1 = match.Team1;
        ViewBag.Team2 = match.Team2;
        ViewBag.MatchId = match.Id;
        ViewData["Wrong"] = wrongMessage;

        return View(tickets);
    }

    public IActionResult Checkout(int ticketId, int matchId)
    {
        var sessId = HttpContext.Session.GetInt32("Id");

        if (sessId == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        if (ticketId == 0)
        {
            return RedirectToAction("BookMatch", "Process", new { matchId = matchId, wrongMessage = "Select a ticket is required" });
        }
        var ticket = _context.Tickets
                              .Include(t => t.Match)
                              .FirstOrDefault(t => t.Id == ticketId);

        var match = ticket.Match;
        var matchDetails = _context.Matches
       .Where(m => m.Id == matchId)
       .Select(m => new MatchDetails
       {
           matchId = m.Id,
           Team1 = m.Team1,
           Team2 = m.Team2,
           Referee = m.Referee,
           Description = m.Description,
           Date = m.Date,
           StartTime = m.StartTime,
           StadiumName = m.Stadium.Name,
           GmapLocation = m.Stadium.GmapLocation,
           StadiumLocation = m.Stadium.Location,
       })
       .FirstOrDefault();
        matchDetails.StartTimeFormatted = DateTime.Today.Add(matchDetails.StartTime ?? TimeSpan.Zero).ToString("h:mm tt");

        ViewBag.MatchDetails = matchDetails;
        return View(ticket);
    }

    public IActionResult Bill(int ticketId, int userId)
    {
        var sessId = HttpContext.Session.GetInt32("Id");

        if (sessId == null)
        {
            return RedirectToAction("Login", "Authentication");
        }
        var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (ticket.TicketCount <= 0)
        {
            return BadRequest("No tickets available.");
        }

        var bill = new Bill
        {
            TicketId = ticketId,
            UserId = userId,
            Date = DateTime.Now
        };

        // Save bill to database
        _context.Bills.Add(bill);
        _context.SaveChanges();

        // Retrieve the Id of the newly created bill
        var billId = bill.Id;

        // Generate QR code using the retrieved billId
        using (MemoryStream ms = new MemoryStream())
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"Bill Id: {billId}", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            var qrCodeBitmap = qrCode.GetGraphic(20);
            qrCodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bill.QrCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
        }

        // Update ticket count
        ticket.TicketCount -= 1;
        _context.SaveChanges();

        return RedirectToAction("Successful", new { billId });
    }

    public IActionResult Successful(int billId)
    {
        var sessId = HttpContext.Session.GetInt32("Id");

        if (sessId == null)
        {
            return RedirectToAction("Login", "Authentication");
        }
        if (billId==null)
            return RedirectToAction("Index", "Home");
        var bill = _context.Bills
              .Include(b => b.Ticket)
              .ThenInclude(t => t.Match)
              .ThenInclude(s => s.Stadium)
              .FirstOrDefault(b => b.Id == billId);

        if (bill == null)
        {
            return NotFound();
        }


        return View(bill);
    }

    public IActionResult Search(string searchString)
    {
        var results = _context.Matches
            .Include(m => m.Stadium) // Include the Stadium to use in the search criteria
            .Where(m => m.Team1.Contains(searchString) ||
                        m.Team2.Contains(searchString) ||
                        m.Referee.Contains(searchString) ||
                        m.Stadium.Name.Contains(searchString))
            .Select(m => new MatchDetails
            {
                matchId = m.Id,
                Team1 = m.Team1,
                Team2 = m.Team2,
                Referee = m.Referee,
                Description = m.Description,
                Date = m.Date,
                StartTime = m.StartTime,
                StartTimeFormatted = m.StartTime.ToString(@"hh\:mm"),
                TicketCount = m.Tickets.Sum(t => t.TicketCount), // Assuming you want the sum of ticket counts
                StadiumName = m.Stadium.Name,
                StadiumImage = m.Stadium.Image,
                StadiumLocation = m.Stadium.Location,
                GmapLocation = m.Stadium.GmapLocation,

                LowestTicket = m.Tickets.Min(t => t.Price) // Assuming you want the lowest ticket price
            })
            .ToList();

        return View("Index", results);
    }


}
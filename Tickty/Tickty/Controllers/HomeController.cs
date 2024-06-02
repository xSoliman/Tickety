using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tickty.Models;
using Tickty.ViewModels;

namespace Tickty.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TicktyContext _context;


        public HomeController(TicktyContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;

        }

        public IActionResult Index()
        {

            var currentDate = DateTime.Now;

            var upcomingMatches = _context.Matches
                .Include(m => m.Stadium)
                .Include(t => t.Tickets)
                .Where(m => m.Date > currentDate)
                .OrderBy(m => m.Date)
                .ThenBy(m => m.StartTime)
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

            return View(upcomingMatches);

        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult NotFound()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

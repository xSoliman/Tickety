using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickty.Models;
using System.Security.Cryptography;
using System.Text;
using Tickty.ViewModels;


public class UserProfile : Controller
{
    private readonly TicktyContext _db;

    public UserProfile(TicktyContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        int? userId = HttpContext.Session.GetInt32("Id");

        if (userId == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        User user = _db.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        return View(user);
    }



    public IActionResult UpdateProfile()
    {
        int? userId = HttpContext.Session.GetInt32("Id");

        if (userId == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        User user = _db.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        return View(user);
    }
    [HttpPost]
    public IActionResult UpdateProfile(User updatedUser)
    {
        int? userId = HttpContext.Session.GetInt32("Id");

        if (userId == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        var user = _db.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }



        var isEmailExist = _db.Users.Any(x => x.Email == updatedUser.Email);
        updatedUser.Role = user.Role;
        if (isEmailExist && updatedUser.Email != user.Email)
        {
            ModelState.AddModelError("Email", "Email is already exists");
            return View(updatedUser);
        }

        ModelState.Remove(nameof(updatedUser.Password));
        ModelState.Remove(nameof(updatedUser.Role));
        ModelState.Remove(nameof(updatedUser.Id));
        ModelState.Remove(nameof(updatedUser.Bills));

        if (!ModelState.IsValid)
        {
            return View();
        }
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Phone = updatedUser.Phone;



        _db.SaveChanges();

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return View(user); // Return the original user data without clientFile if no new photo is uploaded
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        // Retrieve user ID from the session
        int? userId = HttpContext.Session.GetInt32("Id");

        if (userId == null)
        {
            return RedirectToAction("Login", "Authentication"); // Redirect to login if the user is not authenticated
        }

        // You can load user data and pass it to the view if needed
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Authentication"); // Redirect to login if the user is not found
        }

        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        var userId = HttpContext.Session.GetInt32("Id");

        var user = _db.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Authentication");
        }

        if (currentPassword == null)
        {
            ViewBag.ErrorMessage = "Current password is Required";
            return View();
        }

        if (newPassword == null)
        {
            ViewBag.ErrorMessage = "New password is Required";
            return View();
        }
        // Check if the current password matches the hashed password in the database
        var hashedCurrentPassword = HashPassword(currentPassword);

        if (hashedCurrentPassword != user.Password)
        {
            ViewBag.ErrorMessage = "Current password is incorrect";
            return View();
        }


        // Check if the new password and confirm password match
        if (newPassword != confirmPassword)
        {
            ViewBag.ErrorMessage = "Passwords do not match";
            return View();
        }

        // Check if the new password meets the minimum length requirement
        if (newPassword.Length < 8)
        {
            ViewData["ErrorMessage"] = "Password must be at least 8 characters long";
            return View();
        }

        // Update the user's password in the database
        user.Password = HashPassword(newPassword);
        _db.SaveChanges();

        TempData["SuccessMessage"] = "Password changed successfully!";

        // Display the success message on the same page
        ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;

        // Render the view with the success message
        return View();
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

        int? userId = HttpContext.Session.GetInt32("Id");

        if (userId == null)
        {
            TempData["ErrorMessage"] = "Please login first.";
            return RedirectToAction("Login", "Authentication");
        }

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
            MatchDate = bill.Ticket.Match.Date,
            StartTime = bill.Ticket.Match.StartTime,
            StadiumName = bill.Ticket.Match.Stadium.Name,
            StadiumLocation = bill.Ticket.Match.Stadium.Location,
            QrCode = bill.QrCode,
            TicketType = bill.Ticket.Type,
            GmapLocation = bill.Ticket.Match.Stadium.GmapLocation,
            TicketDescription = bill.Ticket.Description
        };

        return View(ticketDetailsViewModel);
    }



    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}

using Tickty.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Tickty.ViewModels;
namespace Tickty.Controllers;

public class AuthenticationController : Controller
{
    private readonly TicktyContext _db;

    public AuthenticationController(TicktyContext db)
    {
        _db = db;

    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(User user)
    {
        // Remove model state errors for properties we don't care about during login
        ModelState.Remove(nameof(user.Name));
        ModelState.Remove(nameof(user.Phone));
        ModelState.Remove(nameof(user.Role));
        ModelState.Remove(nameof(user.Id));
        ModelState.Remove(nameof(user.Bills));


        if (ModelState.IsValid)
        {
            var authentUser = _db.Users
                .Where(u => u.Email == user.Email)
                .FirstOrDefault();

            if (authentUser != null &&
                authentUser.Password == HashPassword(user.Password))
            {
                HttpContext.Session.SetInt32("Id", authentUser.Id);
                HttpContext.Session.SetString("Role", authentUser.Role);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Wrong"] = "Wrong Email or Password";
                return View(user);
            }
        }
        else
        {
            return View(user);
        }
    }



    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(string ConfirmPassword, User signUpRequest)
    {
        ModelState.Remove(nameof(signUpRequest.Id));
        ModelState.Remove(nameof(signUpRequest.Role));
        ModelState.Remove(nameof(signUpRequest.Bills));

        if (!ModelState.IsValid)
        {
            return View(signUpRequest);
        }

        var isEmailExist = _db.Users.Any(x => x.Email == signUpRequest.Email);
        if (isEmailExist)
        {
            ModelState.AddModelError(nameof(signUpRequest.Email), "Email already exists");
            return View(signUpRequest);
        }

        if (signUpRequest.Password != ConfirmPassword)
        {
            ViewBag.Mismatch = "Password and confirm password doesn't match!";
            return View(signUpRequest);
        }

        var user = new User
        {
            Name = signUpRequest.Name,
            Email = signUpRequest.Email,
            Password = HashPassword(signUpRequest.Password),
            Phone = signUpRequest.Phone,
            Role = "User"
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        return RedirectToAction("Login", "Authentication");
    }


    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
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

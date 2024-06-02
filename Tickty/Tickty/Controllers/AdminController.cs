using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Tickty.Models;
using Microsoft.IdentityModel.Tokens;

namespace Tickty.Controllers
{
    public class AdminController : Controller
    {
        private readonly TicktyContext _context;

        public AdminController(TicktyContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index(string? error)
        {
            var role = HttpContext.Session.GetString("Role");

            if ( role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }

            if (error != null)
                ViewBag.Deny = error;
            return _context.Users != null ?
                View(await _context.Users.Where(u => u.Role == "Admin" || u.Role == "Owner").ToListAsync()) :
                Problem("Entity set 'TicktyContext.Users' is null.");
        }

       
        // GET: Admin/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string ConfirmPassword, User admin)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.Remove(nameof(admin.Id));
            ModelState.Remove(nameof(admin.Role));
            ModelState.Remove(nameof(admin.Bills));
            if (ConfirmPassword.IsNullOrEmpty())
            {
                Console.WriteLine("hi");
                ViewBag.Mismatch = "Confirm passowrd feild is required";
                return View(admin);
            }
            else if (admin.Password != ConfirmPassword)
            {
                ViewBag.Mismatch = "Password and confirm password doesn't match!";
                return View(admin);
            }
            if (!ModelState.IsValid)
            {
                return View(admin);
            }

            var isEmailExist = _context.Users.Any(x => x.Email == admin.Email);
            if (isEmailExist)
            {
                ModelState.AddModelError(nameof(admin.Email), "Email already exists");
                return View(admin);
            }
         

            var user = new User
            {
                Name = admin.Name,
                Email = admin.Email,
                Password = HashPassword(admin.Password),
                Phone = admin.Phone,
                Role = "Admin"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner")
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.Users == null)
            {
                return Problem("Entity set 'TicktyContext.Users' is null.");
            } 
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }


            if (user.Role == "Owner")
            {
                return RedirectToAction("Index", new { error = "Owner cannot be deleted!" });
            }
            else
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id && e.Role == "Admin")).GetValueOrDefault();
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
}

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
    public class UsersController : Controller
    {
        private readonly TicktyContext _context;

        public UsersController(TicktyContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            var users = await _context.Users.Where(u => u.Role == "user").ToListAsync();
            return View(users);
        }


        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (_context.Users == null)
            {
                return Problem("Entity set 'TicktyContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

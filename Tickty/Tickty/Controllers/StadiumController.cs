using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Tickty.Models;

namespace Tickty.Controllers
{
    public class StadiumController : Controller
    {
        private readonly TicktyContext _context;
        private readonly IHostingEnvironment _host;

        public StadiumController(TicktyContext context, IHostingEnvironment host)
        {
            _context = context;
            _host = host;

        }

        // GET: Stadia
        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return _context.Stadiums != null ? 
                          View(await _context.Stadiums.ToListAsync()) :
                          Problem("Entity set 'TicktyContext.Stadiums'  is null.");
        }

      
        // GET: Stadia/Create
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Description,GmapLocation,clientFile")] Stadium stadium)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.Remove(nameof(stadium.Matches)); // Remove unnecessary ModelState entries
            ModelState.Remove(nameof(stadium.Image)); // Remove unnecessary ModelState entries

            if (ModelState.IsValid)
            {
                // Check if clientFile is null
                if (stadium.clientFile == null)
                {
                    ModelState.AddModelError("clientFile", "Image is required.");
                    return View(stadium);
                }

                // Handle file upload
                if (stadium.clientFile != null)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(stadium.clientFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("clientFile", "Invalid file type. Only JPG, JPEG, PNG, and GIF files are allowed.");
                        return View(stadium);
                    }

                    // Validate file size (e.g., max 5MB)
                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    if (stadium.clientFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("clientFile", "File size must be less than 5MB.");
                        return View(stadium);
                    }

                    var filePath = Path.Combine(_host.WebRootPath, "img", stadium.clientFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await stadium.clientFile.CopyToAsync(stream);
                    }
                    stadium.Image = stadium.clientFile.FileName;
                }

                _context.Add(stadium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stadium);
        }


        // GET: Stadia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null || _context.Stadiums == null)
            {
                return NotFound();
            }

            var stadium = await _context.Stadiums.FindAsync(id);
            if (stadium == null)
            {
                return NotFound();
            }
            return View(stadium);
        }

        // POST: Stadia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Image,Description,GmapLocation,clientFile")] Stadium stadium)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            if (id != stadium.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(stadium.Matches));
            ModelState.Remove(nameof(stadium.Id));
            ModelState.Remove(nameof(stadium.Image));

            if (ModelState.IsValid)
            {
                // Handle file upload
                if (stadium.clientFile != null)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(stadium.clientFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("clientFile", "Invalid file type. Only JPG, JPEG, PNG, and GIF files are allowed.");
                        return View(stadium);
                    }

                    // Validate file size (e.g., max 5MB)
                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    if (stadium.clientFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("clientFile", "File size must be less than 5MB.");
                        return View(stadium);
                    }

                    var filePath = Path.Combine(_host.WebRootPath, "img", stadium.clientFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await stadium.clientFile.CopyToAsync(stream);
                    }
                    stadium.Image = stadium.clientFile.FileName;
                }
                else
                {
                    // Retain the existing image if no new file is uploaded
                    var existingStadium = await _context.Stadiums.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                    if (existingStadium != null)
                    {
                        stadium.Image = existingStadium.Image;
                    }
                }

                try
                {
                    _context.Update(stadium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StadiumExists(stadium.Id))
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
            return View(stadium);
        }

        private bool StadiumExists(int id)
        {
            return _context.Stadiums.Any(e => e.Id == id);
        }




        // POST: Stadia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Owner" && role != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine(id);
      
            var stadium = await _context.Stadiums.FindAsync(id);
           
               _context.Stadiums.Remove(stadium);
            
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Stadium");
        }

        
    }
}

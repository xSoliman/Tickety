using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Tickty.Models;
using System.Text;
using System.Security.Cryptography;


namespace Tickty.Controllers;

public class DashboardController : Controller
{

    private readonly TicktyContext DbContext;
    public DashboardController(TicktyContext context)
    {
        DbContext = context;
    }

    public IActionResult Dashboard()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role == "Admin" || role == "Owner")
        {
            return View();
        }
        return RedirectToAction("Index", "Home");

    }


}

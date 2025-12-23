using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;

namespace Kinoteatr.Controllers.Admin;

[Authorize(Roles = "Administrator")]
public class AdminPricesController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminPricesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sessions = await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .ToListAsync();
        return View(sessions);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if (session == null)
        {
            return NotFound();
        }
        
        return View(session);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, decimal price)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        session.Price = price;
        _context.Update(session);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}


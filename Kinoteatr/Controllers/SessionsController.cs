using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;

namespace Kinoteatr.Controllers;

[Authorize]
public class SessionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public SessionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? date)
    {
        var query = _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.Status == "active");

        if (date.HasValue)
        {
            query = query.Where(s => s.DateTime.Date == date.Value.Date);
        }
        else
        {
            query = query.Where(s => s.DateTime >= DateTime.Now);
        }

        var sessions = await query.OrderBy(s => s.DateTime).ToListAsync();
        return View(sessions);
    }

    public async Task<IActionResult> Details(int id)
    {
        var session = await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .ThenInclude(h => h.Seats.OrderBy(seat => seat.Row).ThenBy(seat => seat.SeatNumber))
            .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null)
        {
            return NotFound();
        }

        // Получаем занятые места для этого сеанса
        var bookedSeatIds = await _context.Tickets
            .Where(t => t.SessionId == id && (t.Status == "reserved" || t.Status == "paid"))
            .Select(t => t.SeatId)
            .ToListAsync();

        ViewBag.BookedSeatIds = bookedSeatIds;
        return View(session);
    }
}


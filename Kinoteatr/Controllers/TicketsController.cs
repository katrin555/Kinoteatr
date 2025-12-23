using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers;

[Authorize]
public class TicketsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public TicketsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Reserve(int sessionId, int seatId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var session = await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session == null || session.Status != "active")
        {
            return NotFound();
        }

        // Проверяем, не занято ли место
        var isSeatTaken = await _context.Tickets
            .AnyAsync(t => t.SessionId == sessionId && t.SeatId == seatId && 
                          (t.Status == "reserved" || t.Status == "paid"));

        if (isSeatTaken)
        {
            return BadRequest("Место уже занято");
        }

        var ticket = new Ticket
        {
            SessionId = sessionId,
            SeatId = seatId,
            Date = DateTime.Now,
            Price = session.Price,
            Status = "reserved"
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return RedirectToAction("Cart");
    }

    public async Task<IActionResult> Cart()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var reservedTickets = await _context.Tickets
            .Include(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(t => t.Session)
            .ThenInclude(s => s.Hall)
            .Include(t => t.Seat)
            .Where(t => t.PurchaseId == null && t.Status == "reserved")
            .ToListAsync();

        return View(reservedTickets);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket != null && ticket.Status == "reserved" && ticket.PurchaseId == null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Cart");
    }
}


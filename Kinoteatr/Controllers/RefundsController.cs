using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers;

[Authorize]
public class RefundsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public RefundsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int ticketId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var ticket = await _context.Tickets
            .Include(t => t.Purchase)
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null || ticket.Purchase == null || ticket.Purchase.UserId != user.Id)
        {
            return NotFound();
        }

        if (ticket.Status != "paid")
        {
            return BadRequest("Можно вернуть только оплаченный билет");
        }

        // Проверяем, нет ли уже активного запроса на возврат
        var existingRefund = await _context.Refunds
            .FirstOrDefaultAsync(r => r.TicketId == ticketId && r.Status == "pending");

        if (existingRefund != null)
        {
            return BadRequest("Запрос на возврат уже существует");
        }

        var refund = new Refund
        {
            TicketId = ticketId,
            UserId = user.Id,
            RequestDate = DateTime.Now,
            Status = "pending",
            Amount = ticket.Price
        };

        _context.Refunds.Add(refund);
        await _context.SaveChangesAsync();

        return RedirectToAction("MyRefunds");
    }

    public async Task<IActionResult> MyRefunds()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var refunds = await _context.Refunds
            .Include(r => r.Ticket)
            .ThenInclude(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(r => r.Ticket)
            .ThenInclude(t => t.Seat)
            .Where(r => r.UserId == user.Id)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

        return View(refunds);
    }
}


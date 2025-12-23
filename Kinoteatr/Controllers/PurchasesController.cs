using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers;

[Authorize]
public class PurchasesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public PurchasesController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(string paymentMethod)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var reservedTickets = await _context.Tickets
            .Where(t => t.PurchaseId == null && t.Status == "reserved")
            .ToListAsync();

        if (!reservedTickets.Any())
        {
            return RedirectToAction("Cart", "Tickets");
        }

        var totalAmount = reservedTickets.Sum(t => t.Price);

        var purchase = new Purchase
        {
            UserId = user.Id,
            DateTime = DateTime.Now,
            PaymentMethod = paymentMethod,
            TotalAmount = totalAmount,
            Status = "completed"
        };

        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();

        // Привязываем билеты к покупке и меняем статус
        foreach (var ticket in reservedTickets)
        {
            ticket.PurchaseId = purchase.Id;
            ticket.Status = "paid";
        }

        await _context.SaveChangesAsync();

        // Здесь должна быть отправка email с билетами
        // Для упрощения просто перенаправляем на страницу покупки

        return RedirectToAction("Details", new { id = purchase.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var purchase = await _context.Purchases
            .Include(p => p.Tickets)
            .ThenInclude(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(p => p.Tickets)
            .ThenInclude(t => t.Seat)
            .Include(p => p.Tickets)
            .ThenInclude(t => t.Session)
            .ThenInclude(s => s.Hall)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == user.Id);

        if (purchase == null)
        {
            return NotFound();
        }

        return View(purchase);
    }

    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var purchases = await _context.Purchases
            .Include(p => p.Tickets)
            .ThenInclude(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.DateTime)
            .ToListAsync();

        return View(purchases);
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers.Admin;

[Authorize(Roles = "Administrator")]
public class AdminRefundsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminRefundsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var refunds = await _context.Refunds
            .Include(r => r.Ticket)
            .ThenInclude(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(r => r.Ticket)
            .ThenInclude(t => t.Seat)
            .Include(r => r.User)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();
        return View(refunds);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var refund = await _context.Refunds
            .Include(r => r.Ticket)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (refund == null)
        {
            return NotFound();
        }

        refund.Status = "approved";
        refund.Ticket.Status = "returned";
        
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var refund = await _context.Refunds.FindAsync(id);
        if (refund == null)
        {
            return NotFound();
        }

        refund.Status = "rejected";
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}


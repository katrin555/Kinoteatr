using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers.Admin;

[Authorize(Roles = "Administrator")]
public class AdminSeatsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminSeatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? hallId)
    {
        var query = _context.Seats.Include(s => s.Hall).AsQueryable();
        
        if (hallId.HasValue)
        {
            query = query.Where(s => s.HallId == hallId.Value);
        }

        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", hallId);
        return View(await query.ToListAsync());
    }

    public IActionResult Create()
    {
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Seat seat)
    {
        if (ModelState.IsValid)
        {
            _context.Add(seat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", seat.HallId);
        return View(seat);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat == null)
        {
            return NotFound();
        }
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", seat.HallId);
        return View(seat);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Seat seat)
    {
        if (id != seat.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(seat);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeatExists(seat.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", seat.HallId);
        return View(seat);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var seat = await _context.Seats
            .Include(s => s.Hall)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (seat == null)
        {
            return NotFound();
        }
        return View(seat);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var seat = await _context.Seats.FindAsync(id);
        if (seat != null)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool SeatExists(int id)
    {
        return _context.Seats.Any(e => e.Id == id);
    }
}


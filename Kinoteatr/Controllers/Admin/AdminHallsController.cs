using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers.Admin;

[Authorize(Roles = "Administrator")]
public class AdminHallsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminHallsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Halls.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Hall hall)
    {
        if (ModelState.IsValid)
        {
            _context.Add(hall);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(hall);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var hall = await _context.Halls.FindAsync(id);
        if (hall == null)
        {
            return NotFound();
        }
        return View(hall);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Hall hall)
    {
        if (id != hall.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(hall);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HallExists(hall.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(hall);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var hall = await _context.Halls.FindAsync(id);
        if (hall == null)
        {
            return NotFound();
        }
        return View(hall);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var hall = await _context.Halls.FindAsync(id);
        if (hall != null)
        {
            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool HallExists(int id)
    {
        return _context.Halls.Any(e => e.Id == id);
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers.Admin;

[Authorize(Roles = "Administrator")]
public class AdminSessionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminSessionsController(ApplicationDbContext context)
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

    public async Task<IActionResult> Create()
    {
        ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Session session)
    {
        if (ModelState.IsValid)
        {
            _context.Add(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", session.MovieId);
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
        return View(session);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null)
        {
            return NotFound();
        }
        ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", session.MovieId);
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
        return View(session);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Session session)
    {
        if (id != session.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(session);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionExists(session.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", session.MovieId);
        ViewData["HallId"] = new SelectList(_context.Halls, "Id", "Name", session.HallId);
        return View(session);
    }

    public async Task<IActionResult> Delete(int id)
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

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool SessionExists(int id)
    {
        return _context.Sessions.Any(e => e.Id == id);
    }
}


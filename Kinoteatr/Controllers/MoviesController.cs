using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kinoteatr.Data;
using Kinoteatr.Models;

namespace Kinoteatr.Controllers;

[Authorize]
public class MoviesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MoviesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? genre, string? search)
    {
        var movies = _context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(genre))
        {
            movies = movies.Where(m => m.Genre == genre);
        }

        if (!string.IsNullOrEmpty(search))
        {
            movies = movies.Where(m => m.Title.Contains(search) || m.Description.Contains(search));
        }

        ViewBag.Genres = await _context.Movies.Select(m => m.Genre).Distinct().ToListAsync();
        return View(await movies.ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies
            .Include(m => m.Sessions)
            .ThenInclude(s => s.Hall)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }
}


using Microsoft.AspNetCore.Mvc;

namespace Kinoteatr.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}


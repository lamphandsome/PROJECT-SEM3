using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using System.Diagnostics;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Post()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Blog_details()
    {
        return View();
    }

    public IActionResult Blog()
    {
        var posts = _context.Posts.OrderByDescending(p => p.CreatedAt).ToList(); // Lấy danh sách bài viết
        return View(posts); // Truyền danh sách này đến View
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

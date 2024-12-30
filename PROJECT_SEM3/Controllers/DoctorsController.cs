using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.ViewModels;

namespace PROJECT_SEM3.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var doctors = _context.Users
                .Where(u => u.Thumbnail != null && u.Thumbnail != "")
                .Select(u => new DoctorViewModel
                {
                    FullName = u.FullName,
                    Thumbnail = u.Thumbnail
                })
                .ToList();

            return View(doctors);

        }
        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View("SearchResults", new List<DoctorViewModel>());
            }

            var results = _context.Users
                .Include(u => u.Location)
                .Where(u => u.FullName.Contains(query) ||
                            (u.Location != null &&
                             (u.Location.City.Contains(query) || u.Location.Country.Contains(query))))
                .Select(u => new DoctorViewModel
                {
                    FullName = u.FullName,
                    Thumbnail = u.Thumbnail,
                    Locations = u.Location != null ? $"{u.Location.City}, {u.Location.Country}" : "N/A"
                })
                .ToList();

            return View("SearchResults", results);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
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
    }
}

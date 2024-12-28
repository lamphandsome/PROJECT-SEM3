using Microsoft.AspNetCore.Mvc;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.ViewModels;


namespace PROJECT_SEM3.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        AppDbContext db = new AppDbContext();
        [Route("")]
        [Route("index")] 
        public IActionResult Index()
        {
            return View();
        }
        [Route("ListDoctors")]
        public IActionResult ListDoctors()
        {
            var doctors = db.Users.Select(u => new DoctorViewModel
            {
                FullName = u.FullName,
                Thumbnail = u.Thumbnail,
                Locations = u.Location.City ?? "N/A"
            }).ToList();

            return View(doctors);
        }
    }
}
